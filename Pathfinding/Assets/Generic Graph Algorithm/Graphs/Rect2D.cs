using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rect2D : MonoBehaviour, IGraphSearchable<Rect2DNode, Vector2Int>
{
    public bool DebugGraph;

    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Rect2DNode[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    public Transform seeker;
    public Transform target;

    GreedyBestFirst<Rect2DNode, Vector2Int> aStar;
    BreadthFirstSearchShortestPath<Rect2DNode, Vector2Int> bfs;

    public List<Rect2DNode> Path;

    void Start()
    {
        Path = new List<Rect2DNode>();

        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();

        aStar = new GreedyBestFirst<Rect2DNode, Vector2Int>(this);
        bfs = new BreadthFirstSearchShortestPath<Rect2DNode, Vector2Int>(this);
        
        StartCoroutine(FindPath());
    }

    IEnumerator FindPath()
    {
        aStar.Initialize(NodeFromWorldPoint(seeker.position), NodeFromWorldPoint(target.position));

        while (aStar.IsRunning)
        {
            aStar.Step();
            yield return new WaitForSeconds(0.1f);
        }

        if (aStar.IsSucceeded)
        {
            Path = aStar.PathList;
        }
        else
        {
            print("No Path");
        }

        //bfs.Initialize(NodeFromWorldPoint(seeker.position), NodeFromWorldPoint(target.position));

        //while (bfs.IsRunning)
        //{
        //    bfs.Step();
        //    yield return new WaitForSeconds(0.01f);
        //}
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid()
    {
        grid = new Rect2DNode[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
                grid[x, y] = new Rect2DNode(new Vector2Int(x, y), worldPoint, walkable);
            }
        }
    }

    public Rect2DNode NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.Clamp(Mathf.FloorToInt(gridSizeX * percentX), 0, gridSizeX - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(gridSizeY * percentY), 0, gridSizeY - 1);
        return grid[x, y];
    }


    public List<Rect2DNode> GetNeighbourNodes(Rect2DNode node)
    {
        List<Rect2DNode> neighbours = new List<Rect2DNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.Value.x + x;
                int checkY = node.Value.y + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && grid[checkX, checkY].walkable)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public float NodeTraversalCost(Rect2DNode nodeA, Rect2DNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.Value.x - nodeB.Value.x);
        int dstY = Mathf.Abs(nodeA.Value.y - nodeB.Value.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    public float HeuristicCost(Rect2DNode nodeA, Rect2DNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.Value.x - nodeB.Value.x);
        int dstY = Mathf.Abs(nodeA.Value.y - nodeB.Value.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 0.1f, gridWorldSize.y));

        if(DebugGraph)
        {
            if (grid != null)
            {
                foreach (Rect2DNode n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;

                    if (aStar.OpenList != null)
                    {
                        if (aStar.OpenList.Contains(n))
                        {
                            Gizmos.color = Color.cyan;
                        }
                    }

                    if (aStar.ClosedList != null)
                    {
                        if (aStar.ClosedList.Contains(n))
                        {
                            Gizmos.color = Color.yellow;
                        }
                    }

                    if (aStar.CurrentNode == n) Gizmos.color = Color.blue;

                    if (Path != null)
                    {
                        if (Path.Contains(n))
                        {
                            Gizmos.color = Color.green;
                        }
                    }

                    if (bfs.Visited != null)
                    {
                        if (bfs.Visited.Contains(n))
                        {
                            Gizmos.color = Color.yellow;
                        }
                    }

                    if (bfs.Queue != null)
                    {
                        if (bfs.Queue.Contains(n))
                        {
                            Gizmos.color = Color.cyan;
                        }
                    }

                    if (bfs.CurrentNode == n) Gizmos.color = Color.blue;

                    if (bfs.PathList != null)
                    {
                        if (bfs.PathList.Contains(n))
                        {
                            Gizmos.color = Color.green;
                        }
                    }

                    Gizmos.DrawCube(n.WorldPoint, new Vector3(nodeDiameter - 0.1f, 0.1f, nodeDiameter - 0.1f));
                }    
            }
        }
    }


}
public class Rect2DNode : GraphSearchNode<Rect2DNode, Vector2Int>
{
    public Vector3 WorldPoint { get; set; }
    public bool walkable;
    public Rect2DNode(Vector2Int value, Vector3 worldPoint, bool walkable) : base(value)
    {
        WorldPoint = worldPoint;
        this.walkable = walkable;
    }
}
