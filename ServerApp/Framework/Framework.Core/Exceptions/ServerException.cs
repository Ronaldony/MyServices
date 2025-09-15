namespace Framework.Core.Exceptions
{
    public class ServerException : Exception
    {
        public ServerException(string msg) 
        {
            Msg = msg;
        }

        /// <summary>
        /// Exception message.
        /// </summary>
        public string Msg { get; set; }
    }
}
