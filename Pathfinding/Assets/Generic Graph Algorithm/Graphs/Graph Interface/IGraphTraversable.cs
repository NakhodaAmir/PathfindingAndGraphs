using System.Collections.Generic;

public interface IGraphTraversable<Node, T>
    where Node : GraphNode<Node, T>
{
    public List<Node> GetNeighbourNodes(Node node);
}
