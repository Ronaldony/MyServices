namespace ServerEngine.Config.Database
{
    /// <summary>
    /// Config_Couchbase.
    /// </summary>
    public class Config_CouchbaseConnection
    {
        public string Alias { get; set; }

        public string Bucket { get; set; }

        public string Scope { get; set; }

        public string Collection { get; set; }
    }
}
