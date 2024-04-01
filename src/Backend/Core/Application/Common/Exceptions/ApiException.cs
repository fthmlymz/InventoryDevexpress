using System.Runtime.Serialization;

namespace Application.Common.Exceptions
{
    [Serializable]
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public List<string> Errors { get; private set; } = new List<string>();

        public ApiException(int statuscode, List<string> errors)
        {
            StatusCode = statuscode;
            Errors = errors;
        }
        public ApiException(int statuscode, string error)
        {
            StatusCode = statuscode;
            Errors.Add(error);
        }
        public ApiException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
