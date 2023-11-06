using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DebugColors : MonoBehaviour
{
    private Flock flock;
    private Fish fish;
    private SpriteRenderer sprite;

    void Start()
    {
        fish = GetComponent<Fish>();    
        sprite = GetComponent<SpriteRenderer>();
        flock = FindAnyObjectByType<Flock>();
    }

    void Update()
    {
        sprite.color = fish.CheckFrontDirectionForColisions() ? Color.red : Color.green;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Vector3 p = fish.TargetDirection + fish.Position;
        Gizmos.DrawSphere(p, 0.1f);
    }
}
