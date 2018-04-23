Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports System.Runtime.InteropServices

Namespace OpenDialogTest.Imported
	Public Class Win32FunctionService
		Public Shared Function GetFileSystemImage(ByVal fileName As String) As Image
			Dim shinfo As New SHFILEINFO()
			Win32.SHGetFileInfo(fileName, 0, shinfo, CUInt(Marshal.SizeOf(shinfo)), Win32.SHGFI_ICON Or Win32.SHGFI_SMALLICON)
			Dim image As Image = Icon.FromHandle(shinfo.hIcon).ToBitmap()
			Return image
		End Function
	End Class
End Namespace
