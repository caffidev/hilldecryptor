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
        private string key3x3 { get; set; } = "hillciphe";
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
            string decoded = "cipher";
            string encoded = "jcqint";
            using (var algorithm = new CryptoHill(key3x3, Alphabet))
            {
                Assert.Matches(algorithm.Encrypt(decoded), encoded);
            }
        }

        [Fact]
        public void Decrypt_String_3x3()
        {
            InitializeEnglishAlphabet();
            string decoded = "cipher";
            string encoded = "jcqint";
            using (var algorithm = new CryptoHill(key3x3, Alphabet))
            {
                Assert.Matches(algorithm.Decrypt(encoded), decoded);
            }
        }

        [Fact]
        public void Decrypt_InvalidKey_InvalidKeyException()
        {
            InitializeEnglishAlphabet();
            string errorKey = "densdsdsdisdsdssloxx";
            using (var algorithm = new CryptoHill(errorKey, Alphabet))
            {
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
    }
}
