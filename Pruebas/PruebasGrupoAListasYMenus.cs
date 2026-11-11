using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;
using static Pruebas.InterfazGrupoA;

namespace Pruebas
{
    [TestClass]
    public class PruebasGrupoAListasYMenus
    {
        // CP-83 — RF-10, RN-35; PA-18, 8.23/8.25: lista ordenada y propietario sin listas.
        [DataTestMethod]
        [DataRow(false, DisplayName = "CP-83: Lista agotada muestra columnas, prioridad y contadores")]
        [DataRow(true, DisplayName = "CP-83: Propietario sin listas ve el combo deshabilitado")]
        public void CP83_ListasDeEspera(bool vacia)
        {
            EnSta(() =>
            {
                Usuario propietario = vacia ? Fake.PropietarioSinLocales() : Fake.Administrador();
                Evento evento = null;
                Cliente primero = null;
                if (!vacia)
                {
                    evento = Agotar();
                    Cliente ordinario = Fake.Cliente(NivelFidelidad.Bronce);
                    primero = Fake.Cliente(NivelFidelidad.Oro);
                    Fake.SuscripcionActiva(primero, GestorSuscripciones.PlanMensual);
                    GestorListaEspera.Instance.Inscribir(evento.Id, ordinario.Id);
                    GestorListaEspera.Instance.Inscribir(evento.Id, primero.Id);
                }
                using (var form = new Visual.frmListasEspera { IDUsuario = propietario.Id })
                {
                    Mostrar(form);
                    ComboBox combo = ControlDe<ComboBox>(form, "cmbEventos");
                    Label cantidad = ControlDe<Label>(form, "lblCantidad");
                    if (vacia)
                    {
                        Assert.IsFalse(combo.Enabled);
                        Assert.AreEqual("No hay eventos agotados ni listas de espera en sus locales.", cantidad.Text);
                        return;
                    }
                    string texto = evento.Nombre + " · " + evento.Fecha.ToString("dd/MM/yyyy HH:mm") + " · agotado";
                    combo.SelectedIndex = Enumerable.Range(0, combo.Items.Count).Single(i => combo.Items[i].ToString() == texto);
                    Assert.AreEqual(texto, combo.SelectedItem.ToString());
                    DataGridView grilla = ControlDe<DataGridView>(form, "dgvLista");
                    CollectionAssert.AreEqual(new[] { "Posición", "Cliente", "Mail", "Suscriptor", "Inscripto el" },
                        grilla.Columns.Cast<DataGridViewColumn>().Select(c => c.HeaderText).ToArray());
                    Assert.AreEqual(2, grilla.Rows.Count);
                    Assert.AreEqual(primero.Mail, Convert.ToString(grilla.Rows[0].Cells["Mail"].Value));
                    Assert.AreEqual("Sí", Convert.ToString(grilla.Rows[0].Cells["Suscriptor"].Value));
                    Assert.AreEqual(1, Convert.ToInt32(grilla.Rows[0].Cells["Posicion"].Value));
                    Assert.AreEqual("2 inscripto(s), 1 con suscripción. Los suscriptores van primero; dentro de cada grupo, por orden de inscripción.", cantidad.Text);
                    Assert.IsTrue(grilla.Columns.Cast<DataGridViewColumn>().All(c => c.SortMode == DataGridViewColumnSortMode.NotSortable));
                }
            });
        }

        // CP-84 — RF-10, RN-35; PA-19, 8.24: la prioridad se congela al inscribirse.
        [TestMethod("CP-84: Cancelar el plan no cambia la prioridad ya registrada en la espera")]
        public void CP84_PrioridadCongelada()
        {
            EnSta(() =>
            {
                Evento evento = Agotar();
                Cliente bronce = Fake.Cliente(NivelFidelidad.Bronce);
                Cliente oro = Fake.Cliente(NivelFidelidad.Oro);
                Fake.SuscripcionActiva(oro, GestorSuscripciones.PlanMensual);
                GestorListaEspera.Instance.Inscribir(evento.Id, bronce.Id);
                GestorListaEspera.Instance.Inscribir(evento.Id, oro.Id);
                GestorSuscripciones.Instance.Cancelar(oro.Id);
                Assert.IsFalse(GestorSuscripciones.Instance.TieneActiva(oro.Id));
                var lista = GestorListaEspera.Instance.DeEvento(evento.Id);
                CollectionAssert.AreEqual(new[] { oro.Id, bronce.Id }, lista.Select(x => x.IdUsuario).ToArray());
                Assert.IsTrue(lista[0].ConSuscripcion);
                Assert.IsFalse(lista[1].ConSuscripcion);
            });
        }

        // CP-85 — RF-11/RF-12, RNF-02/RNF-03; PA-20, 9.1/9.16/8.26: menús y separación de capas.
        [DataTestMethod]
        [DataRow("propietario", DisplayName = "CP-85: Menú de propietario mantiene acciones visibles al 100 y 150 por ciento")]
        [DataRow("cliente", DisplayName = "CP-85: El menú de cliente no ofrece reportes")]
        [DataRow("arquitectura", DisplayName = "CP-85: Visual no contiene conexiones ni comandos SQL")]
        public void CP85_MenusYCapas(string escenario)
        {
            if (escenario == "arquitectura")
            {
                string carpeta = Path.Combine(BaseDePrueba.RaizDelRepositorio(), "Visual");
                foreach (string archivo in Directory.GetFiles(carpeta, "*.cs", SearchOption.AllDirectories)
                    .Where(p => !p.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar) &&
                        !p.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar)))
                    foreach (string prohibido in new[] { "SqlConnection", "SqlCommand", "ConexionSql" })
                        Assert.IsFalse(File.ReadAllText(archivo).Contains(prohibido), archivo + ": " + prohibido);
                foreach (Type tipo in typeof(Visual.frmInicio).Assembly.GetTypes().Where(t => typeof(Form).IsAssignableFrom(t)))
                    Assert.IsFalse(tipo.GetConstructors().SelectMany(c => c.GetParameters()).Any(p =>
                        new[] { "SqlConnection", "SqlCommand", "ConexionSql" }.Contains(p.ParameterType.Name)));
                return;
            }
            EnSta(() =>
            {
                if (escenario == "cliente")
                {
                    using (var menu = new Visual.frmMenuCliente { IDUsuario = Fake.Cliente(NivelFidelidad.Bronce).Id })
                    {
                        Mostrar(menu);
                        Assert.IsFalse(menu.Controls.Cast<Control>().Any(c => c.Text.IndexOf("Reporte", StringComparison.OrdinalIgnoreCase) >= 0));
                    }
                    return;
                }
                foreach (float escala in new[] { 1f, 1.5f })
                using (var menu = new Visual.frmMenuPropietario { IDUsuario = Fake.Administrador().Id })
                {
                    Mostrar(menu);
                    Control[] controles = menu.Controls.Cast<Control>().Prepend(menu).ToArray();
                    Font[] fuentes = controles.Select(c => c.Font).ToArray();
                    menu.Scale(new SizeF(escala, escala));
                    for (int i = 0; i < controles.Length; i++) controles[i].Font = new Font(fuentes[i].FontFamily, fuentes[i].SizeInPoints * escala, fuentes[i].Style);
                    menu.Size = new Size(Math.Min(menu.Width, 1366), Math.Min(menu.Height, 728));
                    string[] nombres = { "btnRegistrarLocal", "btnMisLocales", "btnListasEspera", "btnReportes", "lblvolver" };
                    string[] textos = { "Registrar local", "Mis locales", "Listas de espera", "Reportes", "Volver al inicio" };
                    for (int i = 0; i < nombres.Length; i++)
                    {
                        Control c = ControlDe<Control>(menu, nombres[i]);
                        Assert.AreEqual(textos[i], c.Text);
                        Assert.IsTrue(c.Visible && menu.ClientRectangle.Contains(c.Bounds), nombres[i] + " fuera del menú a " + escala);
                    }
                }
            });
        }

        private static Evento Agotar()
        {
            Evento evento = Fake.EventoPublicado(1000m, 1);
            Fake.CompraConfirmada(Fake.Cliente(NivelFidelidad.Bronce), evento, 1, false);
            return evento;
        }
    }
}
