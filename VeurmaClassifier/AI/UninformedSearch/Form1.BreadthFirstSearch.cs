using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeurmaClassifier;
using Veurma.Utility.Graph;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Msagl.Drawing;

namespace VeurmaClassifier
{
    public partial class Form1
    {

        public Queue<Vertex<char>> openQueueList;
        public string closedQueueList;

        public void doBFS(Vertex<char> startNode, Vertex<char> goalNode)
        {
            openQueueList = new Queue<Vertex<char>>();
            closedQueueList = "";

            int step = 1;   //algorithm step
            StringBuilder sb = new StringBuilder();
            openQueueList.Enqueue(startNode);   //initialize


            bool found = false;
            sb.Append("Open: " + startNode.Data.ToString());

            AISearchLog.Text = sb.ToString();

            while (openQueueList.Count > 0 && !found)    //open is not empty
            {
                sb.Append("\n\nOpen:");
                AISearchLog.Text = sb.ToString();
                foreach (Vertex<char> n in openQueueList)
                {
                    sb.Append(" " + n.Data.ToString()); //print out open set(queue)
                    AISearchLog.Text = sb.ToString();
                }

                Vertex<char> node = openQueueList.Dequeue();    //pop node, to push children of node 

                //use multithreading to update the GUI (for the visualization effect.
                //color each node in the closed set
                colorNodeInUI(node, step);
                step++;


                closedQueueList = closedQueueList + " " + node.Data.ToString();

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
                foreach (Vertex<char> n in openQueueList)
                {
                    sb.Append(" " + n.Data.ToString());
                    AISearchLog.Text = sb.ToString();
                }
                sb.Append("\nClosed: " + closedQueueList + "\nFound? : " + found + "\nCurrent Node : " + node.Data.ToString()); //print out BFS properties. closed set, state and current node
                AISearchLog.Text = sb.ToString();

                sb.Append("\nNode's Unvisited Children :");
                if (node.WeightedNeighbors != null)
                {
                    foreach (KeyValuePair<Vertex<char>, int> n in node.WeightedNeighbors)
                    {
                        if (!closedQueueList.Contains(n.Key.Data.ToString()))
                        {
                            sb.Append(" " + n.Key.Data.ToString());     //print out current node neighbors (that have not been visited)
                            AISearchLog.Text = sb.ToString();
                        }
                    }
                }

                Dictionary<Vertex<char>, int> neighbours = node.WeightedNeighbors;    //find neighbors (children)

                string nodeData = node.ToString();

                if (neighbours != null)
                {
                    foreach (KeyValuePair<Vertex<char>, int> neighbour in neighbours)
                    {
                        if (!closedQueueList.Contains(neighbour.Key.ToString()))
                        {
                            openQueueList.Enqueue(neighbour.Key);   //put the children (unvisited) of n into open

                        }

                    }
                    Debug.Write("Failure");
                }


            }

            sb.Append("\nClosed set: " + closedQueueList);
            AISearchLog.Text = sb.ToString();
        }
    }
}
