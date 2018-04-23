Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace ImageHashHelper
	Public Class ImageComparer
		Implements IEqualityComparer(Of Bitmap)
		Public Sub New()

		End Sub

		Public Overloads Function Equals(ByVal x As Bitmap, ByVal y As Bitmap) As Boolean Implements IEqualityComparer(Of Bitmap).Equals
			If x.Size.Height = y.Size.Height AndAlso x.Size.Width = y.Size.Width Then
				Return GetHashCode(x) = GetHashCode(y)
			Else
				Return False
			End If
		End Function

		Public Overloads Function GetHashCode(ByVal obj As Bitmap) As Integer Implements IEqualityComparer(Of Bitmap).GetHashCode
			Dim hash As Integer = 0
			Dim x As Integer
			Dim y As Integer
			Dim pixelCount As Integer = obj.Width * obj.Height

			For i As Integer = 0 To pixelCount - 1
				x = i Mod obj.Width
                y = Math.Floor(i / obj.Width)
				hash = hash Xor obj.GetPixel(x, y).ToArgb()
			Next i

			Return hash
		End Function
	End Class
End Namespace
