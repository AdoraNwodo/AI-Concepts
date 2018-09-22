using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veurma.Utility.Graph
{
    public static class AdjacencyMatrixGraph
    {
        private static List<string> node_list;
        private static int edges, nodes;
        private static int[,] node_edge_node;
        private static string[] network =
        {
            "A,B",
            "A,C",
            "B,D",
            "B,E",
            "C,F",
            "D,G",
            "E,H",
            "F,H" ,
            "G,I",
            "H,J",
            "H,K",
            "I,L",
            "J,L",
            "K,M" ,
            "L,N",
            "M,N"
        };

        public static List<string> NodeList
        {
            get
            {
                init_graph();
                return node_list;
            }
        }

        public static string[] Network
        {
            get
            {
                return network;
            }
        }

        public static int[,] AdjacencyMatrix
        {
            get
            {
                init_graph();
                return node_edge_node;
            }
        }

        private static void init_graph()
        {
            edges = network.Length;
            nodes = 14;
            node_list = new List<string>();
            node_edge_node = new int[nodes, nodes];

            

            for (int i = 0; i < edges; i++)
            {
                string[] foundNode = network[i].Split(',');

                if (!node_list.Contains((string)foundNode[0]))
                {
                    node_list.Add((string)foundNode[0]);
                }

                if (!node_list.Contains((string)foundNode[1]))
                {
                    node_list.Add((string)foundNode[1]);
                }

                node_edge_node[node_list.IndexOf((string)foundNode[0]), node_list.IndexOf((string)foundNode[1])] = 1;
            }
        }
    }
}
