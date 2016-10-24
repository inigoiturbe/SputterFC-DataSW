using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SputterFCDataTransfer
{
    public partial class Sec_Form : Form
    {
        public Sec_Form()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex<0)
            {
                MessageBox.Show("Please select a Section");
            }
            else
            {
                GV.SelSec = comboBox1.SelectedIndex;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void Sections_Form_Load(object sender, EventArgs e)
        {
            GV.SelSec = -1;
            dataGridView1.Rows.Clear();
            foreach(string[] row in GV.Sections)
            {
                dataGridView1.Rows.Add(row);
            }
            comboBox1.Items.Clear();
            foreach(String Sec in GV.SecNames)
            {
                comboBox1.Items.Add(Sec);
            }
            LabelsDistiance_Text.Text = (GV.LabelsDistance/1000).ToString();
        }
    }
}
