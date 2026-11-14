namespace Visual.Propietario
{
    partial class frmAgregarEvento
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
            this.label1 = new Visual.Componentes.EtiquetaRotulo();
            this.btnRegistrar = new Visual.Componentes.BotonPrimario();
            this.label2 = new Visual.Componentes.EtiquetaRotulo();
            this.lblVolver = new Visual.Componentes.BotonSecundario();
            this.dtEvento = new System.Windows.Forms.DateTimePicker();
            this.label3 = new Visual.Componentes.EtiquetaTitulo();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblCategoria = new Visual.Componentes.EtiquetaRotulo();
            this.cmbCategoria = new System.Windows.Forms.ComboBox();
            this.lblPreventa = new Visual.Componentes.EtiquetaRotulo();
            this.dtpFinPreventa = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(368, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Nombre evento";
            // 
            // btnRegistrar
            // 
            this.btnRegistrar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegistrar.Location = new System.Drawing.Point(224, 368);
            this.btnRegistrar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Size = new System.Drawing.Size(168, 40);
            this.btnRegistrar.TabIndex = 2;
            this.btnRegistrar.Text = "Registrar";
            this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 144);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(368, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Fecha";
            // 
            // lblVolver
            // 
            this.lblVolver.AutoSize = false;
            this.lblVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblVolver.Location = new System.Drawing.Point(24, 368);
            this.lblVolver.Name = "lblVolver";
            this.lblVolver.Size = new System.Drawing.Size(136, 40);
            this.lblVolver.TabIndex = 5;
            this.lblVolver.Text = "Volver";
            this.lblVolver.Click += new System.EventHandler(this.lblVolver_Click);
            // 
            // dtEvento
            // 
            this.dtEvento.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dtEvento.Location = new System.Drawing.Point(24, 168);
            this.dtEvento.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dtEvento.Name = "dtEvento";
            this.dtEvento.Size = new System.Drawing.Size(368, 28);
            this.dtEvento.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(368, 32);
            this.label3.TabIndex = 7;
            this.label3.Text = "Agregar eventos";
            // 
            // txtNombre
            // 
            this.txtNombre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNombre.Location = new System.Drawing.Point(24, 104);
            this.txtNombre.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(368, 28);
            this.txtNombre.TabIndex = 49;
            //
            // lblCategoria
            //
            this.lblCategoria.AutoSize = true;
            this.lblCategoria.Location = new System.Drawing.Point(24, 208);
            this.lblCategoria.Name = "lblCategoria";
            this.lblCategoria.Size = new System.Drawing.Size(368, 20);
            this.lblCategoria.TabIndex = 50;
            this.lblCategoria.Text = "Categoría";
            //
            // cmbCategoria
            //
            this.cmbCategoria.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmbCategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategoria.FormattingEnabled = true;
            this.cmbCategoria.Location = new System.Drawing.Point(24, 232);
            this.cmbCategoria.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbCategoria.Name = "cmbCategoria";
            this.cmbCategoria.Size = new System.Drawing.Size(368, 28);
            this.cmbCategoria.TabIndex = 51;
            //
            // lblPreventa
            //
            this.lblPreventa.AutoSize = true;
            this.lblPreventa.Location = new System.Drawing.Point(24, 272);
            this.lblPreventa.Name = "lblPreventa";
            this.lblPreventa.Size = new System.Drawing.Size(368, 20);
            this.lblPreventa.TabIndex = 53;
            this.lblPreventa.Text = "Preventa para suscriptores hasta";
            //
            // dtpFinPreventa
            //
            this.dtpFinPreventa.Checked = false;
            this.dtpFinPreventa.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dtpFinPreventa.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dtpFinPreventa.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFinPreventa.Location = new System.Drawing.Point(24, 296);
            this.dtpFinPreventa.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dtpFinPreventa.Name = "dtpFinPreventa";
            this.dtpFinPreventa.ShowCheckBox = true;
            this.dtpFinPreventa.Size = new System.Drawing.Size(368, 28);
            this.dtpFinPreventa.TabIndex = 52;
            //
            // frmAgregarEventoo
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(416, 432);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dtEvento);
            this.Controls.Add(this.lblVolver);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnRegistrar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblCategoria);
            this.Controls.Add(this.cmbCategoria);
            this.Controls.Add(this.lblPreventa);
            this.Controls.Add(this.dtpFinPreventa);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmAgregarEventoo";
            this.Text = "EventAura · Registrar evento";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Visual.Componentes.EtiquetaRotulo label1;
        private Visual.Componentes.BotonPrimario btnRegistrar;
        private Visual.Componentes.EtiquetaRotulo label2;
        private Visual.Componentes.BotonSecundario lblVolver;
        private System.Windows.Forms.DateTimePicker dtEvento;
        private Visual.Componentes.EtiquetaTitulo label3;
        private System.Windows.Forms.TextBox txtNombre;
        private Visual.Componentes.EtiquetaRotulo lblCategoria;
        private System.Windows.Forms.ComboBox cmbCategoria;
        private Visual.Componentes.EtiquetaRotulo lblPreventa;
        private System.Windows.Forms.DateTimePicker dtpFinPreventa;
    }
}