namespace InventoryManagement.Frontend.Common.Exceptions
{
    public class ApiException : Exception
    {
        public string ApiErrorMessage { get; }

        public ApiException(string message, string apiErrorMessage) : base(message)
        {
            ApiErrorMessage = apiErrorMessage;
        }
    }
}
