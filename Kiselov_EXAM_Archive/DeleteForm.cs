using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


// COMPLETED 

namespace Kiselov_EXAM_Archive
{
    public partial class DeleteForm : Form
    {
        public string strPathToFile { get; set; }

        public DeleteForm()
        {
            InitializeComponent();
            label2.Text = strPathToFile; 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileInfo fileInfo = new FileInfo(strPathToFile);
            fileInfo.Delete();
            MessageBox.Show("File has been deleted successfully"); 
            Close();
        }
    }
}
