namespace DataDesignerServer.Core.Http
{
    /// <summary>
    /// HttpAttribute.
    /// </summary>
    public class HttpAttribute : Attribute
    {
        public HttpAttribute(string path) 
        {
            Path = path;
        }

        public string Path { get; set; }
    }
}
