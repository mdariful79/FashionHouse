using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Application.Exceptions
{
    public class DuplicateDataException : Exception
    {
        public DuplicateDataException(string message) : base(message)
        {
        }
    }
}
