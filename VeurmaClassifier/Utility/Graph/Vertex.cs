using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veurma.Utility.Graph
{
    public class Vertex<T> where T : IComparable
    {
        private T data;
        private Dictionary<Vertex<T>, int> w_neighbors; //key is neighbour, value is weight
        private List<Vertex<T>> u_neighbors;

        public Vertex(T _data)
        {
            data = _data;
            w_neighbors = null;
            u_neighbors = new List<Vertex<T>>();

        }

        public T Data
        {
            get { return data; }
        }

        public Dictionary<Vertex<T>, int> WeightedNeighbors
        {
            get { return w_neighbors; }
        }

        public List<Vertex<T>> UnweightedNeighbors
        {
            get { return u_neighbors; }
        }

        public void addNeighbors(Dictionary<Vertex<T>, int> _neighbors)
        {
            w_neighbors = _neighbors;
        }

        public void addNeighbors(params Vertex<T>[] _neighbors)
        {
            foreach (Vertex<T> neighbor in _neighbors)
                u_neighbors.Add(neighbor);
        }

        public override string ToString()
        {
            return data+"";
        }
    }
}
