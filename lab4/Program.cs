//Максимальное / Минимальное количество генерируемых вершин
//Максимальное/Минимальное количество генерируемых ребер
//Максимальное количество ребер связанных с одной вершины
//Генерируется ли направленный граф
//Максимальное количество входящи и выходящих ребер

using lab4;
using System.Diagnostics;

var random = new Random();
int minCountNode = 10;
int maxCountNode = 15;
int minCountEdge = 25;
int maxCountEdge = 35;
int maxEgdeAboveNode = 7;
bool isDirectional = random.Next(2) == 0;
int maxInEdges = 7;
int maxOutEdges = 7;

var graph = new Graph();
using (StreamWriter writer = new StreamWriter("result.csv"))
{
    Stopwatch sw = new Stopwatch();
    writer.Write("Count nodes;Depth search;BreadthSearch\n");
    for (int i = 0; i < 10; i++)
    {
        int from = 0;
        int to = 0;

        graph = Generator.GenerateGraph(minCountNode, maxCountNode, minCountEdge, maxCountEdge,
            maxEgdeAboveNode, isDirectional, maxOutEdges, maxInEdges);
        do
        {
            from = random.Next(graph.Length);
            to = random.Next(graph.Length);
        }
        while (from == to);

        //depth search 
        writer.Write(graph.Length + ";");
        sw.Start();
        var result = graph[from].DepthSearch(to);
        sw.Stop();
        var listResult = result.Select(node => node.NodeNumber).ToList();
        if (!listResult.Contains(to))
        {
            writer.Write("null;");
        }
        else
        {
            writer.Write(sw.Elapsed + ";");
        }

        //breadth search
        sw.Restart();
        result = graph[from].BreadthSearch(to).ToList();
        sw.Stop();
        listResult = result.Select(node => node.NodeNumber).ToList();
        if (!listResult.Contains(to))
        {
            writer.WriteLine("null;");
        }
        else
        {
            writer.WriteLine(sw.Elapsed + ";");
        }

        minCountNode += 15;
        maxCountNode += 15;
        minCountEdge += 15;
        maxCountEdge += 15;
        maxEgdeAboveNode += 15;
        isDirectional = random.Next(2) == 0;
        maxInEdges   += 7;
        maxOutEdges  += 7;
    }
}


void TestGraph()
{
    var graph = Generator.GenerateGraph(4, 10, 2, 4, 3, true, 2, 4);
    PrintInformationAboutGraph(graph);
}


static void PrintInformationAboutGraph(Graph graph)
{
    var adjacencyMatrix = graph.MakeAdjacencyMatrix();

    for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
    {
        for (int j = 0; j < adjacencyMatrix.GetLength(1); j++)
        {
            Console.Write(adjacencyMatrix[i, j] + " ");
        }
        Console.WriteLine();
    }


    Console.WriteLine();
    var incidenceMatrix = graph.MakeIncidenceMatrix();
    for (int i = 0; i < incidenceMatrix.GetLength(0); i++)
    {
        for (int j = 0; j < incidenceMatrix.GetLength(1); j++)
        {
            Console.Write(incidenceMatrix[i, j] + " ");
        }
        Console.WriteLine();
    }

    Console.WriteLine("список смежности");
    var adjacencyList = graph.MakeAdjacencyList();
    foreach (var node in adjacencyList)
    {
        Console.Write(node.Item1 + ":" + node.Item2 + "; ");
    }

    Console.WriteLine("\nсписко ребер");
    var edgeList = graph.Edges;
    foreach (var edge in edgeList)
    {
        Console.Write(edge.From.NodeNumber + ":" + edge.To.NodeNumber + "; ");
    }
    Console.WriteLine();

    Console.Write("Depth Search: ");
    Console.WriteLine(graph[0]
                .DepthSearch(2)
                .Select(z => z.NodeNumber.ToString())
                .Aggregate((a, b) => a + " " + b));

    Console.Write("Breadth Search: ");
    Console.WriteLine(graph[0]
        .BreadthSearch(3)
        .Select(z => z.NodeNumber.ToString())
        .Aggregate((a, b) => a + " " + b));
}

public static class NodeExtensions
{
    /// <summary>
    /// Алгоритм поиска в глубину
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNodeNumber"></param>
    /// <returns>Путь до заданной вершины</returns>
    public static IEnumerable<Node> DepthSearch(this Node startNode, int endNodeNumber)
    {
        var visited = new HashSet<Node>();
        var stack = new Stack<Node>();
        stack.Push(startNode);
        while (stack.Count != 0)
        {
            var node = stack.Pop();

            if (node.NodeNumber == endNodeNumber)
            {
                yield return node;
                yield break;
            }

            if (visited.Contains(node)) continue;
            visited.Add(node);
            yield return node;
            foreach (var incidentNode in node.IncidentNodes)
                stack.Push(incidentNode);
        }
    }

    /// <summary>
    /// Реализует алгоритм поиска в ширину
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNodeNumber"></param>
    /// <returns>Путь до заданной вершины</returns>
    public static IEnumerable<Node> BreadthSearch(this Node startNode, int endNodeNumber)
    {
        var visited = new HashSet<Node>();
        var queue = new Queue<Node>();
        queue.Enqueue(startNode);
        while (queue.Count != 0)
        {
            var node = queue.Dequeue();

            if (node.NodeNumber == endNodeNumber)
            {
                yield return node;
                yield break;
            }

            if (visited.Contains(node)) continue;
            visited.Add(node);
            yield return node;
            foreach (var incidentNode in node.IncidentNodes)
                queue.Enqueue(incidentNode);
        }
    }
}