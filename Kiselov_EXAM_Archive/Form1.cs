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
    public partial class Form1 : Form
    {
        // field - object, which was created for operations with files 
        FileOperator fileOperator;

        // array will save 
        List<string> arrPathesToFilesInFolder; 

        public Form1()
        {
            fileOperator = new FileOperator();
            InitializeComponent();
            arrPathesToFilesInFolder = new List<string>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            treeView1.BeginUpdate();

            string[] drives = Directory.GetLogicalDrives();
            fileOperator.GetDriveTree(drives, treeView1.Nodes);

            treeView1.EndUpdate();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            listView1.Items.Clear();
            // save pathes to files in current directory 
            arrPathesToFilesInFolder.Clear();

            DirectoryInfo directoryInfo = new DirectoryInfo(e.Node.FullPath);
            try
            {
                if (!directoryInfo.Exists)
                {
                    FileInfo file = new FileInfo(e.Node.FullPath);
                    listView1.Items.Add(
                        new ListViewItem(new string[]
                        {
                        string.Format(file.Name),
                        string.Format(file.Extension),
                        string.Format(file.Length.ToString()),
                        string.Format(file.CreationTimeUtc.ToString())
                        }));
                    arrPathesToFilesInFolder.Add(file.FullName);
                }
                else
                {
                    if (directoryInfo.GetFiles() != null)
                    {
                        foreach (FileInfo file in directoryInfo.GetFiles())
                        {
                            listView1.Items.Add(
                                new ListViewItem(new string[]
                                {
                                string.Format(file.Name),
                                string.Format(file.Extension),
                                string.Format(file.Length.ToString()),
                                string.Format(file.CreationTimeUtc.ToString())
                                }));
                            arrPathesToFilesInFolder.Add(file.FullName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }
        }

        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            listView1.Items.Clear();
        }

        /// <summary>
        /// Method adds file to existed archive 
        /// User should choose the file from folder tree 
        /// and press this button 
        /// Application will save pointed file into the empty archive 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                string strPathToSelectedFile = string.Empty;

                foreach (int i in listView1.SelectedIndices)
                {
                    strPathToSelectedFile = listView1.Items[i].Text;
                }

                List<FileInfo> listFileInfos = new List<FileInfo>();
                foreach (string strPath in arrPathesToFilesInFolder)
                {
                    listFileInfos.Add(new FileInfo(strPath));
                }

                string strPathFromList = string.Empty;
                for (int i = 0; i < listFileInfos.Count; i++)
                {
                    if (strPathToSelectedFile == listFileInfos[i].Name)
                    {
                        strPathFromList = listFileInfos[i].FullName;
                    }
                }

                if (new FileInfo(strPathFromList).Length != 0)
                {
                    throw new Exception("Please choose empty archive");
                }

                // MessageBox.Show(strPathFromList);
                AddArchive addArchive = new AddArchive();
                addArchive.strPathToFile = strPathFromList; 
                addArchive.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            CreateEmpty createEmptyArchive = new CreateEmpty();
            createEmptyArchive.ShowDialog();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            try
            {
                string strPathToSelectedFile = string.Empty;

                foreach (int i in listView1.SelectedIndices)
                {
                    strPathToSelectedFile = listView1.Items[i].Text;
                }

                List<FileInfo> listFileInfos = new List<FileInfo>();
                foreach (string strPath in arrPathesToFilesInFolder)
                {
                    listFileInfos.Add(new FileInfo(strPath));
                }

                string strPathFromList = string.Empty;
                for (int i = 0; i < listFileInfos.Count; i++)
                {
                    if (strPathToSelectedFile == listFileInfos[i].Name)
                    {
                        strPathFromList = listFileInfos[i].FullName;
                    }
                }

                if (new FileInfo(strPathFromList).Length == 0 || !new FileInfo(strPathFromList).Extension.Equals(".haak"))
                {
                    throw new Exception("Please choose archive");
                }

                // MessageBox.Show(strPathFromList);

                DecodeArchive decoded = new DecodeArchive();
                decoded.strPathToFile = strPathFromList; 
                decoded.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        /// <summary>
        /// button for visiting website with information about Huffman algorithm 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            GetWebBrowser web = new GetWebBrowser();
            web.ShowDialog(); 
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.ShowDialog(); 
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            try
            {
                string strPathToSelectedFile = string.Empty;

                foreach (int i in listView1.SelectedIndices)
                {
                    strPathToSelectedFile = listView1.Items[i].Text;
                }

                List<FileInfo> listFileInfos = new List<FileInfo>();
                foreach (string strPath in arrPathesToFilesInFolder)
                {
                    listFileInfos.Add(new FileInfo(strPath));
                }

                string strPathFromList = string.Empty;
                for (int i = 0; i < listFileInfos.Count; i++)
                {
                    if (strPathToSelectedFile == listFileInfos[i].Name)
                    {
                        strPathFromList = listFileInfos[i].FullName;
                    }
                }

                if (new FileInfo(strPathFromList).Length == 0 || !new FileInfo(strPathFromList).Extension.Equals(".haak"))
                {
                    throw new Exception("Please choose archive");
                }

                // MessageBox.Show(strPathFromList);

                DeleteForm deleted = new DeleteForm();
                deleted.strPathToFile = strPathFromList;
                deleted.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
