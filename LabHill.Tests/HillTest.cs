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
        private string decoded3x3 = "cipher";
        private string encoded3x3 = "jcqint";
        private string key3x3 = "hillciphe";

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
        public void Encrypt_String_3x3()
        {
            InitializeEnglishAlphabet();
            using (var algorithm = new CryptoHill(key3x3, Alphabet))
            {
                Assert.Matches(algorithm.Encrypt(decoded3x3), encoded3x3);
            }
        }

        [Fact]
        public void Decrypt_String_3x3()
        {
            InitializeEnglishAlphabet();
            using (var algorithm = new CryptoHill(key3x3, Alphabet))
            {
                Assert.Matches(algorithm.Decrypt(encoded3x3), decoded3x3);
            }
        }

        [Fact]
        public void Encrypt_String_4x4()
        {
            InitializeEnglishAlphabet();
            using (var algorithm = new CryptoHill(key4x4, Alphabet))
            {
                Assert.Matches(algorithm.Encrypt(decoded3x3), encoded3x3);
            }
        }

        [Fact]
        public void Decrypt_String_4x4()
        {
            InitializeEnglishAlphabet();
            using (var algorithm = new CryptoHill(key4x4, Alphabet))
            {
                Assert.Matches(algorithm.Decrypt( decoded3x3), decoded3x3);
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
            string errorKey = "denislox";
            using (var algorithm = new CryptoHill(errorKey, Alphabet))
            {
                Assert.Throws<InvalidKeyLengthException>(() => algorithm.Decrypt("jcqint"));
            }
        }
        #endregion
    }
}
