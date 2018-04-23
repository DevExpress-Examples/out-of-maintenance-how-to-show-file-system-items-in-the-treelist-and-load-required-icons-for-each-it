Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.InteropServices

Namespace OpenDialogTest.Imported
	Friend Class Win32
		Public Const SHGFI_ICON As UInteger = &H100
		Public Const SHGFI_LARGEICON As UInteger = &H0
		Public Const SHGFI_SMALLICON As UInteger = &H1

		<DllImport("shell32.dll")> _
		Public Shared Function SHGetFileInfo(ByVal pszPath As String, ByVal dwFileAttributes As UInteger, ByRef psfi As SHFILEINFO, ByVal cbSizeFileInfo As UInteger, ByVal uFlags As UInteger) As IntPtr
		End Function
	End Class
End Namespace