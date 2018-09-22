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
        public Stack<Vertex<char>> openStackList;
        public string closedStackList;
       

        public void doDFS(Vertex<char> startNode, Vertex<char> goalNode)
        {
            bool found = false;
            openStackList = new Stack<Vertex<char>>();
            closedStackList = "";

            openStackList.Push(startNode);   //initialize

            int step = 1;   //algorithm step
            StringBuilder sb = new StringBuilder();

            while (openStackList.Count > 0 && !found)    //open is not empty
            {
                sb.Append("\n\nOpen:");
                AISearchLog.Text = sb.ToString();

                foreach (Vertex<char> n in openStackList)
                {
                    sb.Append(" " + n.Data.ToString()); //print out open set(stack)
                    AISearchLog.Text = sb.ToString(); 
                }

                Vertex<char> node = openStackList.Pop();    //pop node, to push children of node 

                colorNodeInUI(node, step);
                step++;

                closedStackList = closedStackList + " " + node.Data.ToString();

                //if N is goal node
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
                    sb.Append("\nNode " + goalNode.Data.ToString() + " found!");
                    AISearchLog.Text = sb.ToString();
                    found = true;
                    break;
                }
                sb.Append("\n\nOpen:");
                AISearchLog.Text = sb.ToString();
                foreach (Vertex<char> n in openStackList)
                {
                    sb.Append(" " + n.Data.ToString());
                    AISearchLog.Text = sb.ToString();
                }

                sb.Append("\nClosed: " + closedStackList + "\nFound? : " + found + "\nCurrent Node : " + node.Data.ToString()); //print out DFS properties. closed set, state and current node
                AISearchLog.Text = sb.ToString();

                sb.Append("\nNode's Unvisited Children :");

                if (node.WeightedNeighbors != null)
                {
                    foreach (KeyValuePair<Vertex<char>, int> n in node.WeightedNeighbors)
                    {
                        if (!closedStackList.Contains(n.Key.Data.ToString()))
                        {
                            sb.Append(" " + n.Key.Data.ToString());     //print out current node neighbors (that have not been visited)
                            AISearchLog.Text = sb.ToString();
                        }
                    }
                }

                Dictionary<Vertex<char>, int> neighbours = node.WeightedNeighbors;    //find neighbors (children)

                if (neighbours != null)
                {
                    foreach (KeyValuePair<Vertex<char>, int> neighbour in neighbours)
                    {
                        if (!closedStackList.Contains(neighbour.Key.ToString()))
                            openStackList.Push(neighbour.Key);   //put the children (unvisited) of n into open
                    }
                    Debug.Write("Failure");
                }


            }
            sb.Append("\nClosed set: " + closedStackList);
            AISearchLog.Text = sb.ToString();
        }
    }
    
}
