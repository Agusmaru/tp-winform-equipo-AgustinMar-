using System;
using System.Windows.Forms;
using dominio;

namespace TPWinFormApp_AgustinMaru
{
    public class FrmDetalleArticulo : Form
    {
        private readonly Articulo articulo;
        private ListBox lstImagenes;
        private PictureBox pbImagen;

        public FrmDetalleArticulo() : this(new Articulo
        {
            Codigo = "A001",
            Nombre = "Articulo demo",
            Descripcion = "Vista previa para el disenador.",
            Precio = 999.99m,
            Marca = new Marca { Descripcion = "Marca demo" },
            Categoria = new Categoria { Descripcion = "Categoria demo" }
        })
        {
        }

        public FrmDetalleArticulo(Articulo articulo)
        {
            this.articulo = articulo;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Detalle del articulo";
            StartPosition = FormStartPosition.CenterParent;
            Width = 900;
            Height = 560;

            Label lblCodigo = new Label { Left = 20, Top = 20, Width = 400, Text = "Codigo: " + articulo.Codigo };
            Label lblNombre = new Label { Left = 20, Top = 50, Width = 400, Text = "Nombre: " + articulo.Nombre };
            Label lblMarca = new Label { Left = 20, Top = 80, Width = 400, Text = "Marca: " + articulo.MarcaDescripcion };
            Label lblCategoria = new Label { Left = 20, Top = 110, Width = 400, Text = "Categoria: " + articulo.CategoriaDescripcion };
            Label lblPrecio = new Label { Left = 20, Top = 140, Width = 400, Text = "Precio: " + articulo.Precio.ToString("C2") };
            Label lblDescripcion = new Label { Left = 20, Top = 180, Width = 100, Text = "Descripcion" };

            TextBox txtDescripcion = new TextBox
            {
                Left = 20,
                Top = 205,
                Width = 370,
                Height = 150,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Text = articulo.Descripcion
            };

            Label lblImagenes = new Label { Left = 430, Top = 20, Width = 150, Text = "Imagenes asociadas" };
            lstImagenes = new ListBox { Left = 430, Top = 45, Width = 420, Height = 140 };
            lstImagenes.SelectedIndexChanged += LstImagenes_SelectedIndexChanged;

            pbImagen = new PictureBox
            {
                Left = 430,
                Top = 205,
                Width = 420,
                Height = 250,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            Button btnCerrar = new Button { Left = 740, Top = 470, Width = 110, Text = "Cerrar" };
            btnCerrar.Click += BtnCerrar_Click;

            Controls.Add(lblCodigo);
            Controls.Add(lblNombre);
            Controls.Add(lblMarca);
            Controls.Add(lblCategoria);
            Controls.Add(lblPrecio);
            Controls.Add(lblDescripcion);
            Controls.Add(txtDescripcion);
            Controls.Add(lblImagenes);
            Controls.Add(lstImagenes);
            Controls.Add(pbImagen);
            Controls.Add(btnCerrar);

            Load += FrmDetalleArticulo_Load;
        }

        private void FrmDetalleArticulo_Load(object sender, EventArgs e)
        {
            if (DesignHelper.EnModoDiseno(this))
                return;

            lstImagenes.DataSource = articulo.Imagenes;
            if (lstImagenes.Items.Count > 0)
                lstImagenes.SelectedIndex = 0;
            else
                ImagenHelper.Cargar(pbImagen, null);
        }

        private void LstImagenes_SelectedIndexChanged(object sender, EventArgs e)
        {
            ImagenArticulo imagen = lstImagenes.SelectedItem as ImagenArticulo;
            ImagenHelper.Cargar(pbImagen, imagen != null ? imagen.ImagenUrl : null);
        }

        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
