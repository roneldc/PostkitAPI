using System.Text.Json.Serialization;

namespace Postkit.Shared.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public T? Data { get; set; }
        public DateTime Timestamp { get; set; }


        public ApiResponse(bool success, int status, string message = "", T? data = default)
        {
            Success = success;
            Status = status;
            Message = message;
            Data = data;
            Timestamp = DateTime.UtcNow;
        }
        public static ApiResponse<T> SuccessResponse(T data, string message = "", int statusCode = 200) =>
            new ApiResponse<T>(true, statusCode, message, data);

        public static ApiResponse<T> ErrorResponse(string message, int statusCode = 400) =>
            new ApiResponse<T>(false, statusCode, message);
    }
}
