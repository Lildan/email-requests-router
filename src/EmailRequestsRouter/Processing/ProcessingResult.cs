namespace EmailRequestsRouter.Processing
{
    public class ProcessingResult
    {
        public string Type { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }

        public static ProcessingResult Successful(string type, string message = null)
        {
            return new ProcessingResult
            {
                Type = type,
                Success = true,
                Message = message
            };
        }

        public static ProcessingResult Failed(string type, string message = null, string error = null)
        {
            return new ProcessingResult
            {
                Type = type,
                Success = false,
                Message = message,
                Error = error
            };
        }
    }
}
