namespace BackendForChat.Application.Common
{
    public class ServiceResult<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static ServiceResult<T> Ok(T data) => new() { Data = data, Success = true };
        public static ServiceResult<T> Fail(string errorMessage) => new() { Success = false, ErrorMessage = errorMessage };
    }
}
