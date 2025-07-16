using System;

namespace GameCore.Http
{
    /// <summary>
    /// HttpRouteAttribute.
    /// </summary>
    public class HttpRouteAttribute : Attribute
    {
        public string Path { get; set; }

        public HttpRouteAttribute(string path)
        {
            Path = path;
        }
    }
}
