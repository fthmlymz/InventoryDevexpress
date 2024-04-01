namespace Application.Common.Exceptions
{
    public class NotFoundExceptionCustom : Exception
    {
        public NotFoundExceptionCustom() : base()
        {
        }

        public NotFoundExceptionCustom(string message) : base(message)
        {
        }

        public NotFoundExceptionCustom(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NotFoundExceptionCustom(string name, object key) : base($"Entity \"{name}\" ({key}) was not found.")
        {
        }
    }
}
