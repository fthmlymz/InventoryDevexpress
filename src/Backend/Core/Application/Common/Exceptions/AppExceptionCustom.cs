using System.Globalization;

namespace Application.Common.Exceptions
{
    public class AppExceptionCustom : Exception
    {
        public AppExceptionCustom() : base() { }

        public AppExceptionCustom(string message) : base(message) { }

        public AppExceptionCustom(string message, params object[] args) : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
