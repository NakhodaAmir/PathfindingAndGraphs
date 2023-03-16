using System;
public class GraphSearchNode<Node, T> : GraphNode<Node, T>, IPairingHeap<Node>, IEquatable<Node>
    where Node : GraphSearchNode<Node, T>
{
    public Node Parent { get; set; }
    public float Fcost { get { return Gcost + Hcost; } }
    public float Gcost { get; set; }
    public float Hcost { get; set; }
    public PairingHeap<Node>.PairingHeapNode HeapNode { get; set; }

    public GraphSearchNode(T value) : base (value) { }

    public int CompareTo(Node otherNode)
    {
        int compare = Fcost.CompareTo(otherNode.Fcost);

        if (compare == 0) compare = Hcost.CompareTo(otherNode.Hcost);

        return compare;
    }

    public bool Equals(Node otherNode)
    {
        if (Value.Equals(otherNode.Value)) return true;

        return false;
    }
}
