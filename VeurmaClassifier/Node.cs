using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeurmaClassifier
{
    public class Node
    {
        private DecisionTree owner;
        private Node parent;

        public double? Value { get; set; }  //set value when node has a parent

        public string Comparison { get; set; }

        public int? Output { get; set; }    //output value to be decided on reaching node. (leaf node)

        public SplitProperties Branches { get; set; }    //non leaf node. Gets collection + attribute to determine process of reasoning

        public Node Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public DecisionTree Owner   //decision tree containing the node
        {
            get { return owner; }
            set { owner = value; }
        }

        public Node(DecisionTree owner)
        {
            Owner = owner;
            Comparison = string.Empty;
            Branches = new SplitProperties(this);
        }

        public bool IsRoot
        {
            get { return Parent == null; }
        }

        public bool IsLeaf
        {
            get { return Branches == null || Branches.Count == 0; }
        }

        public bool Compute<T>(T x) where T : IComparable
        {
            //check the node's condition if the value satisfies it
            switch (Comparison)
            {
                case "==":
                    return ((dynamic)x == Value);

                case ">":
                    return ((dynamic)x > Value);

                case ">=":
                    return ((dynamic)x >= Value);

                case "<":
                    return ((dynamic)x < Value);

                case "<=":
                    return ((dynamic)x <= Value);

                case "!=":
                    return ((dynamic)x != Value);

                default:
                    throw new InvalidOperationException();
            }
        }

        public override string ToString()
        {
            if (IsRoot)
                return "Root";

            string name = Owner.Attributes[Parent.Branches.Index].Name;

            if (string.IsNullOrEmpty(name))
                name = "x" + Parent.Branches.Index;

            string op = Comparison;

            string value = Value.ToString();

            return string.Format("{0} {1} {2}", name, op, value);
        }

        public IEnumerator<Node> GetEnumerator()
        {
            var stack = new Stack<Node>(new[] { this });

            while (stack.Count != 0)
            {
                Node cur = stack.Pop();

                yield return cur;

                if (cur.Branches != null)
                    for (int i = cur.Branches.Count - 1; i >= 0; i--)
                        stack.Push(cur.Branches[i]);
            }
        }

        /*
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        */
    }
}
