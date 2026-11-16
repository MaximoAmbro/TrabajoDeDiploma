namespace Visual
{
    partial class frmReportes
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
            this.lblReporte = new System.Windows.Forms.Label();
            this.cmbReporte = new System.Windows.Forms.ComboBox();
            this.lblDesde = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.lblFiltro = new System.Windows.Forms.Label();
            this.cmbFiltro = new System.Windows.Forms.ComboBox();
            this.dtpCorte = new System.Windows.Forms.DateTimePicker();
            this.chkSoloAgotados = new System.Windows.Forms.CheckBox();
            this.btnGenerar = new Visual.Componentes.BotonPrimario();
            this.btnExportar = new Visual.Componentes.BotonPrimario();
            this.dgvReporte = new System.Windows.Forms.DataGridView();
            this.lblLectura = new System.Windows.Forms.Label();
            this.btnVolver = new Visual.Componentes.BotonSecundario();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReporte)).BeginInit();
            this.SuspendLayout();
            //
            // label1
            //
            this.label1.AutoSize = false;
            this.label1.Location = new System.Drawing.Point(24, 84);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(852, 32);
            this.label1.TabIndex = 20;
            this.label1.Text = "Reportes";
            //
            // lblReporte
            //
            this.lblReporte.AutoSize = false;
            this.lblReporte.Location = new System.Drawing.Point(24, 132);
            this.lblReporte.Name = "lblReporte";
            this.lblReporte.Size = new System.Drawing.Size(440, 20);
            this.lblReporte.TabIndex = 21;
            this.lblReporte.Text = "Reporte";
            //
            // cmbReporte
            //
            this.cmbReporte.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmbReporte.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReporte.Location = new System.Drawing.Point(24, 156);
            this.cmbReporte.Name = "cmbReporte";
            this.cmbReporte.Size = new System.Drawing.Size(440, 28);
            this.cmbReporte.TabIndex = 0;
            this.cmbReporte.SelectedIndexChanged += new System.EventHandler(this.cmbReporte_SelectedIndexChanged);
            //
            // lblDesde
            //
            this.lblDesde.AutoSize = false;
            this.lblDesde.Location = new System.Drawing.Point(488, 132);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.Size = new System.Drawing.Size(184, 20);
            this.lblDesde.TabIndex = 22;
            this.lblDesde.Text = "Desde";
            //
            // dtpDesde
            //
            this.dtpDesde.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(488, 156);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(184, 28);
            this.dtpDesde.TabIndex = 1;
            this.dtpDesde.ValueChanged += new System.EventHandler(this.dtpPeriodo_ValueChanged);
            //
            // lblHasta
            //
            this.lblHasta.AutoSize = false;
            this.lblHasta.Location = new System.Drawing.Point(696, 132);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(180, 20);
            this.lblHasta.TabIndex = 23;
            this.lblHasta.Text = "Hasta";
            //
            // dtpHasta
            //
            this.dtpHasta.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(696, 156);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(180, 28);
            this.dtpHasta.TabIndex = 2;
            this.dtpHasta.ValueChanged += new System.EventHandler(this.dtpPeriodo_ValueChanged);
            //
            // lblFiltro
            //
            this.lblFiltro.AutoSize = false;
            this.lblFiltro.Location = new System.Drawing.Point(24, 204);
            this.lblFiltro.Name = "lblFiltro";
            this.lblFiltro.Size = new System.Drawing.Size(440, 20);
            this.lblFiltro.TabIndex = 24;
            this.lblFiltro.Text = "Categoría";
            //
            // cmbFiltro
            //
            this.cmbFiltro.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmbFiltro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFiltro.Location = new System.Drawing.Point(24, 228);
            this.cmbFiltro.Name = "cmbFiltro";
            this.cmbFiltro.Size = new System.Drawing.Size(440, 28);
            this.cmbFiltro.TabIndex = 3;
            //
            // dtpCorte
            //
            this.dtpCorte.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dtpCorte.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpCorte.Location = new System.Drawing.Point(24, 228);
            this.dtpCorte.Name = "dtpCorte";
            this.dtpCorte.Size = new System.Drawing.Size(440, 28);
            this.dtpCorte.TabIndex = 4;
            this.dtpCorte.Visible = false;
            //
            // chkSoloAgotados
            //
            this.chkSoloAgotados.AutoSize = false;
            this.chkSoloAgotados.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSoloAgotados.Location = new System.Drawing.Point(488, 204);
            this.chkSoloAgotados.Name = "chkSoloAgotados";
            this.chkSoloAgotados.Size = new System.Drawing.Size(388, 24);
            this.chkSoloAgotados.TabIndex = 5;
            this.chkSoloAgotados.Text = "Solo eventos agotados (RN-10)";
            this.chkSoloAgotados.Visible = false;
            //
            // btnGenerar
            //
            this.btnGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerar.Location = new System.Drawing.Point(688, 236);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(188, 40);
            this.btnGenerar.TabIndex = 6;
            this.btnGenerar.Text = "Generar";
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            //
            // btnExportar
            //
            this.btnExportar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportar.Enabled = false;
            this.btnExportar.Location = new System.Drawing.Point(680, 508);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(196, 40);
            this.btnExportar.TabIndex = 7;
            this.btnExportar.Text = "Exportar PDF";
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            //
            // dgvReporte
            //
            this.dgvReporte.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReporte.Location = new System.Drawing.Point(24, 292);
            this.dgvReporte.Name = "dgvReporte";
            this.dgvReporte.RowHeadersWidth = 62;
            this.dgvReporte.Size = new System.Drawing.Size(852, 128);
            this.dgvReporte.TabIndex = 8;
            //
            // lblLectura
            //
            this.lblLectura.Location = new System.Drawing.Point(24, 428);
            this.lblLectura.Name = "lblLectura";
            this.lblLectura.Size = new System.Drawing.Size(852, 64);
            this.lblLectura.TabIndex = 25;
            //
            // btnVolver
            //
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.Location = new System.Drawing.Point(24, 508);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(144, 40);
            this.btnVolver.TabIndex = 9;
            this.btnVolver.Text = "Volver";
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            //
            // frmReportes
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(900, 572);
            this.Controls.Add(this.btnVolver);
            this.Controls.Add(this.lblLectura);
            this.Controls.Add(this.dgvReporte);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.btnGenerar);
            this.Controls.Add(this.chkSoloAgotados);
            this.Controls.Add(this.dtpCorte);
            this.Controls.Add(this.cmbFiltro);
            this.Controls.Add(this.lblFiltro);
            this.Controls.Add(this.dtpHasta);
            this.Controls.Add(this.lblHasta);
            this.Controls.Add(this.dtpDesde);
            this.Controls.Add(this.lblDesde);
            this.Controls.Add(this.cmbReporte);
            this.Controls.Add(this.lblReporte);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmReportes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EventAura · Reportes";
            this.Load += new System.EventHandler(this.frmReportes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReporte)).EndInit();
            this.barraSuperior = new Visual.Componentes.BarraSuperior();
            this.barraSuperior.Name = "barraSuperior";
            this.barraSuperior.Size = new System.Drawing.Size(900, 60);
            this.barraSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Controls.Add(this.barraSuperior);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Visual.Componentes.EtiquetaTitulo label1;
        private System.Windows.Forms.Label lblReporte;
        private System.Windows.Forms.ComboBox cmbReporte;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Label lblFiltro;
        private System.Windows.Forms.ComboBox cmbFiltro;
        private System.Windows.Forms.DateTimePicker dtpCorte;
        private System.Windows.Forms.CheckBox chkSoloAgotados;
        private Visual.Componentes.BotonPrimario btnGenerar;
        private Visual.Componentes.BotonPrimario btnExportar;
        private System.Windows.Forms.DataGridView dgvReporte;
        private System.Windows.Forms.Label lblLectura;
        private Visual.Componentes.BotonSecundario btnVolver;
        private Visual.Componentes.BarraSuperior barraSuperior;
    }
}
