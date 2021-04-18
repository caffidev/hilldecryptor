using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using LabHill.Exceptions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace LabHill
{
    /// <summary>
    /// The <see cref="List{int}"/> is row based. Which means
    /// that the key is given in row-based manner.
    /// <para>It can be used to cryptographic operations, such as encrypting/decrypting information
    /// with help of Hill cryptographic method.</para>
    /// </summary>
    public class CryptoHill : IDisposable
    {
        //Used key
        private List<int> Key { get; }
        private readonly List<char> alphabet;

        #region Constructor and destructor

        /// <summary>
        /// Default constructor, but with string. Key is mandatory.
        /// </summary>
        /// <param name="key">Key in <see cref="string"/> type, that converts then into <para>
        /// <see cref="List{int}"/> and then can be get (not set) by <see cref="Key"/> property.</para></param>
        public CryptoHill(string keyStr, List<char> alphabet)
        {
            this.alphabet = alphabet;
            Key = StringToAlpNumber(keyStr);
        }

        /// <summary>
        /// Default constructor. Key is mandatory.
        /// </summary>
        /// <param name="key">Key in <see cref="List{int}"/></param>
        public CryptoHill(List<int> key, List<char> alphabet)
        {
            this.alphabet = alphabet;
            Key = key;
        }

        /// <summary>
        /// Replaces destructor. GC is freeing the memory. (it's not necessary however)
        /// --todo: use ~destructor along with IDisposable
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Usual Methods

        /// <summary>
        /// Analyses 3x3 and 2x2 matrixes to find a key with help of known decrypted and encrypted text
        /// </summary>
        /// <param name="dec"></param>
        /// <param name="enc"></param>
        /// <param name="keyLength">The Length of The Key in Square Root</param>
        /// <returns></returns>
        public List<int> Analyse(List<int> dec, List<int> enc, int keyLength)
        {
            List<double> decD = dec.ConvertAll(x => (double) x);
            List<double> encD = enc.ConvertAll(x => (double) x);

            int square = (int) Math.Sqrt(decD.Count);
            Matrix<double> decMatrix = DenseMatrix.OfColumnMajor(
                square, (int) dec.Count / square, decD.AsEnumerable());
            Matrix<double> encMatrix = DenseMatrix.OfColumnMajor(
                square, (int) enc.Count / square, encD.AsEnumerable());

            //Constructing List
            List<int> mayBeKey = new List<int>();

            //HERE YOU NEED TO PASTE UNIVERSAL ALGORITHM
            //1. 2x2
            if (keyLength == 2)
            {
                for (int i = 0; i < 26; i++)
                {
                    for (int j = 0; j < 26; j++)
                    {
                        for (int k = 0; k < 26; k++)
                        {
                            for (int l = 0; l < 26; l++)
                            {
                                mayBeKey = new List<int>(new[] {i, j, k, l});
                                List<int> aa = Encrypt(dec, mayBeKey);
                                if (aa.SequenceEqual(enc))
                                {
                                    return mayBeKey;
                                }

                            }
                        }
                    }
                }
                throw new InvalidAnalysisException(
                    "Key was not found.");
            }
            else if (keyLength == 3) // 2. 3x3
            {
                Matrix<double> keyMatrix = DenseMatrix.Create(3, 3, 0);
                decMatrix = ModInMinorCofactor(decMatrix.Transpose(), DetMatrix(encMatrix));
                keyMatrix = (encMatrix * decMatrix);
                mayBeKey = keyMatrix.Transpose().Enumerate().ToList().Select(i => (int)i % alphabet.Count).ToList();
                return mayBeKey;
            }
            //END
            else
            {
                throw new InvalidAnalysisException(
                    "Invalid length of the key. This program doesn't support 4x4 and higher matrixes.");
            }

        }

        /// <summary>
        /// Method that encrypts information using Hill cryptographic method.
        /// Does not work with 4x4 matrixes.
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        public List<int> Encrypt(List<int> dec, List<int> key = null)
        {
            if (key == null) key = Key;
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
                    square, (int) dec.Count / square, decD.AsEnumerable());
                List<int> finalResult = new List<int>();

                //Checking if keyMatrix is different whenever it's float or integer. If numbers are not equal, return.
                if (Math.Abs((int) keyMatrix[0, 0]).ToString() != Math.Abs((double) keyMatrix[0, 0]).ToString())
                {
                    throw new InvalidKeyException("Invalid key. Key matrix determinant does not have modular multiplicative inverse.");
                }

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
            //doesn't work with 2^4 and higher routes
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidKeyLengthException(
                    "Invalid length of the key. It should be able to convert into root square.");
            }
        }

        public List<int> Decrypt(List<int> enc, List<int> key = null)
        {
            if (key == null) key = Key;
            List<double> keyD = key.ConvertAll(x => (double) x);
            List<double> encD = enc.ConvertAll(x => (double) x);

            int square = Convert.ToInt32(Math.Sqrt(key.Count));

            try
            {
                //I'm using Math.Net here, that creates Matrix for me, so I don't have to
                //create a lot of arrays just to contain them
                Matrix<double> keyMatrix = DenseMatrix.OfColumnMajor(
                    square, key.Count / square, keyD.AsEnumerable());
                Matrix<double> encMatrix = DenseMatrix.OfColumnMajor(
                    square, enc.Count / square, encD.AsEnumerable());
                List<int> finalResult = new List<int>();
                //todo: doesn't work
                if (Math.Abs((int)keyMatrix[0, 0]).ToString() != Math.Abs(keyMatrix[0, 0]).ToString())
                {
                    throw new InvalidKeyException("Invalid key. Key matrix determinant does not have modular multiplicative inverse.");
                }

                //4x4 and over matrixes don't work
                if (keyMatrix.ColumnCount == 3)
                {
                    keyMatrix = ModInMinorCofactor(keyMatrix.Transpose(), DetMatrix(keyMatrix));
                }
                else // for 2x2; don't know how to make >=4x4
                {
                    keyMatrix = keyMatrix.Inverse();
                    Console.WriteLine(keyMatrix.ToString());
                    Console.WriteLine(((int) keyMatrix[0, 0]) + ", " + ((int) keyMatrix[0, 0]).ToString());
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
            catch (ArgumentOutOfRangeException)
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

            foreach (char c in keyStr)
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
        /// Not mine. Thanks to zeyadetman.
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
        /// Not mine. Thanks to zeyadetman.
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

        #region Override methods

        /// <summary>
        /// <inheritdoc cref="Encrypt(System.Collections.Generic.List{int})"/>
        /// </summary>
        /// <param name="decString"></param>
        /// <returns></returns>
        public string Encrypt(string decString)
        {
            return AlpNumberToString(Encrypt(StringToAlpNumber(decString)));
        }

        /// <summary>
        /// <inheritdoc cref="Analyse(System.Collections.Generic.List{int})"/>
        /// </summary>
        /// <param name="decString"></param>
        /// <returns></returns>
        public string Analyse(string decString, string encString, int keyLength)
        {
            return AlpNumberToString(Analyse(StringToAlpNumber(decString), StringToAlpNumber(encString), keyLength));
        }

        /// <summary>
        /// <inheritdoc cref="Decrypt(System.Collections.Generic.List{int})"/>
        /// </summary>
        /// <param name="decString"></param>
        /// <returns></returns>
        public string Decrypt(string encString)
        {
            return AlpNumberToString(Decrypt(StringToAlpNumber(encString)));
        }

        //Better to use async and ConcurrentQueue to get a better performance
        /// <summary>
        /// <inheritdoc cref="Encrypt(System.Collections.Generic.List{int})"/>
        /// </summary>
        /// <param name="decStrings"></param>
        /// <returns></returns>
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
                Decrypt(encStr);
                decStrings.Add(encStr);
            }
            return decStrings;
        }
        #endregion

    }
}