using System;
using System.Collections.Generic;
using System.Windows.Forms;
using dominio;
using negocio;

namespace TPWinFormApp_AgustinMaru
{
    public class FrmPrincipal : Form
    {
        private readonly ArticuloNegocio articuloNegocio = new ArticuloNegocio();
        private List<Articulo> articulos = new List<Articulo>();
        private TextBox txtBusquedaRapida;
        private ComboBox cboCampo;
        private ComboBox cboCriterio;
        private TextBox txtFiltroAvanzado;
        private DataGridView dgvArticulos;
        private PictureBox pbArticulo;
        private Label lblResultados;

        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Catalogo de Articulos";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 1180;
            Height = 680;

            Label lblBusquedaRapida = new Label { Left = 20, Top = 18, Width = 120, Text = "Busqueda rapida" };
            txtBusquedaRapida = new TextBox { Left = 145, Top = 14, Width = 250 };
            txtBusquedaRapida.TextChanged += TxtBusquedaRapida_TextChanged;

            Label lblCampo = new Label { Left = 420, Top = 18, Width = 50, Text = "Campo" };
            cboCampo = new ComboBox { Left = 470, Top = 14, Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
            cboCampo.SelectedIndexChanged += CboCampo_SelectedIndexChanged;

            Label lblCriterio = new Label { Left = 625, Top = 18, Width = 55, Text = "Criterio" };
            cboCriterio = new ComboBox { Left = 685, Top = 14, Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };

            txtFiltroAvanzado = new TextBox { Left = 840, Top = 14, Width = 150 };
            Button btnBuscar = new Button { Left = 1005, Top = 12, Width = 70, Text = "Buscar" };
            btnBuscar.Click += BtnBuscar_Click;
            Button btnLimpiar = new Button { Left = 1080, Top = 12, Width = 70, Text = "Limpiar" };
            btnLimpiar.Click += BtnLimpiar_Click;

            dgvArticulos = new DataGridView
            {
                Left = 20,
                Top = 55,
                Width = 810,
                Height = 520,
                ReadOnly = true,
                AutoGenerateColumns = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false
            };
            dgvArticulos.SelectionChanged += DgvArticulos_SelectionChanged;
            dgvArticulos.CellDoubleClick += DgvArticulos_CellDoubleClick;

            pbArticulo = new PictureBox
            {
                Left = 850,
                Top = 55,
                Width = 300,
                Height = 300,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };

            Button btnAgregar = new Button { Left = 850, Top = 380, Width = 140, Height = 35, Text = "Agregar" };
            btnAgregar.Click += BtnAgregar_Click;
            Button btnModificar = new Button { Left = 1010, Top = 380, Width = 140, Height = 35, Text = "Modificar" };
            btnModificar.Click += BtnModificar_Click;
            Button btnEliminar = new Button { Left = 850, Top = 425, Width = 140, Height = 35, Text = "Eliminar" };
            btnEliminar.Click += BtnEliminar_Click;
            Button btnDetalle = new Button { Left = 1010, Top = 425, Width = 140, Height = 35, Text = "Ver detalle" };
            btnDetalle.Click += BtnDetalle_Click;
            Button btnMarcas = new Button { Left = 850, Top = 470, Width = 140, Height = 35, Text = "Gestionar marcas" };
            btnMarcas.Click += BtnMarcas_Click;
            Button btnCategorias = new Button { Left = 1010, Top = 470, Width = 140, Height = 35, Text = "Gestionar categorias" };
            btnCategorias.Click += BtnCategorias_Click;
            Button btnActualizar = new Button { Left = 850, Top = 515, Width = 300, Height = 35, Text = "Actualizar listado" };
            btnActualizar.Click += BtnActualizar_Click;

            lblResultados = new Label { Left = 20, Top = 585, Width = 500, Text = "0 articulos cargados" };

            Controls.Add(lblBusquedaRapida);
            Controls.Add(txtBusquedaRapida);
            Controls.Add(lblCampo);
            Controls.Add(cboCampo);
            Controls.Add(lblCriterio);
            Controls.Add(cboCriterio);
            Controls.Add(txtFiltroAvanzado);
            Controls.Add(btnBuscar);
            Controls.Add(btnLimpiar);
            Controls.Add(dgvArticulos);
            Controls.Add(pbArticulo);
            Controls.Add(btnAgregar);
            Controls.Add(btnModificar);
            Controls.Add(btnEliminar);
            Controls.Add(btnDetalle);
            Controls.Add(btnMarcas);
            Controls.Add(btnCategorias);
            Controls.Add(btnActualizar);
            Controls.Add(lblResultados);

            Load += FrmPrincipal_Load;
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            cboCampo.Items.AddRange(new object[] { "Codigo", "Nombre", "Marca", "Categoria", "Precio" });
            cboCampo.SelectedIndex = 0;

            if (DesignHelper.EnModoDiseno(this))
            {
                CargarGrilla(new List<Articulo>
                {
                    new Articulo
                    {
                        Codigo = "A001",
                        Nombre = "Articulo demo",
                        Descripcion = "Vista previa del disenador",
                        Precio = 999.99m,
                        Marca = new Marca { Descripcion = "Marca demo" },
                        Categoria = new Categoria { Descripcion = "Categoria demo" }
                    }
                });
                return;
            }

            CargarArticulos();
        }

        private void CargarArticulos()
        {
            try
            {
                articulos = articuloNegocio.Listar();
                CargarGrilla(articulos);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error al cargar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarGrilla(List<Articulo> lista)
        {
            dgvArticulos.DataSource = null;
            dgvArticulos.DataSource = lista;

            if (dgvArticulos.Columns["Id"] != null) dgvArticulos.Columns["Id"].Visible = false;
            if (dgvArticulos.Columns["Marca"] != null) dgvArticulos.Columns["Marca"].Visible = false;
            if (dgvArticulos.Columns["Categoria"] != null) dgvArticulos.Columns["Categoria"].Visible = false;
            if (dgvArticulos.Columns["Imagenes"] != null) dgvArticulos.Columns["Imagenes"].Visible = false;
            if (dgvArticulos.Columns["ImagenPrincipal"] != null) dgvArticulos.Columns["ImagenPrincipal"].Visible = false;
            if (dgvArticulos.Columns["MarcaDescripcion"] != null) dgvArticulos.Columns["MarcaDescripcion"].HeaderText = "Marca";
            if (dgvArticulos.Columns["CategoriaDescripcion"] != null) dgvArticulos.Columns["CategoriaDescripcion"].HeaderText = "Categoria";
            if (dgvArticulos.Columns["Descripcion"] != null) dgvArticulos.Columns["Descripcion"].Width = 220;
            if (dgvArticulos.Columns["Precio"] != null) dgvArticulos.Columns["Precio"].DefaultCellStyle.Format = "C2";

            lblResultados.Text = lista.Count + " articulos cargados";

            if (dgvArticulos.Rows.Count > 0)
                dgvArticulos.Rows[0].Selected = true;

            MostrarImagenSeleccionada();
        }

        private void TxtBusquedaRapida_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtBusquedaRapida.Text.Trim().ToUpper();
            if (filtro.Length < 2)
            {
                CargarGrilla(articulos);
                return;
            }

            List<Articulo> filtrada = articulos.FindAll(x =>
                (x.Codigo ?? string.Empty).ToUpper().Contains(filtro) ||
                (x.Nombre ?? string.Empty).ToUpper().Contains(filtro) ||
                (x.Descripcion ?? string.Empty).ToUpper().Contains(filtro) ||
                (x.MarcaDescripcion ?? string.Empty).ToUpper().Contains(filtro) ||
                (x.CategoriaDescripcion ?? string.Empty).ToUpper().Contains(filtro));

            CargarGrilla(filtrada);
        }

        private void CboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboCriterio.Items.Clear();
            if (cboCampo.SelectedItem != null && cboCampo.SelectedItem.ToString() == "Precio")
                cboCriterio.Items.AddRange(new object[] { "Mayor a", "Menor a", "Igual a" });
            else
                cboCriterio.Items.AddRange(new object[] { "Comienza con", "Termina con", "Contiene" });

            if (cboCriterio.Items.Count > 0)
                cboCriterio.SelectedIndex = 0;
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboCampo.SelectedItem == null || cboCriterio.SelectedItem == null)
                {
                    MessageBox.Show("Debe seleccionar campo y criterio.");
                    return;
                }

                CargarGrilla(articuloNegocio.Filtrar(
                    cboCampo.SelectedItem.ToString(),
                    cboCriterio.SelectedItem.ToString(),
                    txtFiltroAvanzado.Text.Trim()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Filtro invalido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            txtBusquedaRapida.Clear();
            txtFiltroAvanzado.Clear();
            CargarArticulos();
        }

        private void DgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            MostrarImagenSeleccionada();
        }

        private void DgvArticulos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            VerDetalle();
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            using (FrmArticulo frm = new FrmArticulo())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                    CargarArticulos();
            }
        }

        private void BtnModificar_Click(object sender, EventArgs e)
        {
            Articulo articulo = ObtenerArticuloSeleccionado();
            if (articulo == null)
                return;

            using (FrmArticulo frm = new FrmArticulo(articuloNegocio.ObtenerPorId(articulo.Id)))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                    CargarArticulos();
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            Articulo articulo = ObtenerArticuloSeleccionado();
            if (articulo == null)
                return;

            if (MessageBox.Show("Se eliminara el articulo y todas sus imagenes. ¿Desea continuar?", "Confirmar eliminacion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                articuloNegocio.Eliminar(articulo.Id);
                CargarArticulos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDetalle_Click(object sender, EventArgs e)
        {
            VerDetalle();
        }

        private void BtnMarcas_Click(object sender, EventArgs e)
        {
            using (FrmGestionCatalogo frm = new FrmGestionCatalogo(CatalogoTipo.Marca))
                frm.ShowDialog();

            CargarArticulos();
        }

        private void BtnCategorias_Click(object sender, EventArgs e)
        {
            using (FrmGestionCatalogo frm = new FrmGestionCatalogo(CatalogoTipo.Categoria))
                frm.ShowDialog();

            CargarArticulos();
        }

        private void BtnActualizar_Click(object sender, EventArgs e)
        {
            CargarArticulos();
        }

        private Articulo ObtenerArticuloSeleccionado()
        {
            if (dgvArticulos.CurrentRow == null)
            {
                MessageBox.Show("Debe seleccionar un articulo.");
                return null;
            }

            return dgvArticulos.CurrentRow.DataBoundItem as Articulo;
        }

        private void MostrarImagenSeleccionada()
        {
            try
            {
                Articulo articulo = dgvArticulos.CurrentRow != null ? dgvArticulos.CurrentRow.DataBoundItem as Articulo : null;
                ImagenHelper.Cargar(pbArticulo, articulo != null ? articulo.ImagenPrincipal : null);
            }
            catch
            {
            }
        }

        private void VerDetalle()
        {
            Articulo articulo = ObtenerArticuloSeleccionado();
            if (articulo == null)
                return;

            using (FrmDetalleArticulo frm = new FrmDetalleArticulo(articuloNegocio.ObtenerPorId(articulo.Id)))
                frm.ShowDialog();
        }
    }
}
