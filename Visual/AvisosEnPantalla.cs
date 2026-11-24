using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Visual
{
    public static class AvisosEnPantalla
    {
        public static void Mostrar(Form formulario, string texto, bool error = false, int duracionMs = 0)
        {
            if (formulario == null || formulario.IsDisposed) return;
            if (formulario.InvokeRequired) { formulario.BeginInvoke((Action)(()=>Mostrar(formulario,texto,error))); return; }
            foreach(var anterior in formulario.Controls.Cast<Control>().Where(c=>c.Name=="avisoSistema").ToArray()) anterior.Dispose();
            var aviso = new Componentes.EtiquetaAyuda { Name="avisoSistema", Text=texto,
                AutoSize=false, Padding=new Padding(16), AccessibleRole=AccessibleRole.Alert };
            Tema.EstiloAviso(aviso,error);
            int ancho=Math.Max(1,Math.Min(480,formulario.ClientSize.Width-32));
            aviso.Size=new Size(ancho,Math.Max(72,aviso.GetPreferredSize(new Size(ancho,0)).Height+32));
            aviso.Location=new Point(Math.Max(0,formulario.ClientSize.Width-ancho-16),
                Math.Min(72,Math.Max(0,formulario.ClientSize.Height-aviso.Height)));
            aviso.Anchor=AnchorStyles.Top|AnchorStyles.Right;
            formulario.Controls.Add(aviso);aviso.BringToFront();
            var reloj=new Timer { Interval=duracionMs > 0 ? duracionMs : error ? 3000 : 6000 };
            reloj.Tick+=(s,e)=>aviso.Dispose();
            aviso.Disposed+=(s,e)=>{reloj.Stop();reloj.Dispose();};
            reloj.Start();
        }
    }
}
