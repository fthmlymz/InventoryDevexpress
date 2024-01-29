using System.Globalization;

namespace InventoryManagement.Application.Common.Exceptions
{
    public class ConflictExceptionCustom : Exception
    {
        public ConflictExceptionCustom() : base() { }

        public ConflictExceptionCustom(string message) : base(message) { }

        public ConflictExceptionCustom(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
