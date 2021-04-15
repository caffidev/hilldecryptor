using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
namespace LabHill
{
    class Program
    {
        private static List<char> Alphabet { get; set; }
        private static string Key { get; set; }
        private static string Path { get; } = "./encryptedWords.txt";
        private static string[] Words { get; set; }
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

            #region Keyword

            Console.Write("Write a decryption-key to use: ");
            Key = Console.ReadLine();

            #endregion

            #region Main

            using (var algorithm = new CryptoHill(Key, Alphabet))
            {
                //All commands that you want to use - here
                //All tests that I have done - in LabHill.Tests
                //The main result of program - below:
                string enc = algorithm.Encrypt("ciph");
                Console.WriteLine(enc);
                string dec = algorithm.Decrypt(enc);
                Console.WriteLine(dec);
            }
                
            #endregion
        }

    }
}
