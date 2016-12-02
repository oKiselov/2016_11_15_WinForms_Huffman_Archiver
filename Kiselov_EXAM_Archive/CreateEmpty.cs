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

namespace Kiselov_EXAM_Archive
{
    public partial class CreateEmpty : Form
    {
        FileOperator fileOperator;

        public CreateEmpty()
        {
            InitializeComponent();
            fileOperator = new FileOperator();
        }

        /// <summary>
        /// Button Browse 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // method opens the dialog window with right to choose necessary directory 
                FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
                DialogResult result = folderBrowser.ShowDialog();
                if (!string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
                {
                    textBox1.Text = folderBrowser.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // inspect textbox1 for its empty value 
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    throw new Exception("The textbox with path to file is empty");
                }

                FileInfo fileInfoBase = new FileInfo(textBox1.Text + Huffman_Algorithm_Items.HuffmanTree.strExtension);

                // if pointed file doesn't exist - exception 
                if (fileInfoBase.Exists)
                {
                    throw new Exception("File with such adress has already existed");
                }

                if (string.IsNullOrEmpty(fileInfoBase.Name))
                {
                    throw new Exception("Enter correct name of file");
                }

                if (!Directory.Exists(fileInfoBase.DirectoryName))
                {
                    throw new Exception("Enter correct name of directory");
                }

                if (fileOperator.WriteFile(new byte[] {}, fileInfoBase.FullName))
                {
                    MessageBox.Show(@"Archive created successfully");
                }
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
