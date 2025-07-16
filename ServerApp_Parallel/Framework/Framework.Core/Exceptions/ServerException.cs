namespace Framework.Core.Exceptions
{
    public class ServerException : Exception
    {
        public ServerException(string msg) 
        {
            msg = Msg;
        }

        /// <summary>
        /// Exception message.
        /// </summary>
        public string Msg { get; set; }
    }
}
