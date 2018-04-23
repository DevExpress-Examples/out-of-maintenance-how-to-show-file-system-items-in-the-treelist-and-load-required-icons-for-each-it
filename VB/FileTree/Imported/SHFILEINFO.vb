Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.InteropServices

Namespace OpenDialogTest.Imported
	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure SHFILEINFO
		Public hIcon As IntPtr
		Public iIcon As IntPtr
		Public dwAttributes As UInteger
		<MarshalAs(UnmanagedType.ByValTStr, SizeConst := 260)> _
		Public szDisplayName As String
		<MarshalAs(UnmanagedType.ByValTStr, SizeConst := 80)> _
		Public szTypeName As String
	End Structure


End Namespace