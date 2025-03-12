//using System.Collections.ObjectModel;

//namespace GameCore
//{
//	/// <summary>
//	/// DataCache Table 
//	/// </summary>
//	public class DataCacheTable<T> where T : class, ITDBDataTable, new()
//	{
//		/// <summary>
//		/// container
//		/// </summary>
//		public ReadOnlyDictionary<int, T> _dict;

//		public DataCacheTable(IEnumerable<T> allDatas)
//		{
//			if (null != allDatas)
//			{
//				_dict = new ReadOnlyDictionary<int, T>(allDatas.ToDictionary(d => d.Id, d => d));
//			}
//		}

//		public T GetById(int id)
//		{
//			if (_dict == null)
//			{
//				return null;
//			}

//			if (_dict.ContainsKey(id))
//			{
//				return _dict[id];
//			}

//			return null;
//		}

//		/// <summary>
//		/// first
//		/// </summary>
//		public T First(Func<T, bool> predExpr)
//		{
//			var find = _dict.Values.Where(predExpr);
//			if (find == null)
//			{
//				return null;
//			}

//			return find.FirstOrDefault();
//		}

//		/// <summary>
//		/// first
//		/// </summary>
//		public T Last(Func<T, bool> predExpr)
//		{
//			var find = _dict.Values.Where(predExpr);
//			if (find == null)
//			{
//				return null;
//			}

//			return find.LastOrDefault();
//		}

//		/// <summary>
//		/// select data from predicate
//		/// </summary>
//		public IEnumerable<T> Select(Func<T, bool> predExpr)
//		{
//			return _dict.Values.Where(predExpr);
//		}

//		/// <summary>
//		/// select all
//		/// </summary>
//		public IEnumerable<T> SelectAll()
//		{
//			return _dict.Values;
//		}

//		public int Count()
//		{
//			return _dict.Count;
//		}

//	}
//}
