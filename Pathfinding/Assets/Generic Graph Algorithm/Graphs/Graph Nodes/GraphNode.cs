public class GraphNode<Node, T>
    where Node : GraphNode<Node, T>
{
    public T Value { get; private set; }

    public GraphNode(T value)
    {
        Value = value;
    }
}
