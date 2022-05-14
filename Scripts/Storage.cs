using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Yorozu.Data
{
    /// <summary>
    /// 各Typeのデータをキャッシュするクラス
    /// </summary>
    internal class Storage 
    {
        private Dictionary<string, object> _dictionary = new Dictionary<string, object>();
        
        internal int Count => _dictionary.Count;
        internal string[] Keys => _dictionary.Keys.ToArray();

        /// <summary>
        /// 追加
        /// </summary>
        internal bool Add(string key, object obj)
        {
            if (_dictionary.TryGetValue(key, out var v))
            {
                _dictionary[key] = obj;
                return false;
            }

            _dictionary.Add(key, obj);
            return true;
        }

        /// <summary>
        /// 削除
        /// </summary>
        internal bool Remove(string key)
        {
            return _dictionary.ContainsKey(key) && _dictionary.Remove(key);
        }

        /// <summary>
        /// 取得
        /// </summary>
        internal bool TryGet<T>(string key, out T data) where T : IData
        {
            if (!_dictionary.ContainsKey(key))
            {
                data = default;
                return false;
            }
            
            data = (T) _dictionary[key];
            return true;
        }

        /// <summary>
        /// 全部取得
        /// </summary>
        internal IEnumerable<T2> All<T2>() where T2 : IData
        {
            return _dictionary.Values.Cast<T2>();
        }

        internal bool Contains(string key) => _dictionary.ContainsKey(key);

        internal void Dispose()
        {
            _dictionary.Clear();
        }
    }
}