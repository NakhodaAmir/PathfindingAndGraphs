using System;
using System.Collections.Generic;
using UnityEngine;

public class BreadthFirstSearch<Node, T> : GraphTraversalAlgorithm<Node, T>
    where Node : GraphNode<Node, T>
{
    public DelegateOnNode OnAddToQueue;
    public Queue<Node> Queue { get; protected set; }

    public BreadthFirstSearch(IGraphTraversable<Node, T> graph) : base(graph)
    {
        Queue = new Queue<Node>();
    }

    public bool Initialize(Node sourceNode, Action specificInitialization = null)
    {
        return BaseTraversalInitialization(sourceNode, () =>
        {
            Visited.Add(CurrentNode);
            OnVisited?.Invoke(CurrentNode);

            Queue.Enqueue(CurrentNode);
            OnAddToQueue?.Invoke(CurrentNode);

            specificInitialization?.Invoke();
        });
    }

    protected override AlgorithmStates SpecificStepImplementation()
    {
        if (Queue.Count == 0)
        {
            AlgorithmState = AlgorithmStates.SUCCEEDED;
            OnSuccess?.Invoke();
            return AlgorithmState;
        }

        CurrentNode = Queue.Dequeue();
        OnChangeCurrentNode?.Invoke(CurrentNode);

        foreach (Node neighbour in graph.GetNeighbourNodes(CurrentNode))
        {
            if (!Visited.Contains(neighbour))
            {
                Visited.Add(neighbour);
                OnVisited?.Invoke(neighbour);

                Queue.Enqueue(neighbour);
                OnAddToQueue?.Invoke(neighbour);

                OnGetNeighbour?.Invoke(CurrentNode, neighbour);
            }
        }

        return base.SpecificStepImplementation();
    }

    protected override void Reset()
    {
        base.Reset();

        Queue.Clear();
    }
}
