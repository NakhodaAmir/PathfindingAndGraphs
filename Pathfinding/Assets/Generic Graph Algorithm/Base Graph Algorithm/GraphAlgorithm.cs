using System;
using System.Collections;
using System.Collections.Generic;


public abstract class GraphAlgorithm<Node, T>
    where Node : GraphNode<Node, T>
{
    public AlgorithmStates AlgorithmState { get; protected set; }
    public Node SourceNode { get; protected set; }
    public Node CurrentNode { get; protected set; }

    public bool IsRunning { get { return AlgorithmState == AlgorithmStates.RUNNING; } }
    public bool IsNotInitialized { get { return AlgorithmState == AlgorithmStates.NOT_INITIALIZED; } }
    public bool IsSucceeded { get { return AlgorithmState == AlgorithmStates.SUCCEEDED; } }
    public bool IsFailed { get { return AlgorithmState == AlgorithmStates.FAILED; } }


    public delegate void DelegateOnNode(Node node);
    public DelegateOnNode OnChangeCurrentNode;

    public Action<Node, Node> OnGetNeighbour;

    public delegate void DelegateOnStatus();
    public DelegateOnStatus OnStart { get; set; }
    public DelegateOnStatus OnSuccess { get; set; }
    public DelegateOnStatus OnRunning { get; set; }

    public GraphAlgorithm()
    {
        SourceNode = null;
    }

    protected bool BaseInitialization(Node sourceNode, Action specificInitialization = null)
    {
        if (AlgorithmState == AlgorithmStates.RUNNING) return false;

        Reset();

        SourceNode = sourceNode;

        specificInitialization?.Invoke();

        AlgorithmState = AlgorithmStates.RUNNING;

        OnStart?.Invoke();

        return true;
    }

    public AlgorithmStates Step()
    {
        if (AlgorithmState != AlgorithmStates.RUNNING) return AlgorithmState;

        return SpecificStepImplementation();    
    }

    protected virtual AlgorithmStates SpecificStepImplementation()
    {
        AlgorithmState = AlgorithmStates.RUNNING;

        OnRunning?.Invoke();
        return AlgorithmState;
    }

    protected virtual void Reset()
    {
        AlgorithmState = AlgorithmStates.NOT_INITIALIZED;

        SourceNode = null;
    }
}
