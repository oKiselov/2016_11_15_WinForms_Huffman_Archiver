using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kiselov_EXAM_Archive.Huffman_Algorithm_Items;

namespace Kiselov_EXAM_Archive
{
    class FileOperator
    {
        // events send information to form about current state of convetrted oblect 
        public delegate void ProgressDelegte(object sender, ProgressEventArgs progressEvent);

        public event ProgressDelegte ReadProcess;

        /// <summary>
        /// field for safe multithread operations  
        /// </summary>
        private object threadLock = new object();

        /// <summary>
        /// Field and property for size of read bytes 
        /// </summary>
        private long iCounterRead;

        public long ReadCounter
        {
            get
            {
                lock (threadLock)
                {
                    return iCounterRead;
                }
            }
            set
            {
                lock (threadLock)
                {
                    iCounterRead = value;
                }
            }
        }

        /// <summary>
        /// Field and property for file size 
        /// </summary>
        private long iCounterSizeOfFile;

        public long FileSize
        {
            get
            {
                lock (threadLock)
                {
                    return iCounterSizeOfFile;
                }
            }
            private set
            {
                lock (threadLock)
                {
                    iCounterSizeOfFile = value;
                }
            }
        }

        /// <summary>
        /// Field and property for amount (in percent) of read bytes 
        /// </summary>
        private int lPercent;

        public int ReadCounterPercent
        {
            get
            {
                lock (threadLock)
                {
                    return lPercent;
                }
            }
            private set
            {
                lock (threadLock)
                {
                    lPercent = value;
                }
            }
        }

        public FileOperator()
        {
            FileSize = 0;
            ReadCounter = 0;
            ReadCounterPercent = 0;
        }

        /// <summary>
        /// Method for increasion of field with percent 
        /// of read bytes 
        /// </summary>
        /// <returns></returns>
        private bool IncreaseReadCounterPercent()
        {
            bool bRet = false;
            double lPercentProcess = (double)ReadCounter / FileSize * 100;
            if ((lPercentProcess - ReadCounterPercent) >= 5)
            {
                lPercent += 5;
                bRet = true;
            }
            return bRet;
        }

        public byte[] ReadFile(string strPathFrom)
        {
            byte[] arrBytes = new byte[(int)SizeCluster.Four];
            try
            {
                using (FileStream fs = new FileStream(strPathFrom, FileMode.Open, FileAccess.Read))
                {
                    FileSize = fs.Length;

                    if (FileSize < arrBytes.Length)
                    {
                        arrBytes = new byte[FileSize];
                    }
                    // sets cursor into needed position 
                    fs.Seek(ReadCounter, SeekOrigin.Begin);

                    if (ReadCounter < FileSize)
                    {
                        int iRead = fs.Read(arrBytes, 0, arrBytes.Length);
                        if (iRead == 0)
                        {
                            return arrBytes;
                        }

                        ReadCounter += iRead;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return arrBytes;
        }

        public bool WriteFile(byte[] arrBytes, string strPathTo)
        {
            try
            {
                FileInfo fi = new FileInfo(strPathTo);
                // variables for demonstation of difference between states of stream 
                // before and after copy process 
                long iSizeOfStreamBefore, iSizeOfStreamAfter;
                using (FileStream fs = new FileStream(strPathTo, FileMode.Append, FileAccess.Write))
                {
                    iSizeOfStreamBefore = fi.Length;
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(arrBytes);
                }
                fi = new FileInfo(strPathTo);
                iSizeOfStreamAfter = fi.Length;

                // addition of event 
                // Each event will send information about current state of FileOperator object - 
                // Amount of read bytes  
                if (IncreaseReadCounterPercent())
                {
                    ReadProcess(this, new ProgressEventArgs(ReadCounterPercent));
                }

                // inspection for difference between states of stream 
                // before and after copy process + length of copied array of bytes  
                // if there is 0 - its true => copy process was successfull 
                return (iSizeOfStreamAfter - iSizeOfStreamBefore) == arrBytes.Length;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Method converts BitArray's collection to array of bytes 
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        public byte[] BitArrayToByteArray(BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }

        /// <summary>
        /// Method receives array of current drives and empty TreeNodeCollection's object 
        /// Then method goes through all drives 
        /// And starts creation of folder's tree for each drive 
        /// </summary>
        /// <param name="arrStringsDrives"></param>
        /// <param name="nodeCollection"></param>
        public void GetDriveTree(string[] arrStringsDrives, TreeNodeCollection nodeCollection)
        {
            foreach (var drive in arrStringsDrives)
            {
                try
                {
                    BuildTree(new DirectoryInfo(drive), nodeCollection);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Method goes through all drives and directories recursively 
        /// Sends information about each node in folder tree 
        /// to TreeNodeCollection's object
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="addInMe"></param>
        private void BuildTree(DirectoryInfo directoryInfo, TreeNodeCollection addInMe)
        {
            // Addition of the new node 
            TreeNode currNode = addInMe.Add(directoryInfo.Name);

            // through all subdirs 
            foreach (DirectoryInfo subdirInfo in directoryInfo.GetDirectories())
            {
                try
                {
                    BuildTree(subdirInfo, currNode.Nodes);
                }
                catch
                {
                }
            }
            // through all files in subdir 
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                try
                {
                    currNode.Nodes.Add(file.Name);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Method for reading array of bytes from current file
        /// Returns array of bytes  
        /// </summary>
        /// <param name="strPathToFile"></param>
        /// <returns></returns>
        public byte[] ReadFileFull(string strPathToFile)
        {
            // Check arguments.
            if (string.IsNullOrEmpty(strPathToFile))
            {
                throw new Exception("Plain path for reading file for encryption/decryprion");
            }

            byte[] arrBytesFromFileRet;
            using (FileStream fs = new FileStream(strPathToFile, FileMode.Open, FileAccess.Read))
            {
                fs.Position = 0;
                BinaryReader binReaderFromFile = new BinaryReader(fs);
                arrBytesFromFileRet = binReaderFromFile.ReadBytes((int)fs.Length);
            }
            return arrBytesFromFileRet;
        }

        /// <summary>
        /// Method for writing array of encorypted/decrypted bytes to new file 
        /// </summary>
        /// <param name="strPathToFile"></param> - new files name 
        /// <param name="arrBytes"></param> - array of encorypted/decrypted bytes 
        /// <returns></returns>
        public bool SaveNewFile(string strPathToFile, byte[] arrBytes)
        {
            // Check arguments.
            if (string.IsNullOrEmpty(strPathToFile))
            {
                throw new Exception("Plain path for new file");
            }
            using (FileStream fs = new FileStream(strPathToFile, FileMode.Append, FileAccess.Write))
            {
                BinaryWriter binWriterToFile = new BinaryWriter(fs);
                binWriterToFile.Write(arrBytes);
            }
            return true;
        }
    }
}
