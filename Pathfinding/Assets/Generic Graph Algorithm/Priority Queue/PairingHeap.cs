#region Pairing Heap
public class PairingHeap<T> where T : IPairingHeap<T>
{
    #region Variables
    PairingHeapNode root;

    int size;

    readonly PairingHeapType type;
    #endregion

    #region Properties
    public bool IsEmpty
    {
        get { return size == 0; }
    }

    public T Peek
    {
        get { return root.Value; }
    }

    public int Count
    {
        get { return size; }
    }
    #endregion

    #region Constructor
    public PairingHeap(PairingHeapType type = PairingHeapType.MIN)
    {
        root = null;
        size = 0;

        this.type = type;
    }
    #endregion

    #region PairingHeapNode
    public class PairingHeapNode
    {
        public T Value { get; set; }
        public PairingHeapNode LeftChild { get; set; }
        public PairingHeapNode RightSibling { get; set; }
        public PairingHeapNode Parent { get; set; }
        public PairingHeap<T> Heap { get; set; }

        public PairingHeapNode(T value, PairingHeap<T> heap)
        {
            Value = value;

            LeftChild = RightSibling = Parent = null;

            Heap = heap;

            Value.HeapNode = this;
        }

        public void AddChild(PairingHeapNode node)//adds a child to this node
        {
            node.Parent = this;
            node.RightSibling = LeftChild;
            LeftChild = node;
        }

        public void Clean()
        {
            LeftChild = null;
            RightSibling = null;
            Parent = null;
            Heap = null;
            Value.HeapNode = null;
        }

        public void Cut()//Remove this node and its children from this node's parent
        {
            var n = Parent;

            if (Equals(n.LeftChild, this))
            {
                n.LeftChild = RightSibling;
            }
            else
            {
                n = n.LeftChild;

                while (!Equals(n.RightSibling, this)) n = n.RightSibling;

                n.RightSibling = RightSibling;
            }

            Parent = null;
            RightSibling = null;
        }

        public PairingHeapNode Extract()//Link this node's children as a prelude to this node's removal
        {
            if (LeftChild == null) return null;

            PairingHeapNode result = null;

            var n = LeftChild;

            while (n != null)
            {
                if (n.RightSibling == null) return Heap.Link(result, n);

                var pair = n.RightSibling;
                n.Parent = null;
                n.RightSibling = null;

                var next = pair.RightSibling;
                pair.Parent = null;
                pair.RightSibling = null;

                result = Heap.Link(result, Heap.Link(n, pair));

                n = next;
            }

            return result;
        }
    }
    #endregion

    #region Public Methods
    public PairingHeapNode Insert(T item)
    {
        var node = new PairingHeapNode(item, this);

        if (root == null)
        {
            root = node;
        }
        else
        {
            root = Link(root, node);
        }

        size++;
        return node;
    }

    public void Meld(PairingHeap<T> other)
    {
        root = Link(other.root, root);
        size += other.size;

        other.size = 0;
        other.root = null;
    }

    public void Update(T item)
    {
        var node = item.HeapNode;

        if (Equals(root, node)) return;

        node.Cut();

        root = Link(node, root);
    }

    public T ExtractMin()
    {
        var oldRoot = root;
        root = root.Extract();
        size -= 1;

        oldRoot.Clean();
        return oldRoot.Value;
    }

    public bool Contains(T item)
    {
        if (item.HeapNode != null && item.HeapNode.Heap == this)
        {
            return Equals(item, item.HeapNode.Value);
        }

        return false;
    }
    #endregion

    #region Private Methods
    int Compare(PairingHeapNode nodeA, PairingHeapNode nodeB)
    {
        int c = nodeA.Value.CompareTo(nodeB.Value);

        if (type == PairingHeapType.MIN)
        {
            return c;
        }
        else
        {
            return -c;
        }
    }

    PairingHeapNode Link(PairingHeapNode nodeA, PairingHeapNode nodeB)
    {
        if (nodeA == null) return nodeB;
        if (nodeB == null) return nodeA;

        if (Compare(nodeA, nodeB) > 0)
        {
            nodeB.AddChild(nodeA);
            return nodeB;
        }
        else
        {
            nodeA.AddChild(nodeB);
            return nodeA;
        }
    }
    #endregion
}
#endregion

#region Pairing Heap Interface
public interface IPairingHeap<T> : System.IComparable<T> where T : IPairingHeap<T>
{
    PairingHeap<T>.PairingHeapNode HeapNode { get; set; }
}
#endregion

#region Enum
public enum PairingHeapType
{
    MIN,
    MAX
}
#endregion


