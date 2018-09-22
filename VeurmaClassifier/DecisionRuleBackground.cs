using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeurmaClassifier
{
    public class DecisionRuleBackground
    {
        private int variableIndex;
        private string comparison;
        private double value;

        public int VariableIndex
        {
            get
            {
                return variableIndex;   //left side
            }
        }

        public string Comparison
        {
            get
            {
                return comparison;
            }
        }

        public double Value
        {
            get
            {
                return value;   //right side
            }
        }

        public DecisionRuleBackground(int index, string Comparison, double val)
        {
            variableIndex = index;
            comparison = Comparison;
            value = val; //value to be compared against
        }



        /// <summary>
        ///   Checks if this antecedent applies to a given input vector.
        /// </summary>
        public bool Match(double[] input)
        {
            double x = input[VariableIndex];

            switch (Comparison)
            {
                case "==":
                    return (x == Value);

                case ">":
                    return (x > Value);

                case ">=":
                    return (x >= Value);

                case "<":
                    return (x < Value);

                case "<=":
                    return (x <= Value);

                case "!=":
                    return (x != Value);

                default:
                    throw new InvalidOperationException();
            }
        }

        public override string ToString()
        {
            string op = Comparison;
            return string.Format("x[{0}] {1} {2}", VariableIndex, op, Value);
        }


    }
}
