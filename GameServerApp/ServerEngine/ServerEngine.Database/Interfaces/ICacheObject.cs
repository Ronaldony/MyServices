namespace ServerEngine.Database.Interfaces
{
	/// <summary>
	/// ICacheObject.
	/// </summary>
	public interface ICacheObject
	{
		T Get<T>(string key);

		bool Set<T>(string key, T value);
	}
}
