using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


// COMPLETED 

namespace Kiselov_EXAM_Archive
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
            label1.Text =
                "2016, Huffman algorithm based archivator (c) \n" +
                "In computer science and information theory, \n" +
                "a Huffman code is a particular type of optimal prefix code \n" +
                "that is commonly used for lossless data compression. \n" +
                "The process of finding and/or using such a code proceeds by means \n" +
                "of Huffman coding, an algorithm developed by David A. Huffman \n" +
                "while he was a Ph.D. student at MIT, and published in the 1952 paper \n" +
                "'A Method for the Construction of Minimum - Redundancy Codes'. \n" +
                "The output from Huffman's algorithm can be viewed as a variable-length \n" +
                "code table for encoding a source symbol (such as a character in a file). \n" +
                "The algorithm derives this table from the estimated probability or \n" +
                "frequency of occurrence (weight) for each possible value of the \n" +
                "source symbol. As in other entropy encoding methods, more common \n" +
                "symbols are generally represented using fewer bits than less common \n" +
                "symbols. Huffman's method can be efficiently implemented, finding a code \n" +
                "in time linear to the number of input weights if these weights are sorted. \n" +
                "However, although optimal among methods encoding symbols separately, \n" +
                "Huffman coding is not always optimal among all compression methods.";
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
