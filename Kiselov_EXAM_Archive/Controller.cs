using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kiselov_EXAM_Archive.Huffman_Algorithm_Items;

namespace Kiselov_EXAM_Archive
{
    public class Controller
    {

        public delegate void ProgressDelegte(object sender, ProgressEventArgs progressEvent);

        public event ProgressDelegte ArchiveProcess;

        HuffmanTree huffmanTree;
        FileOperator fileOperator;

        public Controller()
        {
            huffmanTree = new HuffmanTree();
            fileOperator = new FileOperator();
            // подписан на события 
            huffmanTree.ArchiveProcess += ShowReadProcess;
        }

        /// <summary>
        /// Переброска события из файлового оператора в дочернюю форму с отображением прогресс бара 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="progressEvent"></param>
        public void ShowReadProcess(object sender, ProgressEventArgs progressEvent)
        {
            if (sender is HuffmanTree)
            {
                int index = progressEvent.iCount;
                ArchiveProcess(this, new ProgressEventArgs(index));
            }
        }

        public void Encode(string strPathFrom, string strPathTo)
        {
            byte[] tempFileBytes = new byte[] {};

            tempFileBytes = fileOperator.ReadFileFull(strPathFrom);
            huffmanTree.BuildHuffmanTree(tempFileBytes);
            BitArray bitArrayEncodedFile = huffmanTree.Encode(tempFileBytes);
            byte[] arrBytesEncBytes = new byte[] { };
            arrBytesEncBytes = fileOperator.BitArrayToByteArray(bitArrayEncodedFile);
            fileOperator.SaveNewFile(strPathTo, arrBytesEncBytes);

            // тут уже файл запсан , но без древа 
            var binFormatter = new BinaryFormatter();
            var mStream = new MemoryStream();
            binFormatter.Serialize(mStream, huffmanTree.Frequencies);

            //This gives you the array.
            // это массив байтов,конвертованый с древа 
            // все вместе 
            byte[] newarrBytes = mStream.ToArray();
            // добавляем инту - длину массива - древа 
            int size = newarrBytes.Length;
            byte[] arrsize = BitConverter.GetBytes(size);
            // добавляем расширение файла 
            string strExtension = string.Format(".txt");
            byte[] arrExt = Encoding.Unicode.GetBytes(strExtension);

            int sizeOfExt = arrExt.Length;
            byte[] arrSizeOfExt = BitConverter.GetBytes(sizeOfExt); 


            newarrBytes = newarrBytes.Concat(arrsize).ToArray();
            newarrBytes = newarrBytes.Concat(arrExt).ToArray();
            newarrBytes = newarrBytes.Concat(arrSizeOfExt).ToArray();

            fileOperator.WriteFile(newarrBytes, strPathTo);

            MessageBox.Show("Successful");
        }

        public void Decode(string strPathFrom, string strPathTo)
        {
            byte[] tempFileBytes = new byte[] {};
            tempFileBytes = fileOperator.ReadFileFull(strPathFrom);

            // обрезаем и сохраняем длину расширения 
            int sizeAll = tempFileBytes.Length; 
            byte[] sizeOfExt = new byte[4];
            for (int i = 0; i < sizeOfExt.Length; i++)
            {
                sizeOfExt[sizeOfExt.Length - 1 - i] = tempFileBytes[sizeAll - 1 - i];
            }
            Array.Resize(ref tempFileBytes, tempFileBytes.Length-sizeOfExt.Length);
            int sizeOfExtension = BitConverter.ToInt32(sizeOfExt, 0);

            // обрезаем само расширение 
            byte [] extensionType = new byte[sizeOfExtension];
            for (int i = 0; i < extensionType.Length; i++)
            {
                extensionType[extensionType.Length - 1 - i] = tempFileBytes[tempFileBytes.Length - 1 - i];
            }
            Array.Resize(ref tempFileBytes, tempFileBytes.Length - sizeOfExtension);
            string strExtens = Encoding.Unicode.GetString(extensionType);

            // обрезаем и сохраняем длину древа  
            byte[] sizeOfTree = new byte[4];
            for (int i = 0; i < sizeOfTree.Length; i++)
            {
                sizeOfTree[sizeOfTree.Length - 1 - i] = tempFileBytes[tempFileBytes.Length - 1 - i];
            }
            Array.Resize(ref tempFileBytes, tempFileBytes.Length - sizeOfTree.Length);
            int sizeOfHuffmanTree = BitConverter.ToInt32(sizeOfTree, 0);

            // обрезаем само древо 
            byte [] arrHuffmanTree = new byte[sizeOfHuffmanTree];
            for (int i = 0; i < arrHuffmanTree.Length; i++)
            {
                arrHuffmanTree[arrHuffmanTree.Length - 1 - i] = tempFileBytes[tempFileBytes.Length - 1 - i];
            }
            Array.Resize(ref tempFileBytes, tempFileBytes.Length-arrHuffmanTree.Length);

            var mStream = new MemoryStream();
            var binFormatter = new BinaryFormatter();

            // Where 'objectBytes' is your byte array.
            mStream.Write(arrHuffmanTree, 0, arrHuffmanTree.Length);
            mStream.Position = 0;

            var myObject = binFormatter.Deserialize(mStream) as Dictionary<byte, int>;

            huffmanTree.RestoreHuffmanTree(myObject);
            BitArray bitFromFile = new BitArray(tempFileBytes);

            byte[] arrBytesDecoded = huffmanTree.Decode(bitFromFile);
            fileOperator.SaveNewFile(strPathTo+= strExtens, arrBytesDecoded);

            MessageBox.Show("Successful");
        }
    }
}