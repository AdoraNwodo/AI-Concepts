using ArffTools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ZedGraph;

namespace VeurmaClassifier
{
    public partial class Form4 : Form
    {
        private double[][] Inputs;
        private int[] Actual;
        private int[] Expected;
        private ContingencyTable contingencyTable;
        public List<Color> colorList = ColorStructToList();
        private List<ArffAttribute> attributeList;
        private List<string> attributeStringList;


        public Form4(double[][] inputs, int[] actual, int[] expected, List<ArffAttribute> aL)
        {
            attributeStringList = new List<string>();
            colorList.Reverse();
            this.Size = new Size(664, 550);
            this.CenterToScreen();
            this.MaximizeBox = false;
            this.MaximumSize = new Size(664, 550);
            Inputs = inputs;
            Actual = actual;
            Expected = expected;
            contingencyTable = new ContingencyTable(actual, expected, 1);  // 0);
            attributeList = aL;
            foreach (ArffAttribute attr in attributeList)
                attributeStringList.Add(attr.Name);

            InitializeComponent();
            setBindingSources(comboBox1);
            setBindingSources(comboBox2);


            FunctionPlot(graphInput, inputs, expected.ConvertToDouble(), actual.ConvertToDouble(), aL[0].Name, aL[0].Name);
        }

        private double[] intToDouble(int[] i)
        {
            return i.Select(Convert.ToDouble).ToArray();
        }

        public static List<Color> ColorStructToList()
        {
            List<Color> allColors =  typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public)
                                        .Select(c => (Color)c.GetValue(null, null)) //select dark colors
                                        .ToList();
            List<Color> darkColors = new List<Color>();
            foreach(Color color in allColors)
            {
                if (color.GetBrightness() > 0.4)
                    darkColors.Add(color);
            }

            return darkColors;
        }

        public void FunctionPlot(ZedGraphControl zgc, double[][] inputs, double[] expected, double[] output, string xAxis, string yAxis)
        {
            try
            {
                int x = attributeStringList.IndexOf(xAxis);
                int y = attributeStringList.IndexOf(yAxis);
                GraphPane myPane = zgc.GraphPane;
                myPane.CurveList.Clear();

                // Set the titles
                myPane.Title.IsVisible = false;
                myPane.XAxis.Title.Text = xAxis;
                myPane.YAxis.Title.Text = yAxis;

                string[] classes = attributeList[attributeList.Count - 1].Type.ToString().Split(',');
                List<string> classes2 = new List<string>();

                foreach (string s in classes)
                {
                    classes2.Add(s.Replace("{", "").Replace("}", ""));
                }

                int numberOfElements = output.Distinct().Count();   //+ num of elements
                PointPairList[] list = new PointPairList[numberOfElements * 2]; //1 for OK, 1 for Error

                for (int i = 0; i < output.Length; i++)
                {
                    for (int j = 0; j < numberOfElements; j++)
                    {
                        if (output[i] == j)
                        {
                            if (expected[i] == j)
                            {
                                list[j] = new PointPairList();
                                list[j].Add(inputs[i][x], inputs[i][y]);
                            }
                            else
                            {
                                list[j + numberOfElements] = new PointPairList();

                                list[j + numberOfElements].Add(inputs[i][x], inputs[i][y]);
                            }
                        }
                    }
                }
                // Add the curve

                for (int i = 0; i < classes2.Count; i++)
                {
                    LineItem
                    myCurve = myPane.AddCurve(classes2[i], list[i], colorList[i + 5], SymbolType.Diamond);   //hit
                    myCurve.Line.IsVisible = false;
                    myCurve.Symbol.Border.IsVisible = false;
                    myCurve.Symbol.Fill = new Fill(colorList[i + 5]);

                    myCurve = myPane.AddCurve(classes2[i] + " - miss", list[i + numberOfElements], colorList[i + numberOfElements + 5], SymbolType.Diamond);   //miss
                    myCurve.Line.IsVisible = false;
                    myCurve.Symbol.Border.IsVisible = false;
                    myCurve.Symbol.Fill = new Fill(colorList[i + numberOfElements + 5]);
                }

                myPane.Fill = new Fill(Color.WhiteSmoke);

                zgc.AxisChange();
                zgc.Invalidate();
            }catch(Exception e)
            {
                MessageBox.Show("Plot Unavailable for this dataset");
            }
        }

        public void FunctionPlot(ZedGraphControl zgc, double[][] inputs, double[] expected, double[] output, int x, int y)
        {
            //x is x axis, y is y axis

            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();

            // Set the titles
            myPane.Title.IsVisible = false;
            string[] classes = attributeList[attributeList.Count - 1].Type.ToString().Split(',');
            List<string> classes2 = new List<string>();

            foreach (string s in classes)
            {
                classes2.Add(s.Replace("{", "").Replace("}", ""));  //list of classes - (in my case is dynamic.)
            }

            myPane.XAxis.Title.Text = attributeStringList[x];
            myPane.YAxis.Title.Text = attributeStringList[y];

            int numberOfElements = output.Distinct().Count();   //+ num of elements
            PointPairList[] list = new PointPairList[numberOfElements * 2]; //1 for OK, 1 for Error

            for (int i = 0; i < output.Length; i++)
            {
                for (int j = 0; j < numberOfElements; j++)
                {
                    if (output[i] == j)
                    {
                        if (expected[i] == j)
                        {
                            list[j] = new PointPairList();
                            list[j].Add(inputs[i][x], inputs[i][y]);
                        }
                        else
                        {
                            list[j + numberOfElements] = new PointPairList();
                            list[j + numberOfElements].Add(inputs[i][x], inputs[i][y]);
                        }
                    }
                }
            }


            // Add the curve
            for (int i = 0; i < classes2.Count; i++)
            {
                LineItem
                myCurve = myPane.AddCurve(classes2[i], list[i], colorList[i + 5], SymbolType.Diamond);   //hit
                myCurve.Line.IsVisible = false;
                myCurve.Symbol.Border.IsVisible = false;
                myCurve.Symbol.Fill = new Fill(colorList[i + 5]);

                myCurve = myPane.AddCurve(classes2[i] + " - miss", list[i + numberOfElements], colorList[i + numberOfElements + 5], SymbolType.Diamond);   //miss
                myCurve.Line.IsVisible = false;
                myCurve.Symbol.Border.IsVisible = false;
                myCurve.Symbol.Fill = new Fill(colorList[i + numberOfElements + 5]);
            }

            myPane.Fill = new Fill(Color.WhiteSmoke);

            zgc.AxisChange();
            zgc.Invalidate();
        }

        private void setBindingSources(ComboBox cb)
        {
            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = attributeStringList;
            cb.DataSource = bindingSource;
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //x axis
            FunctionPlot(graphInput, Inputs, Expected.ConvertToDouble(), Actual.ConvertToDouble(), comboBox1.SelectedIndex, comboBox2.SelectedIndex);

        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //y axis
            FunctionPlot(graphInput, Inputs, Expected.ConvertToDouble(), Actual.ConvertToDouble(), comboBox1.SelectedIndex, comboBox2.SelectedIndex);
        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }
    }
}
