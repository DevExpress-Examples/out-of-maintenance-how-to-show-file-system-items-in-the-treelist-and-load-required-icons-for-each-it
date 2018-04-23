Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Linq
Imports System.Windows.Forms
Imports DevExpress.Utils
Imports DevExpress.XtraTreeList
Imports DevExpress.XtraTreeList.Nodes
Imports OpenDialogTest.Imported
Imports ImageHashHelper

Namespace FileTreeList
	Public Class TreeListFileHelper
		Implements IDisposable
		Private Tree As TreeList
		Private _Images As ImageCollection
		Private _ImageStore As New Dictionary(Of Bitmap, Integer)(New ImageComparer())
		Private treeListColumn1 As DevExpress.XtraTreeList.Columns.TreeListColumn
		Private treeListColumn2 As DevExpress.XtraTreeList.Columns.TreeListColumn
		Private treeListColumn3 As DevExpress.XtraTreeList.Columns.TreeListColumn
		Private treeListColumn4 As DevExpress.XtraTreeList.Columns.TreeListColumn
		Private treeListColumn5 As DevExpress.XtraTreeList.Columns.TreeListColumn

		Public Sub New(ByVal tree As TreeList)
			Me.Tree = tree
			_Images = New ImageCollection()

			InitColumns()
			InitOptions()
			SubscribeEvents()
			InitData()
		End Sub

		Private Sub Tree_GetStateImage(ByVal sender As Object, ByVal e As GetStateImageEventArgs)
			Try
				Dim img As Bitmap = CType(Win32FunctionService.GetFileSystemImage(e.Node.GetDisplayText("FullName")), Bitmap)

				If (Not _ImageStore.ContainsKey(img)) Then
					_Images.AddImage(img)
					_ImageStore.Add(img, _Images.Images.Count - 1)
				End If

				e.NodeImageIndex = _ImageStore(img)
			Catch ex As Exception
			End Try
		End Sub

		Private Sub InitColumns()
			treeListColumn1 = New DevExpress.XtraTreeList.Columns.TreeListColumn()
			treeListColumn2 = New DevExpress.XtraTreeList.Columns.TreeListColumn()
			treeListColumn3 = New DevExpress.XtraTreeList.Columns.TreeListColumn()
			treeListColumn4 = New DevExpress.XtraTreeList.Columns.TreeListColumn()
			treeListColumn5 = New DevExpress.XtraTreeList.Columns.TreeListColumn()

			treeListColumn1.Caption = "FullName"
			treeListColumn1.FieldName = "FullName"

			treeListColumn2.Caption = "Name"
			treeListColumn2.FieldName = "Name"
			treeListColumn2.VisibleIndex = 0
			treeListColumn2.Visible = True

			treeListColumn3.Caption = "Type"
			treeListColumn3.FieldName = "Type"
			treeListColumn3.VisibleIndex = 1
			treeListColumn3.Visible = False

			treeListColumn4.AppearanceCell.Options.UseTextOptions = True
			treeListColumn4.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
			treeListColumn4.Caption = "Size(Bytes)"
			treeListColumn4.FieldName = "Size"
			treeListColumn4.VisibleIndex = 2
			treeListColumn4.Visible = False

			treeListColumn5.Caption = "treeListColumn5"
			treeListColumn5.FieldName = "Info"
			treeListColumn5.Name = "treeListColumn5"
			treeListColumn5.Visible = False

			Tree.Columns.AddRange(New DevExpress.XtraTreeList.Columns.TreeListColumn() { treeListColumn1, treeListColumn2, treeListColumn3, treeListColumn4, treeListColumn5 })

		End Sub

		Private Sub InitData()
			InitDocuments()
			InitDrivers()
		End Sub

		Private Sub InitDocuments()
			Dim myDocumentsPath As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			Dim di As New DirectoryInfo(myDocumentsPath)
            Dim node As TreeListNode = Tree.AppendNode(New Object() {myDocumentsPath, di.Name, "Folder", Nothing, di}, -1)
			InitFolders(myDocumentsPath, node)
		End Sub
		Private Sub InitDrivers()
			For Each dInfo As DriveInfo In DriveInfo.GetDrives()
                Dim driverNode As TreeListNode = Tree.AppendNode(New Object() {dInfo.Name, dInfo.Name, "Folder", Nothing, dInfo}, -1)
				InitFolders(dInfo.RootDirectory.Name, driverNode)
			Next dInfo
		End Sub
		Private Sub InitOptions()
			Tree.StateImageList = _Images

			Tree.OptionsBehavior.Editable = False
			Tree.OptionsView.ShowColumns = False
			Tree.OptionsView.ShowHorzLines = False
			Tree.OptionsView.ShowVertLines = False
		End Sub

		Public Sub SubscribeEvents()
			AddHandler Tree.BeforeExpand, AddressOf treeList1_BeforeExpand
			AddHandler Tree.AfterExpand, AddressOf treeList1_AfterExpand
			AddHandler Tree.AfterCollapse, AddressOf treeList1_AfterCollapse
			AddHandler Tree.GetStateImage, AddressOf Tree_GetStateImage
		End Sub

		Public Sub UnSubscribeEvents()
			RemoveHandler Tree.BeforeExpand, AddressOf treeList1_BeforeExpand
			RemoveHandler Tree.AfterExpand, AddressOf treeList1_AfterExpand
			RemoveHandler Tree.AfterCollapse, AddressOf treeList1_AfterCollapse
			RemoveHandler Tree.GetStateImage, AddressOf Tree_GetStateImage
		End Sub



		Private Sub InitFolders(ByVal path As String, ByVal pNode As TreeListNode)
			Tree.BeginUnboundLoad()
			Dim node As TreeListNode
			Dim di As DirectoryInfo
			Try
				Dim root() As String = Directory.GetDirectories(path)
				For Each s As String In root
					Try
						di = New DirectoryInfo(s)
						node = Tree.AppendNode(New Object() { s, di.Name, "Folder", Nothing, di }, pNode)
						node.StateImageIndex = 0
						node.HasChildren = HasFiles(s)
						If node.HasChildren Then
							node.Tag = True
						End If
					Catch
					End Try
				Next s
			Catch
			End Try
			InitFiles(path, pNode)
			Tree.EndUnboundLoad()
		End Sub

		Private Sub InitFiles(ByVal path As String, ByVal pNode As TreeListNode)
			Dim node As TreeListNode
			Dim fi As FileInfo
			Try
				Dim root() As String = Directory.GetFiles(path)
				For Each s As String In root
					fi = New FileInfo(s)
					node = Tree.AppendNode(New Object() { s, fi.Name, "File", fi.Length, fi }, pNode)
					node.StateImageIndex = 1
					node.HasChildren = False
				Next s
			Catch
			End Try
		End Sub

		Private Shared Function HasFiles(ByVal path As String) As Boolean
			Dim root() As String = Directory.GetFiles(path)
			If root.Length > 0 Then
				Return True
			End If
			root = Directory.GetDirectories(path)
			If root.Length > 0 Then
				Return True
			End If
			Return False
		End Function

		Private Sub treeList1_BeforeExpand(ByVal sender As Object, ByVal e As BeforeExpandEventArgs)
			If e.Node.Tag IsNot Nothing Then
				Dim currentCursor As Cursor = Cursor.Current
				Cursor.Current = Cursors.WaitCursor
				InitFolders(e.Node.GetDisplayText("FullName"), e.Node)
				e.Node.Tag = Nothing
				Cursor.Current = currentCursor
			End If
		End Sub

		Private Sub treeList1_AfterExpand(ByVal sender As Object, ByVal e As NodeEventArgs)
			If e.Node.StateImageIndex <> 1 Then
				e.Node.StateImageIndex = 2
			End If
		End Sub

		Private Sub treeList1_AfterCollapse(ByVal sender As Object, ByVal e As NodeEventArgs)
			If e.Node.StateImageIndex <> 1 Then
				e.Node.StateImageIndex = 0
			End If
		End Sub


		Public Sub Dispose() Implements IDisposable.Dispose
			UnSubscribeEvents()
			Tree = Nothing
		End Sub
	End Class
End Namespace
