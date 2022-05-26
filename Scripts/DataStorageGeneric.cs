using System;
using System.Collections.Generic;

namespace Yorozu.Data
{
    public static class DataStorage<T> where T : IData
    {
        private static Storage _storage;
        
        static DataStorage()
        {
            _storage = new Storage();
            DataStorage.Add(typeof(T), Clear);
        }

        private static void Clear()
        {
            _storage.Dispose();
            _storage = new Storage();
            ClearAddEvent();
            ClearDeleteEvent();            
        }

        /// <summary>
        /// データの個数
        /// </summary>
        public static int Count() => _storage.Count;

        /// <summary>
        /// 指定Keyが保存されているか
        /// </summary>
        public static bool Contains(string key) => _storage.Contains(key);
        
        /// <summary>
        /// 全部取得
        /// </summary>
        public static IEnumerable<T> All()
        {
            return _storage.All<T>();
        }
        
        /// <summary>
        /// データ取得
        /// </summary>
        public static T Get(string key)
        {
            if (TryGet(key, out var data))
            {
                return data;
            }

            return default;
        }
        
        /// <summary>
        /// データ取得
        /// </summary>
        public static bool TryGet(string key, out T data)
        {
            return _storage.TryGet(key, out data);
        }

        /// <summary>
        /// 登録されていないランダムな文字列をKeyとして返す
        /// </summary>
        public static string GetRandomKey()
        {
            const string CHARS = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var size = 32;
            var stringBuilder = new System.Text.StringBuilder(size);
            while (true)
            {
                stringBuilder.Clear();
                for (var i = 0; i < size; ++i)
                {
                    var range = UnityEngine.Random.Range(0, CHARS.Length);
                    stringBuilder.Append(CHARS[range]);
                }

                var randomKey = stringBuilder.ToString();
                if (!Contains(randomKey))
                    return randomKey;
            }
        }
        
        /// <summary>
        /// 特定のデータのみの更新
        /// </summary>
        public delegate void UpdateDelegate(T data);
        private static Dictionary<string, UpdateDelegate> _updates = new Dictionary<string, UpdateDelegate>();

        /// <summary>
        /// 更新イベント監視
        /// </summary>
        public static void AddUpdateListener(string key, UpdateDelegate @delegate)
        {
            if (_updates.TryGetValue(key, out var a))
            {
                a += @delegate;
                _updates[key] = a;
            }
            else
            {
                _updates.Add(key, @delegate);
            }
        }
        
        /// <summary>
        /// イベント削除
        /// </summary>
        public static void RemoveUpdateListener(string key, UpdateDelegate @delegate)
        {
            if (!_updates.TryGetValue(key, out var a))
                return;

            if (a == null)
                return;

            a -= @delegate;
            if (a == null)
                _updates.Remove(key);
            else
                _updates[key] = a;
        }

        /// <summary>
        /// 全Updateイベント削除
        /// </summary>
        public static void RemoveAllUpdateListener(string key)
        {
            if (!_updates.TryGetValue(key, out var a))
                return;
            
            _updates.Remove(key);
        }
        
        /// <summary>
        /// 全データ追加更新イベント
        /// </summary>
        public delegate void AllUpdateDelegate(T data, bool isNew);

        private static event AllUpdateDelegate _allUpdateEvent;
        public static event AllUpdateDelegate AllUpdateEvent
        {
            add => _allUpdateEvent += value;
            remove => _allUpdateEvent -= value;
        }

        /// <summary>
        /// 追加
        /// </summary>
        public static void Add(IList<T> data)
        {
            foreach (var d in data)
            {
                Add(d);
            }
        }
 
        /// <summary>
        /// 追加
        /// </summary>
        public static void Add(T data)
        {
            var isNew = _storage.Add(data.Key, data);
            _allUpdateEvent?.Invoke(data, isNew);
            if (_updates.ContainsKey(data.Key))
            {
                _updates[data.Key]?.Invoke(data);
            }
        }

        /// <summary>
        /// データ更新した際に呼び出す
        /// </summary>
        public static void Update(T data)
        {
            Add(data);
        }
        
        /// <summary>
        /// 全部の追加イベントを初期化
        /// </summary>
        public static void ClearAddEvent()
        {
            _allUpdateEvent = null;
        }
        
        public delegate void Delete(IData data);
        
        private static event Delete _deleteEvent;
        public static event Delete DeleteEvent
        {
            add => _deleteEvent += value;
            remove => _deleteEvent -= value;
        }
        
        /// <summary>
        /// 削除
        /// </summary>
        public static bool Remove(IList<T> data)
        {
            var fail = false;
            foreach (var d in data)
            {
                if (!Remove(d))
                {
                    fail = true;
                }
            }
            return fail;
        }
        
        public static bool Remove(T data)
        {
            return Remove(data.Key);
        }

        public static bool Remove(string key)
        {
            if (TryGet(key, out T data))
            {
                _deleteEvent?.Invoke(data);
            }

            return _storage.Remove(key);
        }

        /// <summary>
        /// 全データを削除
        /// </summary>
        public static bool RemoveAll()
        {
            var keys = _storage.Keys;
            foreach (var key in keys)
            {
                Remove(key);
            }

            _storage.Dispose();
            return true;
        }

        /// <summary>
        /// 全部の削除イベントを初期化
        /// </summary>
        public static void ClearDeleteEvent()
        {
            _deleteEvent = null;
        }
    }
}