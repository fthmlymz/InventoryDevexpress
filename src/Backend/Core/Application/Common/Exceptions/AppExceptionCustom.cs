using System.Globalization;

namespace InventoryManagement.Application.Common.Exceptions
{
    public class AppExceptionCustom : Exception
    {
        public AppExceptionCustom() : base() { }

        public AppExceptionCustom(string message) : base(message) { }

        public AppExceptionCustom(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
