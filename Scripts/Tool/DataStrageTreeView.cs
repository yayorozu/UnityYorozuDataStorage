#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace Yorozu.Data.Tool
{
    internal class DataStorageTreeView : TreeView
    {
        private string _typeName;
        
        internal DataStorageTreeView(TreeViewState state) : base(state)
        {
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            Reload();
            ExpandAll();
        }

        internal void Reload(string name)
        {
            _typeName = name;
            Reload();
            ExpandAll();
        }

        protected override TreeViewItem BuildRoot()
        {
            return EditorDataStorageUtility.GetDataTreeViewItem(_typeName);
        }
        
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            return !root.hasChildren ? new List<TreeViewItem>() : base.BuildRows(root);
        }
    }
}

#endif