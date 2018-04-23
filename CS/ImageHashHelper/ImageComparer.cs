using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageHashHelper
{
    public class ImageComparer : IEqualityComparer<Bitmap>
    {
        public ImageComparer()
        {

        }

        public bool Equals(Bitmap x, Bitmap y)
        {
            if (x.Size.Height == y.Size.Height
                && x.Size.Width == y.Size.Width)
                return GetHashCode(x) == GetHashCode(y);
            else
                return false;
        }

        public int GetHashCode(Bitmap obj)
        {
            int hash = 0;
            int x;
            int y;
            int pixelCount = obj.Width * obj.Height;

            for (int i = 0; i < pixelCount; i++)
            {
                x = i % obj.Width;
                y = i / obj.Width;
                hash ^= obj.GetPixel(x, y).ToArgb();
            }

            return hash;
        }
    }
}
