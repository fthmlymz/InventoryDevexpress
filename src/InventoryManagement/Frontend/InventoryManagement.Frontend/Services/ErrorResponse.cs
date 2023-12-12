namespace InventoryManagement.Frontend.Services
{
    public class ErrorResponse
    {
        public string? Message { get; set; }
        public ExceptionResponse? Exception { get; set; }
        public int Code { get; set; }
    }

    public class ExceptionResponse
    {
        public string? TargetSite { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public ExceptionResponse? InnerException { get; set; }
        public string? HelpLink { get; set; }
        public string? Source { get; set; }
        public int HResult { get; set; }
        public string? StackTrace { get; set; }
    }

}
