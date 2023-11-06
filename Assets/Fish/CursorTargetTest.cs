using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTarget : MonoBehaviour
{
    public Fish fish;
    
    void Update()
    {
        fish.TargetDirection = fish.Position + (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
