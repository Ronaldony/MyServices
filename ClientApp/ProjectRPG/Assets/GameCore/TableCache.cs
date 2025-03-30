using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GameCore
{
    /// <summary>
    /// Table cache. 
    /// </summary>
    public class TableCache<T> where T : class, ITableBase, new()
	{
		/// <summary>
		/// container
		/// </summary>
		public ReadOnlyDictionary<int, T> _dict;

		public TableCache(IEnumerable<T> allDatas)
		{
			if (null != allDatas)
			{
				var ss = allDatas.ToDictionary(d => d.Id, d => d);

                _dict = new ReadOnlyDictionary<int, T>(allDatas.ToDictionary(d => d.Id, d => d));
			}
		}

		/// <summary>
		/// GetById.
		/// </summary>
		public T GetById(int id)
		{
			if (_dict.ContainsKey(id))
			{
				return _dict[id];
			}

			return null;
		}

		/// <summary>
		/// First.
		/// </summary>
		public T First(Func<T, bool> predExpr)
		{
			var find = _dict.Values.Where(predExpr);
			if (find == null)
			{
				return null;
			}

			return find.FirstOrDefault();
		}

		/// <summary>
		/// Last.
		/// </summary>
		public T Last(Func<T, bool> predExpr)
		{
			var find = _dict.Values.Where(predExpr);
			if (find == null)
			{
				return null;
			}

			return find.LastOrDefault();
		}

		/// <summary>
		/// Select.
		/// </summary>
		public IEnumerable<T> Select(Func<T, bool> predExpr)
		{
			return _dict.Values.Where(predExpr);
		}

		/// <summary>
		/// select all
		/// </summary>
		public IEnumerable<T> SelectAll()
		{
			return _dict.Values;
		}

		/// <summary>
		/// Count.
		/// </summary>
		public int Count()
		{
			return _dict.Count;
		}
	}
}
