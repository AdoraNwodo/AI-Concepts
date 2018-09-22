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

        string closedList;

        public int depth = 0;
        bool Found = false;
        Stack<Vertex<char>> openList;// = new Stack<Vertex<char>>();
        int step = 1;
        StringBuilder sb = new StringBuilder();


        public void doIDS(Vertex<char> start, Vertex<char> goal, int numOfNodes)
        {
            openList = new Stack<Vertex<char>>();
            closedList = "";

            // loops through until a goal node is found
            for (int _depth = 1; _depth < numOfNodes; _depth++)
            {
                Clear();
                bool found = DLS(start, goal, _depth);
                if (found)
                {
                    sb.Append(goal.ToString() + " was found.");
                    AISearchLog.Text = sb.ToString();
                    break;
                }
            }

            // this will never be reached as it 
            // loops forever until goal is found
            if (!Found)
            {
                sb.Append(goal.ToString() + "was not found.");
                AISearchLog.Text = sb.ToString();
            }
        }

        public bool DLS(Vertex<char> start, Vertex<char> goal, int _maximumDepth)
        {
            openList.Push(start);  //initialize

            while (openList.Count > 0 && !Found)    //open is not empty and maximum depth hasn't been reached
            {
                Vertex<char> node = openList.Pop();    //pop node, to push children of node 

                colorNodeInUI(node, step);
                step++;

                closedList = closedList + " " + node.Data.ToString();
                if (node.Data.ToString() == goal.Data.ToString())
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
                    Found = true;
                    break;

                }

                if (depth < _maximumDepth)
                {
                    Dictionary<Vertex<char>, int> neighbours = node.WeightedNeighbors;   //find neighbors (children)

                    if (neighbours != null)
                    {
                        depth++;    //increment depth since children are deeper into the tree (graph)

                        foreach (Vertex<char> neighbour in neighbours.Keys)
                        {
                            if (!closedList.Contains(neighbour.ToString()))
                                openList.Push(neighbour);   //put the children (unvisited) of n into open
                        }
                        Debug.Write("Failure");
                    }

                }
            }
            sb.Append("\nAt depth: " + _maximumDepth);
            sb.Append("\nClose List: "+closedList + "\n");
            AISearchLog.Text = sb.ToString();

            return Found;
        }

        private void Clear()
        {
            openList.Clear();
            closedList = "";
            depth = 0;
           // step = 1;
        }
    }
}
