using LabHill;
using System.Collections.Generic;
using LabHill.Exceptions;
using Xunit;

namespace LabHill.Tests
{
    public class HillTest
    {
        #region Initialize
        private List<char> Alphabet { get; set; }
        private List<int> keyAlph = new() { 3, 2, 8, 5 };
        private List<int> decodedAlph = new () { 15, 0, 24, 12, 14, 17, 4, 12, 14, 13, 4, 24 };
        private List<int> encodedAlph = new() { 19, 16, 18, 18, 24, 15, 10, 14, 16, 21, 8, 22 };

        private List<int> keyAlph3x3 = new() { 1, 10, 0, 0, 20, 1, 2, 15, 2 };
        private List<int> decodedAlph3x3 = new () { 5, 21, 2, 5, 2, 16, 19, 14, 1 };
        private List<int> encodedAlph3x3 = new () { 7, 6, 17, 25, 4, 20, 3, 21, 16 };
        

        private string decodedStr = "cipher";

        private string key2x2 = "dcif";
        private string encoded2x2 = "wehzun";

        private string key3x3 = "hillciphe";
        private string encoded3x3 = "jcqint";

        private string key4x4 = "paymoremonessssy";
        //private string encoded4x4 = "";
        internal void InitializeEnglishAlphabet()
        {
            Alphabet = new List<char>();
            int i = 0;
            for (char c = 'a'; c <= 'z'; ++c)
            {
                Alphabet.Add(c);
                i++;
            }
        }
        #endregion

        #region Encrypting and Decrypting List<int>
        [Fact]
        public void Encrypt_Int_2x2()
        {
            InitializeEnglishAlphabet();
            using (var algorithm = new CryptoHill(keyAlph, Alphabet))
            {
                Assert.Equal(algorithm.Encrypt(decodedAlph), encodedAlph);
            }
        }

        [Fact]
        public void Decrypt_Int_2x2()
        {
            InitializeEnglishAlphabet();
            using (var algorithm = new CryptoHill(keyAlph, Alphabet))
            {
                Assert.Equal(algorithm.Decrypt(encodedAlph), decodedAlph);
            }
        }

        [Fact]
        public void Encrypt_Int_3x3()
        {
            InitializeEnglishAlphabet();
            using (var algorithm = new CryptoHill(keyAlph3x3, Alphabet))
            {
                Assert.Equal(algorithm.Encrypt(decodedAlph3x3), encodedAlph3x3);
            }
        }

        [Fact]
        public void Decrypt_Int_3x3()
        {
            InitializeEnglishAlphabet();
            using (var algorithm = new CryptoHill(keyAlph3x3, Alphabet))
            {
                Assert.Equal(algorithm.Decrypt(encodedAlph3x3), decodedAlph3x3);
            }
        }

        #endregion

        #region Encrypting and Decrypting String
        [Fact]
        public void Encrypt_String_2x2()
        {
            InitializeEnglishAlphabet();
            using (var algorithm = new CryptoHill(key2x2, Alphabet))
            {
                Assert.Matches(algorithm.Encrypt(decodedStr), encoded2x2);
            }
        }

        [Fact]
        public void Decrypt_String_2x2()
        {
            InitializeEnglishAlphabet();
            using (var algorithm = new CryptoHill(key2x2, Alphabet))
            {
                Assert.Matches(algorithm.Decrypt(encoded2x2), decodedStr);
            }
        }

        [Fact]
        public void Encrypt_String_3x3()
        {
            InitializeEnglishAlphabet();
            using (var algorithm = new CryptoHill(key3x3, Alphabet))
            {
                Assert.Matches(algorithm.Encrypt(decodedStr), encoded3x3);
            }
        }

        [Fact]
        public void Decrypt_String_3x3()
        {
            InitializeEnglishAlphabet();
            using (var algorithm = new CryptoHill(key3x3, Alphabet))
            {
                Assert.Matches(algorithm.Decrypt(encoded3x3), decodedStr);
            }
        }

        [Fact]
        public void Encrypt_String_4x4()
        {
            InitializeEnglishAlphabet();
            using (var algorithm = new CryptoHill(key4x4, Alphabet))
            {
                Assert.Matches(algorithm.Encrypt(decodedStr), decodedStr);
            }
        }

        [Fact]
        public void Decrypt_String_4x4()
        {
            InitializeEnglishAlphabet();
            using (var algorithm = new CryptoHill(key4x4, Alphabet))
            {
                Assert.Matches(algorithm.Decrypt(decodedStr), decodedStr);
            }
        }

#endregion

        #region Analyse Testing

        [Fact]
        public void Analyse_String_2x2()
        {
            InitializeEnglishAlphabet();
            using (var algorithm = new CryptoHill(decodedStr, Alphabet))
            {
                Assert.Matches(algorithm.Analyse(encoded2x2, 2, decodedStr), key2x2);
            }
        }

        [Fact]
        public void Analyse_String_3x3()
        {
            InitializeEnglishAlphabet();
            using (var algorithm = new CryptoHill(decodedStr, Alphabet))
            {
                Assert.Matches(algorithm.Analyse(encoded3x3, 3, decodedStr), key3x3);
            }
        }

        [Fact]
        public void Analyse_Int_2x2()
        {
            InitializeEnglishAlphabet();
            using (var algorithm = new CryptoHill(decodedStr, Alphabet))
            {
                Assert.Matches(algorithm.AlpNumberToString(algorithm.Analyse(encodedAlph, 2, decodedAlph)), algorithm.AlpNumberToString(keyAlph));
            }
        }

        [Fact]
        public void Analyse_Int_3x3()
        {
            InitializeEnglishAlphabet();
            using (var algorithm = new CryptoHill(decodedStr, Alphabet))
            {
                Assert.Matches(algorithm.AlpNumberToString(algorithm.Analyse(encodedAlph3x3, 2, decodedAlph3x3)), algorithm.AlpNumberToString(keyAlph3x3));
            }
        }

        #endregion

        #region Exception Testing
        [Fact]
        public void Encrypt_InvalidKey_InvalidKeyException()
        {
            InitializeEnglishAlphabet();
            string errorKey = "hdls";
            using (var algorithm = new CryptoHill(errorKey, Alphabet))
            {
                //Doesn't work because there's no 
                Assert.Throws<InvalidKeyException>(() => algorithm.Encrypt("cipher"));
            }
        }

        [Fact]
        public void Encrypt_InvalidKey_InvalidKeyLengthException()
        {
            InitializeEnglishAlphabet();
            string errorKey = "lololololssdsdsdsdsdsddsdsdsdsdsdsds";
            using (var algorithm = new CryptoHill(errorKey, Alphabet))
            {
                Assert.Throws<InvalidKeyLengthException>(() => algorithm.Encrypt("cipsdsher"));
            }
        }

        [Fact]
        public void Decrypt_InvalidKey_InvalidKeyException()
        {
            InitializeEnglishAlphabet();
            string errorKey = "hils"; //has 3^2 size, but it's wrong because matrixes doesn't come up
            using (var algorithm = new CryptoHill(errorKey, Alphabet))
            {
                //Doesn't work because there's no 
                Assert.Throws<InvalidKeyException>(() => algorithm.Decrypt("jcqisddsnt"));
            }
        }

        [Fact]
        public void Decrypt_InvalidKey_InvalidKeyLengthException()
        {
            InitializeEnglishAlphabet();
            string errorKey = "denisloxkujgb";
            using (var algorithm = new CryptoHill(errorKey, Alphabet))
            {
                Assert.Throws<InvalidKeyLengthException>(() => algorithm.Decrypt("jcqint"));
            }
        }
        #endregion
    }
}
