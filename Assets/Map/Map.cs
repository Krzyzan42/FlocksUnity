using System;
using UnityEngine;
using math = Unity.Mathematics.math;

public class Map : MonoBehaviour
{
	public Vector2 size;
	public Vector2 center;
	public Vector2Int resolution;
	public Vector2 origin;
	public float scale;
	[Range(0f, 1f)]
	public float density;
	public Color fillColor, groundColor;

	private bool[][] map;

	void Start(){ 
		Generate();
	}

	void Update() {
		if(Input.GetKeyDown(KeyCode.R))
			Generate();
	}

	public void Generate() {
		GenerateMesh();

		Color[] pixels = new Color[resolution.x * resolution.y];
		Texture2D noise = new Texture2D(resolution.x, resolution.y);
		map = new bool[resolution.x][];
		for (int x = 0; x < resolution.x; x++)
		{
			map[x] = new bool[resolution.y];
		}

		for (int y = 0; y < resolution.y; y++)
		{
			for (int x = 0; x < resolution.x; x++)
			{
				float xp = origin.x + 1f * x / resolution.x * scale;
				float yp = origin.y + 1f * y / resolution.x * scale;
				float val = Mathf.PerlinNoise(xp, yp);
				if(val > 1f - density) {
					map[x][y] = true;
					pixels[y * resolution.x + x] = fillColor;
				}
				else {
					map[x][y] = false;
					pixels[y * resolution.x + x] = groundColor;
				}
			}
		}

		noise.SetPixels(pixels);
		noise.Apply();
		GetComponent<Renderer>().material.mainTexture = noise;
	} 

    void GenerateMesh() {
		float bottom = center.y - size.y;
		float top = center.y + size.y;
		float left = center.x - size.x;
		float right = center.x + size.x;

        Mesh mesh = new();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = new Vector3[] {new (left, top), new (right, top), new (right, bottom), new (left, bottom)};
        mesh.uv = new Vector2[] {new (0, 1), new (1, 1), new (1, 0), new (0, 0)};
        mesh.triangles = new int[] {0, 1, 2, 0, 2, 3};
    }

	public bool IsAWall(Vector2 worldPoint) {
		float bottom = center.y - size.y;
		float top = center.y + size.y;
		float left = center.x - size.x;
		float right = center.x + size.x;
		
		if(worldPoint.x < left || worldPoint.x > right)
			return true;
		if(worldPoint.y < bottom||worldPoint.y > top)
			return true;

		int x = (int)math.remap(left, right, 0, resolution.x, worldPoint.x);
		int y = (int)math.remap(bottom, top, 0, resolution.y, worldPoint.y);

		return map[x][y];
	}

}
