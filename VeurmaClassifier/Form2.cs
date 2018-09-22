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
    public partial class Form2 : Form
    {

        private List<Query> queries;
        public double[] queryArray;

        public Form2(List<Query> _queries)
        {
            queries = _queries;
            queryArray = new double[queries.Count];
            this.CenterToParent();
            this.MaximumSize = new Size(300, 300);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            InitializeComponent();
            button1.TabStop = false;
            createControls();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void createControls()
        {
            ComboBox[] comboBoxes = new ComboBox[queries.Count];
            TextBox[] textBoxes = new TextBox[queries.Count];
            Label[] label = new Label[queries.Count];
            int labelX, labelY, textComboBoxX, textComboBoxY;

            labelX = 20;
            labelY = 20;
            textComboBoxX = 140;
            textComboBoxY = 20;

            for (int i = 0; i < queries.Count; i++)
            {
                queryArray[i] = 0;
                int a = i;

                if (queries[i].Options.Count <= 1 || queries[i].Options[0].ToString() == "numeric")
                {
                    textBoxes[i] = new TextBox();
                    textBoxes[i].Name = "dynamicTextBox" + i;
                    textBoxes[i].Location = new Point(textComboBoxX, textComboBoxY);
                    textBoxes[i].KeyUp += new KeyEventHandler((sender, e) => UpdateResultArrayTB(sender, e, a, textBoxes[a].Text.Trim()));
                    textBoxes[i].KeyPress += new KeyPressEventHandler((sender, e) => tb_OnKeyPress(sender, e));
                    this.Controls.Add(textBoxes[i]);
                    //input should only be numbers or and 1 decimal point
                }
                else
                {
                    comboBoxes[i] = new ComboBox();
                    comboBoxes[i].Name = "dynamicComboBox" + i;
                    comboBoxes[i].Location = new Point(textComboBoxX, textComboBoxY);
                    comboBoxes[i].SelectedIndexChanged += new EventHandler((sender, e) => UpdateResultArrayCB(sender, e, a, 
                        comboBoxes[a].SelectedValue+"", queries[a].Options));
                    BindingSource bindingSource = new BindingSource();
                    bindingSource.DataSource = queries[i].Options;
                    //options should be string, value should be number.
                    comboBoxes[i].DataSource = bindingSource;
                    comboBoxes[i].DropDownStyle = ComboBoxStyle.DropDownList;
                    this.Controls.Add(comboBoxes[i]);
                }


                label[i] = new Label();
                label[i].Name = "dynamicLabel" + i;
                label[i].Text = queries[i].QueryName;
                label[i].Location = new Point(labelX, labelY);
                this.Controls.Add(label[i]);

                labelY += 25;
                textComboBoxY += 25;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void UpdateResultArrayCB(object sender, EventArgs e, int i, string value, List<string> options)
        {
            queryArray[i] = options.FindIndex(a => a.Contains(value));
        }

        private void UpdateResultArrayTB(object sender, EventArgs e, int i, string value)
        {
            try
            {
                queryArray[i] = double.Parse(value);
            }catch(Exception ex)
            {
                MessageBox.Show("Please type in an integer or float");
            }
        }

        private void tb_OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
    }
}
