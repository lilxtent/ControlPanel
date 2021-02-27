using System;
using System.Collections.Generic;
using System.Text;

namespace ControlPanel.Exceptions
{
    class PhoneNumberException : Exception
    {
        public PhoneNumberException() { }
        public PhoneNumberException(string message) : base (message){}
    }
}
