using System;
using System.Collections.Generic;

public abstract class GraphTraversalAlgorithm<Node, T> : GraphAlgorithm<Node, T>
    where Node : GraphNode<Node, T>
{
    public DelegateOnNode OnVisited;

    protected IGraphTraversable<Node, T> graph;

    public HashSet<Node> Visited { get; protected set; }

    public GraphTraversalAlgorithm(IGraphTraversable<Node, T> graph) : base()
    {
        this.graph = graph;

        CurrentNode = null;

        Visited = new HashSet<Node>();
    }

    protected bool BaseTraversalInitialization(Node sourceNode, Action specificInitialization = null)
    {
        return BaseInitialization(sourceNode, () =>
        {
            CurrentNode = SourceNode;
            OnChangeCurrentNode?.Invoke(CurrentNode);

            specificInitialization?.Invoke();
        });
    }

    protected override void Reset()
    {
        base.Reset();

        CurrentNode = null;

        Visited.Clear();
    }
}
