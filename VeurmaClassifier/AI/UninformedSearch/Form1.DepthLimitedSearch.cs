using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veurma.Utility.Graph;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Msagl.Drawing;

namespace VeurmaClassifier
{
    public partial class Form1
    {
        public void doDLS(Vertex<char> startNode, Vertex<char> goalNode, int maxDepth)
        {
            StringBuilder sb = new StringBuilder();
            int step = 1;

            bool found = false;
            openStackList = new Stack<Vertex<char>>();
            closedStackList = "";

            openStackList.Push(startNode);   //initialize
            int depth = 0;

            while (openStackList.Count > 0 && !found && depth < maxDepth)     //open is not empty and maximum depth hasn't been reached
            {
                Vertex<char> node = openStackList.Pop();    //pop node, to push children of node 
                colorNodeInUI(node, step);
                step++;

                closedStackList = closedStackList + " " + node.Data.ToString();
                if (node.Data.ToString() == goalNode.Data.ToString())
                {
                    //use multithreading to update the GUI (for the visualization effect.
                    //change the shape of the goal node to a diamond

                    #region Maniplate UI Thread for animation effect
                    Thread thread2 = new Thread(delegate ()
                    {
                        Thread.CurrentThread.IsBackground = true;
                        Thread.Sleep(1000);
                        graph2.FindNode(node.ToString()).Attr.Shape = Shape.Diamond;     //change goal node's shape, so it's different from other nodes
                        viewer2.Graph = graph2;
                    });
                    thread2.Start();

                    while (thread2.IsAlive)
                        Application.DoEvents();
                    #endregion

                    Debug.Write("Success");
                    sb.Append("\nNode " + goalNode.Data.ToString() + " found at level " + depth + "!");     //print that node is found
                    AISearchLog.Text = sb.ToString();
                    found = true;
                    break;

                }
                sb.Append("\nNode " + node.Data.ToString() + " at level " + depth + ", " + goalNode.Data.ToString() + " not yet found at this level");   //print out properties
                AISearchLog.Text = sb.ToString();

                Dictionary<Vertex<char>, int> neighbours = node.WeightedNeighbors;    //find neighbors (children)

               

                if (neighbours != null)
                {
                    depth++;    //increment depth since children are deeper into the tree (graph)

                    foreach (Vertex<char> neighbour in neighbours.Keys)
                    {
                        if (!closedStackList.Contains(neighbour.ToString()))
                            openStackList.Push(neighbour);   //put the children (unvisited) of n into open
                    }
                    Debug.Write("Failure");
                }


            }

            sb.Append("\nClosed set: " + closedStackList);
            AISearchLog.Text = sb.ToString();
        }
    }
}
