
namespace ServerEngine.Test.Database.Data
{
    using ServerEngine.Core.DataObject;

    /// <summary>
    /// DTO for Benchmark.
    /// </summary>
    public class DTO_Benchmark : DataObjectBase
    {
        public DTO_Benchmark() 
        {
            Dummy1 = new DummyData();
            Dummy2 = new DummyData();
            Dummy3 = new DummyData();
            Dummy4 = new DummyData();
            Dummy5 = new DummyData();
            Dummy6 = new DummyData();
        }

        /// <summary>
        /// Dummy 1.
        /// </summary>
        public DummyData Dummy1 { get; set; }

        /// <summary>
        /// Dummy 2.
        /// </summary>
        public DummyData Dummy2 { get; set; }

        /// <summary>
        /// Dummy 3.
        /// </summary>
        public DummyData Dummy3 { get; set; }

        /// <summary>
        /// Dummy 4.
        /// </summary>
        public DummyData Dummy4 { get; set; }

        /// <summary>
        /// Dummy 5.
        /// </summary>
        public DummyData Dummy5 { get; set; }

        /// <summary>
        /// Dummy 6.
        /// </summary>
        public DummyData Dummy6 { get; set; }

        public override void TryReset()
        {
        }
    }
}
