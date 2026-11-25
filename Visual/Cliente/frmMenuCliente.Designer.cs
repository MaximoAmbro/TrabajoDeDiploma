namespace Visual
{
    partial class frmMenuCliente
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnInicio = new Visual.Componentes.BotonSecundario();
            this.btnEventos = new Visual.Componentes.BotonSecundario();
            this.btnHistorial = new Visual.Componentes.BotonSecundario();
            this.btnSuscripcion = new Visual.Componentes.BotonSecundario();
            this.btnAtras = new Visual.Componentes.BotonSecundario();
            this.label1 = new Visual.Componentes.EtiquetaTitulo();
            this.lblvolver = new Visual.Componentes.BotonSecundario();
            this.lblNombre = new Visual.Componentes.EtiquetaTitulo();
            this.SuspendLayout();
            // 
            // btnEventos
            // 
            this.btnEventos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInicio.Location = new System.Drawing.Point(24, 172);
            this.btnInicio.Name = "btnInicio";
            this.btnInicio.Size = new System.Drawing.Size(240, 44);
            this.btnInicio.TabIndex = 4;
            this.btnInicio.Text = "Inicio";
            this.btnInicio.Click += new System.EventHandler(this.btnInicio_Click);
            //
            // btnEventos
            //
            this.btnEventos.Location = new System.Drawing.Point(24, 224);
            this.btnEventos.Margin = new System.Windows.Forms.Padding(2);
            this.btnEventos.Name = "btnEventos";
            this.btnEventos.Size = new System.Drawing.Size(240, 44);
            this.btnEventos.TabIndex = 5;
            this.btnEventos.Text = "Eventos disponibles";
            this.btnEventos.Click += new System.EventHandler(this.btnEventos_Click);
            // 
            // btnHistorial
            // 
            this.btnHistorial.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHistorial.Location = new System.Drawing.Point(24, 276);
            this.btnHistorial.Margin = new System.Windows.Forms.Padding(2);
            this.btnHistorial.Name = "btnHistorial";
            this.btnHistorial.Size = new System.Drawing.Size(240, 44);
            this.btnHistorial.TabIndex = 6;
            this.btnHistorial.Text = "Mis entradas";
            this.btnHistorial.Click += new System.EventHandler(this.btnHistorial_Click);
            //
            // btnSuscripcion
            //
            this.btnSuscripcion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSuscripcion.Location = new System.Drawing.Point(24, 328);
            this.btnSuscripcion.Margin = new System.Windows.Forms.Padding(2);
            this.btnSuscripcion.Name = "btnSuscripcion";
            this.btnSuscripcion.Size = new System.Drawing.Size(240, 44);
            this.btnSuscripcion.TabIndex = 7;
            this.btnSuscripcion.Text = "Mi suscripción";
            this.btnSuscripcion.Click += new System.EventHandler(this.btnSuscripcion_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 84);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(240, 32);
            this.label1.TabIndex = 30;
            this.label1.Text = "Mi cuenta";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblvolver
            // 
            this.lblvolver.AutoSize = false;
            this.lblvolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblvolver.Location = new System.Drawing.Point(24, 680);
            this.lblvolver.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblvolver.Name = "lblvolver";
            this.lblvolver.Size = new System.Drawing.Size(240, 40);
            this.lblvolver.TabIndex = 32;
            this.lblvolver.Text = "Cerrar sesión";
            this.lblvolver.Click += new System.EventHandler(this.lblvolver_Click);
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(24, 124);
            this.lblNombre.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(240, 32);
            this.lblNombre.TabIndex = 33;
            this.lblNombre.Text = "[USUARIO]";
            this.lblNombre.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmMenuCliente
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1200, 760);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.lblvolver);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSuscripcion);
            this.Controls.Add(this.btnHistorial);
            this.Controls.Add(this.btnEventos);
            this.Controls.Add(this.btnInicio);
            this.btnAtras.Location = new System.Drawing.Point(24, 392);
            this.btnAtras.Name = "btnAtras";
            this.btnAtras.Size = new System.Drawing.Size(240, 40);
            this.btnAtras.TabIndex = 8;
            this.btnAtras.Text = "‹  Atrás";
            this.btnAtras.Enabled = false;
            this.btnAtras.Click += new System.EventHandler(this.btnAtras_Click);
            this.Controls.Add(this.btnAtras);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmMenuCliente";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EventAura · Mi cuenta";
            this.Load += new System.EventHandler(this.frmMenuCliente_Load);
            this.barraSuperior = new Visual.Componentes.BarraSuperior();
            this.barraSuperior.Name = "barraSuperior";
            this.barraSuperior.Size = new System.Drawing.Size(400, 60);
            this.barraSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Controls.Add(this.barraSuperior);
            this.inicioCliente = new Visual.Componentes.InicioCliente();
            this.inicioCliente.Name = "inicioCliente";
            this.inicioCliente.Location = new System.Drawing.Point(300, 84);
            this.inicioCliente.Size = new System.Drawing.Size(876, 652);
            this.Controls.Add(this.inicioCliente);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Visual.Componentes.BotonSecundario btnInicio;
        private Visual.Componentes.BotonSecundario btnEventos;
        private Visual.Componentes.BotonSecundario btnHistorial;
        private Visual.Componentes.BotonSecundario btnSuscripcion;
        private Visual.Componentes.BotonSecundario btnAtras;
        private Visual.Componentes.EtiquetaTitulo label1;
        private Visual.Componentes.BotonSecundario lblvolver;
        private Visual.Componentes.EtiquetaTitulo lblNombre;
        private Visual.Componentes.BarraSuperior barraSuperior;
        private Visual.Componentes.InicioCliente inicioCliente;
    }
}
