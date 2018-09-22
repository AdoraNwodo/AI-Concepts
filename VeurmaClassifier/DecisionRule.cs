using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeurmaClassifier
{
    /// <summary>
    ///   Decision Rule.
    /// </summary>
    /// 
    public class DecisionRule : IEnumerable<DecisionRuleBackground>, IComparable<DecisionRule>
    {

        private List<DecisionRuleBackground> expressions;
        private AttributeCollection variables;
        private double output;
        private AttributeType attributeType;

        public AttributeCollection Variables { get { return variables; } }   //decision variables for this rule.

        //expressions that must be fulfilled in order for this rule to be applicable.
        public IList<DecisionRuleBackground> Expressions { get { return expressions; } }

        public double Output    //output when all decision rule background conditions are met
        {
            get { return output; }
            set { output = value; }
        }

        public DecisionRule(IList<Attribute> variables,
            double output, IEnumerable<DecisionRuleBackground> conditions)
        {
            this.variables = new AttributeCollection(variables);
            this.expressions = new List<DecisionRuleBackground>(conditions);
            this.output = output;
        }

        public int Count
        {
            get { return Expressions.Count; }   //Gets the number of background rules contained in this decision rule
        }

        public bool Match(double[] input)   //Methid to check whether the rule matches a given input vector.
        {
            foreach (var expr in Expressions)
            {
                if (!expr.Match(input))
                    return false;
            }

            return true;
        }

        public static DecisionRule NewRuleFromNode(Node node)
        {
            Node current = node;
            DecisionTree owner = current.Owner;
            double output = current.Output.Value;

            var backgroundRules = new List<DecisionRuleBackground>();

            while (current.Parent != null)
            {
                int index = current.Parent.Branches.Index;
                string comparison = current.Comparison;
                double value = current.Value.Value;

                backgroundRules.Insert(0, new DecisionRuleBackground(index, comparison, value));

                current = current.Parent;
            }

            return new DecisionRule(node.Owner.Attributes, output, backgroundRules);
        }

        public bool IsInconsistentWith(DecisionRule rule)
        {
            //do two rules have same background rule but different outputs?
            return Expressions.AreEqual(rule.Expressions) && Output != rule.Output;
        }

        public IEnumerator<DecisionRuleBackground> GetEnumerator()
        {
            return expressions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return expressions.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DecisionRule);
        }

        public bool Equals(DecisionRule other)
        {
            if ((object)other == null)
                return false;

            return this.Output == other.output
              && this.Expressions.AreEqual(other.Expressions);
        }

        public int CompareTo(DecisionRule other)
        {
            int order = this.Output.CompareTo(other.Output);

            if (order == 0)
                return this.Expressions.Count.CompareTo(other.Expressions.Count);

            return order;
        }
        public static bool operator <(DecisionRule a, DecisionRule b)
        {
            if (a.Output == b.Output)
                return a.Expressions.Count < b.Expressions.Count;

            return a.Output < b.Output;
        }

        public static bool operator >(DecisionRule a, DecisionRule b)
        {
            if (a.Output == b.Output)
                return a.Expressions.Count > b.Expressions.Count;

            return a.Output > b.Output;
        }

        public static bool operator ==(DecisionRule a, DecisionRule b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Output == b.output
              && a.Expressions.AreEqual(b.Expressions);
        }

        public static bool operator !=(DecisionRule a, DecisionRule b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return 0;
        }


        public string toString(Dictionary<string, AttributeListInfo> attributes, AttributeType dvt)
        {
            attributeType = dvt;
            StringBuilder sb = new StringBuilder();

            var expr = expressions.ToArray();

            for (int i = 0; i < expr.Length - 1; i++)
                sb.AppendFormat("({0}) && ", toString(expr[i], attributes));
            sb.AppendFormat("({0})", toString(expr[expr.Length - 1], attributes));

            string name = attributes.Values.Last().Inputs[(int)Output];//Output.ToString(); //codebook.Revert(outputColumn, (int)Output);
            return String.Format("{0}  ---->  {1}", name, sb);
        }

        private string toString(DecisionRuleBackground drb, Dictionary<string, AttributeListInfo> attributes)
        {
            int index = drb.VariableIndex;
            String name = Variables[index].Name;

            if (String.IsNullOrEmpty(name))
                name = "x[" + index + "]";

            String op = drb.Comparison;

            String value;


            try
            {
                if(attributeType == AttributeType.Discrete)
                {
                    AttributeListInfo attributeInfo = attributes.ElementAt(Int32.Parse(name)).Value;
                    string originalName = attributes.ElementAt(Int32.Parse(name)).Key;

                    if (attributeInfo.NumberOfInputs <= 1 || attributeInfo.Inputs[0].ToString() == "real" || attributeInfo.Inputs[0].ToString() == "numeric")
                    {
                        value = drb.Value.ToString();
                    }
                    else
                    {
                        value = attributeInfo.Inputs[(int)drb.Value];   //index of value
                    }


                    return String.Format("{0} {1} {2}", originalName, op, value);
                }
                else
                {
                    AttributeListInfo attributeInfo = attributes.ElementAt(Int32.Parse(name)).Value;
                    string originalName = attributes.ElementAt(Int32.Parse(name)).Key;

                    return String.Format("{0} {1} {2}", originalName, op, drb.Value.ToString());
                }
                
            }
            catch (Exception ex) { }

            value = attributes[name].Inputs[(int)drb.Value];   //index of value
            return String.Format("{0} {1} {2}", name, op, value);
        }
    }
}
