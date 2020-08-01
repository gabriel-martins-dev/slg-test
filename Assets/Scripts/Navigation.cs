using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    public List<Cell> TryGetPath(Vector2Int start, Vector2Int end, Maze maze)
    {
        HashSet<Cell> openNodes = new HashSet<Cell>();
        HashSet<Cell> closedNodes = new HashSet<Cell>();

        Cell startCell = maze.GetCell(start);
        startCell.ResetScore();

        openNodes.Add(startCell);

        while (openNodes.Count > 0)
        {
            float minF = openNodes.Min(node => node.F);

            Cell currentNode = openNodes.First(n => n.F == minF);

            if (currentNode.Position == end)
            {
                return BacktrackPath(start, currentNode);
            }

            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            Cell[] availableNodes = maze.UnvisitedCells(maze.CellWalkableNeighbors(currentNode), closedNodes);

            foreach (Cell node in availableNodes)
            {
                float gScore = currentNode.G + 1;

                if (!openNodes.Contains(node))
                {
                    node.ResetScore();

                    node.Parent = currentNode;
                    node.G = gScore;
                    node.H = Vector2Int.Distance(node.Position, currentNode.Position);

                    openNodes.Add(node);
                }
                else if(gScore < node.G)
                {
                    node.Parent = currentNode;
                    node.G = gScore;
                }
            }
        }

        return null;
    }

    private List<Cell> BacktrackPath(Vector2Int start, Cell end)
    {
        List<Cell> path = new List<Cell>();
        Cell current = end;

        while (current.Position != start)
        {
            path.Add(current);
            current = current.Parent;
        }

        path.Reverse();
        return path;
    }
}
