namespace Visual.Vendedor
{
    partial class frmAgregarSector
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
            this.label5 = new Visual.Componentes.EtiquetaRotulo();
            this.txtPrecio = new System.Windows.Forms.TextBox();
            this.btnregistrarse = new Visual.Componentes.BotonPrimario();
            this.txtCapacidad = new System.Windows.Forms.TextBox();
            this.lblmail = new Visual.Componentes.EtiquetaRotulo();
            this.label6 = new Visual.Componentes.EtiquetaRotulo();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.label1 = new Visual.Componentes.EtiquetaTitulo();
            this.lblVolver = new Visual.Componentes.EtiquetaRotulo();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(71, 262);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 19);
            this.label5.TabIndex = 50;
            this.label5.Text = "Precio";
            // 
            // txtPrecio
            // 
            this.txtPrecio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPrecio.Location = new System.Drawing.Point(74, 282);
            this.txtPrecio.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtPrecio.Name = "txtPrecio";
            this.txtPrecio.Size = new System.Drawing.Size(237, 27);
            this.txtPrecio.TabIndex = 51;
            // 
            // btnregistrarse
            // 
            this.btnregistrarse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnregistrarse.Location = new System.Drawing.Point(89, 461);
            this.btnregistrarse.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.btnregistrarse.Name = "btnregistrarse";
            this.btnregistrarse.Size = new System.Drawing.Size(214, 43);
            this.btnregistrarse.TabIndex = 48;
            this.btnregistrarse.Text = "Agregar sector";
            this.btnregistrarse.Click += new System.EventHandler(this.btnregistrarse_Click);
            // 
            // txtCapacidad
            // 
            this.txtCapacidad.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCapacidad.Location = new System.Drawing.Point(78, 352);
            this.txtCapacidad.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.txtCapacidad.Name = "txtCapacidad";
            this.txtCapacidad.Size = new System.Drawing.Size(237, 27);
            this.txtCapacidad.TabIndex = 46;
            // 
            // lblmail
            // 
            this.lblmail.AutoSize = true;
            this.lblmail.Location = new System.Drawing.Point(71, 333);
            this.lblmail.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblmail.Name = "lblmail";
            this.lblmail.Size = new System.Drawing.Size(80, 19);
            this.lblmail.TabIndex = 45;
            this.lblmail.Text = "Capacidad";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(71, 188);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 19);
            this.label6.TabIndex = 44;
            this.label6.Text = "Nombre";
            // 
            // txtNombre
            // 
            this.txtNombre.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtNombre.Location = new System.Drawing.Point(74, 208);
            this.txtNombre.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(237, 27);
            this.txtNombre.TabIndex = 49;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 80);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(295, 50);
            this.label1.TabIndex = 38;
            this.label1.Text = "Registrar sector";
            // 
            // lblVolver
            // 
            this.lblVolver.AutoSize = true;
            this.lblVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblVolver.Location = new System.Drawing.Point(165, 528);
            this.lblVolver.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblVolver.Name = "lblVolver";
            this.lblVolver.Size = new System.Drawing.Size(60, 23);
            this.lblVolver.TabIndex = 47;
            this.lblVolver.Text = "Volver";
            this.lblVolver.Click += new System.EventHandler(this.lblVolver_Click);
            // 
            // frmAgregarSector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(394, 595);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtPrecio);
            this.Controls.Add(this.btnregistrarse);
            this.Controls.Add(this.lblVolver);
            this.Controls.Add(this.txtCapacidad);
            this.Controls.Add(this.lblmail);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "frmAgregarSector";
            this.Text = "EventAura · Agregar sector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Visual.Componentes.EtiquetaRotulo label5;
        private System.Windows.Forms.TextBox txtPrecio;
        private Visual.Componentes.BotonPrimario btnregistrarse;
        private System.Windows.Forms.TextBox txtCapacidad;
        private Visual.Componentes.EtiquetaRotulo lblmail;
        private Visual.Componentes.EtiquetaRotulo label6;
        private System.Windows.Forms.TextBox txtNombre;
        private Visual.Componentes.EtiquetaTitulo label1;
        private Visual.Componentes.EtiquetaRotulo lblVolver;
    }
}