using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridOptimizerTest : MonoBehaviour
{
    public Flock flock;
    public Map map;

    private GridOptimizer<Fish> fishGridOptimizer = new();
    private Color[][] colors;

    void Start()
    {
        fishGridOptimizer.GridStart = map.center - map.size / 2;
        fishGridOptimizer.GridSize = map.size;
        fishGridOptimizer.CellSize = flock.ScanRadius;
        fishGridOptimizer.AddAll(flock.GetAllFish());

        Vector2Int cellCount = fishGridOptimizer.GetCellCount();
        colors = new Color[cellCount.x][];
        for (int x = 0; x < cellCount.x; x++)
        {
            colors[x] = new Color[cellCount.y];
            for (int y = 0; y < cellCount.y; y++)
            {
                colors[x][y] = Random.ColorHSV();
            }
        }
    }


    void Update() {
        fishGridOptimizer.CalculateGrid();
        Vector2Int cellCount = fishGridOptimizer.GetCellCount();
        for (int x = 0; x < cellCount.x; x++)
        {
            for (int y = 0; y < cellCount.y; y++)
            {
                List<Fish> fishList = fishGridOptimizer.GetFishInCell(x, y);
                foreach (Fish fish in fishList)
                {
                    fish.GetComponent<SpriteRenderer>().color = colors[x][y];
                }
            }
        }
    }
}
