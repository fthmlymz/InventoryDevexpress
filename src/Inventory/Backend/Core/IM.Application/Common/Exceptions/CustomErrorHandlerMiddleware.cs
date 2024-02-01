using InventoryManagement.Shared;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace InventoryManagement.Application.Common.Exceptions
{
    public class UseCustomExceptionHandler
    {
        private readonly RequestDelegate _next;

        public UseCustomExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    case AppExceptionCustom e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case BadRequestExceptionCustom e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case NotFoundExceptionCustom e: //case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case ConflictExceptionCustom e:
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        break;
                    case ArgumentNullException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                //var result = JsonSerializer.Serialize(new { message = error?.Message });
                //await response.WriteAsync(result);


                var result = Result<Exception>.Failure(new BadRequestExceptionCustom(error.Message));
                await response.WriteAsync(JsonSerializer.Serialize(result)); //Daha once result.Result olarak yapılmıstı kontrol edilecek exceptionlarda
            }
        }
    }
}

/*
 public class BackofficeExceptionHandlerMiddleware : AbstractExceptionHandlerMiddleware
{
    public BackofficeExceptionHandlerMiddleware(RequestDelegate next) : base(next)
    {
    }

    public override (HttpStatusCode code, string message) GetResponse(Exception exception)
    {
        HttpStatusCode code;
        switch (exception)
        {
            case KeyNotFoundException
                or NoSuchUserException
                or FileNotFoundException:
                code = HttpStatusCode.NotFound;
                break;
            case EntityAlreadyExists:
                code = HttpStatusCode.Conflict;
                break;
            case UnauthorizedAccessException
                or ExpiredPasswordException
                or UserBlockedException:
                code = HttpStatusCode.Unauthorized;
                break;
            case CreateUserException
                or ResetPasswordException
                or ArgumentException
                or InvalidOperationException:
                code = HttpStatusCode.BadRequest;
                break;
            default:
                code = HttpStatusCode.InternalServerError;
                break;
        }
        return (code, JsonConvert.SerializeObject(new SimpleResponse(exception.Message)));
    }
}
*/

