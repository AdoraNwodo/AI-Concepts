using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeurmaClassifier
{
    public class DecisionTree 
    {
        private Node root;
        private AttributeCollection attributes;

        private int inputs;
        private int outputs;

        public int Inputs
        {
            get { return inputs; }
            set { inputs = value; }
        }

        public int Outputs
        {
            get { return outputs; }
            set { outputs = value; }
        }

        public int[] ComputeDecision(double[][] input)
        {
            return ComputeDecision(input, new int[input.Length]);
        }

        public int[] ComputeDecision(double[][] input, int[] result)
        {
            for (int i = 0; i < input.Length; i++)
                result[i] = ComputeDecision(input[i]);
            return result;
        }


        public int ComputeDecision(int[] input)
        {   //start reasoning process
            return ComputeDecision(input.ConvertToDouble());
        }

        public Node Root
        {
            get { return root; }
            set { root = value; }
        }

        public AttributeCollection Attributes
        {
            get { return attributes; }  //attributes processsed by this tree
        }


        public DecisionTree(IList<Attribute> inputs, int classes)    //decision tree instance
        {
            this.attributes = new AttributeCollection(inputs);
            this.Inputs = inputs.Count;
            this.Outputs = classes;
        }

        public int ComputeDecision(double[] input)
        {
            return computeDecision(input, Root);
        }


        public int ComputeDecision(double[] input, Node subtree)
        {
            return computeDecision(input, subtree);
        }

        private static int computeDecision(double[] input, Node subtree)
        {
            Node curr = subtree;

            // Start reasoning - analogy of marker and moving down each branch
            while (curr != null)
            {
                // Check if this is a leaf
                if (curr.IsLeaf)
                {
                    // Leaf node. Class found.

                    return (curr.Output.HasValue) ? curr.Output.Value : -1;
                }

                // This node is not a leaf. Continue reasoning on children
                int attribute = curr.Branches.Index;

                Node nextNode = null;

                foreach (Node branch in curr.Branches)
                {
                    if (branch.Compute(input[attribute]))
                    {
                        nextNode = branch;
                        break;
                    }
                }

                curr = nextNode;
            }

            return -1;
        }

        private static int ComputeDecision(int[] input, Node subtree)
        {
            Node current = subtree;

            // Start reasoning
            while (current != null)
            {
                // Check if this is a leaf
                if (current.IsLeaf)
                {
                    // Leaf node. Class found.
                    return (current.Output.HasValue) ? current.Output.Value : -1;
                }

                int attribute = current.Branches.Index;

                Node nextNode = null;

                foreach (Node branch in current.Branches)
                {
                    if (branch.Compute(input[attribute]))
                    {
                        nextNode = branch; break;
                    }
                }

                current = nextNode;
            }

            return -1;
        }



        public IEnumerable<Node> GetEnumerable()
        {
            if (Root == null)
                yield break;

            var stack = new Stack<Node>(new[] { Root });

            while (stack.Count != 0)
            {
                Node current = stack.Pop();

                yield return current;

                if (current.Branches != null)
                    for (int i = current.Branches.Count - 1; i >= 0; i--)
                        stack.Push(current.Branches[i]);
            }
        }

        public IEnumerator<Node> GetEnumerator()
        {
            if (Root == null)
                yield break;

            var stack = new Stack<Node>(new[] { Root });

            while (stack.Count != 0)
            {
                Node current = stack.Pop();

                yield return current;

                if (current.Branches != null)
                    for (int i = current.Branches.Count - 1; i >= 0; i--)
                        stack.Push(current.Branches[i]);
            }
        }


        public int GetLeaves()
        {
            IEnumerable<Node> nodeList = GetEnumerable();
            List<Node> leaveList = new List<Node>();

            foreach(Node node in nodeList)
            {
                if (node.IsLeaf)
                    leaveList.Add(node);
            }

            return leaveList.Count();
        }

        //   Transforms the tree into a set of decision rules</see>.
        public DecisionRuleSet ToRules(Dictionary<string, AttributeListInfo> attributes, AttributeType dvt)
        {
            return DecisionRuleSet.FromDecisionTree(this, attributes, dvt);
        }


    }
}
