using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Flock : MonoBehaviour
{
    public int FishCount = 50;
    public float ScanRadius = 2;
    public float SeparationRadius = 1f;
    public float CohesionStrength = 1;
    public float AlignmentStrength = 4;
    public float SeparationStrength = 0.5f;
    public float SeparationScaling = 3;
    public float CollisionAvoidanceStrength = 4;

    // All of this is passed down to fish-------------------
    public CollisionDetectionParams collisionParams;
    public float MaxSpeed;
    public float MaxTargetDistance;
    public float MaxAcceleration;
    //-----------------------------------------------------

    public Fish fishPrefab;
    public Rect bounds;
    public Map map;

    private List<Fish> fishList;
    private GridOptimizer<Fish> fishGridOptimizer = new();

    public void Start()
    {
        Application.targetFrameRate = 60;
        fishList = new List<Fish>(FishCount);
        for (int i = 0; i < FishCount; i++)
           fishList.Add(RandomFish());

        fishGridOptimizer.GridStart = map.center - map.size / 2;
        fishGridOptimizer.GridSize = map.size;
        fishGridOptimizer.CellSize = ScanRadius;
        fishGridOptimizer.AddAll(fishList);
    }

    private Fish RandomFish() {
        Fish fish = Instantiate(fishPrefab, transform);
        float x = Random.Range(bounds.xMin, bounds.xMax);
        float y = Random.Range(bounds.yMin, bounds.yMax);
        Vector2 direction = Random.insideUnitCircle;
        float speed = Random.Range(0, fish.MaxSpeed);

        fish.Position = new Vector2(x, y);
        fish.Velocity = direction * speed;
        fish.TargetDirection = direction;
        fish.bounds = bounds;
        fish.collisionParams = collisionParams;
        return fish;
    }

    public void Update()
    {
        fishGridOptimizer.CalculateGrid();
        foreach (Fish fish in fishList) {
            fish.MaxAcceleration = MaxAcceleration;
            fish.MaxSpeed = MaxSpeed;
            fish.MaxTargetDistance = MaxTargetDistance;

            List<Tuple<Fish, float>> distances = GetDistances(fish, ScanRadius);

            Vector2 cohesion = GetCohesionVector(fish, distances);
            Vector2 alignment = GetAlignmentVector(fish, distances);
            Vector2 separation = GetSeparationVector(fish, distances);
            Vector2 avoidance = GetAvoidanceVector(fish);
            
            fish.TargetDirection = cohesion + alignment + separation + avoidance;
        }
    }

    private List<Tuple<Fish, float>> GetDistances(Fish fish, float maxDist) {
        List<Fish> closeFish = fishGridOptimizer.GetEverythingAround(fish);

        List<Tuple<Fish, float>> distances = new(closeFish.Count - 1);
        foreach(Fish f in closeFish) {
            if(f == fish)
                continue;
            
            float dist = (f.Position - fish.Position).magnitude;
            if(dist > maxDist)
                continue;

            distances.Add(new Tuple<Fish, float>(f, dist));
        }
        return distances;
    }

    private Vector2 GetCohesionVector(Fish fish, List<Tuple<Fish, float>> distances) {
        if(distances.Count == 0)
            return Vector2.zero;
        
        Vector2 groupCenter = Vector2.zero;
        foreach ((Fish f, float dist) in distances)
            groupCenter += f.Position;
        groupCenter /= distances.Count;

        Vector2 coherenceVector = groupCenter - fish.Position;
        coherenceVector.Normalize();
        return coherenceVector * CohesionStrength;
    }

    private Vector2 GetAlignmentVector(Fish fish, List<Tuple<Fish, float>> distances) {
        if(distances.Count == 0)
            return Vector2.zero;
        
        Vector2 avgDirection = Vector2.zero;
        foreach ((Fish f, float dist) in distances)
            avgDirection += f.Velocity.normalized;
        avgDirection /= distances.Count;

        avgDirection.Normalize();
        return avgDirection * AlignmentStrength;
    }

    private Vector2 GetSeparationVector(Fish fish, List<Tuple<Fish, float>> distances) {
        if(distances.Count == 0)
            return Vector2.zero;

        Vector2 separationVector = Vector2.zero;
        foreach ((Fish f, float dist) in distances) {
            if(dist > SeparationRadius)
                continue;

            float force = (ScanRadius - dist) / ScanRadius;
            force = Mathf.Pow(force, SeparationScaling);
            separationVector += (fish.Position - f.Position).normalized * force;
        }
        separationVector /= distances.Count;

        return separationVector * SeparationStrength;
    }

    private Vector2 GetAvoidanceVector(Fish fish) {
        if(fish.CheckFrontDirectionForColisions())
            return fish.FindDirectionWithoutCollision() * CollisionAvoidanceStrength;
        else
            return Vector2.zero;
    }

    public List<Fish> GetAllFish() {
        return fishList;
    }
}
