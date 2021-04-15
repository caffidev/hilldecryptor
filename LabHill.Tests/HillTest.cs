using LabHill;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LabHill.Exceptions;
using Xunit;

namespace LabHill.Tests
{
    public class HillTest
    {
        private List<char> Alphabet { get; set; }
        private List<int> keyAlph = new() { 3, 2, 8, 5 };
        List<int> decodedAlph = new () { 15, 0, 24, 12, 14, 17, 4, 12, 14, 13, 4, 24 };
        List<int> encodedAlph = new() { 19, 16, 18, 18, 24, 15, 10, 14, 16, 21, 8, 22 };

        private string decodedStr = "cipher";

        private string key2x2 = "dcif";
        private string encoded2x2 = "wehzun";

        private string key3x3 = "hillciphe";
        private string encoded3x3 = "jcqint";

        private string key4x4 = "hillcipherworker";
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
                Assert.Matches(algorithm.Decrypt( decodedStr), decodedStr);
            }
        }

        #region Exception Testing
        [Fact]
        public void Decrypt_InvalidKey_InvalidKeyException()
        {
            InitializeEnglishAlphabet();
            string errorKey = "denisloxx";
            using (var algorithm = new CryptoHill(errorKey, Alphabet))
            {
                //Doesn't work because there's no 
                Assert.Throws<InvalidKeyException>(() => algorithm.Decrypt("jcqint"));
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
