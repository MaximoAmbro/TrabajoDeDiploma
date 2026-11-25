namespace Visual
{
    partial class frmListasEspera
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
            this.lblEvento = new System.Windows.Forms.Label();
            this.cmbEventos = new System.Windows.Forms.ComboBox();
            this.dgvLista = new System.Windows.Forms.DataGridView();
            this.lblCantidad = new System.Windows.Forms.Label();
            this.btnVolver = new Visual.Componentes.BotonSecundario();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLista)).BeginInit();
            this.SuspendLayout();
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 84);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(792, 32);
            this.label1.TabIndex = 28;
            this.label1.Text = "Listas de espera";
            //
            // lblEvento
            //
            this.lblEvento.AutoSize = true;
            this.lblEvento.Location = new System.Drawing.Point(24, 140);
            this.lblEvento.Name = "lblEvento";
            this.lblEvento.Size = new System.Drawing.Size(792, 20);
            this.lblEvento.TabIndex = 29;
            this.lblEvento.Text = "Evento";
            //
            // cmbEventos
            //
            this.cmbEventos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmbEventos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEventos.Location = new System.Drawing.Point(24, 164);
            this.cmbEventos.Name = "cmbEventos";
            this.cmbEventos.Size = new System.Drawing.Size(792, 28);
            this.cmbEventos.TabIndex = 0;
            this.cmbEventos.SelectedIndexChanged += new System.EventHandler(this.cmbEventos_SelectedIndexChanged);
            //
            // dgvLista
            //
            this.dgvLista.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLista.Location = new System.Drawing.Point(24, 212);
            this.dgvLista.Margin = new System.Windows.Forms.Padding(2);
            this.dgvLista.Name = "dgvLista";
            this.dgvLista.RowHeadersWidth = 62;
            this.dgvLista.Size = new System.Drawing.Size(792, 208);
            this.dgvLista.TabIndex = 1;
            //
            // lblCantidad
            //
            this.lblCantidad.Location = new System.Drawing.Point(24, 428);
            this.lblCantidad.Name = "lblCantidad";
            this.lblCantidad.Size = new System.Drawing.Size(616, 48);
            this.lblCantidad.TabIndex = 30;
            //
            // btnVolver
            //
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.Location = new System.Drawing.Point(648, 444);
            this.btnVolver.Margin = new System.Windows.Forms.Padding(2);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(168, 40);
            this.btnVolver.TabIndex = 2;
            this.btnVolver.Text = "Volver";
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            //
            // frmListasEspera
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(840, 508);
            this.Controls.Add(this.btnVolver);
            this.Controls.Add(this.lblCantidad);
            this.Controls.Add(this.dgvLista);
            this.Controls.Add(this.cmbEventos);
            this.Controls.Add(this.lblEvento);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmListasEspera";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EventAura · Listas de espera";
            this.Load += new System.EventHandler(this.frmListasEspera_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLista)).EndInit();
            this.barraSuperior = new Visual.Componentes.BarraSuperior();
            this.barraSuperior.Name = "barraSuperior";
            this.barraSuperior.Size = new System.Drawing.Size(840, 60);
            this.barraSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Controls.Add(this.barraSuperior);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Visual.Componentes.EtiquetaTitulo label1;
        private System.Windows.Forms.Label lblEvento;
        private System.Windows.Forms.ComboBox cmbEventos;
        private System.Windows.Forms.DataGridView dgvLista;
        private System.Windows.Forms.Label lblCantidad;
        private Visual.Componentes.BotonSecundario btnVolver;
        private Visual.Componentes.BarraSuperior barraSuperior;
    }
}
