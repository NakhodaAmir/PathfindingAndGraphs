public class GreedyBestFirst<Node, T> : GraphSearchAlgorithm<Node, T>
    where Node : GraphSearchNode<Node, T>
{
    public GreedyBestFirst(IGraphSearchable<Node, T> graph, bool searchToSource = true) : base(graph, searchToSource) { }

    protected override void AlgorithmSpecificImplementation(Node neighbour)
    {
        float gCost = 0;
        float hCost = graph.HeuristicCost(TargetNode, neighbour);

        AlgorithmCommonImplementation(neighbour, gCost, hCost);
    }
}
