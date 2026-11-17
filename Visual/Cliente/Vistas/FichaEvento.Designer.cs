namespace Visual.Componentes
{
    partial class FichaEvento
    {
        private PortadaEvento portada;
        private EtiquetaTitulo titulo;
        private EtiquetaAyuda detalle,seleccion;
        private MapaSectores mapa;
        private BotonPrimario comprar;
        private BotonSecundario volver;
        private System.Windows.Forms.Panel contenido,acciones;
        private void InitializeComponent()
        {
            this.SuspendLayout();
            portada=new PortadaEvento {Name="portadaFicha",Location=new System.Drawing.Point(16,16),Size=new System.Drawing.Size(336,192)};
            titulo=new EtiquetaTitulo {Name="tituloFicha",Location=new System.Drawing.Point(376,16),Size=new System.Drawing.Size(392,60)};
            detalle=new EtiquetaAyuda {Name="datosFicha",Location=new System.Drawing.Point(376,80),Size=new System.Drawing.Size(392,144)};
            mapa=new MapaSectores {Name="mapaFicha",Location=new System.Drawing.Point(16,240),Size=new System.Drawing.Size(752,240),Anchor=System.Windows.Forms.AnchorStyles.Top|System.Windows.Forms.AnchorStyles.Left|System.Windows.Forms.AnchorStyles.Right};
            seleccion=new EtiquetaAyuda {Name="seleccionFicha",Location=new System.Drawing.Point(16,496),Size=new System.Drawing.Size(752,48)};
            contenido=new System.Windows.Forms.Panel {Name="contenidoFicha",Dock=System.Windows.Forms.DockStyle.Fill,AutoScroll=true};
            contenido.Controls.Add(portada);contenido.Controls.Add(titulo);contenido.Controls.Add(detalle);contenido.Controls.Add(mapa);contenido.Controls.Add(seleccion);
            volver=new BotonSecundario {Name="volverFicha",Text="Volver a eventos",Location=new System.Drawing.Point(16,12),Size=new System.Drawing.Size(200,36)};
            comprar=new BotonPrimario {Name="comprarFicha",Text="Armar paquete",Location=new System.Drawing.Point(568,12),Size=new System.Drawing.Size(200,36),Anchor=System.Windows.Forms.AnchorStyles.Top|System.Windows.Forms.AnchorStyles.Right};
            acciones=new System.Windows.Forms.Panel {Name="accionesFicha",Dock=System.Windows.Forms.DockStyle.Bottom,Size=new System.Drawing.Size(784,60)};
            acciones.Controls.Add(volver);acciones.Controls.Add(comprar);
            this.AutoScaleDimensions=new System.Drawing.SizeF(96F,96F);this.AutoScaleMode=System.Windows.Forms.AutoScaleMode.Dpi;
            this.Size=new System.Drawing.Size(784,604);this.Controls.Add(contenido);this.Controls.Add(acciones);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
