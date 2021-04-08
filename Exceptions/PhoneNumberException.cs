using System;

namespace ControlPanel.Exceptions
{
    class PhoneNumberException : Exception
    {
        public PhoneNumberException() { }
        public PhoneNumberException(string message) : base(message) { }
    }
}
