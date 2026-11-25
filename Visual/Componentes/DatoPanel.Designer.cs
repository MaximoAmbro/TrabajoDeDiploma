namespace Visual.Componentes
{
    partial class DatoPanel
    {
        private EtiquetaAyuda titulo;
        private EtiquetaSeccion valor;
        private void InitializeComponent()
        {
            this.SuspendLayout();
            titulo=new EtiquetaAyuda {Name="tituloDato",Location=new System.Drawing.Point(16,16),Size=new System.Drawing.Size(272,28),Anchor=System.Windows.Forms.AnchorStyles.Top|System.Windows.Forms.AnchorStyles.Left|System.Windows.Forms.AnchorStyles.Right};
            valor=new EtiquetaSeccion {Name="valorDato",Location=new System.Drawing.Point(16,52),Size=new System.Drawing.Size(272,56),Anchor=System.Windows.Forms.AnchorStyles.Top|System.Windows.Forms.AnchorStyles.Left|System.Windows.Forms.AnchorStyles.Right};
            AutoScaleDimensions=new System.Drawing.SizeF(96F,96F);AutoScaleMode=System.Windows.Forms.AutoScaleMode.Dpi;Size=new System.Drawing.Size(304,128);Margin=new System.Windows.Forms.Padding(8);Controls.Add(titulo);Controls.Add(valor);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
