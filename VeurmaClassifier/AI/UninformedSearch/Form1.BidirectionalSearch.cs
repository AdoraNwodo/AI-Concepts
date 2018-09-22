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
using Veurma.Utility.Graph;

namespace VeurmaClassifier
{
    public partial class Form1
    {

        public void doBIDI(string startNode, string goalNode)
        {
            int edges, nodes, indexOfStartingNode,
            indexOfEndingNode, collision_node_1 = 0, collision_node_2 = 0,
            length = 0;

            nodes = adjMatrixGraphNodeList.Count;
            edges = network.Length;

            string route = "", collision_node_name = "";
            bool collision_found = false;
            int[] parent_node = new int[nodes];
            bool[] visited_node = new bool[nodes];
            string[] direction = new string[nodes];
            int[,] node_edge_node = adjacencyMatrix;
            string s = "";

            List<string> node_list = adjMatrixGraphNodeList;
            Queue<int> queue_start = new Queue<int>();
            Queue<int> queue_destination = new Queue<int>();
            StringBuilder sb = new StringBuilder();

            //init
            indexOfStartingNode = node_list.IndexOf(startNode);
            indexOfEndingNode = node_list.IndexOf(goalNode);
            queue_start.Enqueue(indexOfStartingNode);
            queue_destination.Enqueue(indexOfEndingNode);
            visited_node[indexOfStartingNode] = true;
            direction[indexOfStartingNode] = "forward";
            visited_node[indexOfEndingNode] = true;
            direction[indexOfEndingNode] = "backward";

            while (!collision_found)
            {
                if (!(queue_start.Count == 0))
                {
                    indexOfStartingNode = (int)queue_start.Dequeue();
                    for (int i = 0; i < nodes; i++)
                    {
                        if (node_edge_node[indexOfStartingNode, i] == 1 || node_edge_node[i, indexOfStartingNode] == 1)
                        {
                            if (visited_node[i] == false)
                            {
                                queue_start.Enqueue(i);
                                visited_node[i] = true;
                                parent_node[i] = indexOfStartingNode;
                                direction[i] = "forward";

                                # region Maniplate UI Thread for animation effect
                                Thread thread = new Thread(delegate ()
                                {
                                    Thread.CurrentThread.IsBackground = true;
                                    Thread.Sleep(2000);
                                    graph2.FindNode(node_list[indexOfStartingNode]).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;     //highlight a visited node, so it's different from other children
                                    Thread.Sleep(2000);
                                    graph2.FindNode(node_list[i]).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;     //highlight a visited node, so it's different from other children
                                    viewer2.Graph = graph2;
                                });

                                thread.Start();

                                while (thread.IsAlive)
                                    Application.DoEvents();
                                #endregion

                                sb.Append(node_list[indexOfStartingNode] + "----->" + node_list[i]);
                                sb.AppendLine();
                                AISearchLog.Text = sb.ToString();
                            }
                            else if (parent_node[indexOfStartingNode] != i && direction[i].Equals("backward"))
                            {
                                collision_found = true;
                                collision_node_name = node_list[i];
                                collision_node_1 = indexOfStartingNode;
                                collision_node_2 = i;
                                break;
                            }
                            else
                            {
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
                if (collision_found)
                    break;

                if (!(queue_destination.Count == 0))
                {
                    indexOfEndingNode = (int)queue_destination.Dequeue();
                    for (int i = 0; i < nodes; i++)
                    {
                        if (node_edge_node[indexOfEndingNode, i] == 1 || node_edge_node[i, indexOfEndingNode] == 1)
                        {
                            if (visited_node[i] == false)
                            {

                                #region Maniplate UI Thread for animation effect
                                Thread thread = new Thread(delegate ()
                                {
                                    Thread.CurrentThread.IsBackground = true;
                                    Thread.Sleep(3000);
                                    graph2.FindNode(node_list[indexOfEndingNode]).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;     //highlight a visited node, so it's different from other children
                                    graph2.FindNode(node_list[i]).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;     //highlight a visited node, so it's different from other children
                                    viewer2.Graph = graph2;
                                });

                                thread.Start();

                                while (thread.IsAlive)
                                    Application.DoEvents();

                                #endregion

                                queue_destination.Enqueue(i);
                                visited_node[i] = true;
                                direction[i] = "backward";
                                parent_node[i] = indexOfEndingNode;
                                sb.Append(node_list[indexOfEndingNode] + "----->" + node_list[i]);
                                sb.AppendLine();
                                AISearchLog.Text = sb.ToString();
                            }
                            else if (parent_node[indexOfEndingNode] != i && direction[i].Equals("forward"))
                            {
                                collision_found = true;
                                collision_node_name = node_list[i];
                                collision_node_1 = i;
                                collision_node_2 = indexOfEndingNode;
                                break;
                            }
                            else
                            {
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            if (collision_found)
            {
                string s1 = node_list[collision_node_2];
                while (!s1.Equals(goalNode))
                {
                    route = route + s1 + "--->";
                    collision_node_2 = parent_node[collision_node_2];
                    s1 = node_list[collision_node_2];
                    length++;
                }
                route = route + goalNode;

                s1 = node_list[collision_node_1];
                while (!s1.Equals(startNode))
                {
                    route = s1 + "--->" + route;
                    collision_node_1 = parent_node[collision_node_1];
                    s1 = node_list[collision_node_1];
                    length++;
                }
                route = startNode + "--->" + route;
                length++;

                sb.Append("Route: " + route);
                sb.AppendLine();
                sb.Append("Length: " + length);
                sb.AppendLine();
                sb.Append("Collision Node: " + collision_node_name);
                sb.AppendLine();
                AISearchLog.Text = sb.ToString();

                #region Maniplate UI Thread for animation effect
                //use multithreading to update the GUI (for the visualization effect.
                //change the shape of the goal node to a diamond
                Thread thread2 = new Thread(delegate ()
                {
                    string[] routeArray = route.Split(new string[] { "--->" }, StringSplitOptions.RemoveEmptyEntries);
                    Thread.CurrentThread.IsBackground = true;
                    Thread.Sleep(1000);

                    foreach (string r in routeArray)
                    {
                        graph2.FindNode(r).Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightGreen;     //change goal node's shape, so it's different from other nodes
                    }
                   
                    
                    viewer2.Graph = graph2;
                });
                thread2.Start();
                #endregion
            }
            else
            {
                sb.Append("No Collision found but list is empty");
                sb.AppendLine();
                AISearchLog.Text = sb.ToString();
            }

            Console.Read();


        }



    }
}
