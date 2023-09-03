#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.IMGUI.Controls;

namespace Yorozu.Data.Tool
{
    internal static class EditorDataStorageUtility
    {
        internal static string[] GetActiveDataTypeNames()
        {
            if (DataStorage.activeStorages == null)
            {
                return Array.Empty<string>();
            }
            
            return DataStorage.activeStorages.Keys
                .Select(t => t.Name)
                .ToArray();
        }
        
        internal static TreeViewItem GetDataTreeViewItem(string name)
        {
            var root = new TreeViewItem(0, -1, "root");
            var targetType = DataStorage.activeStorages.Keys.FirstOrDefault(t => t.Name == name);
            if (targetType == null)
                return root;
            
            var flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            var dataStorageType = typeof(DataStorage<>);
            var genericType = dataStorageType.MakeGenericType(targetType);
            var storage = genericType.GetField("_storage", flags).GetValue(null) as Storage;
            var storageType = storage.GetType();
            var data = storageType.GetField("_dictionary", flags).GetValue(storage) as Dictionary<string, object>;

            var id = 1;
            foreach (var value in data.Values)
            {
                var dataKey = (value as IData).Key;
                var dataItem = new TreeViewItem(id++, 0, dataKey);
                root.AddChild(dataItem);
                Recursive(ref id, dataItem, value, targetType, 1);
            }

            return root;
        }

        private static void Recursive(ref int id, TreeViewItem root, object value, Type type, int depth)
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;
            var fields = type.GetFields(flags);
            foreach (var field in fields)
            {
                var ft = field.FieldType;
                var v = field.GetValue(value);
                // Propertyが特殊な名前になってるため
                var name = field.Name;
                if (name.StartsWith("<"))
                {
                    name = name.Substring(1, name.IndexOf(">", StringComparison.Ordinal) - 1);
                }
                
                if (v == null)
                {
                    root.AddChild(new TreeViewItem(id++, depth, $"{name}: NULL"));
                    continue;
                }

                var valueType = v.GetType();
                if (valueType.IsArray)
                {
                    var arrayType = valueType.GetElementType();
                    var array = v as Array;
                    var dataItem = new TreeViewItem(id++, depth, $"{name}: [{array.Length}]");
                    root.AddChild(dataItem);
                    var index = 0;
                    foreach (var element in array)
                    {
                        if (arrayType.IsPrimitive)
                        {
                            dataItem.AddChild(new TreeViewItem(id++, depth + 1, $"[{index}]: {element}"));
                            continue;
                        }
                        
                        var elementItem = new TreeViewItem(id++, depth + 1, $"{index++}");
                        dataItem.AddChild(elementItem);
                        Recursive(ref id, elementItem, element, arrayType, depth + 2);
                    }
                }
                else if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var listType = valueType.GetGenericArguments()[0];
                    var list = v as IList;
                    var dataItem = new TreeViewItem(id++, depth, $"{name}: [{list.Count}]");
                    root.AddChild(dataItem);
                    var index = 0;
                    foreach (var element in list)
                    {
                        if (listType.IsPrimitive)
                        {
                            dataItem.AddChild(new TreeViewItem(id++, depth + 1, $"[{index}]: {element}"));
                            continue;
                        }
                        
                        var elementItem = new TreeViewItem(id++, depth + 1, $"{index++}");
                        dataItem.AddChild(elementItem);
                        Recursive(ref id, elementItem, element, listType, depth + 2);
                    }
                }
                else if (valueType.IsPrimitive || valueType == typeof(string))
                {
                    root.AddChild(new TreeViewItem(id++, depth, $"{name}: {v}"));
                }
                else
                {
                    var dataItem = new TreeViewItem(id++, depth, $"{name}");
                    root.AddChild(dataItem);
                    Recursive(ref id, dataItem, v, v.GetType(), depth + 1);
                }
            }
        }
    }
}

#endif