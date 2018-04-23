using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace OpenDialogTest.Imported
{
    public class Win32FunctionService
    {
        public static Image GetFileSystemImage(string fileName)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);
            Image image = Icon.FromHandle(shinfo.hIcon).ToBitmap();
            return image;
        }
    }
}
