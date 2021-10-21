using System;

namespace LabHill
{
    public class Helper
    {
        public static void WriteOnAPreviousLine(string str, int count = 1)
        {
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - count);
            //Fixes unpleasant bug
            string s = new string(' ', 60);
            Console.WriteLine(str + s);
        }
    }
}