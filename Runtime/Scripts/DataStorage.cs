using System;
using System.Collections.Generic;

namespace Yorozu.Data
{
    public static class DataStorage
    {
        internal static Dictionary<Type, Action> activeStorages;
        
        static DataStorage()
        {
            activeStorages = new Dictionary<Type, Action>();
        }

        internal static void Add(Type type, Action clear)
        {
            activeStorages.Add(type, clear);
            
#if UNITY_EDITOR
            if (Tool.DataStorageWindow.instance != null)
            {
                Tool.DataStorageWindow.instance.Refresh();
            }
#endif
        }

        /// <summary>
        /// 登録してある全データを削除
        /// </summary>
        public static void Clear()
        {
            foreach (var storage in activeStorages)
            {
                storage.Value?.Invoke();
            }
        }

        public static void Add<T>(T data) where T : IData
        {
            DataStorage<T>.Add(data);
        }

        public static void Add<T>(IList<T> data) where T : IData
        {
            DataStorage<T>.Add(data);
        }
        
        public static void Remove<T>(T data) where T : IData
        {
            DataStorage<T>.Remove(data);
        }
        
        public static void Remove<T>(IList<T> data) where T : IData
        {
            DataStorage<T>.Remove(data);
        }
    }
}