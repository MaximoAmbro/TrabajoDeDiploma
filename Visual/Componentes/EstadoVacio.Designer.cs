namespace Visual.Componentes
{
    partial class EstadoVacio
    {
        private EtiquetaAyuda icono,explicacion;
        private BotonSecundario boton;
        private void InitializeComponent()
        {
            this.SuspendLayout();
            icono=new EtiquetaAyuda {Name="iconoVacio",Text="\uE8A5",Location=new System.Drawing.Point(24,12),Size=new System.Drawing.Size(64,64)};
            explicacion=new EtiquetaAyuda {Name="explicacionVacio",Location=new System.Drawing.Point(104,24),Size=new System.Drawing.Size(440,72)};
            boton=new BotonSecundario {Name="accionVacio",Location=new System.Drawing.Point(104,112),Size=new System.Drawing.Size(240,36)};
            AutoScaleDimensions=new System.Drawing.SizeF(96F,96F);AutoScaleMode=System.Windows.Forms.AutoScaleMode.Dpi;
            Size=new System.Drawing.Size(568,172);Controls.Add(icono);Controls.Add(explicacion);Controls.Add(boton);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
