public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; } = 200;
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    

    // public ApiResponse(){}
    // public ApiResponse(T data, string message = "", int statusCode = 200)
    // {
    //     Success = true;
    //     StatusCode = statusCode;
    //     Message = message;
    //     Data = data;
    // }
}
