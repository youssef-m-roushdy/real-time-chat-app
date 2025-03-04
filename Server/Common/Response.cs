using System;

namespace Server.Common
{
    public class Response<T>
    {
        public bool IsSuccess { get; }
        public T? Data { get; }
        public string? Error { get; }
        public string? Message { get; }

        public Response(bool isSuccess, T? data, string? error, string? message)
        {
            IsSuccess = isSuccess;
            Data = data;
            Error = error;
            Message = message;
        }

        public static Response<T> Success(T data, string? message = "") =>
            new Response<T>(true, data, null, message);

        public static Response<T> Failure(string error, string? message = "") =>
            new Response<T>(false, default, error, null);
    }
}
