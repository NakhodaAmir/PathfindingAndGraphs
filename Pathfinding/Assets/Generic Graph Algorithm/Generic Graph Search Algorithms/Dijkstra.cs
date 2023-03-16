public class Dijkstra<Node, T> : GraphSearchAlgorithm<Node, T>
    where Node : GraphSearchNode<Node, T>
{
    public Dijkstra(IGraphSearchable<Node, T> graph, bool searchToSource = true) : base(graph, searchToSource) { }

    protected override void AlgorithmSpecificImplementation(Node neighbour)
    {
        float gCost = CurrentNode.Gcost + graph.NodeTraversalCost(CurrentNode, neighbour);
        float hCost = 0;

        AlgorithmCommonImplementation(neighbour, gCost, hCost);
    }
}
