using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeurmaClassifier
{
    public enum AttributeType
    {
        Discrete, Continuous, Both
    }

    public class Attribute
    {
        public string Name { get; set; }    //Attribute name

        public AttributeType Nature { get; set; }    //real or discrete-valued?

        public Range<double> Range { get; set; }    //range of valid values for attribute

        public Attribute(string name, Range<double> range)
        {
            this.Name = name;
            this.Nature = AttributeType.Continuous;
            this.Range = range;
        }

        public Attribute(string name, Range<int> range)
        {
            this.Name = name;
            this.Nature = AttributeType.Discrete;
            this.Range = new Range<double>(range.Min, range.Max);
        }

        public Attribute(string name, int symbols)   //discrete
            : this(name, new Range<int>(0, symbols - 1))
        {
        }

        public static Attribute Continuous(string name, Range<double> range)
        {
            return new Attribute(name, range);
        }

        public static Attribute Discrete(string name, Range<int> range)
        {
            return new Attribute(name, range);
        }

        public static Attribute Discrete(string name, int symbols)
        {
            return new Attribute(name, symbols);
        }

        public override string ToString()
        {
            return String.Format("{0} : {1} ({2} - {3})", Name, Nature, Range.Min, Range.Max);
        }

        public static Attribute[] FromData(double[][] inputs)
        {
            int cols = inputs.Columns();
            var variables = new Attribute[cols];
            for (int i = 0; i < variables.Length; i++)
            {
                double min, max;
                Matrix.Range(inputs.GetColumn(i), out min, out max);
                Range<double> myRange = new Range<double>(min, max);
                variables[i] = new Attribute(i.ToString(), myRange);
            }

            return variables;
        }

        public static Attribute[] FromData(int[][] inputs)
        {
            int cols = inputs.Columns();
            var variables = new Attribute[cols];
            for (int i = 0; i < variables.Length; i++)
            {
                int min, max;
                Matrix.Range(inputs.GetColumn(i), out min, out max);
                Range<int> myRange = new Range<int>(min, max);
                variables[i] = new Attribute(i.ToString(), myRange);
            }

            return variables;
        }

        public static string ToString(AttributeType dvt)
        {
            switch (dvt)
            {
                case AttributeType.Continuous:
                    return "Continuous";
                case AttributeType.Discrete:
                    return "Discrete";
                case AttributeType.Both:
                    return "Continous & Discrete";
                default:
                    return "undefined";

            }
        }
    }

    public class AttributeCollection : ReadOnlyCollection<Attribute>
    {
        public AttributeCollection(IList<Attribute> list)
            : base(list) { }
    }
}
