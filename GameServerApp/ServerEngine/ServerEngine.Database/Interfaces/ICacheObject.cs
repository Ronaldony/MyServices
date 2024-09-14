namespace ServerEngine.Database.Interfaces
{
	/// <summary>
	/// ICacheObject.
	/// </summary>
	public interface ICacheObject
	{
		T Get<T>(string key) where T: class;

		bool Set<T>(string key, T value) where T : class;
	}
}
