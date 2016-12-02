using System.Collections.Generic;

namespace Kiselov_EXAM_Archive.Huffman_Algorithm_Items
{
    /// <summary>
    /// Class creates object - Node of tree 
    /// for operations with tree in Huffman algorithm 
    /// </summary>
    public class HuffmanNode
    {
        // field for each symbol 
        public byte Symbol { get; set; }
        // field saves info of frequency of using current symbol for this node 
        public int Frequency { get; set; }
        // children leafs for current parental node  
        public HuffmanNode Right { get; set; }
        public HuffmanNode Left { get; set; }

        /// <summary>
        /// Method receives current symbol and list of bool variables - data 
        /// Method goes through all needed nodes to leaf 
        /// Criteria of choose - sequence of bits in current byte   
        /// Returns list of bool vars - type of bit's sequence 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<bool> Traverse(byte symbol, List<bool> data)
        {
            // Leaf
            if (Right == null && Left == null)
            {
                if (symbol.Equals(this.Symbol))
                {
                    return data;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                List<bool> left = null;
                List<bool> right = null;

                if (Left != null)
                {
                    List<bool> leftPath = new List<bool>();
                    leftPath.AddRange(data);
                    leftPath.Add(false);

                    left = Left.Traverse(symbol, leftPath);
                }

                if (Right != null)
                {
                    List<bool> rightPath = new List<bool>();
                    rightPath.AddRange(data);
                    rightPath.Add(true);
                    right = Right.Traverse(symbol, rightPath);
                }

                if (left != null)
                {
                    return left;
                }
                else
                {
                    return right;
                }
            }
        }
    }
}
