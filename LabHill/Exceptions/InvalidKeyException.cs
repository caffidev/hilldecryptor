using System;

namespace LabHill.Exceptions
{
    [Serializable]
    public class InvalidKeyException : Exception
    {
        //Lazy class
        public InvalidKeyException() : base()
        {
        }

        public InvalidKeyException(string message) : base(message)
        {
        }
    }
}