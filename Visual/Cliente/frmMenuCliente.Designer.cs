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
            this.btnEventos = new Visual.Componentes.BotonPrimario();
            this.btnHistorial = new Visual.Componentes.BotonPrimario();
            this.btnSuscripcion = new Visual.Componentes.BotonPrimario();
            this.label1 = new Visual.Componentes.EtiquetaTitulo();
            this.lblvolver = new Visual.Componentes.EtiquetaRotulo();
            this.lblNombre = new Visual.Componentes.EtiquetaTitulo();
            this.SuspendLayout();
            // 
            // btnEventos
            // 
            this.btnEventos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEventos.Location = new System.Drawing.Point(67, 248);
            this.btnEventos.Margin = new System.Windows.Forms.Padding(2);
            this.btnEventos.Name = "btnEventos";
            this.btnEventos.Size = new System.Drawing.Size(258, 78);
            this.btnEventos.TabIndex = 5;
            this.btnEventos.Text = "Eventos disponibles";
            this.btnEventos.Click += new System.EventHandler(this.btnEventos_Click);
            // 
            // btnHistorial
            // 
            this.btnHistorial.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHistorial.Location = new System.Drawing.Point(67, 376);
            this.btnHistorial.Margin = new System.Windows.Forms.Padding(2);
            this.btnHistorial.Name = "btnHistorial";
            this.btnHistorial.Size = new System.Drawing.Size(258, 77);
            this.btnHistorial.TabIndex = 6;
            this.btnHistorial.Text = "Mis eventos";
            this.btnHistorial.Click += new System.EventHandler(this.btnHistorial_Click);
            //
            // btnSuscripcion
            //
            this.btnSuscripcion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSuscripcion.Location = new System.Drawing.Point(67, 504);
            this.btnSuscripcion.Margin = new System.Windows.Forms.Padding(2);
            this.btnSuscripcion.Name = "btnSuscripcion";
            this.btnSuscripcion.Size = new System.Drawing.Size(258, 77);
            this.btnSuscripcion.TabIndex = 7;
            this.btnSuscripcion.Text = "Mi suscripción";
            this.btnSuscripcion.Click += new System.EventHandler(this.btnSuscripcion_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(82, 62);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(228, 50);
            this.label1.TabIndex = 30;
            this.label1.Text = "Bienvenido ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblvolver
            // 
            this.lblvolver.AutoSize = true;
            this.lblvolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblvolver.Location = new System.Drawing.Point(127, 636);
            this.lblvolver.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblvolver.Name = "lblvolver";
            this.lblvolver.Size = new System.Drawing.Size(127, 23);
            this.lblvolver.TabIndex = 32;
            this.lblvolver.Text = "Volver al inicio";
            this.lblvolver.Click += new System.EventHandler(this.lblvolver_Click);
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(82, 120);
            this.lblNombre.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(214, 50);
            this.lblNombre.TabIndex = 33;
            this.lblNombre.Text = "[USUARIO]";
            this.lblNombre.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmMenuCliente
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(394, 723);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.lblvolver);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSuscripcion);
            this.Controls.Add(this.btnHistorial);
            this.Controls.Add(this.btnEventos);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmMenuCliente";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EventAura · Mi cuenta";
            this.Load += new System.EventHandler(this.frmMenuCliente_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Visual.Componentes.BotonPrimario btnEventos;
        private Visual.Componentes.BotonPrimario btnHistorial;
        private Visual.Componentes.BotonPrimario btnSuscripcion;
        private Visual.Componentes.EtiquetaTitulo label1;
        private Visual.Componentes.EtiquetaRotulo lblvolver;
        private Visual.Componentes.EtiquetaTitulo lblNombre;
    }
}