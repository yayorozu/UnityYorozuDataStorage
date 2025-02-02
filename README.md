# UnityYorozuDataStorage

DataStorage ライブラリは、Unity プロジェクト向けの汎用でイベント駆動型のデータ管理ソリューションです。<br>
IData インターフェースを実装した任意のデータ型を対象に、データの追加、更新、取得、削除を簡単に行うことができます。<br>
本ドキュメントでは、ライブラリの概要、特徴、サンプルコードの解説、および使用方法について説明します。

# 概要
DataStorage ライブラリは、Unity 内でのデータ管理を簡素化するための集中管理システムです。
主な機能は以下の通りです：

- データの追加: 単一または複数のデータを追加可能
- データの取得: ユニークなキーによりデータを取得
- データの更新: 既存データの変更を簡単に反映
- データの削除: データの削除時にイベントを発火し、外部での処理が可能
  
また、イベントを利用することで、データの追加・更新や削除が行われた際にリアルタイムで処理を実行できます。



# 使用方法
### IData インターフェースの実装
自身のデータクラスを作成し、IData インターフェースを実装します。必ずユニークなキーを用意してください。

### データの追加
単一または複数のデータを DataStorage<T>.Add() メソッドを使って追加します。

### イベントの登録
データの追加・更新には AllUpdateEvent、削除には DeleteEvent を必要に応じて登録してください。

### データの取得と更新
TryGet(key, out var data) メソッドを利用して、必要なデータを取得し、値の更新や参照を行います。

### データの削除
DataStorage<T>.Remove(key) を呼び出して、特定のキーに対応するデータを削除します。

# イベントハンドリング
### AllUpdateEvent
データが追加または更新されるたびに発火します。イベントハンドラには、データオブジェクトと新規追加かどうかを示すブール値が渡されます。

### DeleteEvent
データが削除されると発火します。データ削除時の後処理（例えばログ出力やリソースの解放など）を実装する際に利用します。

# データ確認

`Tools/DataStorageWindow` で現在登録されているデータを確認することができます

<img src="https://github.com/yayorozu/ImageUploader/blob/master/Storage/Top.png" width="700">


# サンプルコード解説
以下は、DataStorage ライブラリの使用例となるサンプルコードです。各部分の解説を行います。

```csharp
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
            // 複数のデータサンプルを作成
            var data = new List<DataSample>()
            {
                new DataSample("a", 1),
                new DataSample("b", 2),
                new DataSample("c", 3),
            };
            
            // データ追加または更新時に発火するイベントに登録
            DataStorage<DataSample>.AllUpdateEvent += (d, isNew) =>
            { 
                Debug.Log($"Add {d.Key}");
            };
            
            // データ削除時のイベントに登録
            DataStorage<DataSample>.DeleteEvent += Delete;

            // リストのデータをストレージに追加
            DataStorage<DataSample>.Add(data);

            // ランダムなキーを取得し、そのキーで新しいデータを追加
            var key = DataStorage<DataSample>.GetRandomKey();
            DataStorage<DataSample>.Add(new DataSample(key, 4));
            
            // キー "a" のデータを取得して更新
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
            
            // "a" のデータ更新を再度確認（100になっているはず）
            if (DataStorage<DataSample>.TryGet("a", out var d1_))
            {
                Debug.Log(d1_.Value);
            }
            
            // キー "a" のデータを削除
            DataStorage<DataSample>.Remove("a");
            
            // 削除イベントからハンドラを解除
            DataStorage<DataSample>.DeleteEvent -= Delete;
            
            // キー "b" のデータを削除
            DataStorage<DataSample>.Remove("b");

            // 削除済みデータ "a" の取得を試み、取得できなければ削除済みと判断
            if (!DataStorage<DataSample>.TryGet("a", out var _))
            {
                Debug.Log("delete");
            }
        }

        // データ削除時に呼ばれるイベントハンドラ
        private void Delete(IData data)
        {
            var dd = data as DataSample;
            Debug.Log($"Delete {dd.Key}");   
        }

        // IData インターフェースを実装したサンプルデータクラス
        private class DataSample : IData
        {
            public string Key => _key;
            private string _key;

            // サンプルとしての追加フィールド
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
```
## 各処理の説明
#### 初期化とデータ生成
Awake メソッド内で、キーと数値のペアからなる DataSample オブジェクトのリストを生成しています。

#### イベント登録

AllUpdateEvent：データの追加・更新時に発火し、データのキーをログに出力します。
DeleteEvent：データ削除時に発火し、削除されたデータのキーをログに出力するハンドラ Delete を登録しています。
データの追加
最初にリスト内のデータを一括で追加し、その後ランダムなキーを取得して追加のデータを登録しています。

#### データの取得と更新
TryGet メソッドを利用して、キーを指定してデータを取得し、データの値を更新・確認しています。

#### データの削除
Remove メソッドを使用して、特定のキーに対応するデータを削除します。削除時には、登録されたイベントハンドラが呼ばれます。

#### イベント解除
必要に応じて、イベントハンドラの登録解除を行うことができます。

# ライセンス

本プロジェクトは [MIT License](LICENSE) の下でライセンスされています。  
詳細については、LICENSE ファイルをご覧ください。
