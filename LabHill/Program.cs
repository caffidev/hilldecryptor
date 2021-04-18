using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace LabHill
{
    /// <summary>
    /// Program that decrypts 2x2 and 3x3 matrixes with help of Hill algorithm.
    /// Doesn't work with >4x4 so, because of the not universal code within.
    /// Also don't InvalidKeyException doesnt work.
    /// </summary>
    class Program
    {
        private static List<char> Alphabet { get; set; }
        private static string Key { get; set; }
        private static string Path { get; } = "./encryptedWords.txt";
        private static string[] Words { get; set; }
        private static Dictionary<string, int> EnglishWords { get; set; }
        public static void Main(string[] args)
        {
            #region Initialization
            if (!File.Exists(Path))
            {
                using (StreamWriter writer = File.CreateText(Path))
                {
                    Console.WriteLine($"Error: enter words into the file {Path}");
                    writer.WriteAsync("//Enter words here");
                    writer.Close();
                    Environment.Exit(1);
                }
            }
            else if(File.ReadAllText(Path) == "//Enter words here")
            {
                Console.WriteLine("You haven't written anything, have you?");
            }
            else
            {
                //Grabbing all words from txt path file
                Words = File.ReadAllLines(Path);
            }
            #endregion

            #region Alphabet choosing

            Alphabet = new List<char>();
            int i = 0;
            for (char c = 'a'; c <= 'z'; ++c)
            {
                Alphabet.Add(c);
                i++;
            }

            //If you want to use additional symbols, uncomment it
            //Alphabet.AddRange(new List<char>() {'.', ',', ' ', '?'});

            Console.Write("Current alphabet: ");
            foreach (char c in Alphabet)
            {
                Console.Write(c);
            }

            Console.WriteLine();
            #endregion

            #region Initialization of English Word List

            //it's better to use File Manager, but I'm lazy asf
            //Tried to use Resource Manager - but this idea was fucked up by Microsoft's stupidity
            using (StreamReader reader = new StreamReader("./words_dictionary.json"))
            {
                string json = reader.ReadToEnd();
                if (json == null) throw new FileNotFoundException("Words_dictionary.json is empty");
                //Using dictionary can lead to slower results, so better use List<word> in this case. But it's ta time loss to change it now.
                EnglishWords = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
                Console.WriteLine($"Dictionary of English words successfully loaded, amount of words: " + EnglishWords.Count);
            }

            #endregion
            #region Keyword

            //To do first, you need a key, if you want a analysis you need to input it, but it will not be read
            Console.Write("Write a decryption-key to use: ");
            Key = Console.ReadLine();

            #endregion

            #region Main

            using (var algorithm = new CryptoHill(Key, Alphabet))
            {
                //All commands that you want to use - here
                //All tests that I have done - in LabHill.Tests
                //The main result of program - below:

                //1. Test encrypting and decrypting

                //string enc = algorithm.Encrypt("cipher");
                //Console.WriteLine(enc);
                //string dec = algorithm.Decrypt(enc);
                //Console.WriteLine(dec);

                //2. Processing words with Brute-Forcing(only works for 2x2)
                algorithm.Analyse(Words[0],2, Words: EnglishWords);

            }
                
            #endregion
        }

    }
}
