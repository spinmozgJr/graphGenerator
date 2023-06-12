using System.Runtime;
using System.Transactions;

public class Edge
{
    public readonly Node From;
    public readonly Node To;
    public Edge(Node first, Node second)
    {
        this.From = first;
        this.To = second;
    }

    /// <summary>
    /// Возвращает, инцидентна ли данная вершина, заданной
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public bool IsIncident(Node node)
    {
        return From == node || To == node;
    }

    /// <summary>
    /// Возвращает вершину на противоположном конце ребра
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public Node OtherNode(Node node)
    {
        if (!IsIncident(node)) throw new ArgumentException();
        if (From == node) return To;
        return From;
    }
}

public class Node
{
    readonly List<Edge> edges = new List<Edge>();
    public readonly int NodeNumber;

    public Node(int number)
    {
        NodeNumber = number;
    }

    /// <summary>
    /// Возвращает инцидентные узлы, данного узла
    /// </summary>
    public IEnumerable<Node> IncidentNodes
    {
        get
        {
            return edges.Select(z => z.OtherNode(this));
        }
    }

    /// <summary>
    /// Возвращает ребра, на которых находится данная вершина
    /// </summary>
    public IEnumerable<Edge> IncidentEdges
    {
        get
        {
            foreach (var e in edges) yield return e;
        }
    }

    /// <summary>
    /// Соединяет узлы графа в определенном направлении
    /// </summary>
    /// <param name="node1"> Первый узел </param>
    /// <param name="node2"> Второй узел </param>
    /// <param name="graph"> Граф, в котором надо соединить </param>
    /// <returns>Возвращает сформированное ребро</returns>
    /// <exception cref="ArgumentException"></exception>
    public static Edge DirectionalConnect(Node node1, Node node2, Graph graph)
    {
        if (!graph.Nodes.Contains(node1) || !graph.Nodes.Contains(node2)) throw new ArgumentException();
        var edge = new Edge(node1, node2);
        node1.edges.Add(edge);
        return edge;
    }

    /// <summary>
    /// Соединяет узлы графа в двух направлениях
    /// </summary>
    /// <param name="node1"> Первый узел </param>
    /// <param name="node2"> Второй узел </param>
    /// <param name="graph"> Граф, в котором надо соединить </param>
    /// <returns>Возвращает сформированное ребро</returns>
    /// <exception cref="ArgumentException"></exception>
    public static Edge NonDirectionalConnect(Node node1, Node node2, Graph graph)
    {
        if (!graph.Nodes.Contains(node1) || !graph.Nodes.Contains(node2)) throw new ArgumentException();
        var edge = new Edge(node1, node2);
        node1.edges.Add(edge);
        node2.edges.Add(edge);
        return edge;
    }   
}

public class Graph
{
    private Node[] nodes;

    public Graph() { }

    public Graph(int nodesCount)
    {
        nodes = Enumerable.Range(0, nodesCount).Select(z => new Node(z)).ToArray();
    }

    public int Length { get { return nodes.Length; } }

    public Node this[int index] { get { return nodes[index]; } }

    /// <summary>
    /// Возвращает список узлов графа
    /// </summary>
    public IEnumerable<Node> Nodes
    {
        get
        {
            foreach (var node in nodes) yield return node;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns>Возвращает матрицу смежности для графа</returns>
    public int[,] MakeAdjacencyMatrix()
    {
        var matrix = new int[Length, Length];
        for (int i = 0; i < Length; i++)
        {
            for (int j = 0; j < Length; j++)
            {
                if (nodes[i].IncidentNodes.Contains(nodes[j]))
                {
                    matrix[j, i] = 1;
                }
            }
        }

        return matrix;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Возвращает матрицу ицидентности для графа</returns>
    public int[,] MakeIncidenceMatrix()
    {
        var pairOfNodes = new List<(int, int)>();
        foreach (var node in nodes)
        {
            var edges = node.IncidentEdges;
            foreach (var edge in edges)
            {
                var reverseEdge = (edge.To.NodeNumber, edge.From.NodeNumber);
                if (pairOfNodes.Contains(reverseEdge))
                    continue;
                pairOfNodes.Add((edge.From.NodeNumber, edge.To.NodeNumber));
            }
        }

        var matrix = new int[Length, pairOfNodes.Count];
        for (int i = 0; i < pairOfNodes.Count; i++)
        {
            matrix[pairOfNodes[i].Item1, i] = 1;
            matrix[pairOfNodes[i].Item2, i] = 1;
        }

        return matrix;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Возвращает список смежности</returns>
    public List<(int, int)> MakeAdjacencyList()
    {
        var result = new List<(int, int)>();
        foreach (var node in nodes)
        {
            var incidentNodes = node.IncidentNodes;
            foreach (var incident in incidentNodes)
            {
                result.Add((node.NodeNumber, incident.NodeNumber));
            }
        }

        return result;
    }

    /// <summary>
    /// Возвращает список ребер графа
    /// </summary>
    /// <returns></returns>
    public List<Edge> MakeEdgeList()
    {
        var result = new List<Edge>();
        foreach (var node in nodes)
        {
            foreach (var incident in node.IncidentEdges)
            {
                result.Add(incident);
            }
        }

        return result;
    }

    /// <summary>
    /// Соединяет узлы в определенном напрадении
    /// </summary>
    /// <param name="index1"></param>
    /// <param name="index2"></param>
    public void DirectionalConnect(int index1, int index2)
    {
        Node.DirectionalConnect(nodes[index1], nodes[index2], this);
    }

    /// <summary>
    /// Соединяет узлы в обоих напрадениях
    /// </summary>
    /// <param name="index1"></param>
    /// <param name="index2"></param>
    public void NonDirectionalConnect(int index1, int index2)
    {
        Node.NonDirectionalConnect(nodes[index1], nodes[index2], this);
    }


    /// <summary>
    /// Список ребер графа
    /// </summary>
    public IEnumerable<Edge> Edges
    {
        get
        {
            return nodes.SelectMany(z => z.IncidentEdges).Distinct();
        }
    }

    /// <summary>
    /// Создает граф по списку узлов
    /// </summary>
    /// <param name="directional"></param>
    /// <param name="incidentNodes"></param>
    /// <returns>Возвращает сформированный граф</returns>
    //public static Graph MakeGraph(bool directional, params int[] incidentNodes)
    public static Graph MakeGraph(bool directional, List<int> incidentNodes)
    {
        var graph = new Graph(incidentNodes.Max() + 1);
        if (directional)
        {
            //for (int i = 0; i < incidentNodes.Length - 1; i += 2)
            for (int i = 0; i < incidentNodes.Count - 1; i += 2)
                graph.DirectionalConnect(incidentNodes[i], incidentNodes[i + 1]);
        }
        else
        {
            for (int i = 0; i < incidentNodes.Count - 1; i += 2)
                graph.NonDirectionalConnect(incidentNodes[i], incidentNodes[i + 1]);
        }

        return graph;
    }
}