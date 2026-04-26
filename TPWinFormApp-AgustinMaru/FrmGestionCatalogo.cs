using System;
using System.Collections.Generic;
using System.Windows.Forms;
using dominio;
using negocio;

namespace TPWinFormApp_AgustinMaru
{
    public enum CatalogoTipo
    {
        Marca,
        Categoria
    }

    public class FrmGestionCatalogo : Form
    {
        private readonly CatalogoTipo tipo;
        private readonly MarcaNegocio marcaNegocio;
        private readonly CategoriaNegocio categoriaNegocio;
        private DataGridView dgvItems;
        private TextBox txtDescripcion;

        public FrmGestionCatalogo() : this(CatalogoTipo.Marca)
        {
        }

        public FrmGestionCatalogo(CatalogoTipo tipo)
        {
            this.tipo = tipo;
            marcaNegocio = new MarcaNegocio();
            categoriaNegocio = new CategoriaNegocio();
            Text = tipo == CatalogoTipo.Marca ? "Gestion de Marcas" : "Gestion de Categorias";
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            StartPosition = FormStartPosition.CenterParent;
            Width = 600;
            Height = 430;

            dgvItems = new DataGridView
            {
                Left = 20,
                Top = 20,
                Width = 540,
                Height = 240,
                ReadOnly = true,
                AutoGenerateColumns = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false
            };
            dgvItems.SelectionChanged += DgvItems_SelectionChanged;

            Label lblDescripcion = new Label { Left = 20, Top = 285, Width = 100, Text = "Descripcion" };
            txtDescripcion = new TextBox { Left = 125, Top = 281, Width = 300 };

            Button btnAgregar = new Button { Left = 20, Top = 325, Width = 110, Text = "Agregar" };
            btnAgregar.Click += BtnAgregar_Click;
            Button btnModificar = new Button { Left = 140, Top = 325, Width = 110, Text = "Modificar" };
            btnModificar.Click += BtnModificar_Click;
            Button btnEliminar = new Button { Left = 260, Top = 325, Width = 110, Text = "Eliminar" };
            btnEliminar.Click += BtnEliminar_Click;
            Button btnCerrar = new Button { Left = 450, Top = 325, Width = 110, Text = "Cerrar" };
            btnCerrar.Click += BtnCerrar_Click;

            Controls.Add(dgvItems);
            Controls.Add(lblDescripcion);
            Controls.Add(txtDescripcion);
            Controls.Add(btnAgregar);
            Controls.Add(btnModificar);
            Controls.Add(btnEliminar);
            Controls.Add(btnCerrar);

            Load += FrmGestionCatalogo_Load;
        }

        private void FrmGestionCatalogo_Load(object sender, EventArgs e)
        {
            if (DesignHelper.EnModoDiseno(this))
            {
                dgvItems.DataSource = new List<Marca>
                {
                    new Marca { Id = 1, Descripcion = "Ejemplo" },
                    new Marca { Id = 2, Descripcion = "Demo" }
                };
                return;
            }

            CargarItems();
        }

        private void CargarItems()
        {
            dgvItems.DataSource = null;
            if (tipo == CatalogoTipo.Marca)
                dgvItems.DataSource = marcaNegocio.Listar();
            else
                dgvItems.DataSource = categoriaNegocio.Listar();
        }

        private IEntidadDescripcion ObtenerSeleccionado()
        {
            return dgvItems.CurrentRow != null ? dgvItems.CurrentRow.DataBoundItem as IEntidadDescripcion : null;
        }

        private void DgvItems_SelectionChanged(object sender, EventArgs e)
        {
            IEntidadDescripcion item = ObtenerSeleccionado();
            txtDescripcion.Text = item != null ? item.Descripcion : string.Empty;
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                if (tipo == CatalogoTipo.Marca)
                    marcaNegocio.Agregar(txtDescripcion.Text.Trim());
                else
                    categoriaNegocio.Agregar(txtDescripcion.Text.Trim());

                txtDescripcion.Clear();
                CargarItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo agregar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnModificar_Click(object sender, EventArgs e)
        {
            IEntidadDescripcion item = ObtenerSeleccionado();
            if (item == null)
            {
                MessageBox.Show("Debe seleccionar un registro.");
                return;
            }

            try
            {
                if (tipo == CatalogoTipo.Marca)
                    marcaNegocio.Modificar(item.Id, txtDescripcion.Text.Trim());
                else
                    categoriaNegocio.Modificar(item.Id, txtDescripcion.Text.Trim());

                CargarItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo modificar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            IEntidadDescripcion item = ObtenerSeleccionado();
            if (item == null)
            {
                MessageBox.Show("Debe seleccionar un registro.");
                return;
            }

            if (MessageBox.Show("¿Desea eliminar el registro seleccionado?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                if (tipo == CatalogoTipo.Marca)
                    marcaNegocio.Eliminar(item.Id);
                else
                    categoriaNegocio.Eliminar(item.Id);

                txtDescripcion.Clear();
                CargarItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
