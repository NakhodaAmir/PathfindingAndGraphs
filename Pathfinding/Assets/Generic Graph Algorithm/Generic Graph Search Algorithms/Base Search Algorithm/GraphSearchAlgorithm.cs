using System;
using System.Collections.Generic;

public abstract class GraphSearchAlgorithm<Node, T> : GraphAlgorithm<Node, T>
    where Node : GraphSearchNode<Node, T>
{
    public Node TargetNode { get; protected set; }
    public List<Node> PathList { get; protected set; }

    public DelegateOnStatus OnFailed { get; set; }

    public DelegateOnNode OnAddToOpenList;
    public DelegateOnNode OnAddToClosedList;

    protected IGraphSearchable<Node, T> graph;
    protected bool searchToSource;

    public PairingHeap<Node> OpenList { get; protected set; }
    public HashSet<Node> ClosedList { get; protected set; }

    public GraphSearchAlgorithm(IGraphSearchable<Node, T> graph, bool searchToSource = true) : base()
    {
        this.graph = graph;

        TargetNode = null;

        OpenList = new PairingHeap<Node>();
        ClosedList = new HashSet<Node>();

        PathList = new List<Node>();

        this.searchToSource = searchToSource;
    }

    public bool Initialize(Node sourceNode, Node targetNode)
    {
        return BaseInitialization(sourceNode, () =>
        {
            TargetNode = targetNode;

            SourceNode.Parent = null;
            SourceNode.Gcost = 0;
            SourceNode.Hcost = graph.HeuristicCost(sourceNode, targetNode);

            CurrentNode = SourceNode;
            OnChangeCurrentNode?.Invoke(CurrentNode);

            OpenList.Insert(CurrentNode);
            OnAddToOpenList?.Invoke(CurrentNode);
        });
    }

    protected override AlgorithmStates SpecificStepImplementation()
    {
        ClosedList.Add(CurrentNode);
        OnAddToClosedList?.Invoke(CurrentNode);

        if (OpenList.IsEmpty)
        {
            AlgorithmState = AlgorithmStates.FAILED;
            OnFailed?.Invoke();
            return AlgorithmState;
        }

        CurrentNode = OpenList.ExtractMin();
        OnChangeCurrentNode?.Invoke(CurrentNode);

        if (CurrentNode.Equals(TargetNode))
        {
            AlgorithmState = AlgorithmStates.SUCCEEDED;
            RetracePath();
            OnSuccess?.Invoke();
            return AlgorithmState;
        }

        foreach (Node neighbour in graph.GetNeighbourNodes(CurrentNode))
        {
            if (ClosedList.Contains(neighbour)) continue;

            OnGetNeighbour?.Invoke(CurrentNode, neighbour);

            AlgorithmSpecificImplementation(neighbour);
        }

        return base.SpecificStepImplementation();
    }

    protected abstract void AlgorithmSpecificImplementation(Node neighbour);

    protected void AlgorithmCommonImplementation(Node neighbour, float gCost, float hCost)
    {
        if (gCost < neighbour.Gcost || !OpenList.Contains(neighbour))
        {
            neighbour.Gcost = gCost;
            neighbour.Hcost = hCost;
            neighbour.Parent = CurrentNode;

            if (!OpenList.Contains(neighbour))
            {
                OpenList.Insert(neighbour);
                OnAddToOpenList?.Invoke(CurrentNode);
            }
            else
            {
                OpenList.Update(neighbour);
            }
        }
    }

    private void RetracePath()
    {
        var currentNode = CurrentNode;

        while (!Equals(currentNode, searchToSource ? null : SourceNode))
        {
            PathList.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        PathList.Reverse();
    }

    protected override void Reset()
    {
        base.Reset();

        TargetNode = null;
        CurrentNode = null;

        OpenList = new PairingHeap<Node>();
        ClosedList.Clear();

        PathList.Clear();
    }
}
