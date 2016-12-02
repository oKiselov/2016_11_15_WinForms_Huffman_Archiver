using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kiselov_EXAM_Archive.Huffman_Algorithm_Items
{
    public class HuffmanTree
    {
        // events send information to form about current state of convetrted oblect 
        public delegate void ProgressDelegte(object sender, ProgressEventArgs progressEvent);

        public event ProgressDelegte ArchiveProcess;

        /// <summary>
        /// field for safe multithread operations  
        /// </summary>
        private object threadLock = new object();

        /// <summary>
        /// Field and property for size of read bytes 
        /// </summary>
        private long iCounterCode;

        public long CodeCounter
        {
            get
            {
                lock (threadLock)
                {
                    return iCounterCode;
                }
            }
            private set
            {
                lock (threadLock)
                {
                    iCounterCode = value;
                }
            }
        }

        /// <summary>
        /// Field and property for file size 
        /// </summary>
        private long iCounterSizeOfArray;

        public long ArraySize
        {
            get
            {
                lock (threadLock)
                {
                    return iCounterSizeOfArray;
                }
            }
            private set
            {
                lock (threadLock)
                {
                    iCounterSizeOfArray = value;
                }
            }
        }

        /// <summary>
        /// Field and property for amount (in percent) of read bytes 
        /// </summary>
        private int lPercent;

        public int CodeCounterPercent
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
        
        public static string strExtension = string.Format(".haak");

        // Binary tree with nodes. Each node is one byte 
        private List<HuffmanNode> nodes = new List<HuffmanNode>();
        // Root node 
        public HuffmanNode Root { get; set; }
        // Table of symbols(bytes) and amount of their repetitions 
        public Dictionary<byte, int> Frequencies = new Dictionary<byte, int>();

        public HuffmanTree()
        {
            ArraySize = 0;
            CodeCounter = 0;
            CodeCounterPercent = 0; 
        }


        /// <summary>
        /// Method for increasion of field with percent 
        /// of read bytes 
        /// </summary>
        /// <returns></returns>
        private bool IncreaseCopyCounterPercent()
        {
            bool bRet = false;
            double lPercentProcess = (double)CodeCounter / ArraySize * 100;
            if ((lPercentProcess - CodeCounterPercent) >= 5)
            {
                lPercent += 5;
                bRet = true;
            }
            return bRet;
        }

        /// <summary>
        /// Method goes through all bytes and create binary tree and table of frequency 
        /// </summary>
        /// <param name="source"></param>
        public void BuildHuffmanTree(byte[] source)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (!Frequencies.ContainsKey(source[i]))
                {
                    Frequencies.Add(source[i], 0);
                }

                Frequencies[source[i]]++;
            }

            foreach (KeyValuePair<byte, int> symbol in Frequencies)
            {
                nodes.Add(new HuffmanNode() { Symbol = symbol.Key, Frequency = symbol.Value });
            }

            while (nodes.Count > 1)
            {
                List<HuffmanNode> orderedNodes = nodes.OrderBy(node => node.Frequency).ToList<HuffmanNode>();

                if (orderedNodes.Count >= 2)
                {
                    // Take first two items
                    List<HuffmanNode> taken = orderedNodes.Take(2).ToList<HuffmanNode>();

                    // Create a parent node by combining the frequencies
                    HuffmanNode parent = new HuffmanNode()
                    {
                        Symbol = Encoding.Unicode.GetBytes("*")[0],
                        Frequency = taken[0].Frequency + taken[1].Frequency,
                        Left = taken[0],
                        Right = taken[1]
                    };

                    nodes.Remove(taken[0]);
                    nodes.Remove(taken[1]);
                    nodes.Add(parent);
                }

                this.Root = nodes.FirstOrDefault();
            }
        }

        /// <summary>
        /// Method restores tree after saving file 
        /// </summary>
        /// <param name="Frequencies"></param>
        public void RestoreHuffmanTree(Dictionary<byte, int> Frequencies)
        {
            foreach (KeyValuePair<byte, int> symbol in Frequencies)
            {
                nodes.Add(new HuffmanNode() { Symbol = symbol.Key, Frequency = symbol.Value });
            }

            while (nodes.Count > 1)
            {
                List<HuffmanNode> orderedNodes = nodes.OrderBy(node => node.Frequency).ToList<HuffmanNode>();

                if (orderedNodes.Count >= 2)
                {
                    // Take first two items
                    List<HuffmanNode> taken = orderedNodes.Take(2).ToList<HuffmanNode>();

                    // Create a parent node by combining the frequencies
                    HuffmanNode parent = new HuffmanNode()
                    {
                        Symbol = Encoding.Unicode.GetBytes("*")[0],
                        Frequency = taken[0].Frequency + taken[1].Frequency,
                        Left = taken[0],
                        Right = taken[1]
                    };

                    nodes.Remove(taken[0]);
                    nodes.Remove(taken[1]);
                    nodes.Add(parent);
                }

                this.Root = nodes.FirstOrDefault();
            }
        }

        public BitArray Encode(byte[] source)
        {
            CodeCounter = 0;
            ArraySize = source.Length; 
            List<bool> encodedSource = new List<bool>();

            for (int i = 0; i < source.Length; i++)
            {
                List<bool> encodedSymbol = this.Root.Traverse(source[i], new List<bool>());
                encodedSource.AddRange(encodedSymbol);

                CodeCounter++;
                if (IncreaseCopyCounterPercent())
                {
                    ArchiveProcess(this, new ProgressEventArgs(CodeCounterPercent));
                }
            }

            BitArray bits = new BitArray(encodedSource.ToArray());

            return bits;
        }

        public byte[] Decode(BitArray bits)
        {
            CodeCounter = 0;
            ArraySize = bits.Length;

            HuffmanNode current = this.Root;
            byte[] decoded = new byte[0];

            foreach (bool bit in bits)
            {
                if (bit)
                {
                    if (current.Right != null)
                    {
                        current = current.Right;
                    }
                }
                else
                {
                    if (current.Left != null)
                    {
                        current = current.Left;
                    }
                }

                if (IsLeaf(current))
                {
                    //decoded += current.Symbol;

                    Array.Resize(ref decoded, decoded.Length + 1);
                    decoded[decoded.Length - 1] = current.Symbol;
                    current = this.Root;
                }

                CodeCounter++;
                if (IncreaseCopyCounterPercent())
                {
                    ArchiveProcess(this, new ProgressEventArgs(CodeCounterPercent));
                }
            }

            return decoded;
        }

        public bool IsLeaf(HuffmanNode node)
        {
            return (node.Left == null && node.Right == null);
        }
    }
}