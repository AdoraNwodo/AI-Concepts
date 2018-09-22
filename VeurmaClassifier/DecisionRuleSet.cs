using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeurmaClassifier
{
    public class DecisionRuleSet : IEnumerable<DecisionRule>
    {
        HashSet<DecisionRule> rules;
        private static Dictionary<string, AttributeListInfo> _attributes;
        private static AttributeType decisionVariableType;

        public int OutputClassesInSet { get; private set; }

        public DecisionRuleSet(IEnumerable<DecisionRule> rules)
        {
            this.rules = new HashSet<DecisionRule>(rules);
        }

        public static DecisionRuleSet FromDecisionTree(DecisionTree tree, Dictionary<string, AttributeListInfo> attributes, AttributeType dvt)
        {
            _attributes = attributes;
            decisionVariableType = dvt; 
            //creates a new decision set from decision tree
            var rules = new List<DecisionRule>();

            foreach (var node in tree)
            {
                if (node.IsLeaf && !node.IsRoot && node.Output.HasValue)
                {
                    rules.Add(DecisionRule.NewRuleFromNode(node));
                }
            }

            return new DecisionRuleSet(rules)
            {
                OutputClassesInSet = tree.Outputs
            };
        }

        public double? GetDecisionOutput(double[] input)
        {
            foreach (DecisionRule rule in rules)
            {
                if (rule.Match(input))
                    return rule.Output;
            }

            return null;
        }

        public void Add(DecisionRule item)
        {
            rules.Add(item);    //insert new rule to set
        }

        public void AddCollection(IEnumerable<DecisionRule> items)
        {
            foreach (var rule in items)     //insert decision rules to decision rule set
                rules.Add(rule);
        }

        public int Count
        {
            get { return rules.Count; }
        }

        public IEnumerator<DecisionRule> GetEnumerator()
        {
            return rules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return rules.GetEnumerator();
        }

        public override string ToString()
        {
            return toString();
        }

        private string toString()
        {
            var rulesArray = new DecisionRule[this.rules.Count];

            rules.CopyTo(rulesArray);
            Array.Sort(rulesArray);

            StringBuilder sb = new StringBuilder();
            foreach (DecisionRule rule in rulesArray)
                sb.AppendLine("\n"+rule.toString(_attributes, decisionVariableType));

            return sb.ToString();
        }



    }
}

