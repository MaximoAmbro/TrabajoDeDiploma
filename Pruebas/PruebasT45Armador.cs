using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Entidades;
using Negocio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Visual.Componentes;
using static Pruebas.InterfazGrupoA;

namespace Pruebas
{
    [TestClass]
    public class PruebasT45Armador
    {
        public TestContext TestContext { get; set; }

        // CP-373 — RNF-02/RNF-03: el resumen del armador debe permanecer visible en el shell a ambos tamaños objetivo.
        [TestMethod("CP-373: Total y acción del armador visibles a 1200x760 y 1100x720 con eventos de 1 a 4 sectores")]
        public void CP373_ArmadorVisibleEnShell()
        {
            EnSta(() =>
            {
                var cliente = Fake.Cliente(NivelFidelidad.Bronce);
                Fake.ComoCliente(cliente);
                var eventos = new System.Collections.Generic.List<Evento>();
                for (int cantidadSectores = 1; cantidadSectores <= 4; cantidadSectores++)
                {
                    var evento = Fake.EventoPublicado(1000m, 20);
                    for (int i = 1; i < cantidadSectores; i++) Fake.AgregarSector(evento, 1000m + i * 100m, 20);
                    eventos.Add(evento);
                }
                var fallas = new System.Collections.Generic.List<string>();
                foreach (var tamano in new[] { new Size(1200, 760), new Size(1100, 720) })
                {
                    using (var menu = new Visual.frmMenuCliente { IDUsuario = cliente.Id, ClientSize = tamano })
                    {
                        Mostrar(menu);
                        foreach (var evento in eventos)
                        {
                            Assert.IsTrue(Visual.NavegacionCliente.AbrirDesdeMenu(new Visual.FrmCompraCliente { IDEvento = evento.Id, IDUsuario = cliente.Id }));
                            var host = ControlDe<HostFormularioCliente>(menu, "paginaFrmCompraCliente");
                            var compra = (Visual.FrmCompraCliente)host.Pantalla;
                            var web = ControlDe<Visual.Web.PanelWeb>(compra, "resumenCompraWeb");
                            EsperarVistaWeb(web);
                            TestContext?.WriteLine("CP-373 ventana objetivo=" + tamano + " cliente=" + menu.ClientSize + " menú=" + menu.Size + " mínimo=" + menu.MinimumSize + " host=" + web.Parent?.ClientSize + " vista=" + web.ClientSize);
                            string resultado = EjecutarVistaWeb(web,
                                "(()=>{const sectores=[...document.querySelectorAll('.sector')];sectores.forEach(s=>s.querySelector('.mas').click());const a=document.querySelector('.area-web').getBoundingClientRect(),g=document.querySelector('.armado'),serv=document.querySelector('[aria-labelledby=titulo-servicios]').getBoundingClientRect(),r=document.querySelector('.resumen').getBoundingClientRect(),t=document.querySelector('.total').getBoundingClientRect(),b=document.querySelector('#continuar').getBoundingClientRect();const gap=r.left>=serv.right?r.left-serv.right:r.top-serv.bottom;const visible=t.top>=a.top&&t.bottom<=a.bottom&&b.top>=a.top&&b.bottom<=a.bottom;return [visible,gap>=24,sectores.length,a.width,a.height,g.getBoundingClientRect().width,getComputedStyle(g).gridTemplateColumns,serv.bottom,r.top,t.top,t.bottom,b.top,b.bottom,innerHeight] .join('|')})()");
                            TestContext?.WriteLine("CP-373 geometría " + tamano + " / " + evento.Sectores.Count + " sectores: " + resultado);
                            if (!resultado.StartsWith("true|true|" + evento.Sectores.Count + "|", StringComparison.Ordinal))
                                fallas.Add(tamano + " / " + evento.Sectores.Count + " sectores: " + resultado);
                            PruebasInteraccionPantallas.CapturarSiSeSolicito(menu,
                                "ArmadorT45-" + tamano.Width + "x" + tamano.Height + "-" + evento.Sectores.Count + "-sectores");
                        }
                    }
                }
                Assert.AreEqual(0, fallas.Count,
                    "Total y Continuar deben estar visibles sin scroll y con 24 px respecto de Servicios. " + string.Join("; ", fallas));
            });
        }


        private static void EsperarVistaWeb(Visual.Web.PanelWeb panel)
        {
            var reloj = Stopwatch.StartNew();
            while (!panel.InicializacionTerminada && reloj.Elapsed < TimeSpan.FromSeconds(20))
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }
            Assert.IsTrue(panel.Preparado && panel.Visible, "La vista web del armador debe estar disponible para medir.");
        }

        private static string EjecutarVistaWeb(Visual.Web.PanelWeb panel, string script)
        {
            var tarea = panel.EjecutarScriptAsync(script);
            var reloj = Stopwatch.StartNew();
            while (!tarea.IsCompleted && reloj.Elapsed < TimeSpan.FromSeconds(10))
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }
            Assert.IsTrue(tarea.IsCompleted, "WebView2 debe devolver las medidas geométricas.");
            return (tarea.GetAwaiter().GetResult() ?? string.Empty).Trim('"');
        }
    }
}
