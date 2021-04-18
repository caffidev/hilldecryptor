using System;

namespace LabHill.Exceptions
{
    [Serializable]
    public class InvalidAnalysisException : Exception
    {
        //Lazy class
        public InvalidAnalysisException() : base()
        {
        }

        public InvalidAnalysisException(string message) : base(message)
        {
        }
    }
}