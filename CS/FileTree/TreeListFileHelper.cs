using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using OpenDialogTest.Imported;
using ImageHashHelper;

namespace FileTreeList
{
    public class TreeListFileHelper : IDisposable
    {
        TreeList Tree;
        ImageCollection _Images;
        private Dictionary<Bitmap, int> _ImageStore = new Dictionary<Bitmap, int>(new ImageComparer());
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn3;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn4;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn5;

        public TreeListFileHelper(TreeList tree)
        {
            Tree = tree;
            _Images = new ImageCollection();

            InitColumns();
            InitOptions();
            SubscribeEvents();
            InitData();
        }

        void Tree_GetStateImage(object sender, GetStateImageEventArgs e)
        {
            try
            {
                Bitmap img = (Bitmap)Win32FunctionService.GetFileSystemImage(e.Node.GetDisplayText("FullName"));

                if (!_ImageStore.ContainsKey(img))
                {
                    _Images.AddImage(img);
                    _ImageStore.Add(img, _Images.Images.Count - 1);
                }

                e.NodeImageIndex = _ImageStore[img];
            }
            catch (Exception ex)
            {
            }
        }

        void InitColumns()
        {
            treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            treeListColumn3 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            treeListColumn4 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            treeListColumn5 = new DevExpress.XtraTreeList.Columns.TreeListColumn();

            treeListColumn1.Caption = "FullName";
            treeListColumn1.FieldName = "FullName";

            treeListColumn2.Caption = "Name";
            treeListColumn2.FieldName = "Name";
            treeListColumn2.VisibleIndex = 0;
            treeListColumn2.Visible = true;

            treeListColumn3.Caption = "Type";
            treeListColumn3.FieldName = "Type";
            treeListColumn3.VisibleIndex = 1;
            treeListColumn3.Visible = false;

            treeListColumn4.AppearanceCell.Options.UseTextOptions = true;
            treeListColumn4.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            treeListColumn4.Caption = "Size(Bytes)";
            treeListColumn4.FieldName = "Size";
            treeListColumn4.VisibleIndex = 2;
            treeListColumn4.Visible = false;

            treeListColumn5.Caption = "treeListColumn5";
            treeListColumn5.FieldName = "Info";
            treeListColumn5.Name = "treeListColumn5";
            treeListColumn5.Visible = false;

            Tree.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            treeListColumn1,
            treeListColumn2,
            treeListColumn3,
            treeListColumn4,
            treeListColumn5,
            });

        }

        private void InitData()
        {
            InitDocuments();
            InitDrivers();
        }

        private void InitDocuments()
        {
            string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            DirectoryInfo di = new DirectoryInfo(myDocumentsPath);
            TreeListNode node = Tree.AppendNode(new object[] { myDocumentsPath, di.Name, "Folder", null, di }, null);
            InitFolders(myDocumentsPath, node);
        }
        private void InitDrivers()
        {
            foreach (DriveInfo dInfo in DriveInfo.GetDrives())
            {
                TreeListNode driverNode = Tree.AppendNode(new object[] { dInfo.Name, dInfo.Name, "Folder", null, dInfo }, null);
                InitFolders(dInfo.RootDirectory.Name, driverNode);
            }
        }
        private void InitOptions()
        {
            Tree.StateImageList = _Images;

            Tree.OptionsBehavior.Editable = false;
            Tree.OptionsView.ShowColumns = false;
            Tree.OptionsView.ShowHorzLines = false;
            Tree.OptionsView.ShowVertLines = false;
        }

        public void SubscribeEvents()
        {
            Tree.BeforeExpand += treeList1_BeforeExpand;
            Tree.AfterExpand += treeList1_AfterExpand;
            Tree.AfterCollapse += treeList1_AfterCollapse;
            Tree.GetStateImage += Tree_GetStateImage;
        }

        public void UnSubscribeEvents()
        {
            Tree.BeforeExpand -= treeList1_BeforeExpand;
            Tree.AfterExpand -= treeList1_AfterExpand;
            Tree.AfterCollapse -= treeList1_AfterCollapse;
            Tree.GetStateImage -= Tree_GetStateImage;
        }



        private void InitFolders(string path, TreeListNode pNode)
        {
            Tree.BeginUnboundLoad();
            TreeListNode node;
            DirectoryInfo di;
            try
            {
                string[] root = Directory.GetDirectories(path);
                foreach (string s in root)
                {
                    try
                    {
                        di = new DirectoryInfo(s);
                        node = Tree.AppendNode(new object[] { s, di.Name, "Folder", null, di }, pNode);
                        node.StateImageIndex = 0;
                        node.HasChildren = HasFiles(s);
                        if (node.HasChildren)
                            node.Tag = true;
                    }
                    catch { }
                }
            }
            catch { }
            InitFiles(path, pNode);
            Tree.EndUnboundLoad();
        }

        private void InitFiles(string path, TreeListNode pNode)
        {
            TreeListNode node;
            FileInfo fi;
            try
            {
                string[] root = Directory.GetFiles(path);
                foreach (string s in root)
                {
                    fi = new FileInfo(s);
                    node = Tree.AppendNode(new object[] { s, fi.Name, "File", fi.Length, fi }, pNode);
                    node.StateImageIndex = 1;
                    node.HasChildren = false;
                }
            }
            catch { }
        }

        private static bool HasFiles(string path)
        {
            string[] root = Directory.GetFiles(path);
            if (root.Length > 0) return true;
            root = Directory.GetDirectories(path);
            if (root.Length > 0) return true;
            return false;
        }

        private void treeList1_BeforeExpand(object sender, BeforeExpandEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                Cursor currentCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                InitFolders(e.Node.GetDisplayText("FullName"), e.Node);
                e.Node.Tag = null;
                Cursor.Current = currentCursor;
            }
        }

        private void treeList1_AfterExpand(object sender, NodeEventArgs e)
        {
            if (e.Node.StateImageIndex != 1) e.Node.StateImageIndex = 2;
        }

        private void treeList1_AfterCollapse(object sender, NodeEventArgs e)
        {
            if (e.Node.StateImageIndex != 1) e.Node.StateImageIndex = 0;
        }


        public void Dispose()
        {
            UnSubscribeEvents();
            Tree = null;
        }
    }
}
