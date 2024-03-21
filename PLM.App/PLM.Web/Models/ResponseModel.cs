﻿namespace PLM.Web.Models;

public record Response<T>
{
    public T? Model { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}
