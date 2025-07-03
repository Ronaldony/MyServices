namespace Framework.WebAPI
{
    /// <summary>
    /// ServerErrorException.
    /// </summary>
    public class ServerErrorException : Exception
    {
        public int ErrorCode { get; set; }
        public string ErrorMsg { get; set; }

        public ServerErrorException(int code)
        {
            ErrorCode = code;
        }

        public ServerErrorException(int code, string msg)
        {
            ErrorCode = code;
            ErrorMsg = msg;
        }
    }
}
