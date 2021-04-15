using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using LabHill.Exceptions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace LabHill
{
    /// <summary>
    /// The <see cref="List{int}"/>&lt;<see cref="int"/>&gt; is row based. Which means
    /// that the key is given in row-based manner.
    /// </summary>
    public class CryptoHill : IDisposable
    {
        //Used key
        private readonly List<int> key;
        private readonly List<char> alphabet;

        #region Constructor and destructor

        /// <summary>
        /// Default constructor. Needed key
        /// </summary>
        /// <param name="key"></param>
        public CryptoHill(string keyStr, List<char> alphabet)
        {
            this.alphabet = alphabet;
            key = StringToAlpNumber(keyStr);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Usual Methods
        //Algorithm was made by Сосочек бога(Denis)
        public List<int> Encrypt(List<int> dec)
        {
            try
            {
                //List<int> key => List<double> keyD with help of the ListConverter
                List<double> keyD = key.ConvertAll(x => (double) x);
                //Converting int[] text => double[] text (List)
                List<double> decD = dec.ConvertAll(x => (double) x);
                //Getting the square root number of the length in List<int>
                //Pointless to use keyD because it just slows down the program
                int square = Convert.ToInt32(Math.Sqrt(key.Count));
                //Creating a matrix for a key using Math.Net
                Matrix<double> keyMatrix = DenseMatrix.OfColumnMajor(
                    square, key.Count / square, keyD.AsEnumerable());
                //Creating a matrix for a decrypted text(int) using Math.Net
                Matrix<double> decMatrix = DenseMatrix.OfColumnMajor(
                    square, dec.Count / square, decD.AsEnumerable());
                List<int> finalResult = new List<int>();
                //Hill method
                for (int i = 0; i < decMatrix.ColumnCount; i++)
                {
                    List<double> res = new List<double>();
                    int alpNumber = alphabet?.Count ?? 26;
                    res = (((decMatrix.Column(i)).ToRowMatrix() * keyMatrix) % alpNumber).Enumerate().ToList();

                    for (int j = 0; j < res.Count; j++)
                    {
                        finalResult.Add((int) res[j]);
                    }
                }

                return finalResult;
            }
            catch (ArgumentOutOfRangeException arg)
            {
                throw new InvalidKeyLengthException(
                    "Invalid length of the key. It should be able to convert into root square.");
            }
        }

        public List<int> Decrypt(List<int> enc)
        {
            List<double> keyD = key.ConvertAll(x => (double) x);
            List<double> encD = enc.ConvertAll(x => (double) x);

            int square = Convert.ToInt32(Math.Sqrt(key.Count));

            try
            {
                Matrix<double> keyMatrix = DenseMatrix.OfColumnMajor(
                    square, key.Count / square, keyD.AsEnumerable());
                Matrix<double> encMatrix = DenseMatrix.OfColumnMajor(
                    square, enc.Count / square, encD.AsEnumerable());
                List<int> finalResult = new List<int>();


                //Inversed code. Doesn't work yet.
                if (keyMatrix.ColumnCount == 3)
                {
                    keyMatrix = ModInMinorCofactor(keyMatrix.Transpose(), DetMatrix(keyMatrix));
                }
                else
                {
                    keyMatrix = keyMatrix.Inverse();
                    Console.WriteLine(keyMatrix.ToString());
                    Console.WriteLine(((int) keyMatrix[0, 0]) + ", " + ((int) keyMatrix[0, 0]).ToString());
                }

                if (Math.Abs((int) keyMatrix[0, 0]).ToString() != Math.Abs((double) keyMatrix[0, 0]).ToString())
                {
                    throw new InvalidKeyException("Invalid key. Try using another.");
                }

                //Hill method (with matrix)
                for (int i = 0; i < encMatrix.ColumnCount; i++)
                {
                    List<double> res = new List<double>();
                    res = ((encMatrix.Column(i).ToRowMatrix() * keyMatrix) % 26).Enumerate().ToList();
                    for (int j = 0; j < res.Count; j++)
                    {
                        int x = (int) res[j] >= 0 ? (int) res[j] : (int) res[j] + (alphabet?.Count ?? 26);
                        finalResult.Add(x);
                    }
                }

                return finalResult;
            }
            catch (ArgumentOutOfRangeException arg)
            {
                throw new InvalidKeyLengthException(
                    "Invalid length of the key. It should be able to convert into root square.");
            }
        }

        #endregion

        #region Additional Methods
        public List<int> StringToAlpNumber(string keyStr)
        {
            List<int> key = new List<int>();

            foreach (char c in keyStr.ToCharArray())
            {
                key.Add(alphabet.IndexOf(c));
            }

            return key;
        }

        public string AlpNumberToString(List<int> keys)
        {
            string s = "";
            foreach (int k in keys)
            {
                s = string.Concat(s, alphabet[k].ToString());
            }

            return s;
        }

        /// <summary>
        /// Not mine.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public Matrix<double> ModInMinorCofactor(Matrix<double> m, int a)
        {
            Matrix<double> resultMatrix = DenseMatrix.Create(3, 3, 0.0);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int x = i == 0 ? 1 : 0, y = j == 0 ? 1 : 0, x1 = i == 2 ? 1 : 2, y1 = j == 2 ? 1 : 2;
                    double r = ((m[x,y] * m[x1, y1] - m[x,y1] * m[x1,y]) * Math.Pow(-1, i + j)*a)% alphabet.Count;
                    resultMatrix[i, j] = r >= 0 ? r : r + alphabet.Count;
                }
            }

            return resultMatrix;
        }

        /// <summary>
        /// Test code.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public int DetMatrix(Matrix<double> m)
        {
            double d = m[0, 0] * (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1]) -
                       m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0]) +
                       m[0, 2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);

            int di = (int) d % alphabet.Count >= 0
                ? (int) d % alphabet.Count
                : (int) d % alphabet.Count + alphabet.Count;

            for (int i = 0; i < alphabet.Count; i++)
            {
                if(di * i % alphabet.Count == 1)
                {
                    return i;
                }
            }

            return -1;
        }
        #endregion

        #region Overriden methods

        public string Encrypt(string decString)
        {
            return AlpNumberToString(Encrypt(StringToAlpNumber(decString)));
        }

        public string Decrypt(string encString)
        {
            return AlpNumberToString(Decrypt(StringToAlpNumber(encString)));
        }

        //Better to use async and ConcurrentQueue to get a better performance;
        public List<string> Encrypt(List<string> decStrings)
        {
            List<string> encStrings = new List<string>();
            foreach (string decStr in decStrings)
            {
                Encrypt(decStr);
                encStrings.Add(decStr);
            }
            return encStrings;
        }

        public List<string> Decrypt(List<string> encStrings)
        {
            List<string> decStrings = new List<string>();
            foreach (string encStr in encStrings)
            {
                Encrypt(encStr);
                decStrings.Add(encStr);
            }
            return decStrings;
        }
        #endregion

    }
}