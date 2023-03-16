
using System.Collections.Generic;

public class DepthFirstSearch<Node, T> : GraphTraversalAlgorithm<Node, T>
    where Node : GraphNode<Node, T>
{
    public DelegateOnNode OnAddToStack;

    public Stack<Node> Stack { get; protected set; }

    public DepthFirstSearch(IGraphTraversable<Node, T> graph) : base(graph)
    {
        Stack = new Stack<Node>();
    }

    public bool Initialize(Node sourceNode)
    {
        return BaseTraversalInitialization(sourceNode, () =>
        {
            Stack.Push(CurrentNode);
        });
    }

    protected override AlgorithmStates SpecificStepImplementation()
    {
        if (Stack.Count == 0)
        {
            AlgorithmState = AlgorithmStates.SUCCEEDED;
            OnSuccess?.Invoke();
            return AlgorithmState;
        }

        CurrentNode = Stack.Pop();
        OnChangeCurrentNode?.Invoke(CurrentNode);

        if (!Visited.Contains(CurrentNode))
        {
            Visited.Add(CurrentNode);
            OnVisited?.Invoke(CurrentNode);
        }

        foreach (Node neighbour in graph.GetNeighbourNodes(CurrentNode))
        {
            if (!Visited.Contains(neighbour))
            {
                Stack.Push(neighbour);
                OnAddToStack?.Invoke(neighbour);

                OnGetNeighbour?.Invoke(CurrentNode, neighbour);
            }
        }

        return base.SpecificStepImplementation();
    }

    protected override void Reset()
    {
        base.Reset();

        Stack.Clear(); 
    }
}
