using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTarget : MonoBehaviour
{
    public Fish fish;
    
    void Update()
    {
        fish.TargetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
