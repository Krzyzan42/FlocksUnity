using UnityEngine;

public class IsObstacleTest : MonoBehaviour
{
    public Map map;

    void Update()
    {
        Vector2 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(map.IsAWall(cursor));         
    }
}
