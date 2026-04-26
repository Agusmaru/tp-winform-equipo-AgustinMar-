using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using dominio;
using negocio;

namespace TPWinFormApp_AgustinMaru
{
    public class FrmArticulo : Form
    {
        private readonly ArticuloNegocio articuloNegocio = new ArticuloNegocio();
        private readonly MarcaNegocio marcaNegocio = new MarcaNegocio();
        private readonly CategoriaNegocio categoriaNegocio = new CategoriaNegocio();
        private readonly Articulo articulo;

        private TextBox txtCodigo;
        private TextBox txtNombre;
        private TextBox txtDescripcion;
        private ComboBox cboMarca;
        private ComboBox cboCategoria;
        private TextBox txtPrecio;
        private TextBox txtNuevaImagen;
        private ListBox lstImagenes;
        private PictureBox pbPreview;

        public FrmArticulo() : this(null)
        {
        }

        public FrmArticulo(Articulo articulo)
        {
            this.articulo = articulo ?? new Articulo();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = articulo.Id > 0 ? "Modificar articulo" : "Agregar articulo";
            StartPosition = FormStartPosition.CenterParent;
            Width = 980;
            Height = 620;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            Label lblCodigo = new Label { Left = 20, Top = 22, Width = 130, Text = "Codigo" };
            txtCodigo = new TextBox { Left = 160, Top = 18, Width = 250 };
            Label lblNombre = new Label { Left = 20, Top = 57, Width = 130, Text = "Nombre" };
            txtNombre = new TextBox { Left = 160, Top = 53, Width = 250 };
            Label lblDescripcion = new Label { Left = 20, Top = 92, Width = 130, Text = "Descripcion" };
            txtDescripcion = new TextBox { Left = 160, Top = 88, Width = 320, Height = 90, Multiline = true, ScrollBars = ScrollBars.Vertical };
            Label lblMarca = new Label { Left = 20, Top = 195, Width = 130, Text = "Marca" };
            cboMarca = new ComboBox { Left = 160, Top = 191, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            Label lblCategoria = new Label { Left = 20, Top = 230, Width = 130, Text = "Categoria" };
            cboCategoria = new ComboBox { Left = 160, Top = 226, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            Label lblPrecio = new Label { Left = 20, Top = 265, Width = 130, Text = "Precio" };
            txtPrecio = new TextBox { Left = 160, Top = 261, Width = 150 };

            Label lblImagenes = new Label { Left = 520, Top = 22, Width = 150, Text = "Imagenes del articulo" };
            txtNuevaImagen = new TextBox { Left = 520, Top = 50, Width = 300 };
            Button btnExaminar = new Button { Left = 830, Top = 48, Width = 100, Text = "Archivo..." };
            btnExaminar.Click += BtnExaminar_Click;
            Button btnAgregarImagen = new Button { Left = 520, Top = 85, Width = 130, Text = "Agregar imagen" };
            btnAgregarImagen.Click += BtnAgregarImagen_Click;
            Button btnQuitarImagen = new Button { Left = 660, Top = 85, Width = 130, Text = "Quitar imagen" };
            btnQuitarImagen.Click += BtnQuitarImagen_Click;

            lstImagenes = new ListBox { Left = 520, Top = 125, Width = 410, Height = 170 };
            lstImagenes.SelectedIndexChanged += LstImagenes_SelectedIndexChanged;

            pbPreview = new PictureBox
            {
                Left = 520,
                Top = 315,
                Width = 410,
                Height = 210,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };

            Button btnAceptar = new Button { Left = 690, Top = 540, Width = 110, Text = "Guardar" };
            btnAceptar.Click += BtnAceptar_Click;
            Button btnCancelar = new Button { Left = 820, Top = 540, Width = 110, Text = "Cancelar" };
            btnCancelar.Click += BtnCancelar_Click;

            Controls.Add(lblCodigo);
            Controls.Add(txtCodigo);
            Controls.Add(lblNombre);
            Controls.Add(txtNombre);
            Controls.Add(lblDescripcion);
            Controls.Add(txtDescripcion);
            Controls.Add(lblMarca);
            Controls.Add(cboMarca);
            Controls.Add(lblCategoria);
            Controls.Add(cboCategoria);
            Controls.Add(lblPrecio);
            Controls.Add(txtPrecio);
            Controls.Add(lblImagenes);
            Controls.Add(txtNuevaImagen);
            Controls.Add(btnExaminar);
            Controls.Add(btnAgregarImagen);
            Controls.Add(btnQuitarImagen);
            Controls.Add(lstImagenes);
            Controls.Add(pbPreview);
            Controls.Add(btnAceptar);
            Controls.Add(btnCancelar);

            Load += FrmArticulo_Load;
        }

        private void FrmArticulo_Load(object sender, EventArgs e)
        {
            if (DesignHelper.EnModoDiseno(this))
            {
                cboMarca.DataSource = new[] { new Marca { Id = 1, Descripcion = "Marca demo" } };
                cboMarca.DisplayMember = "Descripcion";
                cboCategoria.DataSource = new[] { new Categoria { Id = 1, Descripcion = "Categoria demo" } };
                cboCategoria.DisplayMember = "Descripcion";
                txtCodigo.Text = "A001";
                txtNombre.Text = "Articulo demo";
                txtDescripcion.Text = "Contenido de ejemplo para el disenador.";
                txtPrecio.Text = "999,99";
                return;
            }

            try
            {
                cboMarca.DataSource = marcaNegocio.Listar();
                cboMarca.ValueMember = "Id";
                cboMarca.DisplayMember = "Descripcion";

                cboCategoria.DataSource = categoriaNegocio.Listar();
                cboCategoria.ValueMember = "Id";
                cboCategoria.DisplayMember = "Descripcion";

                if (articulo.Id > 0)
                {
                    txtCodigo.Text = articulo.Codigo;
                    txtNombre.Text = articulo.Nombre;
                    txtDescripcion.Text = articulo.Descripcion;
                    txtPrecio.Text = articulo.Precio.ToString("0.##");
                    cboMarca.SelectedValue = articulo.Marca.Id;
                    cboCategoria.SelectedValue = articulo.Categoria.Id;
                }

                RefrescarListaImagenes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo abrir el formulario", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void BtnExaminar_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialogo = new OpenFileDialog();
            dialogo.Filter = "Imagenes|*.jpg;*.jpeg;*.png;*.bmp;*.webp";
            if (dialogo.ShowDialog() == DialogResult.OK)
                txtNuevaImagen.Text = dialogo.FileName;
        }

        private void BtnAgregarImagen_Click(object sender, EventArgs e)
        {
            string ruta = txtNuevaImagen.Text.Trim();
            if (string.IsNullOrWhiteSpace(ruta))
            {
                MessageBox.Show("Ingrese una URL o seleccione un archivo de imagen.");
                return;
            }

            if (!ruta.StartsWith("http", StringComparison.OrdinalIgnoreCase) && !File.Exists(ruta))
            {
                MessageBox.Show("La ruta local seleccionada no existe.");
                return;
            }

            articulo.Imagenes.Add(new ImagenArticulo { ImagenUrl = ruta });
            txtNuevaImagen.Clear();
            RefrescarListaImagenes();
        }

        private void BtnQuitarImagen_Click(object sender, EventArgs e)
        {
            ImagenArticulo seleccionada = lstImagenes.SelectedItem as ImagenArticulo;
            if (seleccionada == null)
                return;

            articulo.Imagenes.Remove(seleccionada);
            RefrescarListaImagenes();
        }

        private void LstImagenes_SelectedIndexChanged(object sender, EventArgs e)
        {
            ImagenArticulo seleccionada = lstImagenes.SelectedItem as ImagenArticulo;
            ImagenHelper.Cargar(pbPreview, seleccionada != null ? seleccionada.ImagenUrl : null);
        }

        private void BtnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                decimal precio;
                if (!decimal.TryParse(txtPrecio.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out precio) &&
                    !decimal.TryParse(txtPrecio.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out precio))
                    throw new ArgumentException("Ingrese un precio valido.");

                articulo.Codigo = txtCodigo.Text.Trim();
                articulo.Nombre = txtNombre.Text.Trim();
                articulo.Descripcion = txtDescripcion.Text.Trim();
                articulo.Precio = precio;
                articulo.Marca = cboMarca.SelectedItem as Marca;
                articulo.Categoria = cboCategoria.SelectedItem as Categoria;
                articulo.Imagenes = articulo.Imagenes
                    .Where(x => !string.IsNullOrWhiteSpace(x.ImagenUrl))
                    .GroupBy(x => x.ImagenUrl.Trim().ToUpper())
                    .Select(g => new ImagenArticulo { ImagenUrl = g.First().ImagenUrl.Trim() })
                    .ToList();

                if (articulo.Id > 0)
                    articuloNegocio.Modificar(articulo);
                else
                    articuloNegocio.Agregar(articulo);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo guardar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void RefrescarListaImagenes()
        {
            lstImagenes.DataSource = null;
            lstImagenes.DataSource = articulo.Imagenes.ToList();

            if (lstImagenes.Items.Count > 0)
                lstImagenes.SelectedIndex = 0;
            else
                ImagenHelper.Cargar(pbPreview, null);
        }
    }
}
