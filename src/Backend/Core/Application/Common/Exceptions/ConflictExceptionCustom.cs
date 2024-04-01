using System.Globalization;

namespace Application.Common.Exceptions
{
    public class ConflictExceptionCustom : Exception
    {
        public ConflictExceptionCustom() : base() { }

        public ConflictExceptionCustom(string message) : base(message) { }

        public ConflictExceptionCustom(string message, params object[] args) : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
