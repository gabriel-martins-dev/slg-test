using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector2Int Position = default;
    public HashSet<Vector2Int> Neighbors = new HashSet<Vector2Int>();

    [SerializeField] GameObject wallUp = default;
    [SerializeField] GameObject wallDown = default;
    [SerializeField] GameObject wallRight = default;
    [SerializeField] GameObject wallLeft = default;

    public Cell Parent;
    public float G;
    public float H;
    public float F => G + H;

    public void AddNeighbor(Vector2Int n)
    {
        Neighbors.Add(Position + n);
    }

    public void BuildWalls()
    {
        if (!Neighbors.Any(neighbor => neighbor == Up))
            Instantiate(wallUp, transform);
        if (!Neighbors.Any(neighbor => neighbor == Down))
            Instantiate(wallDown, transform);
        if (!Neighbors.Any(neighbor => neighbor == Right))
            Instantiate(wallRight, transform);
        if (!Neighbors.Any(neighbor => neighbor == Left))
            Instantiate(wallLeft, transform);
    }

    public void ResetScore()
    {
        Parent = null;
        G = 0;
        H = 0;
    }

    public Vector2Int Up => Position + new Vector2Int(0, 1);
    public Vector2Int Down => Position + new Vector2Int(0, -1);
    public Vector2Int Right => Position + new Vector2Int(1, 0);
    public Vector2Int Left => Position + new Vector2Int(-1, 0);
}
