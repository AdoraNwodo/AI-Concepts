using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeurmaClassifier
{
    /// <summary>
    ///   Collection of decision nodes. Specifies the best split attribute
    /// </summary>
    public class SplitProperties : Collection<Node>
    {

        public SplitProperties(Node parent)
        {
            this.parent = parent;
        }

        private Node parent;

        public int Index { get; set; }     //index at this stage of decision process

        public Attribute Attribute
        {   // Get the attribute used in this stage of the decision process using the index
            get
            {
                if (parent == null)
                    return null;

                return parent.Owner.Attributes[Index];
            }
        }

        public Node Parent   //owner of the collection
        {
            get { return parent; }
            set { parent = value; }
        }

        public void AddChildren(IEnumerable<Node> children)
        {
            foreach (var node in children) Add(node);   //Adds the child nodes to the collection
        }
    }
}
