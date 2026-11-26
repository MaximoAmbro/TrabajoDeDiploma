namespace Visual
{
    partial class frmControlIngreso
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.cmbEventos = new System.Windows.Forms.ComboBox();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.lblResultado = new System.Windows.Forms.Label();
            this.lblContador = new System.Windows.Forms.Label();
            this.lblEvento = new System.Windows.Forms.Label();
            this.lblCodigo = new System.Windows.Forms.Label();
            this.btnRegistrar = new Visual.Componentes.BotonPrimario();
            this.btnVolver = new Visual.Componentes.BotonSecundario();
            this.lblTituloIngreso = new Visual.Componentes.EtiquetaTitulo();
            this.lblTituloIngreso.Name = "lblTituloIngreso";
            this.lblTituloIngreso.Text = "Control de ingreso";
            this.lblTituloIngreso.Location = new System.Drawing.Point(24, 84);
            this.lblTituloIngreso.Size = new System.Drawing.Size(672, 32);
            this.Controls.Add(this.lblTituloIngreso);
            this.SuspendLayout();
            this.lblEvento.Location = new System.Drawing.Point(24, 140);
            this.lblEvento.Size = new System.Drawing.Size(672, 20);
            this.lblEvento.Text = "Evento con ingreso abierto";
            this.cmbEventos.Location = new System.Drawing.Point(24, 164);
            this.cmbEventos.Size = new System.Drawing.Size(672, 28);
            this.cmbEventos.Name = "cmbEventos";
            this.cmbEventos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEventos.TabIndex = 0;
            this.cmbEventos.SelectedIndexChanged += new System.EventHandler(this.EventoCambiado);
            this.lblCodigo.Location = new System.Drawing.Point(24, 212);
            this.lblCodigo.Size = new System.Drawing.Size(672, 20);
            this.lblCodigo.Text = "Código del ticket (lector USB o texto del PDF)";
            this.txtCodigo.Location = new System.Drawing.Point(24, 236);
            this.txtCodigo.Size = new System.Drawing.Size(464, 32);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.TabIndex = 1;
            this.btnRegistrar.Location = new System.Drawing.Point(512, 236);
            this.btnRegistrar.Size = new System.Drawing.Size(184, 40);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Text = "Registrar ingreso";
            this.btnRegistrar.TabIndex = 2;
            this.btnRegistrar.Click += new System.EventHandler(this.Registrar);
            this.lblResultado.Location = new System.Drawing.Point(24, 300);
            this.lblResultado.Size = new System.Drawing.Size(672, 72);
            this.lblResultado.Name = "lblResultado";
            this.lblContador.Location = new System.Drawing.Point(24, 388);
            this.lblContador.Size = new System.Drawing.Size(672, 24);
            this.lblContador.Name = "lblContador";
            this.btnVolver.Location = new System.Drawing.Point(24, 444);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(144, 40);
            this.btnVolver.Text = "Volver";
            this.btnVolver.TabIndex = 3;
            this.btnVolver.Click += new System.EventHandler(this.Volver);
            this.AcceptButton = this.btnRegistrar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1100, 720);
            this.MinimumSize = new System.Drawing.Size(1000, 680);
            this.Controls.Add(this.lblEvento);
            this.Controls.Add(this.cmbEventos);
            this.Controls.Add(this.lblCodigo);
            this.Controls.Add(this.txtCodigo);
            this.Controls.Add(this.btnRegistrar);
            this.Controls.Add(this.lblResultado);
            this.Controls.Add(this.lblContador);
            this.Controls.Add(this.btnVolver);
            this.Name = "frmControlIngreso";
            this.Text = "EventAura · Control de ingreso";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.barraSuperior = new Visual.Componentes.BarraSuperior();
            this.barraSuperior.Name = "barraSuperior";
            this.barraSuperior.Size = new System.Drawing.Size(720, 60);
            this.barraSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Controls.Add(this.barraSuperior);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Visual.Componentes.EtiquetaTitulo lblTituloIngreso;
        private System.Windows.Forms.ComboBox cmbEventos;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Label lblResultado;
        private System.Windows.Forms.Label lblContador;
        private System.Windows.Forms.Label lblEvento;
        private System.Windows.Forms.Label lblCodigo;
        private Visual.Componentes.BotonPrimario btnRegistrar;
        private Visual.Componentes.BotonSecundario btnVolver;
        private Visual.Componentes.BarraSuperior barraSuperior;
    }
}
