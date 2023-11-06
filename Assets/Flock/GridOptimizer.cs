using System.Collections.Generic;
using UnityEngine;

class GridOptimizer<T> where T : Component{

	public Vector2 GridStart {
		get => _gridStart;
		set {
			_gridStart = value;
			recomputeGrid = true;
		}
	}
	public Vector2 GridSize{
		get => _gridSize;
		set {
			_gridSize = value;
			recomputeGrid = true;
		}
	}
	public float CellSize {
		get => _cellSize;
		set {
			_cellSize = value;
			recomputeGrid = true;
		}
	}

	private	List<T> objectList = new();
	private List<T>[][] grid;
	private bool recomputeGrid = true;
	private Vector2Int cellCount;
	private Vector2 _gridStart, _gridSize;
	private float _cellSize;

	public void AddAll(List<T> objects) {
		objectList.AddRange(objects);
	}

	public void CalculateGrid() {
		Clear();
		if(recomputeGrid)
			RecomputeGrid();

		foreach(T obj in objectList) {
			Vector2Int coords = WorldToGridCoords(obj.transform.position);
			grid[coords.x][coords.y].Add(obj);
		}
	}

	private void Clear() {
		for (int x = 0; x < cellCount.x; x++)
		{
			for (int y = 0; y < cellCount.y; y++)
			{
				grid[x][y].Clear();	
			}
		}
	}

	private void RecomputeGrid() {
		cellCount.x = Mathf.CeilToInt(GridSize.x / CellSize);
		cellCount.y = Mathf.CeilToInt(GridSize.y / CellSize);
		grid = new List<T>[cellCount.x][];
		for (int x = 0; x < cellCount.x; x++)
		{
			grid[x] = new List<T>[cellCount.y];
			for (int y = 0; y < cellCount.y; y++)
			{
				grid[x][y] = new List<T>();
			}
		}
		recomputeGrid = false;
	}

	private Vector2Int WorldToGridCoords(Vector2 pos) {
		Vector2 relativePos = pos - GridStart;
		int x = Mathf.FloorToInt(relativePos.x / CellSize);
		x = Mathf.Clamp(x, 0, cellCount.x-1);
		int y = Mathf.FloorToInt(relativePos.y / CellSize);
		y = Mathf.Clamp(y, 0, cellCount.y-1);
		return new(x, y);
	}

	public List<T> GetEverythingAround(T obj) {
		return GetEverythingAround(obj.transform.position);
	}

	public List<T> GetEverythingAround(Vector2 position) {
		Vector2Int coords = WorldToGridCoords(position);
		int x = coords.x;
		int y = coords.y;

		Vector2Int[] neighbours = {
			new(x-1, y-1),
			new(x-1, y),
			new(x-1, y+1),
			new(x, y-1),
			new(x, y),
			new(x, y+1),
			new(x+1, y-1),
			new(x+1, y),
			new(x+1, y+1)
		};

		List<T> result = new();
		foreach(Vector2Int n in neighbours) {
			result.AddRange(GetGridCellSafe(n.x, n.y));
		}
		return result;
	}

	private List<T> GetGridCellSafe(int x, int y) {
		if(x < 0 || x >= cellCount.x || y < 0 || y >= cellCount.y)
			return new();
		else
			return grid[x][y];
	}

	public Vector2Int GetCellCount() {
		if(recomputeGrid)
			RecomputeGrid();
		return cellCount;
	}

	public List<T> GetFishInCell(int x, int y) {
		if(recomputeGrid)
			RecomputeGrid();
		return grid[x][y];
	}
}