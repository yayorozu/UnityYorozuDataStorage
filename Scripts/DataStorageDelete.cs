using System;
using System.Collections.Generic;

namespace Yorozu.Data
{
    /// <summary>
    /// 削除処理
    /// </summary>
    public static partial class DataStorage
    {
        private static Dictionary<Type, DeleteDelegate> _deleteEvents = new Dictionary<Type, DeleteDelegate>();
        
        public delegate void DeleteDelegate(IData data);
        
        /// <summary>
        /// 削除
        /// </summary>
        public static bool Remove<T>(IList<T> data) where T : IData
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
        
        public static bool Remove<T>(T data) where T : IData
        {
            return Remove<T>(data.Key);
        }

        public static bool Remove<T>(string key) where T : IData
        {
            var t = typeof(T);
            if (!_storages.ContainsKey(t))
                return false;

            if (_deleteEvents.TryGetValue(t, out var a))
            {
                if (TryGet(key, out T data))
                {
                    a?.Invoke(data);
                }
            }
            
            return _storages[t].Remove(key);
        }

        /// <summary>
        /// 全データを削除
        /// </summary>
        public static bool RemoveAll<T>() where T : IData
        {
            var t = typeof(T);
            if (!_storages.ContainsKey(t))
                return false;

            var keys = _storages[t].Keys;
            foreach (var key in keys)
            {
                Remove<T>(key);
            }

            _storages[t].Dispose();
            _storages.Remove(t);
            return true;
        }

        /// <summary>
        /// 全部の削除イベントを初期化
        /// </summary>
        public static void ClearAllDeleteEvent()
        {
            _deleteEvents.Clear();
        }

        /// <summary>
        /// 削除イベントを登録
        /// </summary>
        public static void DeleteListener<T>(DeleteDelegate delete) where T : IData
        {
            var t = typeof(T);
            if (_deleteEvents.TryGetValue(t, out var action))
            {
                action += delete;
                _deleteEvents[t] = action;
            }
            else
            {
                _deleteEvents.Add(t, delete);
            }
        }
        
        /// <summary>
        /// 指定したイベントを削除
        /// </summary>
        public static void RemoveDeleteListener<T>(DeleteDelegate delete) where T : IData
        {
            var t = typeof(T);
            if (!_deleteEvents.TryGetValue(t, out var action)) 
                return;
            
            action -= delete;
            if (action == null)
            {
                _deleteEvents.Remove(t);
            }
            else
            {
                _deleteEvents[t] = action;
            }
        }
        
        /// <summary>
        /// 指定したイベントを削除
        /// </summary>
        public static void RemoveAllDeleteListener<T>() where T : IData
        {
            var t = typeof(T);
            if (!_deleteEvents.TryGetValue(t, out var _)) 
                return;
            
            _deleteEvents.Clear();
        }
    }
}