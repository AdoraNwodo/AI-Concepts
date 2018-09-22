using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veurma.Utility.Graph;

namespace Veurma.Utility
{
    public class UCSProperties
    {
        private Vertex<char> node;
        private string path;

        public Vertex<char> Node
        {
            get { return node; }
            set { node = value; }
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }
    }
}
