using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Visual.Componentes
{
    public class PortadaEvento : EtiquetaAyuda
    {
        private string nombre=string.Empty,categoria=string.Empty;
        public PortadaEvento(){SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);}
        public void Cargar(string titulo,string tipo){nombre=titulo??string.Empty;categoria=tipo??string.Empty;AccessibleName=nombre;Invalidate();}
        public static int ColorDelNombre(string titulo)
        {
            using(var sha=SHA256.Create()) return sha.ComputeHash(Encoding.UTF8.GetBytes(titulo??string.Empty))[0]%Tema.PaletaPortadas.Length;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if(Width<1||Height<1)return;
            int indice=ColorDelNombre(nombre);
            using(var fondo=new LinearGradientBrush(ClientRectangle,Tema.PaletaPortadas[indice],
                Tema.PaletaPortadas[(indice+2)%Tema.PaletaPortadas.Length],45f)) e.Graphics.FillRectangle(fondo,ClientRectangle);
            e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;
            if(nombre.Length>0) using(var fuente=Tema.FuenteInicialPortada(Height)) using(var pincel=new SolidBrush(Tema.MarcaAguaPortada))
                e.Graphics.DrawString(StringInfo.GetNextTextElement(nombre),fuente,pincel,new PointF(12,Height-fuente.Height));
            int margen=Math.Max(8,Font.Height/2);
            int ancho=Math.Min(Width-margen*2,TextRenderer.MeasureText(e.Graphics,categoria,Tema.Ayuda).Width+margen*2);
            var caja=new Rectangle(Width-ancho-margen,margen,ancho,Tema.Ayuda.Height+margen*2);
            using(var camino=new GraphicsPath()) {
                int radio=Math.Max(2,caja.Height/2);
                camino.AddArc(caja.Left,caja.Top,radio,radio,180,90);camino.AddArc(caja.Right-radio,caja.Top,radio,radio,270,90);
                camino.AddArc(caja.Right-radio,caja.Bottom-radio,radio,radio,0,90);camino.AddArc(caja.Left,caja.Bottom-radio,radio,radio,90,90);camino.CloseFigure();
                using(var sello=new SolidBrush(Tema.Superficie))e.Graphics.FillPath(sello,camino);
            }
            TextRenderer.DrawText(e.Graphics,categoria,Tema.Ayuda,caja,Tema.Texto,TextFormatFlags.HorizontalCenter|TextFormatFlags.VerticalCenter|TextFormatFlags.NoPrefix);
        }
    }
}
