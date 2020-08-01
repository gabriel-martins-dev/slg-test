using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [Header("Maze Settings"), SerializeField] int mazeSize = 10;
    [SerializeField] Vector2Int startCellPosition = Vector2Int.zero;
    [SerializeField] Vector2Int endCellPosition = default;
    [SerializeField] Cell cellPrefab = default;

    [Header("Navigation Settings"), SerializeField] Navigation navigation = default;
    [SerializeField] Agent agent = default;

    private Cell[,] mazeCells;

    private void Awake()
    {
        mazeCells = new Cell[mazeSize, mazeSize];
        startCellPosition = Vector2Int.zero;
        endCellPosition = Vector2Int.one * (mazeSize - 1);
        GenerateMaze();
        SetupNavigation();
    }

    private void GenerateMaze()
    {
        GenerateCells(mazeSize);

        HashSet<Cell> visitedCells = new HashSet<Cell>();
        Stack<Cell> cellsStack = new Stack<Cell>();

        Cell startingCell = GetCell(startCellPosition);
        startingCell.AddNeighbor(new Vector2Int(0, -1));

        Cell endingCell = GetCell(endCellPosition);
        endingCell.AddNeighbor(new Vector2Int(0, 1));

        cellsStack.Push(GetCell(startCellPosition));

        while (cellsStack.Count > 0)
        {
            Cell currentCell = cellsStack.Pop();

            visitedCells.Add(currentCell);

            Cell[] unvisitedNeighbors = UnvisitedCells(CellNeighbors(currentCell), visitedCells);

            if (unvisitedNeighbors.Length > 0)
            {
                cellsStack.Push(currentCell);

                Cell nextCell = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Length)];

                currentCell.AddNeighbor(nextCell.Position - currentCell.Position);
                nextCell.AddNeighbor(currentCell.Position - nextCell.Position);

                visitedCells.Add(nextCell);
                cellsStack.Push(nextCell);
            }
        }

        for (int x = 0; x < mazeSize; x++)
        {
            for (int y = 0; y < mazeSize; y++)
            {
                mazeCells[x, y].BuildWalls();
            }
        }
    }

    private void GenerateCells(int size)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Cell cell = Instantiate(cellPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                cell.name = $"Cell ({x},{y})";
                cell.Position.Set(x,y);
                mazeCells[x, y] = cell;
            }
        }
    }

    public Cell GetCell(Vector2Int pos)
    {
        return mazeCells[pos.x, pos.y];
    }

    public bool TryGetCell(Vector2Int pos, out Cell cell)
    {
        if(pos.x >= 0 && pos.x < mazeSize
            && pos.y >= 0 && pos.y < mazeSize)
        {
            cell = GetCell(pos);
            return true;
        }
        cell = default;
        return false;
    }

    public List<Cell> CellNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();

        if (TryGetCell(cell.Up, out Cell cellRight))
            neighbors.Add(cellRight);

        if (TryGetCell(cell.Down, out Cell cellLeft))
            neighbors.Add(cellLeft);

        if (TryGetCell(cell.Right, out Cell cellUp))
            neighbors.Add(cellUp);

        if (TryGetCell(cell.Left, out Cell cellDown))
            neighbors.Add(cellDown);

        return neighbors;
    }

    public List<Cell> CellWalkableNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();

        foreach (var pos in cell.Neighbors)
        {
            if (TryGetCell(pos, out Cell neighborCell))
                neighbors.Add(neighborCell);
        }

        return neighbors;
    }

    public Cell[] UnvisitedCells(List<Cell> neighbors, HashSet<Cell> visited)
    {
        return neighbors.Where(cell => !visited.Any(visitor => visitor.Position == cell.Position)).ToArray();
    }

    private void SetupNavigation()
    {
        agent.Position = startCellPosition;
        agent.OnCellHit += CellHitHandler;

        if (TryGetCell(agent.Position, out Cell cell))
            agent.Teleport(cell.transform.position);
    }

    private void CellHitHandler(Agent agent, Cell cell)
    {
        agent.SetPath(navigation.TryGetPath(agent.Position, cell.Position, this));
    }
}
