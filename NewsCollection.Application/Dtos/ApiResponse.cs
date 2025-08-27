using System;

namespace NewsCollection.Application.Dtos;

public class ApiResponse<T>
{
    public string Status { get; set; } = "success";
    public string? Message { get; set; }
    public T? Data { get; set; }

    public static ApiResponse<T> Success(T? data, string? message = null) => new()
    {
        Status = "success",
        Message = message,
        Data = data
    };

    public static ApiResponse<T> Failure(string message) => new()
    {
        Status = "failed",
        Message = message,
        Data = default
    };
}