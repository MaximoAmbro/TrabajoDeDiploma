using System;
using System.Linq;
using System.Windows.Forms;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;
using static Pruebas.InterfazGrupoA;

namespace Pruebas
{
    [TestClass]
    public class PruebasGrupoAPreventa
    {
        // CP-81 — RF-03/RF-05, RN-39; PA-16, 8.21/8.22/C9.4/C9.5/C9.8: acceso según preventa.
        [DataTestMethod]
        [DataRow("sin-plan", DisplayName = "CP-81: Preventa se lista y rechaza al cliente sin plan con fecha y hora")]
        [DataRow("con-plan", DisplayName = "CP-81: Suscriptor puede abrir la compra en preventa")]
        [DataRow("sin-preventa", DisplayName = "CP-81: Sin preventa compran clientes con y sin suscripción")]
        public void CP81_AccesoDesdeListado(string escenario)
        {
            EnSta(() =>
            {
                Evento evento = Fake.EventoPublicado(1000m, 5);
                DateTime fin = DateTime.Today.AddDays(1).AddHours(20);
                bool preventa = escenario != "sin-preventa";
                if (preventa) GestorEventos.Instance.CambiarPreventa(evento.Id, fin);
                foreach (bool suscripto in escenario == "sin-preventa" ? new[] { false, true } : new[] { escenario == "con-plan" })
                {
                    Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
                    if (suscripto) Fake.SuscripcionActiva(cliente, GestorSuscripciones.PlanMensual);
                    using (var listado = new Visual.frmEventosCliente { IDUsuario = cliente.Id })
                    {
                        Mostrar(listado);
                        DataGridView grilla = ControlDe<DataGridView>(listado, "dgveventos");
                        DataGridViewRow fila = grilla.Rows.Cast<DataGridViewRow>().Single(f => Convert.ToInt32(f.Cells["ID"].Value) == evento.Id);
                        grilla.CurrentCell = fila.Cells["Nombre"];
                        bool rechazado = preventa && !suscripto;
                        DialogosPrueba.Comprobar(() => ControlDe<Button>(listado, "btnComprar").PerformClick(),
                            rechazado ? new[] { new DialogoEsperado { Titulo = "No se puede armar el paquete",
                                Texto = "hasta el " + fin.ToString("dd/MM/yyyy") + " a las 20:00" } } : new DialogoEsperado[0]);
                        if (rechazado)
                        {
                            Assert.IsTrue(Application.OpenForms.OfType<Visual.frmEventosCliente>().Any(f => f.Visible));
                            Assert.IsFalse(Application.OpenForms.OfType<Visual.FrmCompraCliente>().Any(f => f.Visible),
                                "Un cliente rechazado no debe quedar en una pantalla de compra visible.");
                        }
                        else
                        {
                            using (Visual.FrmCompraCliente compra = Application.OpenForms.OfType<Visual.FrmCompraCliente>().Single(f => f.Visible))
                            {
                                Assert.AreEqual(evento.Id, compra.IDEvento);
                                Assert.AreEqual(cliente.Id, compra.IDUsuario);
                                Assert.IsTrue(ControlDe<DataGridView>(compra, "dgvSectores").Rows.Count > 0);
                            }
                        }
                    }
                }
            });
        }

        // CP-82 — RF-12, RN-06/RN-39; PA-17, C9.2/C9.3/C9.6/C9.7/C9.9: alta y edición de preventa.
        [DataTestMethod]
        [DataRow("alta", DisplayName = "CP-82: Alta guarda preventa y conserva la fecha al publicar")]
        [DataRow("posterior", DisplayName = "CP-82: Preventa posterior al evento se rechaza sin insertar")]
        [DataRow("administrar-con", DisplayName = "CP-82: Administrar muestra preventa tildada y fecha persistida")]
        [DataRow("quitar", DisplayName = "CP-82: Quitar preventa informa y persiste NULL")]
        [DataRow("administrar-sin", DisplayName = "CP-82: Administrar sin preventa muestra selector destildado")]
        public void CP82_PrevantaDelPropietario(string escenario)
        {
            EnSta(() =>
            {
                Evento referencia = Fake.EventoBorradorSinSectores();
                Local local = GestorLocales.Instance.PorId(referencia.IdLocal);
                DateTime fin = DateTime.Today.AddDays(1).AddHours(20);
                if (escenario == "alta" || escenario == "posterior")
                {
                    int antes = GestorEventos.Instance.PorLocal(local.Id).Count;
                    string nombre = "test Preventa " + Guid.NewGuid().ToString("N");
                    using (var alta = new Visual.Propietario.frmAgregarEvento { IDlocal = local.Id, IDUsuario = local.IdPropietario })
                    {
                        Mostrar(alta);
                        ControlDe<TextBox>(alta, "txtNombre").Text = nombre;
                        ControlDe<ComboBox>(alta, "cmbCategoria").SelectedIndex = 0;
                        ControlDe<DateTimePicker>(alta, "dtEvento").Value = escenario == "alta" ? fin.AddDays(10) : fin.AddHours(-1);
                        DateTimePicker selector = ControlDe<DateTimePicker>(alta, "dtpFinPreventa");
                        selector.Value = fin;
                        selector.Checked = true;
                        DialogosPrueba.Comprobar(() => ControlDe<Button>(alta, "btnRegistrar").PerformClick(),
                            new DialogoEsperado { Titulo = escenario == "alta" ? "" : "Advertencia", Texto = escenario == "alta" ?
                                "Evento guardado como borrador." : "La preventa tiene que terminar antes de la fecha del evento." });
                        if (escenario == "posterior")
                        {
                            Assert.AreEqual(antes, GestorEventos.Instance.PorLocal(local.Id).Count);
                            Assert.IsTrue(alta.Visible);
                            return;
                        }
                    }
                    Evento creado = GestorEventos.Instance.PorLocal(local.Id).Single(e => e.Nombre == nombre);
                    Assert.AreEqual(fin, creado.FinPreventa);
                    using (var administrar = Administrar(creado, local))
                    {
                        Mostrar(administrar);
                        DialogosPrueba.Comprobar(() => ControlDe<Button>(administrar, "btnAgregarSector").PerformClick());
                        Visual.Vendedor.frmAgregarSector sector = Application.OpenForms.OfType<Visual.Vendedor.frmAgregarSector>().Single(f => f.Visible);
                        ControlDe<TextBox>(sector, "txtNombre").Text = "Campo";
                        ControlDe<TextBox>(sector, "txtCapacidad").Text = "10";
                        ControlDe<TextBox>(sector, "txtPrecio").Text = "1000";
                        DialogosPrueba.Comprobar(() => ControlDe<Button>(sector, "btnregistrarse").PerformClick(),
                            new DialogoEsperado { Titulo = "", Texto = "Sector agregado correctamente." });
                    }
                    Evento publicado = GestorEventos.Instance.ObtenerPorId(creado.Id);
                    Assert.AreEqual(GestorEventos.EstadoPublicado, publicado.Estado);
                    Assert.AreEqual(fin, publicado.FinPreventa);
                    return;
                }
                bool tiene = escenario != "administrar-sin";
                if (tiene) GestorEventos.Instance.CambiarPreventa(referencia.Id, fin);
                using (var administrar = Administrar(referencia, local))
                {
                    Mostrar(administrar);
                    DateTimePicker selector = ControlDe<DateTimePicker>(administrar, "dtpFinPreventa");
                    Assert.AreEqual(tiene, selector.Checked);
                    if (tiene) Assert.AreEqual(fin, selector.Value);
                    if (escenario == "quitar")
                    {
                        selector.Checked = false;
                        DialogosPrueba.Comprobar(() => ControlDe<Button>(administrar, "btnGuardarPreventa").PerformClick(),
                            new DialogoEsperado { Titulo = "Preventa", Texto = "El evento quedó sin preventa: pueden comprar todos los clientes." });
                        Assert.IsNull(GestorEventos.Instance.ObtenerPorId(referencia.Id).FinPreventa);
                        Assert.IsFalse(selector.Checked);
                    }
                }
            });
        }

        private static Visual.Vendedor.frmAdministrarEvento Administrar(Evento evento, Local local)
        {
            return new Visual.Vendedor.frmAdministrarEvento { IDEvento = evento.Id, IDlocal = local.Id,
                IDUsuario = local.IdPropietario, NombreEvento = evento.Nombre };
        }
    }
}
