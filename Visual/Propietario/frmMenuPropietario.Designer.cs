namespace Visual
{
    partial class frmMenuPropietario
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
            this.lblNombre = new Visual.Componentes.EtiquetaTitulo();
            this.lblvolver = new Visual.Componentes.BotonSecundario();
            this.label1 = new Visual.Componentes.EtiquetaTitulo();
            this.btnRegistrarLocal = new Visual.Componentes.BotonPrimario();
            this.btnMisLocales = new Visual.Componentes.BotonPrimario();
            this.btnListasEspera = new Visual.Componentes.BotonPrimario();
            this.btnReportes = new Visual.Componentes.BotonPrimario();
            this.btnControlIngreso = new Visual.Componentes.BotonPrimario();
            this.btnControlIngreso.Location = new System.Drawing.Point(24, 332);
            this.btnControlIngreso.Size = new System.Drawing.Size(352, 40);
            this.btnControlIngreso.Name = "btnControlIngreso";
            this.btnControlIngreso.Text = "Control de ingreso";
            this.btnControlIngreso.TabIndex = 41;
            this.btnControlIngreso.Click += new System.EventHandler(this.btnControlIngreso_Click);
            this.Controls.Add(this.btnControlIngreso);
            this.SuspendLayout();
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(24, 116);
            this.lblNombre.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(352, 32);
            this.lblNombre.TabIndex = 38;
            this.lblNombre.Text = "[USUARIO]";
            this.lblNombre.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblvolver
            // 
            this.lblvolver.AutoSize = false;
            this.lblvolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblvolver.Location = new System.Drawing.Point(24, 396);
            this.lblvolver.Name = "lblvolver";
            this.lblvolver.Size = new System.Drawing.Size(192, 40);
            this.lblvolver.TabIndex = 37;
            this.lblvolver.Text = "Volver al inicio";
            this.lblvolver.Click += new System.EventHandler(this.lblvolver_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 84);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(352, 32);
            this.label1.TabIndex = 36;
            this.label1.Text = "Administración";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRegistrarLocal
            // 
            this.btnRegistrarLocal.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegistrarLocal.Location = new System.Drawing.Point(24, 200);
            this.btnRegistrarLocal.Margin = new System.Windows.Forms.Padding(2);
            this.btnRegistrarLocal.Name = "btnRegistrarLocal";
            this.btnRegistrarLocal.Size = new System.Drawing.Size(352, 40);
            this.btnRegistrarLocal.TabIndex = 35;
            this.btnRegistrarLocal.Text = "Registrar local";
            this.btnRegistrarLocal.Click += new System.EventHandler(this.btnRegistrarLocal_Click);
            // 
            // btnMisLocales
            // 
            this.btnMisLocales.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMisLocales.Location = new System.Drawing.Point(24, 156);
            this.btnMisLocales.Margin = new System.Windows.Forms.Padding(2);
            this.btnMisLocales.Name = "btnMisLocales";
            this.btnMisLocales.Size = new System.Drawing.Size(352, 40);
            this.btnMisLocales.TabIndex = 34;
            this.btnMisLocales.Text = "Mis locales";
            this.btnMisLocales.Click += new System.EventHandler(this.btnMisLocales_Click);
            //
            // btnListasEspera
            //
            this.btnListasEspera.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnListasEspera.Location = new System.Drawing.Point(24, 244);
            this.btnListasEspera.Margin = new System.Windows.Forms.Padding(2);
            this.btnListasEspera.Name = "btnListasEspera";
            this.btnListasEspera.Size = new System.Drawing.Size(352, 40);
            this.btnListasEspera.TabIndex = 39;
            this.btnListasEspera.Text = "Listas de espera";
            this.btnListasEspera.Click += new System.EventHandler(this.btnListasEspera_Click);
            //
            // btnReportes
            //
            this.btnReportes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReportes.Location = new System.Drawing.Point(24, 288);
            this.btnReportes.Margin = new System.Windows.Forms.Padding(2);
            this.btnReportes.Name = "btnReportes";
            this.btnReportes.Size = new System.Drawing.Size(352, 40);
            this.btnReportes.TabIndex = 40;
            this.btnReportes.Text = "Reportes";
            this.btnReportes.Click += new System.EventHandler(this.btnReportes_Click);
            // 
            // frmMenuVendedor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(400, 508);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.lblvolver);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnReportes);
            this.Controls.Add(this.btnListasEspera);
            this.Controls.Add(this.btnRegistrarLocal);
            this.Controls.Add(this.btnMisLocales);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmMenuVendedor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EventAura · Administración";
            this.Load += new System.EventHandler(this.frmMenuVendedor_Load);
            this.barraSuperior = new Visual.Componentes.BarraSuperior();
            this.barraSuperior.Name = "barraSuperior";
            this.barraSuperior.Size = new System.Drawing.Size(400, 60);
            this.barraSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Controls.Add(this.barraSuperior);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Visual.Componentes.EtiquetaTitulo lblNombre;
        private Visual.Componentes.BotonSecundario lblvolver;
        private Visual.Componentes.EtiquetaTitulo label1;
        private Visual.Componentes.BotonPrimario btnRegistrarLocal;
        private Visual.Componentes.BotonPrimario btnMisLocales;
        private Visual.Componentes.BotonPrimario btnListasEspera;
        private Visual.Componentes.BotonPrimario btnReportes;
        private Visual.Componentes.BotonPrimario btnControlIngreso;
        private Visual.Componentes.BarraSuperior barraSuperior;
    }
}
