using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;
using static Pruebas.InterfazGrupoA;

namespace Pruebas
{
    [TestClass]
    public class PruebasGrupoAPropietario
    {
        // CP-69 — RF-12, RNF-03, RN-39; PA-04, C9.1/9.17: selector y acciones accesibles.
        [DataTestMethod]
        [DataRow(1f, DisplayName = "CP-69: Agregar evento conserva selector y acciones al 100 por ciento")]
        [DataRow(1.5f, DisplayName = "CP-69: Agregar evento conserva acciones en notebook al 150 por ciento")]
        public void CP69_SelectorYAcciones(float factor)
        {
            EnSta(() =>
            {
                Fake.ComoAdministrador();
                using (var form = new Visual.Propietario.frmAgregarEvento())
                {
                    Mostrar(form);
                    DateTimePicker preventa = ControlDe<DateTimePicker>(form, "dtpFinPreventa");
                    Assert.IsTrue(preventa.ShowCheckBox);
                    Assert.IsFalse(preventa.Checked);
                    Assert.AreEqual(DateTimePickerFormat.Custom, preventa.Format);
                    Assert.AreEqual("dd/MM/yyyy HH:mm", preventa.CustomFormat);
                    var controles = form.Controls.Cast<Control>().Prepend(form).ToArray();
                    var fuentes = controles.Select(c => c.Font).ToArray();
                    form.Scale(new SizeF(factor, factor));
                    for (int i = 0; i < controles.Length; i++)
                        controles[i].Font = new Font(fuentes[i].FontFamily, fuentes[i].SizeInPoints * factor, fuentes[i].Style);
                    form.Size = new Size(Math.Min(form.Width, 1366), Math.Min(form.Height, 728));
                    form.PerformLayout();
                    foreach (string nombre in new[] { "btnRegistrar", "lblVolver" })
                    {
                        Control accion = ControlDe<Control>(form, nombre);
                        Assert.IsTrue(accion.Visible);
                        Assert.IsTrue(form.ClientRectangle.Contains(accion.Bounds), nombre + " fuera del área cliente: " + accion.Bounds +
                            "; cliente=" + form.ClientRectangle + "; ventana=" + form.Size);
                        foreach (Control hermano in accion.Parent.Controls)
                            if (hermano != accion && hermano.Visible && !string.IsNullOrEmpty(hermano.Text))
                                Assert.IsFalse(accion.Bounds.IntersectsWith(hermano.Bounds), nombre + " se superpone con " + hermano.Name);
                    }
                }
            });
        }

        // CP-70 — RF-12; PA-05, 4.6/C.4: Tab y alta/listado de un local desde la interfaz.
        [DataTestMethod]
        [DataRow(false, DisplayName = "CP-70: Registrar local recorre los campos con Tab")]
        [DataRow(true, DisplayName = "CP-70: Alta de local aparece en la base y en Mis locales")]
        public void CP70_RegistrarLocal(bool registrar)
        {
            EnSta(() =>
            {
                Fake.ComoAdministrador();
                Usuario propietario = Fake.Administrador();
                Fake.ComoAdministrador();
                using (var form = new Visual.frmRegistrarLocal { IDUsuario = propietario.Id })
                {
                    Mostrar(form);
                    if (!registrar)
                    {
                        ControlDe<TextBox>(form, "txtNombre").Select();
                        foreach (string nombre in new[] { "txtDireccion", "txtAltura", "txtCapacidad", "btnregistrarse" })
                        {
                            Assert.IsTrue(form.SelectNextControl(form.ActiveControl, true, true, true, false));
                            Assert.AreSame(ControlDe<Control>(form, nombre), form.ActiveControl);
                        }
                        return;
                    }
                    string nombreLocal = "test Local " + Guid.NewGuid().ToString("N");
                    ControlDe<TextBox>(form, "txtNombre").Text = nombreLocal;
                    ControlDe<TextBox>(form, "txtDireccion").Text = "Mitre";
                    ControlDe<TextBox>(form, "txtAltura").Text = "123";
                    ControlDe<TextBox>(form, "txtCapacidad").Text = "500";
                    DialogosPrueba.Comprobar(() => ControlDe<Button>(form, "btnregistrarse").PerformClick(),
                        new DialogoEsperado { Titulo = "Registrar local", Texto = "Local registrado correctamente" });
                    Local local = GestorLocales.Instance.PorPropietario(propietario.Id).Single(l => l.Nombre == nombreLocal);
                    Assert.AreEqual(500, local.Capacidad);
                    Assert.AreEqual("Mitre 123", local.Direccion);
                    using (var listado = new Visual.frmMisLocales { IDUsuario = propietario.Id })
                    {
                        Mostrar(listado);
                        DataGridViewRow fila = ControlDe<DataGridView>(listado, "dgvLocales").Rows.Cast<DataGridViewRow>()
                            .Single(f => Convert.ToString(f.Cells["Nombre"].Value) == nombreLocal);
                        Assert.AreEqual(local.Id, Convert.ToInt32(fila.Cells["ID"].Value));
                        Assert.AreEqual(500, Convert.ToInt32(fila.Cells["Capacidad"].Value));
                    }
                }
            });
        }

        // CP-71 — RF-12, RNF-02; PA-06, C.12/C.13: formato y selección de un local con CurrentCell.
        [DataTestMethod]
        [DataRow(false, DisplayName = "CP-71: Mis locales aplica las propiedades y el color del tema")]
        [DataRow(true, DisplayName = "CP-71: CurrentCell abre eventos del segundo local")]
        public void CP71_GrillaYSeleccionDelLocal(bool navegar)
        {
            EnSta(() =>
            {
                Fake.ComoAdministrador();
                Fake.EventoPublicado(1000m, 5);
                Fake.EventoPublicado(1000m, 5);
                Usuario propietario = Fake.Administrador();
                Fake.ComoAdministrador();
                using (var form = new Visual.frmMisLocales { IDUsuario = propietario.Id })
                {
                    Mostrar(form);
                    DataGridView grilla = ControlDe<DataGridView>(form, "dgvLocales");
                    if (!navegar)
                    {
                        Assert.IsFalse(grilla.Columns["ID"].Visible);
                        CollectionAssert.AreEqual(new[] { "Nombre del local", "Dirección", "Capacidad" },
                            grilla.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).Select(c => c.HeaderText).ToArray());
                        Assert.IsTrue(grilla.ReadOnly);
                        Assert.IsFalse(grilla.AllowUserToAddRows);
                        Assert.AreEqual(DataGridViewSelectionMode.FullRowSelect, grilla.SelectionMode);
                        Assert.AreEqual(DataGridViewAutoSizeColumnsMode.Fill, grilla.AutoSizeColumnsMode);
                        Assert.AreEqual(DataGridViewContentAlignment.MiddleRight, grilla.Columns["Capacidad"].DefaultCellStyle.Alignment);
                        Assert.AreEqual(Visual.Tema.FilaAlterna, grilla.AlternatingRowsDefaultCellStyle.BackColor);
                    }
                    else
                    {
                        Assert.IsTrue(grilla.Rows.Count >= 2);
                        int local = Convert.ToInt32(grilla.Rows[1].Cells["ID"].Value);
                        grilla.CurrentCell = grilla.Rows[1].Cells["Nombre"];
                        DialogosPrueba.Comprobar(() => ControlDe<Button>(form, "btnEventosLocal").PerformClick());
                        Visual.Vendedor.frmEventosLocal eventos = Application.OpenForms.OfType<Visual.Vendedor.frmEventosLocal>().Single(f => f.Visible);
                        Assert.AreEqual(local, eventos.IDlocal);
                        Assert.IsFalse(form.Visible);
                    }
                }
            });
        }

        // CP-72 — RF-12; PA-07, 4.2/C.5: texto del botón y dos separadores decimales.
        [DataTestMethod]
        [DataRow("boton", DisplayName = "CP-72: El botón de sectores dice Agregar sector")]
        [DataRow("12500,50", DisplayName = "CP-72: Sector acepta precio con coma decimal")]
        [DataRow("12500.50", DisplayName = "CP-72: Sector acepta precio con punto decimal")]
        public void CP72_PrecioYBotonDelSector(string precio)
        {
            EnSta(() =>
            {
                Fake.ComoAdministrador();
                Evento evento = Fake.EventoBorradorSinSectores();
                using (var form = SectorPara(evento))
                {
                    Mostrar(form);
                    Button boton = ControlDe<Button>(form, "btnregistrarse");
                    if (precio == "boton") { Assert.AreEqual("Agregar sector", boton.Text); return; }
                    CompletarSector(form, "Decimal", "10", precio);
                    DialogosPrueba.Comprobar(() => boton.PerformClick(),
                        new DialogoEsperado { Titulo = "Agregar sector", Texto = "Sector agregado correctamente." });
                    Assert.AreEqual(12500.50m, GestorEventos.Instance.Sectores(evento.Id).Single().Precio);
                }
            });
        }

        // CP-73 — RF-12, RN-06; PA-08, C.6/C.7: alta en borrador y categoría obligatoria.
        [DataTestMethod]
        [DataRow(true, DisplayName = "CP-73: Alta de evento guarda y lista un borrador")]
        [DataRow(false, DisplayName = "CP-73: Sin categoría no se inserta el evento")]
        public void CP73_AltaDeEvento(bool categoria)
        {
            EnSta(() =>
            {
                Fake.ComoAdministrador();
                Evento referencia = Fake.EventoBorradorSinSectores();
                Local local = GestorLocales.Instance.PorId(referencia.IdLocal);
                int antes = GestorEventos.Instance.PorLocal(local.Id).Count;
                string nombre = "test Evento " + Guid.NewGuid().ToString("N");
                using (var form = new Visual.Propietario.frmAgregarEvento { IDlocal = local.Id, IDUsuario = local.IdPropietario })
                {
                    Mostrar(form);
                    ControlDe<TextBox>(form, "txtNombre").Text = nombre;
                    ControlDe<DateTimePicker>(form, "dtEvento").Value = DateTime.Today.AddDays(30);
                    if (categoria) ControlDe<ComboBox>(form, "cmbCategoria").SelectedIndex = 0;
                    DialogosPrueba.Comprobar(() => ControlDe<Button>(form, "btnRegistrar").PerformClick(),
                        new DialogoEsperado { Titulo = "Agregar evento", Texto = categoria ? "Evento guardado como borrador." : "Seleccione la categoría del evento." });
                    if (!categoria)
                    {
                        Assert.AreEqual(antes, GestorEventos.Instance.PorLocal(local.Id).Count);
                        Assert.IsTrue(form.Visible);
                        return;
                    }
                    Evento creado = GestorEventos.Instance.PorLocal(local.Id).Single(e => e.Nombre == nombre);
                    Assert.AreEqual(GestorEventos.EstadoBorrador, creado.Estado);
                    Visual.Vendedor.frmEventosLocal listado = Application.OpenForms.OfType<Visual.Vendedor.frmEventosLocal>().Single(f => f.Visible);
                    DataGridViewRow fila = ControlDe<DataGridView>(listado, "dgvEventos").Rows.Cast<DataGridViewRow>()
                        .Single(f => Convert.ToInt32(f.Cells["ID"].Value) == creado.Id);
                    Assert.AreEqual("Borrador", Convert.ToString(fila.Cells["Estado"].Value));
                }
            });
        }

        // CP-74 — RF-12, RN-06; PA-09, C.8/C.9: capacidad cero conserva borrador; positiva publica.
        [DataTestMethod]
        [DataRow(0, DisplayName = "CP-74: Sector de capacidad cero conserva Borrador")]
        [DataRow(10, DisplayName = "CP-74: Primer sector con capacidad publica el evento")]
        public void CP74_PublicacionAlAgregarSector(int capacidad)
        {
            EnSta(() =>
            {
                Fake.ComoAdministrador();
                Evento evento = Fake.EventoBorradorSinSectores();
                Local local = GestorLocales.Instance.PorId(evento.IdLocal);
                using (var administrar = new Visual.Vendedor.frmAdministrarEvento
                    { IDEvento = evento.Id, IDlocal = local.Id, IDUsuario = local.IdPropietario, NombreEvento = evento.Nombre })
                {
                    Mostrar(administrar);
                    DialogosPrueba.Comprobar(() => ControlDe<Button>(administrar, "btnAgregarSector").PerformClick());
                    Visual.Vendedor.frmAgregarSector sector = Application.OpenForms.OfType<Visual.Vendedor.frmAgregarSector>().Single(f => f.Visible);
                    CompletarSector(sector, "Nuevo", capacidad.ToString(), "1000");
                    DialogosPrueba.Comprobar(() => ControlDe<Button>(sector, "btnregistrarse").PerformClick(),
                        new DialogoEsperado { Titulo = "Agregar sector", Texto = "Sector agregado correctamente." });
                    Assert.AreEqual(capacidad, GestorEventos.Instance.Sectores(evento.Id).Single().Capacidad);
                    string estado = capacidad == 0 ? GestorEventos.EstadoBorrador : GestorEventos.EstadoPublicado;
                    Assert.AreEqual(estado, GestorEventos.Instance.ObtenerPorId(evento.Id).Estado);
                    using (var listado = new Visual.Vendedor.frmEventosLocal { IDlocal = local.Id, IDUsuario = local.IdPropietario })
                    {
                        Mostrar(listado);
                        DataGridViewRow fila = ControlDe<DataGridView>(listado, "dgvEventos").Rows.Cast<DataGridViewRow>()
                            .Single(f => Convert.ToInt32(f.Cells["ID"].Value) == evento.Id);
                        Assert.AreEqual(estado, Convert.ToString(fila.Cells["Estado"].Value));
                    }
                }
            });
        }

        // CP-75 — RF-12, RN-38/RN-06; PA-10, C.10/C.11: avisos y persistencia tras rechazar.
        [DataTestMethod]
        [DataRow(false, DisplayName = "CP-75: RN-38 informa capacidad disponible sin insertar")]
        [DataRow(true, DisplayName = "CP-75: RN-06 informa el código y conserva el último sector")]
        public void CP75_ReglasDeCapacidadDesdeLaPantalla(bool eliminar)
        {
            EnSta(() =>
            {
                Fake.ComoAdministrador();
                Evento evento = eliminar ? Fake.EventoPublicado(1000m, 5) : Fake.EventoBorradorSinSectores();
                Local local = GestorLocales.Instance.PorId(evento.IdLocal);
                if (!eliminar)
                {
                    Fake.CambiarCapacidadDeLocal(local.Id, 1000);
                    using (var form = SectorPara(evento))
                    {
                        Mostrar(form);
                        CompletarSector(form, "Excesivo", "1001", "1000");
                        DialogosPrueba.Comprobar(() => ControlDe<Button>(form, "btnregistrarse").PerformClick(),
                            new DialogoEsperado { Titulo = "Agregar sector", Texto = "Capacidad disponible para este evento: 1000." });
                        Assert.AreEqual(0, GestorEventos.Instance.Sectores(evento.Id).Count);
                    }
                    return;
                }
                int idSector = GestorEventos.Instance.Sectores(evento.Id).Single().Id;
                Fake.ComoAdministrador();
                using (var form = new Visual.Vendedor.frmAdministrarEvento
                    { IDEvento = evento.Id, IDlocal = local.Id, IDUsuario = local.IdPropietario })
                {
                    Mostrar(form);
                    DataGridView grilla = ControlDe<DataGridView>(form, "dgvSectores");
                    grilla.CurrentCell = grilla.Rows[0].Cells["Nombre"];
                    try
                    {
                        DialogosPrueba.Comprobar(() => ControlDe<Button>(form, "btnEliminarSector").PerformClick(),
                            new DialogoEsperado { Titulo = "Administrar evento", Texto = "(RN-06)" });
                    }
                    finally
                    {
                        CollectionAssert.AreEqual(new[] { idSector },
                            GestorEventos.Instance.Sectores(evento.Id).Select(s => s.Id).ToArray());
                    }
                }
            });
        }

        private static Visual.Vendedor.frmAgregarSector SectorPara(Evento evento)
        {
            Local local = GestorLocales.Instance.PorId(evento.IdLocal);
            return new Visual.Vendedor.frmAgregarSector
                { IDEvento = evento.Id, IDlocal = local.Id, IDUsuario = local.IdPropietario, NombreEvento = evento.Nombre };
        }

        private static void CompletarSector(Form form, string nombre, string capacidad, string precio)
        {
            ControlDe<TextBox>(form, "txtNombre").Text = nombre;
            ControlDe<TextBox>(form, "txtCapacidad").Text = capacidad;
            ControlDe<TextBox>(form, "txtPrecio").Text = precio;
        }
    }
}
