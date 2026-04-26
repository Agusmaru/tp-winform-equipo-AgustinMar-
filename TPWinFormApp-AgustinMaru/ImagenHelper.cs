using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace TPWinFormApp_AgustinMaru
{
    internal static class ImagenHelper
    {
        public static void Cargar(PictureBox pictureBox, string ruta)
        {
            string placeholder = ConfigurationManager.AppSettings["placeholder-image"];
            try
            {
                if (string.IsNullOrWhiteSpace(ruta))
                    CargarPlaceholderSeguro(pictureBox, placeholder);
                else
                    pictureBox.Load(ruta);
            }
            catch
            {
                CargarPlaceholderSeguro(pictureBox, placeholder);
            }
        }

        private static void CargarPlaceholderSeguro(PictureBox pictureBox, string placeholder)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(placeholder))
                {
                    pictureBox.Load(placeholder);
                    return;
                }
            }
            catch
            {
            }

            Bitmap bitmap = new Bitmap(600, 400);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Gainsboro);
                using (Font fuente = new Font("Segoe UI", 24, FontStyle.Bold))
                using (SolidBrush pincel = new SolidBrush(Color.DimGray))
                {
                    StringFormat formato = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    graphics.DrawString("Sin imagen", fuente, pincel, new RectangleF(0, 0, bitmap.Width, bitmap.Height), formato);
                }
            }

            pictureBox.Image = bitmap;
        }
    }
}
