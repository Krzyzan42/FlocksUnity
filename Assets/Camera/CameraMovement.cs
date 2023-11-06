using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed = 4f;
    public Map map;
    public float overreach = 0.5f;
    public float maxSize = 10;
    public float minSize = 1;
    public float scrollSpeed = 1f;

    private Camera cam;

    void Awake() {
        cam = GetComponent<Camera>();
    }
    // Update is called once per frame
    void Update()
    {
        float up = Input.GetKey(KeyCode.W) ? 1f : 0f;        
        float left = Input.GetKey(KeyCode.A) ? -1f:0f;        
        float down = Input.GetKey(KeyCode.S) ? -1f:0f;        
        float right = Input.GetKey(KeyCode.D) ? 1f:0f;        
        float zoom = Input.mouseScrollDelta.y;

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + zoom, minSize, maxSize);
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        Vector3 newPos = cam.transform.position + new Vector3(left + right, up+down);
        Rect bounds = Rect.MinMaxRect(
            map.center.x - map.size.x - overreach + camWidth,
            map.center.y - map.size.y - overreach + camHeight,
            map.center.x + map.size.x + overreach - camWidth,
            map.center.y + map.size.y + overreach - camHeight
        );
        newPos = new(
            Mathf.Clamp(newPos.x, bounds.xMin, bounds.xMax),
            Mathf.Clamp(newPos.y, bounds.yMin, bounds.yMax),
            newPos.z
        );
        if(bounds.xMax <= bounds.xMin)
            newPos.x = map.center.x;
        if(bounds.yMax <= bounds.yMin)
            newPos.y = map.center.y;
        cam.transform.position=newPos;
    }
}
