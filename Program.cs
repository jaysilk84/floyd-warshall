using System;
using System.Linq;
using System.Collections.Generic;

namespace floyd_warshall
{
    class Program
    {
        /*  

                                1203
                                 /\
                                 ||
                                 \/       
            1200 <--> 1201 <--> 1202 <--> 1206 <--> 1205
                                 /\
                                 ||
                                 \/
                                1204
        
        */

        static int INF = 99;
        static int[] vertices = new int[] { 1200, 1201, 1202, 1203, 1204, 1205, 1206 };
        static int[,] edges = new int[,] {
            {1200, 1201},
            {1201, 1202},
            {1202, 1206},
            {1206, 1205},
            {1202, 1203},
            {1202, 1204},
            {1201, 1200},
            {1202, 1201},
            {1206, 1202},
            {1205, 1206},
            {1203, 1202},
            {1204, 1202},
            //{1200, 1205},
            //{1205, 1200}
            //{1203, 1207}
        };

        static void Main(string[] args)
        {
            var a = BuildAdjacencyMatrix(vertices, edges);
            var d = InitDistanceMatrix(a);

            PrintMatrix(a);
            PrintMatrix(d);

            CalcShortestDistances(d); // mutates d

            PrintMatrix(d);

            var e = GetEccentricityByNode(d);
            var m = GetRadiusAndDiameter(e);

            Console.Write("Eccentricity:  ");
            for (var i = 0; i < vertices.Length; i++)
                Console.Write($"{vertices[i]}: {e[i]}  ");
            Console.WriteLine();

            Console.WriteLine($"Radius: {m.Item1}  Diameter: {m.Item2}");

            var c = GetCenter(e, m.Item1);

            Console.Write("Center ");
            foreach (var n in c)
                Console.Write(n + "  ");

            Console.WriteLine();
        }
        /// <summary>
        /// Finds the center of the graph by finding all nodes with an eccentricity that equals the diameter
        /// </summary>
        /// <param name="e"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        static List<int> GetCenter(int[] e, int radius)
        {
            return e.Select((x, i) => (x, vertices[i])).Where(x => x.x == radius).Select(x => x.Item2).ToList();
        }

        /// <summary>
        /// Given a distance matrix, find the max distance for each node
        /// </summary>
        /// <param name="distanceMatrix"></param>
        /// <returns></returns>
        static int[] GetEccentricityByNode(int[,] distanceMatrix)
        {
            var size = vertices.Length;
            var e = new int[size];

            // for each node (i), walk through all the destinations (j) and find the max
            for (var i = 0; i < size; i++)
                for (var j = 0; j < size; j++)
                    // TODO: What happens when e[i] is zero due to a destination node with no egress?
                    e[i] = Math.Max(e[i], distanceMatrix[i, j]);

            return e;
        }

        /// <summary>
        /// Calculate the radius and diameter from the node eccentricity.
        /// Radius == node with the smallest eccentricity
        /// Diameter == node with the highest eccentricity
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        static (int, int) GetRadiusAndDiameter(int[] e)
        {
            var rad = INF;
            var diam = 0;

            for (var i = 0; i < vertices.Length; i++)
            {
                rad = Math.Min(rad, e[i]); // node with the smallest max distance
                diam = Math.Max(diam, e[i]); // node with the largest max distance
            }

            return (rad, diam);
        }

        static int[,] InitDistanceMatrix(int[,] aMatrix)
        {
            var size = vertices.Length;
            var d = new int[size, size];

            for (var j = 0; j < size; j++)
                for (var k = 0; k < size; k++)
                    d[j, k] = aMatrix[j, k];

            return d;
        }

        /// <summary>
        /// Use Floyd-Warshalls algorthim for finding the min distance
        /// for each vertices pair
        /// </summary>
        /// <param name="distanceMatrix"></param>
        static void CalcShortestDistances(int[,] distanceMatrix)
        {
            var size = vertices.Length;

            // Floyd-Warshalls algorithm
            for (var k = 0; k < size; k++)
                for (var i = 0; i < size; i++)
                    for (var j = 0; j < size; j++)
                        distanceMatrix[i, j] = Math.Min(distanceMatrix[i, j], distanceMatrix[i, k] + distanceMatrix[k, j]);
        }

        static int[,] BuildAdjacencyMatrix(int[] vertices, int[,] edges)
        {
            var size = vertices.Length;
            var matrix = new int[size, size];
            var sortedVertices = (vertices.Clone() as int[]).OrderBy(x => x); // don't mutate

            for (var j = 0; j < size; j++)
                for (var k = 0; k < size; k++)
                    if (j == k)
                        matrix[j, k] = 0;
                    else
                        matrix[j, k] = IsAdjacent(1200 + j, 1200 + k) ? 1 : INF;


            return matrix;
        }

        static void PrintMatrix(int[,] matrix)
        {
            var size = vertices.Length;

            Console.Write("   ");
            foreach (var n in vertices)
                Console.Write(n % 100 + "  ");

            Console.WriteLine(" ");

            for (var j = 0; j < size; j++)
            {
                Console.Write(vertices[j] % 100 + "  ");
                for (var k = 0; k < size; k++)
                {
                    Console.Write(GetText(matrix[j, k]) + "  ");
                }
                Console.WriteLine(" ");
            }

            Console.WriteLine();
        }

        static string GetText(int val)
        {
            return val == INF ? "X" : val.ToString();
        }

        static bool IsAdjacent(int source, int destination)
        {
            for (var j = 0; j < edges.GetLength(0); j++)
                if (edges[j, 0] == source && edges[j, 1] == destination)
                    return true;
            return false;
        }

    }
}
