using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public float MaxSpeed = 10;
    public float MaxAcceleration = 100;
    public float MaxTargetDistance = 5;

    public Vector2 Position { get => transform.position; set => transform.position = value; }
    public Vector2 Velocity;

    public Vector2 TargetPosition;
    public Rect bounds;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 targetDir = TargetPosition - Position;
        if(targetDir.magnitude > 0.01f) {
            float strength = Mathf.Clamp01(targetDir.magnitude / MaxTargetDistance);
            float acc = strength * MaxAcceleration;
            targetDir.Normalize();

            Velocity = Vector2.ClampMagnitude(Velocity + acc * Time.fixedDeltaTime * targetDir, MaxSpeed);
        }
        Position += Time.fixedDeltaTime * Velocity;
        transform.up = Velocity;
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
}