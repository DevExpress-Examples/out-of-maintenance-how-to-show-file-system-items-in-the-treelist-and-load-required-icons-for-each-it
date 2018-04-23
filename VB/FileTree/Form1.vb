Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports DevExpress.XtraEditors
Imports FileTreeList

Namespace FileTree
	Partial Public Class Form1
		Inherits XtraForm
		Private _helper As TreeListFileHelper
		Public Sub New()
			InitializeComponent()
			_helper = New TreeListFileHelper(treeList1)
		End Sub
	End Class
End Namespace
