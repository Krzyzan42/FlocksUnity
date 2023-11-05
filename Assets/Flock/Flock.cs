using System;
using System.Collections;
using System.Collections.Generic;
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


    public Fish fishPrefab;
    public Rect bounds;

    private List<Fish> fishList;

    // Start is called before the first frame update
    public void Start()
    {
        fishList = new List<Fish>(FishCount);
        for (int i = 0; i < FishCount; i++)
           fishList.Add(RandomFish());
    }

    private Fish RandomFish() {
        Fish fish = Instantiate(fishPrefab);
        float x = Random.Range(bounds.xMin, bounds.xMax);
        float y = Random.Range(bounds.yMin, bounds.yMax);
        Vector2 direction = Random.insideUnitCircle;
        float speed = Random.Range(0, fish.MaxSpeed);

        fish.Position = new Vector2(x, y);
        fish.Velocity = direction * speed;
        fish.TargetDirection = direction;
        fish.bounds = bounds;
        return fish;
    }

    public void Update()
    {
        foreach (Fish fish in fishList) {
            List<Tuple<Fish, float>> distances = GetDistances(fish, ScanRadius);

            Vector2 cohesion = GetCohesionVector(fish, distances);
            Vector2 alignment = GetAlignmentVector(fish, distances);
            Vector2 separation = GetSeparationVector(fish, distances);
            
            fish.TargetDirection = cohesion + alignment + separation;
        }
    }

    private List<Tuple<Fish, float>> GetDistances(Fish fish, float maxDist) {
        List<Tuple<Fish, float>> distances = new(FishCount-1);
        foreach(Fish f in fishList) {
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
}
