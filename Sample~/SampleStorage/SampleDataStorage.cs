using System;
using System.Collections.Generic;
using UnityEngine;
using Yorozu.Data;

namespace Sample
{
    public class SampleDataStorage : MonoBehaviour
    {
        private void Awake()
        {
            var data = new List<DataSample>()
            {
                new DataSample("a", 1),
                new DataSample("b", 2),
                new DataSample("c", 3),
            };
            
            DataStorage<DataSample>.AllUpdateEvent += (d, isNew) =>
            { 
                Debug.Log($"Add {d.Key}");
            };
            
            DataStorage<DataSample>.DeleteEvent += Delete;

            DataStorage<DataSample>.Add(data);

            var key = DataStorage<DataSample>.GetRandomKey();
            DataStorage<DataSample>.Add(new DataSample(key, 4));
            
            // データを取得して更新
            if (DataStorage<DataSample>.TryGet("a", out var d1))
            {
                Debug.Log(d1.Value);
                d1.Value = 100;
            }
            if (DataStorage<DataSample>.TryGet("b", out var d2))
            {
                Debug.Log(d2.Value);
            }
            if (DataStorage<DataSample>.TryGet("c", out var d3))
            {
                Debug.Log(d3.Value);
            }
            
            if (DataStorage<DataSample>.TryGet("a", out var d1_))
            {
                // 100
                Debug.Log(d1_.Value);
            }
            
            DataStorage<DataSample>.Remove("a");
            
            // 削除イベント削除
            DataStorage<DataSample>.DeleteEvent -= Delete;
            
            DataStorage<DataSample>.Remove("b");

            // 削除したデータを取得してみる
            if (!DataStorage<DataSample>.TryGet("a", out var _))
            {
                Debug.Log("delete");
            }
        }

        private void Delete(IData data)
        {
            var dd = data as DataSample;
            Debug.Log($"Delete {dd.Key}");   
        }

        private class DataSample : IData
        {
            public string Key => _key;
            private string _key;

            private List<int> _ivalues = new List<int>()
            {
                2,
            };

            private Vector2[] _values = new Vector2[]
            {
                Vector2.down, 
                Vector2.down, 
            };

            private Vector2 V2 = Vector2.left;
            public int Value;

            public DataSample(string key, int value)
            {
                _key = key;
                Value = value;
            }
        }
    }
}
