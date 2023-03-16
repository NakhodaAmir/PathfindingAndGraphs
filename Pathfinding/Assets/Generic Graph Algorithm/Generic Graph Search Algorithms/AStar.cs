public class AStar<Node, T> : GraphSearchAlgorithm<Node, T>
    where Node : GraphSearchNode<Node, T>
{
    public AStar(IGraphSearchable<Node, T> graph, bool searchToSource = true) : base(graph, searchToSource) { }

    protected override void AlgorithmSpecificImplementation(Node neighbour)
    {
        float gCost = CurrentNode.Gcost + graph.NodeTraversalCost(CurrentNode, neighbour);
        float hCost = graph.HeuristicCost(TargetNode, neighbour);

        AlgorithmCommonImplementation(neighbour, gCost, hCost);
    }
}
