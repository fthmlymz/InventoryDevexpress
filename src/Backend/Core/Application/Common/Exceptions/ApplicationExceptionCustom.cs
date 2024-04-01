using System.Globalization;

namespace Application.Common.Exceptions
{
    public class ApplicationExceptionCustom : Exception
    {
        public ApplicationExceptionCustom() : base()
        {
        }

        public ApplicationExceptionCustom(string message) : base(message)
        {
        }

        public ApplicationExceptionCustom(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ApplicationExceptionCustom(string message, params object[] args) : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
