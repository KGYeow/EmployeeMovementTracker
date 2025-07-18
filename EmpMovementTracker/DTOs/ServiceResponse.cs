﻿namespace EmpMovementTracker.DTOs;

public partial class ServiceResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ServiceResponse<T> Ok(T data, string message = "") =>
        new() { Success = true, Data = data, Message = message };

    public static ServiceResponse<T> Fail(string message) =>
        new() { Success = false, Message = message };
}