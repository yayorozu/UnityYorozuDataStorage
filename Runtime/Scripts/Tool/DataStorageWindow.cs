using System;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR

namespace Yorozu.Data.Tool
{
    internal class DataStorageWindow : EditorWindow
    {
        internal static DataStorageWindow instance;
        
        [MenuItem("Tools/DataStorageWindow")]
        private static void ShowWindow()
        {
            var window = GetWindow<DataStorageWindow>("DataStorageWindow");
            window.Show();
        }

        private string[] _activeNames;
        private DataStorageTreeView _treeView;
        private TreeViewState _state;

        private void OnDisable()
        {
            instance = null;
        }

        private void OnEnable()
        {
            instance = this;
            Refresh();
        }

        internal void Refresh()
        {
            _activeNames = EditorDataStorageUtility.GetActiveDataTypeNames();
            Repaint();
        }

        private void Initialize()
        {
            _state ??= new TreeViewState();
            _treeView ??= new DataStorageTreeView(_state);
        }

        private void OnGUI()
        {
            Initialize();
            
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope(GUILayout.Width(200)))
                {
                    DrawLeft();
                }

                var rect = GUILayoutUtility.GetRect(2, 2, 0, 100000);
                EditorGUI.DrawRect(rect, Color.black);
                _treeView?.OnGUI(rect);
            }
        }

        private void DrawLeft()
        {
            if (_activeNames == null || _activeNames.Length <= 0)
            {
                EditorGUILayout.LabelField("Data is Empty");
                return;
            }

            foreach (var activeName in _activeNames)
            {
                if (GUILayout.Button(activeName))
                {
                    _treeView?.Reload(activeName);
                }
            }
        }
    }
}

#endif