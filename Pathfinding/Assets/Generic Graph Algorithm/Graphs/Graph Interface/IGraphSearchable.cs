public interface IGraphSearchable<Node, T> : IGraphTraversable<Node, T>
    where Node : GraphSearchNode<Node, T>
{
    public float NodeTraversalCost(Node nodeA, Node nodeB);
    public float HeuristicCost(Node nodeA, Node nodeB);    
}
