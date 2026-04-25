using System;
using System.Collections.Generic;
using System.Text;

namespace dominio
{
    public class ImagenArticulo
    {
        public int Id { get; set; }
        public int IdArticulo { get; set; }
        public string ImagenUrl { get; set; }

        public override string ToString()
        {
            return ImagenUrl;
        }
    }
}
