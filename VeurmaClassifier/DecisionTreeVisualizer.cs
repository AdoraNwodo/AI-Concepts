using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VeurmaClassifier
{
    public partial class DecisionTreeVisualizer : Form
    {
        private DecisionTree tree;
        private AttributeType decisionVariableType;
        private Dictionary<string, AttributeListInfo> attributes;
        private GViewer viewer;
        private Graph graph;

        public DecisionTreeVisualizer(DecisionTree dt, AttributeType dvt, Dictionary<string, AttributeListInfo> attr)
        {
            InitializeComponent();
            this.MinimumSize = new System.Drawing.Size(500, 500);
            this.MaximizeBox = false;
            this.CenterToParent();
            tree = dt;
            decisionVariableType = dvt;
            attributes = attr;

            viewer = new GViewer();
            graph = new Graph("graph");

            drawTree();
        }

        private string Revert(string currString)
        {
            if (decisionVariableType == AttributeType.Discrete)
            {
                if (currString == null)
                    return currString;

                string[] splitString = currString.Split(new string[] { "==" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitString.Length > 1)
                {
                    string textEquivOfNumber = attributes[splitString[0].Trim()].Inputs[Int32.Parse(splitString[1].Trim())];
                    return splitString[0] + " == " + textEquivOfNumber.Trim();
                }
                else
                {
                    return currString;
                }

            }
            else
            {
                if (currString == null)
                    return currString;

                string[] splitString = currString.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (splitString.Length > 1)
                {
                    string textEquivofCaption = attributes.Keys.ElementAt(Int32.Parse(splitString[0].Trim()));

                    return textEquivofCaption + " " + splitString[1].Trim() + " " + splitString[2].Trim();
                }
                else
                {
                    return currString;
                }
            }
        }

        private string decodeClass(string currString)
        {
            int currNumber;
            if (Int32.TryParse(currString, out currNumber))
            {
                if (currNumber < 0)
                    return "undefined";

                string textEquiv = attributes.Keys.ElementAt(attributes.Count - 1);
                string textEquivOfNumber = attributes[textEquiv].Inputs[currNumber];
                return textEquivOfNumber;
            }else
            {
                return currString;
            }
        }

        private string decode(string key, string sign, double? value)
        {

            string[] splitString = key.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            if (attributes.ContainsKey(splitString[0]))
            {
                if (attributes[splitString[0].Trim()].Inputs[0].ToString() == "numeric" || attributes[splitString[0].Trim()].Inputs[0].ToString() == "real" || attributes[splitString[0].Trim()].Inputs.Count == 0)
                {
                    return sign + " " + value;
                }
                else
                {
                    return sign + "" + attributes[splitString[0].Trim()].Inputs[(int)value];
                }
            }else
            {
                try
                {
                    if (splitString.Length > 1)
                    {
                        //string textEquivofCaption = attributes.Keys.ElementAt(Int32.Parse(splitString[0].Trim()));

                        return /*textEquivofCaption + " " + */ sign + " " + splitString[2].Trim();
                    }
                    else
                    {
                        return key;
                    }


                }
                catch(Exception e)
                {
                    return null;
                }
            }

        }

        private void drawTree()
        {
            IEnumerable<Node> traversal = tree.GetEnumerable();
            int counter = 0;
            foreach (Node node in traversal)
            {
                counter++;
                if (node.IsLeaf)
                {

                    graph.AddEdge(Revert(node.Parent + ""), decode(node + "", node.Comparison, node.Value), Revert(node + "") + " " + counter);
                    Microsoft.Msagl.Drawing.Node found = graph.FindNode(Revert(node + "") + " " + counter);
                    
                    found.Attr.FillColor = Microsoft.Msagl.Drawing.Color.Aqua;
                    found.Attr.Shape = Shape.Ellipse;
                    found.LabelText = decodeClass(node.Output + "");
                }
                else if (node.IsRoot)
                {
                    Microsoft.Msagl.Drawing.Node rootNode = new Microsoft.Msagl.Drawing.Node(Revert(node + ""));
                    graph.AddNode(rootNode);   
                    rootNode.LabelText = attributes.ElementAt(node.Branches.Index).Key;
                    rootNode.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightPink;
                    rootNode.Attr.Shape = Shape.Box;
                }
                else
                {
                    graph.AddEdge(Revert(node.Parent + ""), decode(node+"", node.Comparison, node.Value), Revert(node + ""));
                    Microsoft.Msagl.Drawing.Node found = graph.FindNode(Revert(node + ""));
                    found.LabelText = attributes.ElementAt(node.Branches.Index).Key;
                    found.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightPink;
                    found.Attr.Shape = Shape.Box;

                }
            }

            //bind the graph to the viewer 
            viewer.Graph = graph;
            viewer.BackColor = System.Drawing.Color.White;
            viewer.ForeColor = viewer.BackColor;
            //associate the viewer with the form 

            viewer.Dock = DockStyle.Fill;
            this.Controls.Add(viewer);



        }

        private void DecisionTreeVisualizer_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.CenterToScreen();
        }
    }
}
