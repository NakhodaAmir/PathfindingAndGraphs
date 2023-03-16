using System.Collections.Generic;
using UnityEngine;

public class BreadthFirstSearchShortestPath<Node, T> : BreadthFirstSearch<Node, T>
    where Node : GraphNode<Node, T>
{
    public Node TargetNode { get; protected set; }

    public List<Node> PathList { get; protected set; }

    protected Dictionary<Node, Node> previousNode;

    public BreadthFirstSearchShortestPath(IGraphTraversable<Node, T> graph) : base(graph)
    {
        OnGetNeighbour = AddToPreviousDict;
        OnSuccess = ReconstructPath;

        previousNode = new Dictionary<Node, Node>();
        PathList = new List<Node>();
    }

    public bool Initialize(Node sourceNode, Node targetNode)
    {
        return Initialize(sourceNode, () =>
        {
            TargetNode = targetNode;

            previousNode.Add(sourceNode, null);
        });
    }

    protected override void Reset()
    {
        base.Reset();

        TargetNode = null;
        CurrentNode = null;

        PathList.Clear();
    }

    private void AddToPreviousDict(Node node, Node neighbour)
    {
        previousNode.Add(neighbour, node);
    }

    private void ReconstructPath()
    {
        List<Node> path = new List<Node>();

        for (Node node = TargetNode; node != null; node = previousNode[node])
        {
            path.Add(node);
        }

        path.Reverse();

        if (path[0] == SourceNode)
        {
            PathList = path;
        }
        else
        {
            PathList = null;
        }
    }


}
