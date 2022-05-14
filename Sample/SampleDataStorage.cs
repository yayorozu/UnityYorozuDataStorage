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
            
            DataStorage.AddListener<DataSample>((o, isNew) =>
            {
                var dd = o as DataSample;
                Debug.Log($"Add {dd.Key}");
            });
            DataStorage.DeleteListener<DataSample>(Delete);

            DataStorage.Add(data);
            
            // データを取得して更新
            if (DataStorage.TryGet<DataSample>("a", out var d1))
            {
                Debug.Log(d1.Value);
                d1.Value = 100;
            }
            if (DataStorage.TryGet<DataSample>("b", out var d2))
            {
                Debug.Log(d2.Value);
            }
            if (DataStorage.TryGet<DataSample>("c", out var d3))
            {
                Debug.Log(d3.Value);
            }
            
            if (DataStorage.TryGet<DataSample>("a", out var d1_))
            {
                // 100
                Debug.Log(d1_.Value);
            }

            DataStorage.Remove<DataSample>("a");
            
            // 削除イベント削除
            DataStorage.RemoveDeleteListener<DataSample>(Delete);
            
            DataStorage.Remove<DataSample>("b");

            // 削除したデータを取得してみる
            if (!DataStorage.TryGet<DataSample>("a", out var _))
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
            public string Key { get; }

            public int Value;

            public DataSample(string key, int value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}
