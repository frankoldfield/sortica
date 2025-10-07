using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Distances;

public class Grid : MonoBehaviour
{

    public int width, height;
    public float tileSize;
    public Dictionary<Vector2Int, Tile> tiles { get; set; }
    public bool debug;
    public GameObject gridPrefab;
    public GameObject obstaclePrefab;

    void Awake()
    {
        tiles = new Dictionary<Vector2Int, Tile>();

        for (int x = -width; x <= width; x++)
        {
            for (int y = -height; y <= height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                tiles[pos] = new Tile(pos, tileSize, gridPrefab, obstaclePrefab, debug);
            }
        }
    }

    public List<Tile> GetNeighbors(Tile currentTile, Distancia DistanceMetric)
    {
        List<Tile> neighbors = new List<Tile>();

        foreach (Tile tile in tiles.Values)
        {
            if (GetDistance(currentTile, tile, DistanceMetric) <= Mathf.Sqrt(2) && tile!=currentTile && !tile.IsObstacle)
            {
                neighbors.Add(tile);
            }
        }
        return neighbors;
    }

    public List<Tile> GetLocalSearchSpace(Tile currentTile, Tile GoalTile, Distancia DistanceMetric, float localSpaceDepth)
    {
        List<Tile> neighbors = new List<Tile>();

        foreach (Tile tile in tiles.Values)
        {
            if (GetDistance(currentTile, tile, DistanceMetric) <= localSpaceDepth && !tile.IsObstacle && tile!=GoalTile)
            {
                neighbors.Add(tile);
            }
        }
        return neighbors;
    }

    public Tile WorldToTile(Vector3 position)
    {
        Vector2Int tileIndex = new Vector2Int(
            Mathf.Min(Mathf.Max((int)Mathf.Round(position.x/tileSize), -width), width), 
            Mathf.Min(Mathf.Max((int)Mathf.Round(position.z / tileSize), -height), height));
        return tiles[tileIndex];
    }
}
