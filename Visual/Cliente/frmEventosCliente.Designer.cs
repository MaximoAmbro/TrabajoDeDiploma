namespace Visual
{
    partial class frmEventosCliente
    {
        private System.ComponentModel.IContainer components=null;
        protected override void Dispose(bool disposing){if(disposing && components!=null)components.Dispose();base.Dispose(disposing);}
        private void InitializeComponent()
        {
            this.btnVolver=new Visual.Componentes.BotonSecundario();
            this.btnComprar=new Visual.Componentes.BotonPrimario();
            this.btnVista=new Visual.Componentes.BotonSecundario();
            this.btnBuscar=new Visual.Componentes.BotonPrimario();
            this.btnLimpiar=new Visual.Componentes.BotonSecundario();
            this.label2=new Visual.Componentes.EtiquetaTitulo();
            this.indicadorPasos=new Visual.Componentes.IndicadorPasos();
            this.dgveventos=new System.Windows.Forms.DataGridView();
            this.txtBuscar=new System.Windows.Forms.TextBox();
            this.cmbCategoria=new System.Windows.Forms.ComboBox();
            this.dtpDesde=new System.Windows.Forms.DateTimePicker();
            this.barraSuperior=new Visual.Componentes.BarraSuperior();
            this.pnlBusqueda=new System.Windows.Forms.Panel();
            this.pnlExplorar=new System.Windows.Forms.Panel();
            this.pnlAcciones=new System.Windows.Forms.Panel();
            this.tarjetas=new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            this.label2.Name="label2";this.label2.Text="Eventos";this.label2.Location=new System.Drawing.Point(24,4);this.label2.Size=new System.Drawing.Size(600,32);
            this.indicadorPasos.Name="indicadorPasos";this.indicadorPasos.Location=new System.Drawing.Point(24,40);this.indicadorPasos.Size=new System.Drawing.Size(792,28);
            this.txtBuscar.Name="txtBuscar";this.txtBuscar.Location=new System.Drawing.Point(24,80);this.txtBuscar.Size=new System.Drawing.Size(248,28);this.txtBuscar.TabIndex=0;
            this.txtBuscar.KeyDown+=new System.Windows.Forms.KeyEventHandler(this.txtBuscar_KeyDown);
            this.cmbCategoria.Name="cmbCategoria";this.cmbCategoria.Location=new System.Drawing.Point(280,80);this.cmbCategoria.Size=new System.Drawing.Size(128,28);this.cmbCategoria.DropDownStyle=System.Windows.Forms.ComboBoxStyle.DropDownList;this.cmbCategoria.TabIndex=1;
            this.dtpDesde.Name="dtpDesde";this.dtpDesde.Location=new System.Drawing.Point(416,80);this.dtpDesde.Size=new System.Drawing.Size(144,28);this.dtpDesde.ShowCheckBox=true;this.dtpDesde.Checked=false;this.dtpDesde.Format=System.Windows.Forms.DateTimePickerFormat.Short;this.dtpDesde.TabIndex=2;
            this.btnBuscar.Name="btnBuscar";this.btnBuscar.Text="Buscar";this.btnBuscar.Location=new System.Drawing.Point(24,124);this.btnBuscar.Size=new System.Drawing.Size(112,32);this.btnBuscar.TabIndex=3;this.btnBuscar.Click+=new System.EventHandler(this.btnBuscar_Click);
            this.btnLimpiar.Name="btnLimpiar";this.btnLimpiar.Text="Limpiar";this.btnLimpiar.Location=new System.Drawing.Point(192,124);this.btnLimpiar.Size=new System.Drawing.Size(120,32);this.btnLimpiar.TabIndex=4;this.btnLimpiar.Click+=new System.EventHandler(this.btnLimpiar_Click);
            this.pnlBusqueda.Name="pnlBusqueda";this.pnlBusqueda.Dock=System.Windows.Forms.DockStyle.Top;this.pnlBusqueda.Size=new System.Drawing.Size(840,172);
            this.pnlBusqueda.Controls.Add(this.label2);this.pnlBusqueda.Controls.Add(this.indicadorPasos);this.pnlBusqueda.Controls.Add(this.txtBuscar);this.pnlBusqueda.Controls.Add(this.cmbCategoria);this.pnlBusqueda.Controls.Add(this.dtpDesde);this.pnlBusqueda.Controls.Add(this.btnBuscar);this.pnlBusqueda.Controls.Add(this.btnLimpiar);
            this.dgveventos.Name="dgveventos";this.dgveventos.Dock=System.Windows.Forms.DockStyle.Fill;this.dgveventos.Visible=false;this.dgveventos.TabIndex=1;
            this.tarjetas.Name="tarjetasEventos";this.tarjetas.Dock=System.Windows.Forms.DockStyle.Fill;this.tarjetas.AutoScroll=true;this.tarjetas.Padding=new System.Windows.Forms.Padding(12);this.tarjetas.TabIndex=0;
            this.pnlExplorar.Name="pnlExplorar";this.pnlExplorar.Dock=System.Windows.Forms.DockStyle.Fill;this.pnlExplorar.Controls.Add(this.dgveventos);this.pnlExplorar.Controls.Add(this.tarjetas);
            this.btnVolver.Name="btnVolver";this.btnVolver.Text="Volver";this.btnVolver.Location=new System.Drawing.Point(24,10);this.btnVolver.Size=new System.Drawing.Size(144,40);this.btnVolver.Click+=new System.EventHandler(this.btnVolver_Click);
            this.btnVista.Name="btnVista";this.btnVista.Text="Ver como tabla";this.btnVista.Location=new System.Drawing.Point(184,10);this.btnVista.Size=new System.Drawing.Size(168,40);
            this.btnComprar.Name="btnComprar";this.btnComprar.Text="Comprar";this.btnComprar.Location=new System.Drawing.Point(656,10);this.btnComprar.Size=new System.Drawing.Size(160,40);this.btnComprar.Anchor=System.Windows.Forms.AnchorStyles.Top|System.Windows.Forms.AnchorStyles.Right;this.btnComprar.Click+=new System.EventHandler(this.btnComprar_Click);
            this.pnlAcciones.Name="pnlAcciones";this.pnlAcciones.Dock=System.Windows.Forms.DockStyle.Bottom;this.pnlAcciones.Size=new System.Drawing.Size(840,60);this.pnlAcciones.Controls.Add(this.btnVolver);this.pnlAcciones.Controls.Add(this.btnVista);this.pnlAcciones.Controls.Add(this.btnComprar);
            this.barraSuperior.Name="barraSuperior";this.barraSuperior.Size=new System.Drawing.Size(840,60);this.barraSuperior.Dock=System.Windows.Forms.DockStyle.Top;
            this.AutoScaleDimensions=new System.Drawing.SizeF(96F,96F);this.AutoScaleMode=System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize=new System.Drawing.Size(1100,720);this.MinimumSize=new System.Drawing.Size(1000,680);this.Name="frmEventosCliente";this.Text="EventAura · Eventos disponibles";this.StartPosition=System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Controls.Add(this.pnlExplorar);this.Controls.Add(this.pnlAcciones);this.Controls.Add(this.pnlBusqueda);this.Controls.Add(this.barraSuperior);
            this.Load+=new System.EventHandler(this.frmEventosCliente_Load);this.ResumeLayout(false);
        }
        private Visual.Componentes.BotonSecundario btnVolver,btnLimpiar,btnVista;
        private Visual.Componentes.BotonPrimario btnComprar,btnBuscar;
        private Visual.Componentes.EtiquetaTitulo label2;
        private Visual.Componentes.IndicadorPasos indicadorPasos;
        private Visual.Componentes.BarraSuperior barraSuperior;
        private System.Windows.Forms.DataGridView dgveventos;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.ComboBox cmbCategoria;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Panel pnlBusqueda,pnlExplorar,pnlAcciones;
        private System.Windows.Forms.FlowLayoutPanel tarjetas;
    }
}
