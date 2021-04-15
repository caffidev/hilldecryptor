using System;

namespace LabHill.Exceptions
{
    [Serializable]
    public class InvalidKeyLengthException : Exception
    {
        //Lazy class
        public InvalidKeyLengthException() : base()
        {
        }

        public InvalidKeyLengthException(string message) : base(message)
        {
        }
    }
}