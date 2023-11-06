using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Fish : MonoBehaviour
{
    public float MaxSpeed = 10;
    public float MaxAcceleration = 100;
    public float MaxTargetDistance = 5;
    public float tol = 4;

    public Vector2 Position { get => transform.position; set => transform.position = value; }
    public Vector2 Velocity;

    public Vector2 TargetDirection;
    public Rect bounds;
    public CollisionDetectionParams collisionParams;

    private Map map;

    void Awake() {
        map = FindAnyObjectByType<Map>();
    }

    void FixedUpdate()
    {
        if(TargetDirection.magnitude < 0.01f)
            TargetDirection = transform.up;
        if(TargetDirection.magnitude > 0.01f) {
            float strength = Mathf.Clamp01(TargetDirection.magnitude / MaxTargetDistance);
            float acc = strength * MaxAcceleration;
            TargetDirection.Normalize();

            Velocity = Vector2.ClampMagnitude(Velocity + acc * Time.fixedDeltaTime * TargetDirection, MaxSpeed);
        }
        Position += Time.fixedDeltaTime * Velocity;
        transform.up = Velocity;

        KeepInBounds();
    }

    void KeepInBounds() {
        if(Position.x > bounds.xMax)
            Position -= new Vector2(bounds.width, 0);
        else if(Position.x < bounds.xMin)
            Position += new Vector2(bounds.width, 0);
        if(Position.y > bounds.yMax)
            Position -= new Vector2(0, bounds.height);
        else if(Position.y < bounds.yMin)
            Position += new Vector2(0, bounds.height);
    }

    public bool CheckFrontDirectionForColisions() {
        float angle = collisionParams.InitialAngle;

        bool left = CheckObstacle(Rotate(transform.up, -angle));
        bool middle = CheckObstacle(transform.up);
        bool right = CheckObstacle(Rotate(transform.up, angle));
        return left || middle || right;
    }

    private bool CheckObstacle(Vector2 direction) {
        float distance = collisionParams.CheckDistance;
        int sampling = collisionParams.Samples;

        Vector2 sample = direction * distance / sampling;
        Vector2 scanPosition = Position + sample;
        for (int i = 0; i < sampling; i++)
        {
            if(map.IsAWall(scanPosition))
                return true;
            scanPosition += sample;
        }
        return false;
    }

    private Vector2 Rotate(Vector2 v, float angle) {
        return Quaternion.AngleAxis(angle, Vector3.forward) * v;
    }

    public Vector2 FindDirectionWithoutCollision() {
        float angle = collisionParams.InitialAngle;
        float angleInterval = collisionParams.AngleInterval;
        float overangle = collisionParams.OverAngle;
        
        while (angle < 180)
        {
            Vector2 right = Rotate(transform.up, angle);
            Vector2 left = Rotate(transform.up, -angle);

            if(!CheckObstacle(right))
                return Rotate(right, overangle);
            if(!CheckObstacle(left))
                return Rotate(left, -overangle);
            angle += angleInterval;
        }
        return -transform.up;
    }
}
