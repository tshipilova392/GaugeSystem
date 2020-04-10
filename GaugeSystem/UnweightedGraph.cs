using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaugeSystem
{
    public class Graph<T>
    {
        private List<Edge> edges = new List<Edge>();
        private List<Node> nodes = new List<Node>();
        private Dictionary<T, Node> TtoNode = new Dictionary<T, Node>();
        private Dictionary<Node, T> NodetoT = new Dictionary<Node, T>();
        private int countNodes;

        public Graph()
        {
            countNodes = 0;
        }

        public Node GetNodeFromDictionary(T tmp)
        {
            return TtoNode[tmp];
        }

        public T GetObjectByNode(Node n)
        {
            return NodetoT[n];
        }

        public List<Node> GetAllNodes()
        {
            return nodes;
        }

        public bool HasNode(T element)
        {
            return TtoNode.ContainsKey(element);
        }

        public IEnumerable<T> BreadthSearch(T start)
        {
            var visited = new HashSet<Node>();
            var queue = new Queue<Node>();
            Node startNode = TtoNode[start];
            queue.Enqueue(startNode);
            while (queue.Count != 0)
            {
                var node = queue.Dequeue();
                if (visited.Contains(node)) continue;
                visited.Add(node);
                yield return NodetoT[node];//graph.GetObjectByNode(node);

                foreach (var incidentEdge in node.incomingEdges)
                {
                    queue.Enqueue(incidentEdge.From);
                }
            }
        }

        public void AddNode(T value)
        {
            if (!TtoNode.ContainsKey(value))
            {
                Node tmpNode = new Node(countNodes);
                TtoNode.Add(value, tmpNode);
                NodetoT.Add(tmpNode, value);
                countNodes++;
                nodes.Add(tmpNode);
            }
        }

        public void AddEdge(T from, T to)
        {
            if (!TtoNode.ContainsKey(from))
            {
                Node tmpNode = new Node(countNodes);
                TtoNode.Add(from, tmpNode);
                NodetoT.Add(tmpNode, from);
                countNodes++;
                nodes.Add(tmpNode);
            }

            if (!TtoNode.ContainsKey(to))
            {
                Node tmpNode = new Node(countNodes);
                TtoNode.Add(to, tmpNode);
                NodetoT.Add(tmpNode, to);
                countNodes++;
                nodes.Add(tmpNode);
            }

            Edge tmpEdge = new Edge(TtoNode[from], TtoNode[to]);
            edges.Add(tmpEdge);
            TtoNode[from].outgoingEdges.Add(tmpEdge);
            TtoNode[to].incomingEdges.Add(tmpEdge);
        }

        public void RemoveEdge(T from, T to)
        {
            Node startNode = TtoNode[from];
            Node endNode = TtoNode[to];
            RemoveEdge(startNode, endNode);
        }

        private void RemoveEdge(Node startNode, Node endNode)
        {
            var tmpList = edges.Where(z => (z.From == startNode) & (z.To == endNode)).ToList();
            foreach (Edge e in tmpList)
            {
                edges.Remove(e);
                e.From.outgoingEdges.Remove(e);
                e.To.incomingEdges.Remove(e);
            }
        }

        public void RemoveNode(T n)
        {
            Node tmpNode = TtoNode[n];

            var tmpList = edges.Where(z => ((z.From == tmpNode) || (z.To == tmpNode))).ToList();

            foreach (Edge e in tmpList)
            {
                edges.Remove(e);
                e.From.outgoingEdges.Remove(e);
                e.To.incomingEdges.Remove(e);
            }

            nodes.Remove(tmpNode);
        }
    }

    public class Node
    {
        public int Id { get; private set; }
        public List<Edge> incomingEdges = new List<Edge>();
        public List<Edge> outgoingEdges = new List<Edge>();

        public Node(int number)
        {
            this.Id = number;
        }
    }

    public class Edge
    {
        public Node From { get; set; }
        public Node To { get; set; }
        public Edge(Node first, Node second)
        {
            this.From = first;
            this.To = second;
        }
    }
}
