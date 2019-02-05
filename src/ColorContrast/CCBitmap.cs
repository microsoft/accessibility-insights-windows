using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Deque.ColorContrast
{

    public class DequeBitmap : DequeImage
    {
        private readonly System.Drawing.Bitmap bitmap;

        public DequeBitmap(System.Drawing.Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        public override int NumColumns()
        {
            return bitmap.Width;
        }

        public override int NumRows()
        {
            return bitmap.Height;
        }

        public override DequeColor GetColor(int row, int column)
        {
            Color color = bitmap.GetPixel(column, row);

            return new DequeColor(color);
        }
    }
}
