using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;
using static Pruebas.InterfazGrupoA;

namespace Pruebas
{
    [TestClass]
    public class PruebasGrupoASuscripcion
    {
        private static readonly CultureInfo Pesos = new CultureInfo("es-AR");

        // CP-76 — RF-13, RNF-02; PA-11, 8.3: estado inicial, precios y geometría falsable.
        [TestMethod("CP-76: Mi suscripción sin plan muestra precios, beneficios y controles completos")]
        public void CP76_SinPlan()
        {
            EnSta(() =>
            {
                Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
                using (var form = SuscripcionDe(cliente))
                {
                    Mostrar(form);
                    Assert.AreEqual("No tenés una suscripción activa.", ControlDe<Label>(form, "lblEstado").Text);
                    Assert.AreEqual("Beneficios de la suscripción:\r\n  •  Seguro de entrada sin cargo\r\n" +
                        "  •  Preventa exclusiva de eventos\r\n  •  Prioridad en las listas de espera\r\n" +
                        "Sin descuento extra: rige el de tu nivel.", ControlDe<Label>(form, "lblBeneficios").Text);
                    foreach (string plan in new[] { GestorSuscripciones.PlanMensual, GestorSuscripciones.PlanAnual })
                    {
                        string periodo = plan == GestorSuscripciones.PlanMensual ? "mes" : "año";
                        string clave = plan == GestorSuscripciones.PlanMensual ? "SUSCRIPCION_MENSUAL_PRECIO" : "SUSCRIPCION_ANUAL_PRECIO";
                        string nombre = plan == GestorSuscripciones.PlanMensual ? "rdbMensual" : "rdbAnual";
                        Assert.AreEqual(plan + ": " + GestorParametros.Instance.ObtenerDecimal(clave).ToString("C0", Pesos) + " por " + periodo,
                            ControlDe<RadioButton>(form, nombre).Text);
                    }
                    Assert.IsFalse(ControlDe<Button>(form, "btnCancelar").Enabled);
                    foreach (Label label in form.Controls.OfType<Label>().Where(l => l.Visible && !l.AutoSize))
                        Assert.IsTrue(label.GetPreferredSize(new System.Drawing.Size(label.Width, 0)).Height <= label.Height,
                            "Etiqueta recortada: " + label.Name);
                    Control[] controles = form.Controls.Cast<Control>().Where(c => c.Visible).ToArray();
                    for (int i = 0; i < controles.Length; i++)
                        for (int j = i + 1; j < controles.Length; j++)
                            Assert.IsFalse(controles[i].Bounds.IntersectsWith(controles[j].Bounds),
                                "Superposición: " + controles[i].Name + " / " + controles[j].Name);
                }
            });
        }

        // CP-77 — RF-13, RN-25/RN-26; PA-12, 8.4/8.6: alta y baja con confirmaciones.
        [DataTestMethod]
        [DataRow(false, DisplayName = "CP-77: Contratar Mensual muestra precio, vencimiento y habilitados")]
        [DataRow(true, DisplayName = "CP-77: Cancelar suscripción pide No por defecto y elimina beneficios")]
        public void CP77_ContratarYCancelar(bool cancelar)
        {
            EnSta(() =>
            {
                Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
                if (cancelar) Fake.SuscripcionActiva(cliente, GestorSuscripciones.PlanMensual);
                using (var form = SuscripcionDe(cliente))
                {
                    Mostrar(form);
                    if (cancelar)
                    {
                        DialogosPrueba.Comprobar(() => ControlDe<Button>(form, "btnCancelar").PerformClick(),
                            new DialogoEsperado { Titulo = "Cancelar suscripción", Texto = "Los beneficios se pierden desde ahora.",
                                Predeterminado = DialogResult.No, Respuesta = DialogResult.Yes });
                        Assert.AreEqual("No tenés una suscripción activa.", ControlDe<Label>(form, "lblEstado").Text);
                        Assert.IsNull(GestorSuscripciones.Instance.ActivaDe(cliente.Id));
                        Assert.IsTrue(ControlDe<Button>(form, "btnContratar").Enabled);
                        Assert.IsFalse(ControlDe<Button>(form, "btnCancelar").Enabled);
                    }
                    else
                    {
                        ContratarDesde(form);
                        Suscripcion activa = GestorSuscripciones.Instance.ActivaDe(cliente.Id);
                        Assert.AreEqual(DateTime.Today.AddMonths(1), activa.FechaFin.Date);
                        Assert.AreEqual("Plan Mensual activo\r\nVence el " + activa.FechaFin.ToString("dd/MM/yyyy") +
                            "\r\nNo se renueva automáticamente", ControlDe<Label>(form, "lblEstado").Text);
                        Assert.IsFalse(ControlDe<RadioButton>(form, "rdbMensual").Enabled);
                        Assert.IsFalse(ControlDe<RadioButton>(form, "rdbAnual").Enabled);
                        Assert.IsFalse(ControlDe<Button>(form, "btnContratar").Enabled);
                        Assert.IsTrue(ControlDe<Button>(form, "btnCancelar").Enabled);
                    }
                }
            });
        }

        // CP-78 — RF-13, RN-25; PA-13, 8.5: dos pantallas desactualizadas no crean dos planes.
        [TestMethod("CP-78: Segunda ventana de suscripción rechaza duplicado con RN-25")]
        public void CP78_DosVentanas()
        {
            EnSta(() =>
            {
                Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
                using (var primera = SuscripcionDe(cliente))
                using (var segunda = SuscripcionDe(cliente))
                {
                    Mostrar(primera);
                    Mostrar(segunda);
                    ContratarDesde(primera);
                    Suscripcion activa = GestorSuscripciones.Instance.ActivaDe(cliente.Id);
                    DialogosPrueba.Comprobar(() => ControlDe<Button>(segunda, "btnContratar").PerformClick(),
                        ConfirmarContrato(), new DialogoEsperado { Titulo = "RN-25", Texto = "vence el " + activa.FechaFin.ToString("dd/MM/yyyy") });
                    Assert.AreEqual(1, Fake.ContarSuscripcionesVigentes(cliente.Id));
                    Assert.AreEqual(activa.Id, GestorSuscripciones.Instance.ActivaDe(cliente.Id).Id);
                }
            });
        }

        // CP-79 — RF-05, RN-28/RN-37; PA-14, 8.7/8.9: seguro gratis o cobrado en el resumen.
        [DataTestMethod]
        [DataRow(true, DisplayName = "CP-79: Suscriptor tiene seguro sin cargo y no recibe pregunta al comprar")]
        [DataRow(false, DisplayName = "CP-79: Sin suscripción el seguro de dos entradas cuesta diez mil")]
        public void CP79_SeguroEnResumen(bool suscripto)
        {
            EnSta(() =>
            {
                Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
                if (suscripto) Fake.SuscripcionActiva(cliente, GestorSuscripciones.PlanMensual);
                using (var form = new Visual.FrmCompraCliente { IDEvento = 1, IDUsuario = cliente.Id })
                {
                    Mostrar(form);
                    DataGridViewRow campo = ControlDe<DataGridView>(form, "dgvSectores").Rows.Cast<DataGridViewRow>()
                        .Single(f => Convert.ToString(f.Cells["Sector"].Value) == "Campo");
                    campo.Cells["Cantidad"].Value = 2;
                    string antes = Linea(ControlDe<Label>(form, "lblResumen").Text, "TOTAL");
                    ControlDe<CheckBox>(form, "chkSeguro").Checked = true;
                    string resumen = ControlDe<Label>(form, "lblResumen").Text;
                    StringAssert.Contains(Normalizar(resumen), suscripto ? "Seguro sin cargo (2 entradas) $ 0,00" : "Seguro (2 entradas) $ 10.000,00");
                    if (!suscripto) return;
                    Assert.AreEqual(antes, Linea(resumen, "TOTAL"));
                    bool abierto = false;
                    using (var timer = new Timer { Interval = 30 })
                    {
                        timer.Tick += (s, e) =>
                        {
                            Visual.frmPago pago = Application.OpenForms.OfType<Visual.frmPago>().FirstOrDefault(f => f.Visible);
                            if (pago == null) return;
                            abierto = true;
                            timer.Stop();
                            pago.DialogResult = DialogResult.Cancel;
                        };
                        timer.Start();
                        // Ningún MessageBox esperado: cualquier pregunta de seguro hace fallar el caso.
                        DialogosPrueba.Comprobar(() => ControlDe<Button>(form, "btnComprar").PerformClick());
                    }
                    Assert.IsTrue(abierto, "No se abrió la pantalla de pago.");
                }
            });
        }

        // CP-80 — RF-06/RF-07, RN-28/RN-37; PA-15, 8.8/C9.10/C9.11: pólizas y confirmación.
        [DataTestMethod]
        [DataRow(true, DisplayName = "CP-80: Pago de suscriptor con seguro guarda una póliza gratuita por entrada")]
        [DataRow(false, DisplayName = "CP-80: Pago sin seguro no crea pólizas")]
        public void CP80_PagoYPolizas(bool seguro)
        {
            EnSta(() =>
            {
                Cliente cliente = Fake.Cliente(NivelFidelidad.Bronce);
                Evento evento = Fake.EventoPublicado(1000m, 5);
                Fake.SuscripcionActiva(cliente, GestorSuscripciones.PlanMensual);
                Compra compra = GestorCompras.Instance.Iniciar(Fake.Paquete(evento, 2, seguro), cliente);
                using (var pago = new Visual.frmPago { Compra = compra })
                {
                    Mostrar(pago);
                    DatosTarjeta tarjeta = Fake.Tarjeta();
                    ControlDe<TextBox>(pago, "txtTitular").Text = tarjeta.Titular;
                    ControlDe<TextBox>(pago, "txtNumero").Text = tarjeta.Numero;
                    ControlDe<TextBox>(pago, "txtVencimiento").Text = tarjeta.Vencimiento;
                    ControlDe<TextBox>(pago, "txtCvv").Text = tarjeta.Cvv;
                    DialogosPrueba.Comprobar(() => ControlDe<Button>(pago, "btnPagar").PerformClick());
                    Assert.AreEqual(DialogResult.OK, pago.DialogResult);
                }
                compra = GestorCompras.Instance.ObtenerPorId(compra.Id);
                Assert.AreEqual(EstadoCompra.Confirmada, compra.Estado);
                Assert.AreEqual(seguro, compra.ConSeguro);
                Assert.AreEqual(0m, compra.MontoSeguro);
                var polizas = GestorCompras.Instance.SegurosDe(compra);
                Assert.AreEqual(seguro ? 2 : 0, polizas.Count);
                if (seguro)
                {
                    CollectionAssert.AreEquivalent(GestorCompras.Instance.EntradasDe(compra).Select(e => e.Id).ToArray(),
                        polizas.Select(p => p.IdEntrada).ToArray());
                    Assert.IsTrue(polizas.All(p => p.Precio == 0m));
                }
                using (var confirmacion = new Visual.frmConfirmacion { Compra = compra })
                {
                    Mostrar(confirmacion);
                    StringAssert.Contains(Normalizar(ControlDe<Label>(confirmacion, "lblDesglose").Text), "Seguro $ 0,00");
                }
            });
        }

        private static Visual.frmSuscripcion SuscripcionDe(Cliente cliente)
        {
            return new Visual.frmSuscripcion { IDUsuario = cliente.Id, NombreUsuario = cliente.Nombre };
        }

        private static DialogoEsperado ConfirmarContrato()
        {
            return new DialogoEsperado { Titulo = "Contratar suscripción",
                Texto = GestorSuscripciones.Instance.PrecioDe(GestorSuscripciones.PlanMensual).ToString("C2", Pesos) +
                    "?\n\nLa suscripción no se renueva automáticamente al vencer.",
                Predeterminado = DialogResult.Yes, Respuesta = DialogResult.Yes };
        }

        private static void ContratarDesde(Form form)
        {
            DialogosPrueba.Comprobar(() => ControlDe<Button>(form, "btnContratar").PerformClick(), ConfirmarContrato(),
                new DialogoEsperado { Titulo = "Suscripción", Texto = "Suscripción Mensual contratada." });
        }

        private static string Normalizar(string texto) => Regex.Replace(texto, @"\s+", " ").Trim();
        private static string Linea(string texto, string inicio) => Normalizar(texto.Split('\n').Single(l => l.StartsWith(inicio)));
    }
}
