namespace Visual.Componentes
{
    partial class TarjetaEvento
    {
        private PortadaEvento portada;
        private EtiquetaSeccion titulo;
        private EtiquetaAyuda fecha,lugar;
        private EtiquetaSeccion precio;
        private BotonPrimario accion;
        private void InitializeComponent()
        {
            this.SuspendLayout();
            portada=new PortadaEvento {Name="portada",Location=new System.Drawing.Point(0,0),Size=new System.Drawing.Size(272,144)};
            titulo=new EtiquetaSeccion {Name="titulo",Location=new System.Drawing.Point(16,156),Size=new System.Drawing.Size(240,44)};
            fecha=new EtiquetaAyuda {Name="fecha",Location=new System.Drawing.Point(16,204),Size=new System.Drawing.Size(240,20)};
            lugar=new EtiquetaAyuda {Name="lugar",Location=new System.Drawing.Point(16,228),Size=new System.Drawing.Size(240,36)};
            precio=new EtiquetaSeccion {Name="precio",Location=new System.Drawing.Point(16,268),Size=new System.Drawing.Size(240,28)};
            accion=new BotonPrimario {Name="verEntradas",Text="Ver entradas",Location=new System.Drawing.Point(16,308),Size=new System.Drawing.Size(240,36)};
            this.AutoScaleDimensions=new System.Drawing.SizeF(96F,96F);
            this.AutoScaleMode=System.Windows.Forms.AutoScaleMode.Dpi;
            this.Size=new System.Drawing.Size(272,360);
            this.Margin=new System.Windows.Forms.Padding(12);
            this.Controls.Add(portada);this.Controls.Add(titulo);this.Controls.Add(fecha);this.Controls.Add(lugar);this.Controls.Add(precio);this.Controls.Add(accion);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
