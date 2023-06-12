using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab4
{
    internal class Generator
    {
        /// <summary>
        /// Генерирует граф по входным параметрам
        /// </summary>
        /// <param name="minCountNode"></param>
        /// <param name="maxCountNode"></param>
        /// <param name="minCountEdge"></param>
        /// <param name="maxCountEdge"></param>
        /// <param name="maxEgdesAboveNode"></param>
        /// <param name="directional"></param>
        /// <param name="maxOutEdges"></param>
        /// <param name="maxInEdges"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Graph GenerateGraph(int minCountNode, int maxCountNode,
            int minCountEdge, int maxCountEdge,
            int maxEgdesAboveNode, bool directional,
            int maxOutEdges, int maxInEdges)
        {
            var random = new Random();
            
            int countOfNodes = random.Next(minCountNode, maxCountNode + 1);
            int countOfEdges = random.Next(minCountEdge, maxCountEdge + 1);
            var matrix = new int[countOfNodes, countOfNodes];
            var edges = new List<(int, int)>();

            //check allowable count of edges
            if (countOfEdges > Enumerable.Range(1, countOfEdges).Sum())
            {
                throw new ArgumentException("too many edges");
            }

            //check allowable count of edge above one node
            if (countOfEdges - 1 < maxEgdesAboveNode)
            {
                throw new ArgumentException("too many egdes above node");
            }

            //generate adjacency matrix
            while (true)
            {
                //generate edge
                var nodes = (random.Next(countOfNodes), random.Next(countOfNodes));

                if (directional && matrix[nodes.Item2, nodes.Item1] != 0)
                    continue;

                if (matrix[nodes.Item1, nodes.Item2] != 0 
                    || matrix[nodes.Item2, nodes.Item1] != 0)
                    continue;
                
                //count in and out edges
                var inEdges = edges.Select(e => e).Where(e => e.Item2 == nodes.Item1).Count();
                var outEdges = edges.Select(e => e).Where(e => e.Item1 == nodes.Item2).Count();

                //count edges above one node
                int countFirstNode = 0;
                int countSecondNode = 0;
                for (int i = 0; i < countOfNodes; i++)
                {
                    if (matrix[i, nodes.Item1] != 0)
                        countFirstNode++;

                    if (matrix[i, nodes.Item2] != 0)
                        countSecondNode++;
                }

                //check count of out, in edges and edges above one node
                if (directional && (outEdges > maxOutEdges || inEdges > maxInEdges)
                    || countFirstNode > maxEgdesAboveNode || countSecondNode > maxEgdesAboveNode)
                    continue;

                //add nodes to matrix
                if (directional)
                {
                    matrix[nodes.Item1, nodes.Item2] = 1;
                }
                else
                {
                    matrix[nodes.Item1, nodes.Item2] = 1;
                    matrix[nodes.Item2, nodes.Item1] = 1;
                }

                edges.Add(nodes);

                if (edges.Count == countOfEdges)
                    break;
            }

            var nodeList = new List<int>();
            foreach (var node in edges)
            {
                nodeList.Add(node.Item1);
                nodeList.Add(node.Item2);
            }

            return Graph.MakeGraph(directional, nodeList);
        }
    }
}
