namespace API.AppResponse
{

    public class AppResponse<T>
    {
        public int StatusCode { get; set; } = 200;
        public bool Success { get; set; } = true;
        public T? Data { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public AppResponse() { }
        public AppResponse(T? data)
        {
            this.Data = data;
        }
        public AppResponse(T? data, int statusCode, bool success)
        {
            this.Data = data;
            this.StatusCode = statusCode;
            this.Success = success;
        }
        public AppResponse(T? data, string errorMessage, int statusCode, bool success)
        {
            this.Data = data;
            this.StatusCode = statusCode;
            this.Success = success;
            this.ErrorMessage = errorMessage;
        }
    }

}