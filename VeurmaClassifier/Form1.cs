using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArffTools;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Veurma.Utility.Graph;
using System.Diagnostics;
using System.Threading;

namespace VeurmaClassifier
{
    public partial class Form1 : Form
    {
        private string filePath = null;
        private List<Attribute> _attributes = new List<Attribute>();
        private List<Query> queryList = new List<Query>();
        private ArffHeader header;
        private Dictionary<string, AttributeListInfo> attributes;
        private List<ArffAttribute> attributeList;
        private double[][] dInputs;
        private double[][] allDInputs;
        private int[][] iInputs;
        private int[][] allIInputs;
        private int[] outputs;
        private DecisionTree tree;
        private LearningAlgorithm learningAlgorithm;
        private AttributeType decisionVariableType;
        private OpenFileDialog ofd;
        private DecisionRuleSet rules;
        private C45Algorithm c45;
        private string lastAttrName = "";
        private int[] predicted;
        private ContingencyTable contingencyTable;
        List<Vertex<char>> graphSample;
        int[,] adjacencyMatrix;
        string[] network;
        List<string> nodeTitles;
        List<string> adjMatrixGraphNodeList;
        private double elapsedTime;

        //For Visualization
        private GViewer viewer2 = new GViewer();
        private Graph graph2 = new Graph("graph");

        public Form1()
        {
            InitializeComponent();
            learningAlgorithm = LearningAlgorithm.C45Learning;
            decisionVariableType = AttributeType.Discrete;
            this.MaximumSize = new Size(809, 497);
            this.MaximizeBox = false;
            ofd = new OpenFileDialog();
            this.CenterToScreen();
            radioButton2.Hide();
            bindAlgorithmType();
            drawGraph();
            nodeTitles = WeightedDirectedGraph.VertexTitleList;
            bindStartNodes();
            bindGoalNodes();
            depthInput.Minimum = 1;
            depthInput.Maximum = graphSample.Count;
            adjMatrixGraphNodeList = AdjacencyMatrixGraph.NodeList;
        }


        private void Reset()
        {
            _attributes.Clear();
            queryList.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)  //file upload button (arff extensions only)
        {
            ofd.Filter = "ARFF|*.arff";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = ofd.FileName;
                filePath = @"" + ofd.FileName;
                disableButtons();
                richTextBox1.Text = "";
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Learn()
        {
            using (ArffReader arffReader = new ArffReader(filePath))
            {
                header = arffReader.ReadHeader();
                attributes = new Dictionary<string, AttributeListInfo>(); //attribute name, attribute info
                attributeList = header.Attributes.ToList();

                foreach (var attr in attributeList)
                {
                    string[] array = attr.Type.ToString().Split(',');
                    if (decisionVariableType == AttributeType.Discrete && (array[0].ToString() == "numeric" || array[0].ToString() == "real" || array.Length == 1))
                        throw new InvalidOperationException("Variable Type Should be Continous, not Discrete");

                    List<string> myList = new List<string>();
                    foreach (string s in array)
                    {
                        //if s contains real, do decision tree learning for real values
                        myList.Add(s.Replace("{", "").Replace("}", "").Trim());
                    }

                    AttributeListInfo myAttributeInfo = new AttributeListInfo();    //define information for each attribute, then add to dictionary
                    myAttributeInfo.NumberOfInputs = myList.Count;      //array.Length
                    myAttributeInfo.Inputs = myList;

                    attributes.Add(attr.Name, myAttributeInfo);

                    if (_attributes.Count < attributeList.Count - 1)
                    {
                        _attributes.Add(new Attribute(attr.Name, myAttributeInfo.NumberOfInputs));
                        queryList.Add(new Query(attr.Name, myList));

                    }
                    else
                    {
                        lastAttrName = attr.Name;
                    }
                }

                List<double[]> inputs2 = new List<double[]>();
                List<double[]> inputsAll = new List<double[]>();
                List<int> outputs2 = new List<int>();
                object[] instance;
                while ((instance = arffReader.ReadInstance()) != null)
                {
                    int Length = instance.Length;
                    double[] newRow = new double[Length - 1];
                    double[] row = new double[Length];
                    for (int i = 0; i < Length - 1; i++)
                    {
                        Double.TryParse(instance[i] + "", out newRow[i]);
                    }
                    for (int i = 0; i < Length; i++)
                    {
                        Double.TryParse(instance[i] + "", out row[i]);
                    }
                    inputs2.Add(newRow);
                    inputsAll.Add(row);
                    outputs2.Add(Int32.Parse(instance[Length - 1] + ""));

                }

                    dInputs = inputs2.ToArray();    //inputs (expect the last index)
                    allDInputs = inputsAll.ToArray();   //all double inputs
                    outputs = outputs2.ToArray();
                
                //decide what algorithm to use based on options.
                if (learningAlgorithm == LearningAlgorithm.C45Learning && decisionVariableType == AttributeType.Continuous)
                {
                    c45 = new C45Algorithm();
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    tree = c45.Learn(dInputs, outputs);   //induce tree from data
                    stopwatch.Stop();
                    elapsedTime = stopwatch.ElapsedMilliseconds;
                }
                else if (learningAlgorithm == LearningAlgorithm.C45Learning && decisionVariableType == AttributeType.Both)
                {
                    c45 = new C45Algorithm();
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    tree = c45.Learn(dInputs, outputs);   //induce tree from data
                    stopwatch.Stop();
                    elapsedTime = stopwatch.ElapsedMilliseconds;
                }
                else if (learningAlgorithm == LearningAlgorithm.C45Learning && decisionVariableType == AttributeType.Discrete)
                {
                    c45 = new C45Algorithm(_attributes.ToArray());
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    tree = c45.Learn(dInputs, outputs);   //induce tree from data
                    stopwatch.Stop();
                    elapsedTime = stopwatch.ElapsedMilliseconds;

                }
                rules = tree.ToRules(attributes, decisionVariableType);

                predicted = tree.ComputeDecision(dInputs);


            }
        }

        private void button3_Click(object sender, EventArgs e)      //learn button
        {
            Reset();
            if (filePath == null)
            {
                MessageBox.Show("Please upload training data");
            }
            else
            {
                try
                {
                    Learn();
                    richTextBox1.Text = filePath + "\nLearning Process Completed! \n\nDecision Tree Constructed";
                    enableButtons();
                    groupBox1.Enabled = false;
                    groupBox2.Enabled = false;

                }
                catch (InvalidOperationException ex)
                {
                    richTextBox1.Text = filePath + "\n\nERROR MESSAGE: Attribute type should be specified as Continous, not Discrete";
                }
                catch (FormatException ex)
                {
                    richTextBox1.Text = filePath + "\n\nERROR MESSAGE: Parse error. Please check that attributes in dataset are of valid format";
                }
                catch (Exception ex)
                {
                    richTextBox1.Text = filePath + "\n\nERROR MESSAGE: An unknown error occured when parsing data. Please check that attributes in dataset are of valid format, or try another uploading another file";
                }

            }
        }

        private void bindAlgorithmType()
        {
            BindingSource bindingSource = new BindingSource();
            List<string> algorithms = new List<string>();
            algorithms.Add("Depth-First Search");
            algorithms.Add("Depth-Limited Search");
            algorithms.Add("Iterative Deepening Search");
            algorithms.Add("Breadth-First Search");
            algorithms.Add("Bidirectional Search");
            algorithms.Add("Uniform-Cost Search");

            bindingSource.DataSource = algorithms;
            algorithmType.DataSource = bindingSource;
            algorithmType.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void bindStartNodes()
        {

            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = nodeTitles;
            startNode.DataSource = bindingSource;
            startNode.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void bindGoalNodes()
        {

            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = nodeTitles;
            goalNode.DataSource = bindingSource;
            goalNode.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public string decodeClass(int currString)
        {
            if (currString < 0)
                return "undefined";

            string textEquiv = attributes.Keys.ElementAt(attributes.Count - 1);
            string textEquivOfNumber = attributes[textEquiv].Inputs[currString];
            return textEquivOfNumber;

        }

        private void dynamicallyCreateInputFields()
        {
            Form2 form2 = new Form2(queryList);
            form2.ShowDialog();
            double[] query = form2.queryArray;
            int predicted = tree.ComputeDecision(query);
            StringBuilder sb = new StringBuilder();
            sb.Append("if ");
            for (int i = 0; i < query.Length - 1; i++)
            {
                if (queryList[i].Options[0] == "real" || queryList[i].Options[0] == "numeric" || queryList[i].Options.Count == 1)
                    sb.Append(queryList[i].QueryName + " is " + query[i] + ", ");    //continous
                else
                    sb.Append(queryList[i].QueryName + " is " + queryList[i].Options[(int)query[i]] + ", ");    //discrete
            }
            //last attribute
            int j = query.Length - 1;
            if (queryList[j].Options[0] == "real" || queryList[j].Options[0] == "numeric" || queryList[j].Options.Count == 1)
                sb.Append("and " + queryList[j].QueryName + " is " + query[j]);    //continous
            else
                sb.Append("and " + queryList[j].QueryName + " is " + queryList[j].Options[(int)query[j]]);    //discrete

            richTextBox1.Text = "QUERY RESULT \n\n" + sb + "\n\n" + lastAttrName + " is: " + decodeClass(predicted);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //show decision tree
            if (tree == null)
                MessageBox.Show("There is no decision tree to view!");
            else
            {
                //Form3 form3 = new Form3(tree, decisionVariableType, attributes);
                DecisionTreeVisualizer dtv = new DecisionTreeVisualizer(tree, decisionVariableType, attributes);
                dtv.ShowDialog();
                //form3.ShowDialog();
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        #region RadioButton Change Events + Enable & Disable Button Helper Methods

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            learningAlgorithm = LearningAlgorithm.C45Learning;
            radioButton3.Enabled = true;
            radioButton4.Enabled = true;
            radioButton5.Enabled = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            learningAlgorithm = LearningAlgorithm.ID3Learning;
            radioButton3.Enabled = false;
            radioButton5.Enabled = false;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            decisionVariableType = AttributeType.Continuous;
            radioButton2.Enabled = false;
            radioButton1.Checked = true;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            decisionVariableType = AttributeType.Discrete;
            radioButton2.Enabled = true;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            decisionVariableType = AttributeType.Both;
            radioButton2.Enabled = false;
            radioButton1.Checked = true;
        }

        private void enableButtons()
        {
            viewTreeBtn.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button4.Enabled = true;
            button9.Enabled = true;
        }

        private void disableButtons()
        {
            viewTreeBtn.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button4.Enabled = false;
            button9.Enabled = false;

        }
        #endregion

        private void button6_Click(object sender, EventArgs e)
        {
            //query decision tree
            dynamicallyCreateInputFields();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "DECISION RULES FOR DATASET\n" + filePath + "\n\n" + rules.ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //properties
            StringBuilder sb = new StringBuilder();
            sb.Append("PROPERTIES\n\n");
            sb.Append("Veurma Decision Tree Classifier. Machine Learning - Supervised Learning\n");
            sb.Append("Date: " + DateTime.Now.ToString() + "\n");
            sb.Append("Training data: " + filePath + "\n");
            sb.Append("Learning Algorithm: " + LA.ToString(learningAlgorithm) + "\n");
            sb.Append("Attribute Types: " + Attribute.ToString(decisionVariableType) + "\n");
            sb.AppendLine();
            sb.Append("Tree Size: " + tree.GetEnumerable().Count());
            sb.AppendLine();
            sb.Append("Number of leaves: " + tree.GetLeaves());
            sb.AppendLine();
            sb.Append("Classification Time: " + elapsedTime + " milliseconds \n");
            sb.Append("Number of Records: " + outputs.Length + "\n");
            sb.Append("\nATTRIBUTES \n");
            for (int i = 0; i < queryList.Count; i++)
            {
                sb.Append(queryList[i].QueryName + " -- ");
                if (queryList[i].Options.Count == 1)
                    sb.Append(queryList[i].Options[0] + "\n");
                else
                {
                    sb.Append("{ ");
                    foreach (string option in queryList[i].Options)
                        sb.Append(option + " , ");
                    sb.Append("} \n");
                }
            }
            sb.Append("\nCLASSES \n");
            string classes = attributes.Keys.ElementAt(attributes.Count - 1);
            foreach (string _class in attributes[classes].Inputs)
                sb.Append(_class + " , ");

            richTextBox1.Text = sb.ToString().Replace(", }", "}");
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (tree == null)
            {
                MessageBox.Show("There is no decision tree to visualize");
            }
            else
            {
                Form4 form4 = new Form4(allDInputs, predicted, outputs, attributeList); //dInputs
                form4.Show();
            }

        }

        private void button9_Click(object sender, EventArgs e)
        {
            contingencyTable = new ContingencyTable(predicted, outputs, 1);//, 0);
            StringBuilder sb = new StringBuilder();
            sb.Append("PERFORMANCE METRICS: ");
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("Training data: " + filePath);
            sb.AppendLine();
            sb.Append("True Positives: " + contingencyTable.TruePositives);
            sb.AppendLine();
            sb.Append("True Negatives: " + contingencyTable.TrueNegatives);
            sb.AppendLine();
            sb.Append("False Positives: " + contingencyTable.FalsePositives);
            sb.AppendLine();
            sb.Append("False Negatives: " + contingencyTable.FalseNegatives);
            sb.AppendLine();
            sb.Append("Accuracy: " + contingencyTable.Accuracy);
            sb.AppendLine();
            sb.Append("Precision: " + contingencyTable.Precision);
            sb.AppendLine();
            sb.Append("Recall: " + contingencyTable.Recall);
            sb.AppendLine();
            sb.Append("F Measure: " + contingencyTable.FRatio);
            sb.AppendLine();

            richTextBox1.Text = sb.ToString();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void dataSetUploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button2_Click(sender, e);
        }

        private void decisionTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button4_Click(sender, e);
        }

        private void plotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button4_Click_1(sender, e);
        }

        private void graphVisualizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabPage2);
        }

        private void unInformedSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabPage2);
        }

        private void drawGraph()
        {
            GViewer viewer = new GViewer();
            //create a graph object 
            Graph graph = new Graph("graph");

            //get the list of vertices
            graphSample = WeightedDirectedGraph.VertexList;

            //create the graph content 
            foreach (Vertex<char> node in graphSample)
            {
                string nodeData = node.ToString();
                //get the neighbours
                if (node.WeightedNeighbors != null)
                {
                    foreach (KeyValuePair<Vertex<char>, int> n in node.WeightedNeighbors)
                    {
                        //get node neighbor and add edge
                        string nodeNeighbour = n.Key.Data.ToString();
                        string cost = n.Value.ToString();

                        graph.AddEdge(nodeData, cost, nodeNeighbour).Attr.Color = Microsoft.Msagl.Drawing.Color.IndianRed;
                    }
                }
            }


            //bind the graph to the viewer 
            viewer.Graph = graph;
            viewer.BackColor = System.Drawing.Color.White;
            viewer.ForeColor = viewer.BackColor;
            //associate the viewer with the form 

            viewer.Dock = DockStyle.Fill;
            tabOriginal.Controls.Clear();
            tabOriginal.Controls.Add(viewer);
        }

        private void algorithmType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (algorithmType.SelectedItem + "" == "Depth-Limited Search")
            {
                depthInput.Enabled = true;
                bindStartNodes();
                bindGoalNodes();
                drawGraph();
                tabSolution.Controls.Clear();
            }
            else
            {
                depthInput.Enabled = false;
                if(algorithmType.SelectedItem + "" == "Bidirectional Search")
                {
                    //update comboboxes
                    BindingSource bindingSource = new BindingSource();
                    bindingSource.DataSource = adjMatrixGraphNodeList;
                    startNode.DataSource = bindingSource;
                    startNode.DropDownStyle = ComboBoxStyle.DropDownList;

                    BindingSource bindingSource2 = new BindingSource();
                    bindingSource2.DataSource = adjMatrixGraphNodeList;
                    goalNode.DataSource = bindingSource2;
                    goalNode.DropDownStyle = ComboBoxStyle.DropDownList;

                    //initialize graph
                    initBIDIGraphOriginal();
                    initBIDIGraph();
                }else
                {
                    bindStartNodes();
                    bindGoalNodes();
                    drawGraph();
                    tabSolution.Controls.Clear();
                }

            }
            tabControl2.SelectTab(tabOriginal); 
        }

        private void colorNodeInUI(Vertex<char> node, int step)
        {
            Thread thread = new Thread(delegate ()
            {
                Thread.CurrentThread.IsBackground = true;
                Thread.Sleep(1000);
                graph2.FindNode(node.ToString()).LabelText = node.ToString() + ": Step " + step;
                //step++;
                graph2.FindNode(node.ToString()).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;     //highlight a visited node, so it's different from other children
                viewer2.Graph = graph2;
            });

            thread.Start();

            while (thread.IsAlive)
                Application.DoEvents();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            tabControl2.SelectTab(tabSolution);
            tabSolution.Controls.Clear();

            viewer2 = new GViewer();
            graph2 = new Graph("graph");


            if (algorithmType.SelectedItem.ToString() != "Bidirectional Search")
            {

                foreach (Vertex<char> node in graphSample)
                {
                    string nodeData = node.ToString();
                    //get the neighbours
                    if (node.WeightedNeighbors != null)
                    {
                        foreach (KeyValuePair<Vertex<char>, int> n in node.WeightedNeighbors)
                        {
                            //get node neighbor and add edge
                            string nodeNeighbour = n.Key.Data.ToString();
                            string cost = n.Value.ToString();

                            graph2.AddEdge(nodeData, cost, nodeNeighbour).Attr.Color = Microsoft.Msagl.Drawing.Color.IndianRed;
                            graph2.Directed = false;
                        }
                    }
                }
                //bind the graph to the viewer 
                viewer2.Graph = graph2;
                //associate the viewer with the form 

                viewer2.Dock = DockStyle.Fill;
                tabSolution.Controls.Add(viewer2);


                Vertex<char> StartNode = WeightedDirectedGraph.getVertexFromTitle(startNode.SelectedItem + "");
                Vertex<char> GoalNode = WeightedDirectedGraph.getVertexFromTitle(goalNode.SelectedItem + "");

                switch (algorithmType.SelectedItem.ToString())
                {
                    case "Depth-First Search":
                        doDFS(StartNode, GoalNode);
                        break;
                    case "Depth-Limited Search":
                        doDLS(StartNode, GoalNode, (int)depthInput.Value);
                        break;
                    case "Iterative Deepening Search":
                        doIDS(StartNode, GoalNode, graphSample.Count);
                        break;
                    case "Breadth-First Search":
                        doBFS(StartNode, GoalNode);
                        break;
                    case "Uniform-Cost Search":
                        doUCS(StartNode, GoalNode);
                        break;
                }
            }
            else
            {
                initBIDIGraph();
                //initBIDIMainGraph();
                string StartNode = startNode.SelectedItem + "";
                string GoalNode = goalNode.SelectedItem + "";

                doBIDI(StartNode, GoalNode);

            }
        }

        private void initBIDIGraph()
        {
            adjacencyMatrix = AdjacencyMatrixGraph.AdjacencyMatrix;
            network = AdjacencyMatrixGraph.Network;

            graph2 = new Graph("graph2");
            viewer2 = new GViewer();

            graph2.Directed = false;

            foreach (string edge in network)
            {
                string[] nodes = edge.Split(',');

                graph2.AddEdge(nodes[0], nodes[1]).Attr.ArrowheadAtTarget = ArrowStyle.None; // = Microsoft.Msagl.Drawing.Color.IndianRed;    //we want one edge

            }

            //bind the graph to the viewer 
            viewer2.Graph = graph2;
            //associate the viewer with the form 

            viewer2.Dock = DockStyle.Fill;
            tabSolution.Controls.Add(viewer2);
            //tabOriginal.Controls.Add(viewer2);
        }

        private void initBIDIGraphOriginal()
        {
            adjacencyMatrix = AdjacencyMatrixGraph.AdjacencyMatrix;
            network = AdjacencyMatrixGraph.Network;

            Graph mygraph = new Graph("graph");
            GViewer gviewer = new GViewer(); 

            foreach (string edge in network)
            {
                string[] nodes = edge.Split(',');

                mygraph.AddEdge(nodes[0], nodes[1]).Attr.ArrowheadAtTarget = ArrowStyle.None; // = Microsoft.Msagl.Drawing.Color.IndianRed;    //we want one edge

            }

            //bind the graph to the viewer 
            gviewer.Graph = mygraph;
            //associate the viewer with the form 

            gviewer.Dock = DockStyle.Fill;
            tabOriginal.Controls.Clear();
            tabOriginal.Controls.Add(gviewer);
        }

        private void initBIDIMaminGraph()
        {
            graph2 = new Graph("graph");
            viewer2 = new GViewer();
            adjacencyMatrix = AdjacencyMatrixGraph.AdjacencyMatrix;
            network = AdjacencyMatrixGraph.Network;

            graph2.Directed = false;

            foreach (string edge in network)
            {
                string[] nodes = edge.Split(',');

                graph2.AddEdge(nodes[0], nodes[1]).Attr.ArrowheadAtTarget = ArrowStyle.None; // = Microsoft.Msagl.Drawing.Color.IndianRed;    //we want one edge

            }

            //bind the graph to the viewer 
            viewer2.Graph = graph2;
            //associate the viewer with the form 

            viewer2.Dock = DockStyle.Fill;
            tabOriginal.Controls.Clear();
            tabOriginal.Controls.Add(viewer2);
            //tabOriginal.Controls.Add(viewer2);
        }

    }
}
