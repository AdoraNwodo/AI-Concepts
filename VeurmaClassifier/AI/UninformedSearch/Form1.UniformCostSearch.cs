using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veurma.Utility;
using Veurma.Utility.Graph;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Msagl.Drawing;

namespace VeurmaClassifier
{
    public partial class Form1
    {
        public void doUCS(Vertex<char> start, Vertex<char> goal)
        {
            PriorityQueueResult res = new PriorityQueueResult();
            StringBuilder sb = new StringBuilder();
            Vertex<char> node;
            int step = 1;
            string Path;

            UCSProperties u = new UCSProperties();

            PriorityQueue queue = new PriorityQueue();
            u.Path = start.ToString();
            u.Node = start;
            //queue.Enqueue(start, 0);
            queue.Enqueue(u, 0);

            while (queue.Count > 0)
            {
                res = queue.Dequeue();
              
                u = (UCSProperties)res.Node;
                Path = u.Path;
                node = u.Node;
                //node = (Vertex<char>)res.Node;
                colorNodeInUI(node, step);
                step++;

                sb.Append("\nDequeuing " + node.Data);
                AISearchLog.Text = sb.ToString();
                if (node.Data.ToString() == goal.Data.ToString())
                {
                    #region Maniplate UI Thread for animation effect
                    //use multithreading to update the GUI (for the visualization effect.
                    //change the shape of the goal node to a diamond
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

                    sb.Append("\nFound. Cost is " + res.Priority);
                    sb.Append("\nPath: " + Path);

                    string[] nodes = Path.Split(' ');

                    #region Maniplate UI Thread for animation effect
                    //change color of path to green
                    Thread thread3 = new Thread(delegate ()
                    {
                        Thread.CurrentThread.IsBackground = true;
                        for(int i = 0; i < nodes.Length; i++)
                        {
                            graph2.FindNode(nodes[i]).Attr.FillColor = Color.LightGreen;    //change color of each node in path to green, so it's different from other nodes
                            viewer2.Graph = graph2;
                        }
                        
                    });
                    thread3.Start();

                    while (thread3.IsAlive)
                        Application.DoEvents();

                    #endregion

                    AISearchLog.Text = sb.ToString();
                    break;
                }
                Dictionary<Vertex<char>, int> neighbours = node.WeightedNeighbors;    //find neighbors (children)

                if (neighbours != null)
                {
                    foreach (KeyValuePair<Vertex<char>, int> neighbour in neighbours)
                    {
                        UCSProperties ucs = new UCSProperties();
                        ucs.Node = neighbour.Key;
                        ucs.Path = Path + " " + neighbour.Key.ToString();   //record path

                        sb.Append("\nEnqueuing " + neighbour.Key.Data);
                        AISearchLog.Text = sb.ToString();
                        
                        queue.Enqueue(ucs, neighbour.Value + (int)res.Priority);

                    }
                }
            }

            queue = null;
        }
    }
}
