using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pruebas
{
    [TestClass]
    public class PruebasEjecucionSinDialogos
    {
        // CP-156 — RF-02, RNF-04: hacer que una excepcion de inicio de formulario falle sin cuadro bloqueante.
        [TestMethod("CP-156: Excepción en Load aborta el proceso de ensayo sin cartel del framework")]
        public void CP156_ExcepcionInmediata()
        {
            string carpeta=AppDomain.CurrentDomain.BaseDirectory;
            string fuente=Path.Combine(carpeta,"EnsayoExcepcionSinDialogo.cs");
            string exe=Path.Combine(carpeta,"EnsayoExcepcionSinDialogo.exe");
            File.WriteAllText(fuente,@"using System;using System.Windows.Forms;using System.Runtime.InteropServices;
class EnsayoExcepcionSinDialogo {
 [DllImport(""kernel32.dll"")] static extern uint SetErrorMode(uint m);
 [STAThread] static void Main(){SetErrorMode(3);Pruebas.BaseDePrueba.ConfigurarErroresDePrueba();
 using(var f=new Form()){f.Load+=(s,e)=>{throw new InvalidOperationException(""Fallo deliberado del ensayo"");};f.Show();}}}");
            string csc=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows),"Microsoft.NET","Framework64","v4.0.30319","csc.exe");
            using(var compilador=Process.Start(new ProcessStartInfo(csc,"/nologo /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:\""+Path.Combine(carpeta,"Pruebas.dll")+"\" /out:\""+exe+"\" \""+fuente+"\"") {UseShellExecute=false,CreateNoWindow=true})) {
                Assert.IsTrue(compilador.WaitForExit(10000));Assert.AreEqual(0,compilador.ExitCode);
            }
            File.Copy(Path.Combine(carpeta,"Pruebas.dll.config"),exe+".config",true);
            using(var proceso=Process.Start(new ProcessStartInfo(exe) {UseShellExecute=false,CreateNoWindow=true,RedirectStandardError=true,RedirectStandardOutput=true})) {
                var error=proceso.StandardError.ReadToEndAsync();var salida=proceso.StandardOutput.ReadToEndAsync();
                try {
                    Assert.IsTrue(proceso.WaitForExit(5000),"El proceso quedó esperando un diálogo.");
                    Assert.AreNotEqual(0,proceso.ExitCode,"La excepción debe rechazar la ejecución, no continuar silenciosamente.");
                    StringAssert.Contains(error.Result+salida.Result,"Fallo deliberado del ensayo");
                } finally {if(!proceso.HasExited)proceso.Kill();}
            }
        }
        // CP-157 — RF-02, RNF-02: comunicar un error inesperado sin detalle tecnico ni dialogo modal.
        [TestMethod("CP-157: Manejador global presenta aviso no modal sin detalle técnico")]
        public void CP157_AvisoNoModal()
        {
            PruebasSeguridad.EnStaSinCambiarDatos(() => {
                using(var form=new Form()) {
                    form.Show();
                    Visual.ManejadorErrores.Mostrar(new InvalidOperationException("Detalle interno deliberado"));
                    var aviso=form.Controls.Find("avisoSistema",true).Single();
                    StringAssert.Contains(aviso.Text,Visual.ManejadorErrores.MensajeGeneral);
                    Assert.IsFalse(aviso.Text.Contains("Detalle interno"));
                    Assert.IsTrue(form.Enabled);
                }
            });
        }
    }
}
