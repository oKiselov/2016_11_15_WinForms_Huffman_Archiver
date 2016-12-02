using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kiselov_EXAM_Archive.Huffman_Algorithm_Items;

namespace Kiselov_EXAM_Archive
{
    public partial class AddArchive : Form
    {
        public string strPathToFile { get; set; }

        FileOperator fileOperator;

        Controller codeController; 
        public AddArchive()
        {
            InitializeComponent();
            codeController = new Controller();
            fileOperator = new FileOperator();
            // addition of event 
            // Each event will send information about current state of FileOperator object - 
            // Amount of read bytes  
            codeController.ArchiveProcess += ShowReadProcess;
        }

        /// <summary>
        /// Button starts addition process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //try
            //{
                // checking textbox1 for its empty value 
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    throw new Exception("The textbox with path to file is empty");
                }

                FileInfo fileInfoBase = new FileInfo(textBox1.Text);
                // if pointed file doesn't exist - exception 
                if (!fileInfoBase.Exists)
                {
                    throw new Exception("File with such adress does not exist");
                }

                byte[] arrBytesEncoded = new byte[] {};
                Action<string, string> action = codeController.Encode;
                IAsyncResult iAsyncResult = action.BeginInvoke(textBox1.Text, strPathToFile, null, null);

            
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        /// <summary>
        /// Each event will call this method 
        /// Current method will describe the state of progressbar using percents 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="progressEvent"></param>
        public void ShowReadProcess(object sender, ProgressEventArgs progressEvent)
        {
            if (sender is Controller)
            {
                if (progressBar1.Value < progressBar1.Maximum)
                {
                    progressBar1.Step = progressEvent.iCount - progressBar1.Value;
                    progressBar1.PerformStep();
                    if (progressBar1.Value == progressBar1.Maximum)
                    {
                        Thread.Sleep(1000);
                        MessageBox.Show("Archivation process successfully completed");
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = fileDialog.FileName;
            }
        }
    }
}
