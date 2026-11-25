namespace Visual
{
    partial class frmSuscripcion
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
            this.label1 = new Visual.Componentes.EtiquetaTitulo();
            this.lblEstado = new Visual.Componentes.EtiquetaRotulo();
            this.lblBeneficios = new System.Windows.Forms.Label();
            this.rdbMensual = new System.Windows.Forms.RadioButton();
            this.rdbAnual = new System.Windows.Forms.RadioButton();
            this.btnContratar = new Visual.Componentes.BotonPrimario();
            this.btnCancelar = new Visual.Componentes.BotonSecundario();
            this.btnVolver = new Visual.Componentes.BotonSecundario();
            this.SuspendLayout();
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 84);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(392, 32);
            this.label1.TabIndex = 30;
            this.label1.Text = "Mi suscripción";
            //
            // lblEstado
            //
            this.lblEstado.Location = new System.Drawing.Point(24, 132);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(392, 48);
            this.lblEstado.TabIndex = 31;
            this.lblEstado.Text = "Estado";
            this.lblEstado.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblBeneficios
            //
            this.lblBeneficios.Location = new System.Drawing.Point(24, 188);
            this.lblBeneficios.Name = "lblBeneficios";
            this.lblBeneficios.Size = new System.Drawing.Size(392, 104);
            this.lblBeneficios.TabIndex = 32;
            this.lblBeneficios.Text = "Beneficios";
            //
            // rdbMensual
            //
            this.rdbMensual.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdbMensual.Location = new System.Drawing.Point(24, 308);
            this.rdbMensual.Name = "rdbMensual";
            this.rdbMensual.Size = new System.Drawing.Size(392, 24);
            this.rdbMensual.TabIndex = 0;
            this.rdbMensual.TabStop = true;
            this.rdbMensual.Text = "Mensual";
            //
            // rdbAnual
            //
            this.rdbAnual.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdbAnual.Location = new System.Drawing.Point(24, 340);
            this.rdbAnual.Name = "rdbAnual";
            this.rdbAnual.Size = new System.Drawing.Size(392, 24);
            this.rdbAnual.TabIndex = 1;
            this.rdbAnual.Text = "Anual";
            //
            // btnContratar
            //
            this.btnContratar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnContratar.Location = new System.Drawing.Point(232, 380);
            this.btnContratar.Margin = new System.Windows.Forms.Padding(2);
            this.btnContratar.Name = "btnContratar";
            this.btnContratar.Size = new System.Drawing.Size(184, 40);
            this.btnContratar.TabIndex = 2;
            this.btnContratar.Text = "Contratar";
            this.btnContratar.Click += new System.EventHandler(this.btnContratar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.Location = new System.Drawing.Point(24, 380);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(192, 40);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "Cancelar suscripción";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // btnVolver
            //
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.Location = new System.Drawing.Point(24, 444);
            this.btnVolver.Margin = new System.Windows.Forms.Padding(2);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(136, 40);
            this.btnVolver.TabIndex = 4;
            this.btnVolver.Text = "Volver";
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            //
            // frmSuscripcion
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(440, 508);
            this.Controls.Add(this.btnVolver);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnContratar);
            this.Controls.Add(this.rdbAnual);
            this.Controls.Add(this.rdbMensual);
            this.Controls.Add(this.lblBeneficios);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmSuscripcion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EventAura · Mi suscripción";
            this.Load += new System.EventHandler(this.frmSuscripcion_Load);
            this.barraSuperior = new Visual.Componentes.BarraSuperior();
            this.barraSuperior.Name = "barraSuperior";
            this.barraSuperior.Size = new System.Drawing.Size(440, 60);
            this.barraSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Controls.Add(this.barraSuperior);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Visual.Componentes.EtiquetaTitulo label1;
        private Visual.Componentes.EtiquetaRotulo lblEstado;
        private System.Windows.Forms.Label lblBeneficios;
        private System.Windows.Forms.RadioButton rdbMensual;
        private System.Windows.Forms.RadioButton rdbAnual;
        private Visual.Componentes.BotonPrimario btnContratar;
        private Visual.Componentes.BotonSecundario btnCancelar;
        private Visual.Componentes.BotonSecundario btnVolver;
        private Visual.Componentes.BarraSuperior barraSuperior;
    }
}
