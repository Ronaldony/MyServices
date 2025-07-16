namespace Framework.WebAPI
{
    /// <summary>
    /// ServerException.
    /// </summary>
    public class ServerException : Exception
    {
        public int ErrorCode { get; set; }
        public string ErrorMsg { get; set; }

        public ServerException(int code)
        {
            ErrorCode = code;
        }

        public ServerException(int code, string msg)
        {
            ErrorCode = code;
            ErrorMsg = msg;
        }
    }
}
