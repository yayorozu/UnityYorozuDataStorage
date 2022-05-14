using System;
using System.Collections.Generic;

namespace Yorozu.Data
{
    public static partial class DataStorage
    {
        private static Dictionary<Type, UpdateDelegate> _addEvents = new Dictionary<Type, UpdateDelegate>();
        
        /// <summary>
        /// データ追加更新イベント
        /// </summary>
        public delegate void UpdateDelegate(IData data, bool isNew);
        
        /// <summary> --------------------------------------------------------------------------------
        /// 追加
        /// </summary> -------------------------------------------------------------------------------
        public static void Add<T>(IList<T> data) where T : IData
        {
            foreach (var d in data)
            {
                Add(d);
            }
        }
 
        public static void Add<T>(T data) where T : IData
        {
            var t = typeof(T);
            if (!_storages.ContainsKey(t))
            {
                _storages.Add(t, new Storage());
            }
            
            var isNew = _storages[t].Add(data.Key, data);
            if (_addEvents.TryGetValue(t, out var a))
            {
                a?.Invoke(data, isNew);
            }
        }
        
        /// <summary>
        /// 全部の追加イベントを初期化
        /// </summary>
        public static void ClearAllAddEvent()
        {
            _addEvents.Clear();
        }

        /// <summary>
        /// 追加イベントを登録
        /// </summary>
        public static void AddListener<T>(UpdateDelegate delete) where T : IData
        {
            var t = typeof(T);
            if (_addEvents.TryGetValue(t, out var action))
            {
                action += delete;
                _addEvents[t] = action;
                return;
            }

            _addEvents.Add(t, delete);
        }
        
        /// <summary>
        /// 指定したイベントを追加
        /// </summary>
        public static void RemoveAddListener<T>(UpdateDelegate delete) where T : IData
        {
            var t = typeof(T);
            if (!_addEvents.TryGetValue(t, out var action)) 
                return;
            
            action -= delete;
            if (action == null)
                _addEvents.Remove(t);
            else
                _addEvents[t] = action;
        }
        
        /// <summary>
        /// 指定したイベントを削除
        /// </summary>
        public static void RemoveAllAddListener<T>() where T : IData
        {
            var t = typeof(T);
            if (!_addEvents.TryGetValue(t, out var _)) 
                return;
            
            _addEvents.Clear();
        }
    }
}