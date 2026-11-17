namespace Visual.Componentes
{
    partial class PanelAdministrador
    {
        private EtiquetaAyuda periodo;
        private System.Windows.Forms.FlowLayoutPanel datos;
        private void InitializeComponent()
        {
            this.SuspendLayout();
            periodo=new EtiquetaAyuda {Name="periodoPanel",Location=new System.Drawing.Point(0,0),Anchor=System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right,Size=new System.Drawing.Size(400,52)};
            datos=new System.Windows.Forms.FlowLayoutPanel {Name="indicadoresPanel",Dock=System.Windows.Forms.DockStyle.Fill,FlowDirection=System.Windows.Forms.FlowDirection.LeftToRight,WrapContents=true,AutoScroll=true};
            AutoScaleDimensions=new System.Drawing.SizeF(96F,96F);AutoScaleMode=System.Windows.Forms.AutoScaleMode.Dpi;Size=new System.Drawing.Size(600,452);Controls.Add(datos);Controls.Add(periodo);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
