using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Agent : MonoBehaviour
{
    public UnityAction<Agent, Cell> OnCellHit;
    public Vector2Int Position;
    [SerializeField] float speed = 4f;
    [SerializeField] List<Cell> path = new List<Cell>();
    [SerializeField] int currentPathCell = 0;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo))
            {
                if(hitInfo.transform.TryGetComponent(out Cell cell))
                {
                    OnCellHit?.Invoke(this, cell);
                }
            }
        }

        if(path != null && path.Count > 0)
        {
            Vector3 targetPosition = path[currentPathCell].transform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);

            if(Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                Position = path[currentPathCell].Position;

                currentPathCell++;

                if (currentPathCell >= path.Count)
                    path.Clear();
            }
        }
    }

    public void Teleport(Vector3 position)
    {
        transform.position = position;
    }

    public void SetPath(List<Cell> path)
    {
        currentPathCell = 0;
        this.path = path;
    }
}
