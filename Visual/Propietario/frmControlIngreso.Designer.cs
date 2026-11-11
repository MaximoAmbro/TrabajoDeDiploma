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
            this.SuspendLayout();
            this.lblEvento.Location = new System.Drawing.Point(24, 20);
            this.lblEvento.Size = new System.Drawing.Size(640, 24);
            this.lblEvento.Text = "Evento de hoy";
            this.cmbEventos.Location = new System.Drawing.Point(24, 48);
            this.cmbEventos.Size = new System.Drawing.Size(640, 28);
            this.cmbEventos.Name = "cmbEventos";
            this.cmbEventos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEventos.TabIndex = 0;
            this.cmbEventos.SelectedIndexChanged += new System.EventHandler(this.EventoCambiado);
            this.lblCodigo.Location = new System.Drawing.Point(24, 100);
            this.lblCodigo.Size = new System.Drawing.Size(640, 24);
            this.lblCodigo.Text = "Código del ticket (lector USB o texto del PDF)";
            this.txtCodigo.Location = new System.Drawing.Point(24, 132);
            this.txtCodigo.Size = new System.Drawing.Size(440, 28);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.TabIndex = 1;
            this.btnRegistrar.Location = new System.Drawing.Point(480, 124);
            this.btnRegistrar.Size = new System.Drawing.Size(184, 44);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Text = "Registrar ingreso";
            this.btnRegistrar.TabIndex = 2;
            this.btnRegistrar.Click += new System.EventHandler(this.Registrar);
            this.lblResultado.Location = new System.Drawing.Point(24, 188);
            this.lblResultado.Size = new System.Drawing.Size(640, 96);
            this.lblResultado.Name = "lblResultado";
            this.lblContador.Location = new System.Drawing.Point(24, 296);
            this.lblContador.Size = new System.Drawing.Size(640, 32);
            this.lblContador.Name = "lblContador";
            this.btnVolver.Location = new System.Drawing.Point(24, 352);
            this.btnVolver.Size = new System.Drawing.Size(144, 44);
            this.btnVolver.Text = "Volver";
            this.btnVolver.TabIndex = 3;
            this.btnVolver.Click += new System.EventHandler(this.Volver);
            this.AcceptButton = this.btnRegistrar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(688, 420);
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
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ComboBox cmbEventos;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Label lblResultado;
        private System.Windows.Forms.Label lblContador;
        private System.Windows.Forms.Label lblEvento;
        private System.Windows.Forms.Label lblCodigo;
        private Visual.Componentes.BotonPrimario btnRegistrar;
        private Visual.Componentes.BotonSecundario btnVolver;
    }
}
