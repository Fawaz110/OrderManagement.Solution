namespace OrderManagement.Errors
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message?? GetMessageByCodeAsync(statusCode);
        }
        public int StatusCode { get; set; }
        public string Message { get; set; }

        private string GetMessageByCodeAsync(int statusCode)
            => statusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized Access",
                404 => "Not Found Resources",
                _ => "Internal Server Error"
            };
    }
}
