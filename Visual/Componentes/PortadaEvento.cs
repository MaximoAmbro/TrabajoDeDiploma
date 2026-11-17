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
        private Bitmap imagen;
        public PortadaEvento(){SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);}
        public bool TieneImagen { get { return imagen != null; } }
        public void Cargar(string titulo,string tipo){Cargar(titulo,tipo,null);}
        public void Cargar(string titulo,string tipo,byte[] datos)
        {
            nombre=titulo??string.Empty;categoria=tipo??string.Empty;AccessibleName=nombre;
            Bitmap nueva=null;
            if(datos!=null && datos.Length>0)
            {
                using(var flujo=new System.IO.MemoryStream(datos))
                using(var origen=Image.FromStream(flujo,true,true)) nueva=new Bitmap(origen);
            }
            if(imagen!=null) imagen.Dispose();
            imagen=nueva;Invalidate();
        }
        public static int ColorDelNombre(string titulo)
        {
            using(var sha=SHA256.Create()) return sha.ComputeHash(Encoding.UTF8.GetBytes(titulo??string.Empty))[0]%Tema.PaletaPortadas.Length;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if(Width<1||Height<1)return;
            if(imagen==null)
            {
                int indice=ColorDelNombre(nombre);
                using(var fondo=new LinearGradientBrush(ClientRectangle,Tema.PaletaPortadas[indice],
                    Tema.PaletaPortadas[(indice+2)%Tema.PaletaPortadas.Length],45f)) e.Graphics.FillRectangle(fondo,ClientRectangle);
            }
            else
            {
                e.Graphics.InterpolationMode=InterpolationMode.HighQualityBicubic;
                float escala=Math.Max((float)Width/imagen.Width,(float)Height/imagen.Height);
                float anchoImagen=imagen.Width*escala, altoImagen=imagen.Height*escala;
                e.Graphics.DrawImage(imagen,new RectangleF((Width-anchoImagen)/2f,(Height-altoImagen)/2f,anchoImagen,altoImagen));
            }
            e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;
            if(imagen==null && nombre.Length>0) using(var fuente=Tema.FuenteInicialPortada(Height)) using(var pincel=new SolidBrush(Tema.MarcaAguaPortada))
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
        protected override void Dispose(bool disposing){if(disposing&&imagen!=null){imagen.Dispose();imagen=null;}base.Dispose(disposing);}
    }
}
