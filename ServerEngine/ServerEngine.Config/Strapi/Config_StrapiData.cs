namespace ServerEngine.Config.Strapi
{
    /// <summary>
    /// Config_StrapiData.
    /// </summary>
    public class Config_StrapiData
    {
        public int Id { get; init; }

        public string DocumentId { get; init; }

        public string Name { get; init; }

        public object Data { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime UpdatedAt { get; init; }

        public DateTime PublishedAt { get; init; }
    }
}
