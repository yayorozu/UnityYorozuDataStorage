using System;
using System.Collections.Generic;
using System.Linq;

namespace Yorozu.Data
{
    public static partial class DataStorage
    {
        private static Dictionary<Type, Storage> _storages;

        static DataStorage()
        {
            _storages = new Dictionary<Type, Storage>();
        }
        
        /// <summary>
        /// データの個数
        /// </summary>
        public static int Count<T>() where T : IData
        {
            var t = typeof(T);
            if (!_storages.ContainsKey(t))
                return 0;

            return _storages[t].Count;
        }
        
        /// <summary>
        /// 指定Keyが保存されているか
        /// </summary>
        public static bool Contains<T>(string key) where T : IData
        {
            var t = typeof(T);
            if (!_storages.ContainsKey(t))
                return false;

            return _storages[t].Contains(key);
        }
        
        /// <summary>
        /// データ取得
        /// </summary>
        public static T Get<T>(string key) where T : IData
        {
            if (TryGet<T>(key, out var data))
            {
                return data;
            }

            return default;
        }
        
        /// <summary>
        /// データ取得
        /// </summary>
        public static bool TryGet<T>(string key, out T data) where T : IData
        {
            var t = typeof(T);
            if (!_storages.ContainsKey(t))
            {
                data = default;
                return false;
            }

            return _storages[t].TryGet(key, out data);
        }
    }
}