using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veurma.Utility.Graph
{
    public static class WeightedDirectedGraph
    {
        private static List<Vertex<char>> vertexList;
        private static List<string> vertexTitleList;

        public static List<Vertex<char>> VertexList
        {
            get
            {
                generateGraphSamples();
                return vertexList;
            }
        }

        public static List<string> VertexTitleList
        {
            get
            {
                generateGraphSamples();
                foreach(Vertex<char> node in vertexList)
                {
                    vertexTitleList.Add(node.ToString());
                }

                return vertexTitleList;
            }
        }

        public static Vertex<char> getVertexFromTitle(string title)
        {
            generateGraphSamples();

            foreach(Vertex<char> vertex in vertexList)
            {
                if (vertex.ToString().Trim() == title.Trim())
                    return vertex;
            }

            return null;    //vertex does not exist
        }

        private static void generateGraphSamples()
        {
            vertexList = new List<Vertex<char>>();
            vertexTitleList = new List<string>();

            //declare vertices
            Vertex<char> A = new Vertex<char>('A');
            Vertex<char> B = new Vertex<char>('B');
            Vertex<char> C = new Vertex<char>('C');
            Vertex<char> D = new Vertex<char>('D');
            Vertex<char> E = new Vertex<char>('E');
            Vertex<char> F = new Vertex<char>('F');
            Vertex<char> G = new Vertex<char>('G');
            Vertex<char> I = new Vertex<char>('I');
            Vertex<char> J = new Vertex<char>('J');
            Vertex<char> K = new Vertex<char>('K');
            Vertex<char> L = new Vertex<char>('L');

            //specify neighbours
            Dictionary<Vertex<char>, int> a = new Dictionary<Vertex<char>, int>();
            Dictionary<Vertex<char>, int> b = new Dictionary<Vertex<char>, int>();
            Dictionary<Vertex<char>, int> c = new Dictionary<Vertex<char>, int>();
            Dictionary<Vertex<char>, int> d = new Dictionary<Vertex<char>, int>();
            Dictionary<Vertex<char>, int> i = new Dictionary<Vertex<char>, int>();

            //specify weights for edges
            a.Add(B, 3);    a.Add(C, 2);

            b.Add(D, 4);    b.Add(E, 2);    

            c.Add(F, 3);    c.Add(G, 6);    c.Add(E, 5);

            d.Add(E, 1);    d.Add(I, 3);    d.Add(J, 5);

            i.Add(K, 5);    i.Add(L, 4);

            //add neighbours to nodes in graph
            A.addNeighbors(a);  B.addNeighbors(b);  C.addNeighbors(c);
            D.addNeighbors(d);  I.addNeighbors(i);

            //add to list of vertices
            vertexList.Add(A);  vertexList.Add(B);  vertexList.Add(C);
            vertexList.Add(D); vertexList.Add(E); vertexList.Add(F);
            vertexList.Add(G); vertexList.Add(I); vertexList.Add(J);
            vertexList.Add(K); vertexList.Add(L); 
        }
    }
}
