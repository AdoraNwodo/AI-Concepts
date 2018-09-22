using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VeurmaClassifier
{
    public class C45Algorithm
    {

        private DecisionTree tree;

        private int maximumHeight;
        private int step;  //split step

        private double[][] thresholds;
        private Range<int>[] inputRanges;

        private int inputVariables;
        private int outputClasses;

        private int join = 1;
        private int[] attributeUsageCount;
        private IList<Attribute> attributes;

        
        public int MaximumHeight
        {
            get { return maximumHeight; }
            set
            {
                maximumHeight = value;
            }
        }

        public IList<Attribute> Attributes
        {
            //attributes for tree induction
            get { return attributes; }
            set { attributes = value; }
        }

        public int MaxVariables { get; set; }   //maximum number of variables in a tree

        public int SplitStep    //new splitting step: Default is one (root).
        {
            get { return step; }
            set
            {
                step = value;
            }
        }

        public DecisionTree ClassificationModel //The Classification model, which in this case is a decision tree.
        {
            get { return tree; }
            set { tree = value; }
        }


        public C45Algorithm()   //constructor
        {
            this.step = 1;  //default split step is 1.
        }

        /// <summary>
        /// C45 Algorithm
        /// </summary>
        /// <param name="attributes">Attributes that would be used in learning</param>
        public C45Algorithm(Attribute[] attributes) 
        {
            this.attributes = new List<Attribute>(attributes);
        }


        private void initialize(DecisionTree tree)
        {
            this.tree = tree;
            inputVariables = tree.Inputs;
            outputClasses = tree.Outputs;
            attributeUsageCount = new int[inputVariables];
            inputRanges = new Range<int>[inputVariables];
            maximumHeight = inputVariables;
            attributes = tree.Attributes;

            for (int i = 0; i < inputRanges.Length; i++)
                inputRanges[i] = tree.Attributes[i].Range.ToIntRange();
        }

        /// <summary>
        ///   Induces the decision tree
        /// </summary>
        /// 
        /// <param name="x">The inputs.</param>
        /// <param name="y">The class labels.</param>
        /// 
        /// <returns>A decision tree</returns>
        /// 
        public DecisionTree Learn(double[][] x, int[] y)
        {
            if (tree == null)
            {
                if (attributes == null)
                    attributes = Attribute.FromData(x);
                int classes = y.Max() + 1;
                initialize(new DecisionTree(this.attributes, classes));
            }

            this.induceTree(x, y);
            return tree;
        }

        /// <summary>
        ///   Induces the decision tree
        /// </summary>
        /// 
        /// <param name="x">The inputs.</param>
        /// <param name="y">The class labels.</param>
        /// 
        /// <returns>A decision tree</returns>
        ///  
        public DecisionTree Learn(int[][] x, int[] y)
        {
            if (tree == null)
            {
                var variables = Attribute.FromData(x);
                int classes = y.Max() + 1;
                initialize(new DecisionTree(variables, classes));
            }

            induceTree(Converter(x), y);
            return tree;
        }

        /// <summary>
        /// Convert Integer Jagged Array to Double Jagged Array
        /// </summary>
        /// <param name="value">integer array</param>
        /// <returns>Newly converted double array</returns>
        public double[][] Converter(int[][] value)
        {
            double[][] result = new double[value.Length][];
            for (int i = 0; i < result.Length; i++)
                result[i] = new double[value[i].Length];

            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Double)value[i][j];

            return result;
        }


        private void induceTree(double[][] inputs, int[] outputs)
        {
            //reset
            for (int i = 0; i < attributeUsageCount.Length; i++)
            {
                attributeUsageCount[i] = 0;
            }

            thresholds = new double[tree.Attributes.Count][];

            var candidates = new List<double>(inputs.Length);

            // create split thresholds
            for (int i = 0; i < tree.Attributes.Count; i++)
            {
                if (tree.Attributes[i].Nature == AttributeType.Continuous)
                {
                    double[] v = inputs.GetColumn(i);
                    int[] o = (int[])outputs.Clone();

                    IGrouping<double, int>[] sortedValueMapping =
                        v.
                            Select((value, index) => new KeyValuePair<double, int>(value, o[index])).
                            GroupBy(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value).
                            OrderBy(keyValuePair => keyValuePair.Key).
                            ToArray();

                    for (int j = 0; j < sortedValueMapping.Length - 1; j++)
                    {
                        IGrouping<double, int> currentValueToClasses = sortedValueMapping[j];
                        IGrouping<double, int> nextValueToClasses = sortedValueMapping[j + 1];
                        double a = nextValueToClasses.Key;
                        double b = currentValueToClasses.Key;
                        if (a - b > 1.11022302462515654042e-16 && currentValueToClasses.Union(nextValueToClasses).Count() > 1)
                            candidates.Add((currentValueToClasses.Key + nextValueToClasses.Key) / 2.0);
                    }

                    thresholds[i] = candidates.ToArray();
                    candidates.Clear();
                }
            }

            // Create a root node for the tree
            tree.Root = new Node(tree);

            // Recursively split the tree nodes
            split(tree.Root, inputs, outputs, 0);
        }

        private void split(Node root, double[][] input, int[] output, int height)
        {
            double entropy = Entropy(output, outputClasses);

            if (entropy == 0)
            {
                if (output.Length > 0)
                    root.Output = output[0];
                return;
            }

            int[] candidates = Matrix.Find(attributeUsageCount, x => x < join);

            if (candidates.Length == 0 || (maximumHeight > 0 && height == maximumHeight))
            {
                root.Output = output.GroupBy(v => v).OrderByDescending(g => g.Count()).First().Key;
                return;
            }

            if (MaxVariables > 0)
            {
                var random = new Random();

                var a = Range(MaxVariables);
                var x = new double[a.Length];
                for (int i = 0; i < x.Length; i++)
                    x[i] = random.NextDouble();

                Array.Sort(x, a);

                int[] idx2 = a;
                candidates = candidates.Sub(idx2);
            }

            var scores = new double[candidates.Length];
            var thresholds = new double[candidates.Length];
            var partitions = new int[candidates.Length][][];

            // For each attribute in the data set
            for (int i = 0; i < scores.Length; i++)
            {
                scores[i] = calculateGainRatio(input, output, candidates[i],
                entropy, out partitions[i], out thresholds[i]);
            }

            // Select the attribute with maximum gain ratio
            int maxGainRatio;
            scores.MaxElement(out maxGainRatio);
            var maxGainPartition = partitions[maxGainRatio];
            var maxGainAttribute = candidates[maxGainRatio];
            var maxGainRange = inputRanges[maxGainAttribute];
            var maxGainThreshold = thresholds[maxGainRatio];

            // Mark this attribute as already used
            attributeUsageCount[maxGainAttribute]++;

            double[][] inputSublist;
            int[] outputSublist;

            if (tree.Attributes[maxGainAttribute].Nature == AttributeType.Discrete)
            {
                Node[] children = new Node[maxGainPartition.Length];

                // Create a branch for each possible value
                for (int i = 0; i < children.Length; i++)
                {
                    children[i] = new Node(tree)
                    {
                        Parent = root,
                        Value = i + maxGainRange.Min,
                        Comparison = "==",
                    };

                    inputSublist = input.Sub(maxGainPartition[i]);
                    outputSublist = output.Sub(maxGainPartition[i]);
                    split(children[i], inputSublist, outputSublist, height + 1); // recursion
                }

                root.Branches.Index = maxGainAttribute;
                root.Branches.AddChildren(children);
            }

            else if (maxGainPartition.Length > 1)
            {
                Node[] children =
                {
                    new Node(tree)
                    {
                        Parent = root, Value = maxGainThreshold,
                        Comparison = "<="
                    },

                    new Node(tree)
                    {
                        Parent = root, Value = maxGainThreshold,
                        Comparison = ">"
                    }
                };

                // Create a branch for lower values
                inputSublist = input.Sub(maxGainPartition[0]);
                outputSublist = output.Sub(maxGainPartition[0]);
                split(children[0], inputSublist, outputSublist, height + 1);

                // Create a branch for higher values
                inputSublist = input.Sub(maxGainPartition[1]);
                outputSublist = output.Sub(maxGainPartition[1]);
                split(children[1], inputSublist, outputSublist, height + 1);

                root.Branches.Index = maxGainAttribute;
                root.Branches.AddChildren(children);
            }
            else
            {
                outputSublist = output.Sub(maxGainPartition[0]);

                root.Output = output.GroupBy(v => v).OrderByDescending(g => g.Count()).First().Key;
            }

            attributeUsageCount[maxGainAttribute]--;
        }


        private double calculateGainRatio(double[][] input, int[] output, int attributeIndex,
            double entropy, out int[][] partitions, out double threshold)
        {
            double infoGain = calculateInformationGain(input, output, attributeIndex, entropy, out partitions, out threshold);
            double splitInfo = SplitInformation(output.Length, partitions);

            return infoGain == 0 || splitInfo == 0 ? 0 : infoGain / splitInfo;
        }


        public static double SplitInformation(int samples, int[][] partitions)
        {   //calculate split information
            double info = 0;

            for (int i = 0; i < partitions.Length; i++)
            {
                double p = (double)partitions[i].Length / samples;

                if (p != 0)
                    info -= p * Math.Log(p, 2);
            }

            return info;
        }

        private double calculateInformationGain(double[][] input, int[] output, int attributeIndex,
            double entropy, out int[][] partitions, out double threshold)
        {
            threshold = 0;

            if (tree.Attributes[attributeIndex].Nature == AttributeType.Discrete)
                return entropy - InformationGainNominal(input, output, attributeIndex, out partitions);

            return entropy + InformationGainNumeric(input, output, attributeIndex, out partitions, out threshold);
        }


        private double InformationGainNominal(double[][] input, int[] output,
            int attributeIndex, out int[][] partitions)
        {
            double info = 0;

            Range<int> valueRange = inputRanges[attributeIndex];
            partitions = new int[(valueRange.Max - valueRange.Min) + 1][];

            for (int i = 0; i < partitions.Length; i++)
            {
                int value = valueRange.Min + i;

                partitions[i] = input.Find(x => x[attributeIndex] == value);

                int[] outputSubset = output.Sub(partitions[i]);

                double e = Entropy(outputSubset, outputClasses);

                info += ((double)outputSubset.Length / output.Length) * e;
            }

            return info;
        }

        private double InformationGainNumeric(double[][] input, int[] output,
            int attributeIndex, out int[][] partitions, out double threshold)
        {
            // use this current attribute as the next decision node.
            double[] t = thresholds[attributeIndex];
            double bestGain = Double.NegativeInfinity;

            // no thresholds for split?
            if (t.Length == 0)
            {
                // Then they all belong to the same partition
                partitions = new int[][] { Range(input.Length) };
                threshold = Double.NegativeInfinity;
                return bestGain;
            }

            double bestThreshold = t[0];
            partitions = null;

            List<int> a1 = new List<int>(input.Length);
            List<int> a2 = new List<int>(input.Length);

            List<int> output1 = new List<int>(input.Length);
            List<int> output2 = new List<int>(input.Length);

            double[] values = new double[input.Length];
            for (int i = 0; i < values.Length; i++)
                values[i] = input[i][attributeIndex];

            // For each possible splitting point of the attribute
            for (int i = 0; i < t.Length; i += step)
            {
                // Partition the remaining data set
                // according to the threshold value
                double value = t[i];

                a1.Clear();
                a2.Clear();

                output1.Clear();
                output2.Clear();

                for (int j = 0; j < values.Length; j++)
                {
                    double x = values[j];

                    if (x <= value)
                    {
                        a1.Add(j);
                        output1.Add(output[j]);
                    }
                    else if (x > value)
                    {
                        a2.Add(j);
                        output2.Add(output[j]);
                    }
                }

                double p1 = (double)output1.Count / output.Length;
                double p2 = (double)output2.Count / output.Length;

                double splitGain =
                    -p1 * Entropy(output1.ToArray(), outputClasses) +
                    -p2 * Entropy(output2.ToArray(), outputClasses);

                if (splitGain > bestGain)
                {
                    bestThreshold = value;
                    bestGain = splitGain;

                    if (a1.Count > 0 && a2.Count > 0)
                        partitions = new int[][] { a1.ToArray(), a2.ToArray() };
                    else if (a1.Count > 0)
                        partitions = new int[][] { a1.ToArray() };
                    else if (a2.Count > 0)
                        partitions = new int[][] { a2.ToArray() };
                    else
                        partitions = new int[][] { };
                }
            }

            threshold = bestThreshold;
            return bestGain;
        }

        /// <summary>
        ///   Entropy Calculator
        /// </summary>
        /// 
        /// <param name="values">An array of integer symbols.</param>
        /// <param name="classes">The number of classes.</param>
        /// <returns>The evaluated entropy.</returns>
        private static double Entropy(int[] values, int classes)
        {
            double entropy = 0;
            double total = values.Length;

            // For each class
            for (int c = 0; c < classes; c++)
            {
                int count = 0;

                // Count the number of instances inside
                foreach (int v in values)
                    if (v == c) count++;

                if (count > 0)
                {
                    double p = count / total;
                    entropy -= p * Math.Log(p, 2);
                }
            }

            return entropy;
        }

        public static int[] Range(int n)
        {
            int[] r = new int[(int)n];
            for (int i = 0; i < r.Length; i++)
                r[i] = i;
            return r;
        }

    }
}
