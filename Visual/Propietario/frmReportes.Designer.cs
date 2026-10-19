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
            this.label1 = new System.Windows.Forms.Label();
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
            this.btnGenerar = new System.Windows.Forms.Button();
            this.btnExportar = new System.Windows.Forms.Button();
            this.dgvReporte = new System.Windows.Forms.DataGridView();
            this.lblLectura = new System.Windows.Forms.Label();
            this.btnVolver = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReporte)).BeginInit();
            this.SuspendLayout();
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.label1.Location = new System.Drawing.Point(12, 8);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(170, 50);
            this.label1.TabIndex = 20;
            this.label1.Text = "Reportes";
            //
            // lblReporte
            //
            this.lblReporte.AutoSize = true;
            this.lblReporte.Location = new System.Drawing.Point(14, 74);
            this.lblReporte.Name = "lblReporte";
            this.lblReporte.Size = new System.Drawing.Size(56, 19);
            this.lblReporte.TabIndex = 21;
            this.lblReporte.Text = "Reporte";
            //
            // cmbReporte
            //
            this.cmbReporte.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmbReporte.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReporte.Location = new System.Drawing.Point(96, 70);
            this.cmbReporte.Name = "cmbReporte";
            this.cmbReporte.Size = new System.Drawing.Size(340, 27);
            this.cmbReporte.TabIndex = 0;
            this.cmbReporte.SelectedIndexChanged += new System.EventHandler(this.cmbReporte_SelectedIndexChanged);
            //
            // lblDesde
            //
            this.lblDesde.AutoSize = true;
            this.lblDesde.Location = new System.Drawing.Point(456, 74);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.Size = new System.Drawing.Size(47, 19);
            this.lblDesde.TabIndex = 22;
            this.lblDesde.Text = "Desde";
            //
            // dtpDesde
            //
            this.dtpDesde.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(512, 70);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(130, 26);
            this.dtpDesde.TabIndex = 1;
            this.dtpDesde.ValueChanged += new System.EventHandler(this.dtpPeriodo_ValueChanged);
            //
            // lblHasta
            //
            this.lblHasta.AutoSize = true;
            this.lblHasta.Location = new System.Drawing.Point(656, 74);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(44, 19);
            this.lblHasta.TabIndex = 23;
            this.lblHasta.Text = "Hasta";
            //
            // dtpHasta
            //
            this.dtpHasta.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(708, 70);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(130, 26);
            this.dtpHasta.TabIndex = 2;
            this.dtpHasta.ValueChanged += new System.EventHandler(this.dtpPeriodo_ValueChanged);
            //
            // lblFiltro
            //
            this.lblFiltro.AutoSize = true;
            this.lblFiltro.Location = new System.Drawing.Point(14, 112);
            this.lblFiltro.Name = "lblFiltro";
            this.lblFiltro.Size = new System.Drawing.Size(68, 19);
            this.lblFiltro.TabIndex = 24;
            this.lblFiltro.Text = "Categoría";
            //
            // cmbFiltro
            //
            this.cmbFiltro.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmbFiltro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFiltro.Location = new System.Drawing.Point(96, 108);
            this.cmbFiltro.Name = "cmbFiltro";
            this.cmbFiltro.Size = new System.Drawing.Size(340, 27);
            this.cmbFiltro.TabIndex = 3;
            //
            // dtpCorte
            //
            this.dtpCorte.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dtpCorte.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpCorte.Location = new System.Drawing.Point(96, 108);
            this.dtpCorte.Name = "dtpCorte";
            this.dtpCorte.Size = new System.Drawing.Size(130, 26);
            this.dtpCorte.TabIndex = 4;
            this.dtpCorte.Visible = false;
            //
            // chkSoloAgotados
            //
            this.chkSoloAgotados.AutoSize = true;
            this.chkSoloAgotados.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSoloAgotados.Location = new System.Drawing.Point(456, 110);
            this.chkSoloAgotados.Name = "chkSoloAgotados";
            this.chkSoloAgotados.Size = new System.Drawing.Size(240, 23);
            this.chkSoloAgotados.TabIndex = 5;
            this.chkSoloAgotados.Text = "Solo eventos agotados (RN-10)";
            this.chkSoloAgotados.UseVisualStyleBackColor = true;
            this.chkSoloAgotados.Visible = false;
            //
            // btnGenerar
            //
            this.btnGenerar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.btnGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerar.FlatAppearance.BorderSize = 0;
            this.btnGenerar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerar.ForeColor = System.Drawing.Color.White;
            this.btnGenerar.Location = new System.Drawing.Point(852, 66);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(116, 34);
            this.btnGenerar.TabIndex = 6;
            this.btnGenerar.Text = "Generar";
            this.btnGenerar.UseVisualStyleBackColor = false;
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            //
            // btnExportar
            //
            this.btnExportar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.btnExportar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportar.Enabled = false;
            this.btnExportar.FlatAppearance.BorderSize = 0;
            this.btnExportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportar.ForeColor = System.Drawing.Color.White;
            this.btnExportar.Location = new System.Drawing.Point(852, 104);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(116, 34);
            this.btnExportar.TabIndex = 7;
            this.btnExportar.Text = "Exportar PDF";
            this.btnExportar.UseVisualStyleBackColor = false;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            //
            // dgvReporte
            //
            this.dgvReporte.BackgroundColor = System.Drawing.Color.White;
            this.dgvReporte.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReporte.Location = new System.Drawing.Point(12, 150);
            this.dgvReporte.Name = "dgvReporte";
            this.dgvReporte.RowHeadersWidth = 62;
            this.dgvReporte.Size = new System.Drawing.Size(956, 290);
            this.dgvReporte.TabIndex = 8;
            //
            // lblLectura
            //
            this.lblLectura.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblLectura.Location = new System.Drawing.Point(12, 450);
            this.lblLectura.Name = "lblLectura";
            this.lblLectura.Size = new System.Drawing.Size(956, 120);
            this.lblLectura.TabIndex = 25;
            //
            // btnVolver
            //
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.Location = new System.Drawing.Point(12, 584);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(113, 36);
            this.btnVolver.TabIndex = 9;
            this.btnVolver.Text = "Volver";
            this.btnVolver.UseVisualStyleBackColor = true;
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            //
            // frmReportes
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(980, 632);
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
            this.Font = new System.Drawing.Font("Nirmala UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(165)))), ((int)(((byte)(169)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmReportes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmReportes";
            this.Load += new System.EventHandler(this.frmReportes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReporte)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
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
        private System.Windows.Forms.Button btnGenerar;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.DataGridView dgvReporte;
        private System.Windows.Forms.Label lblLectura;
        private System.Windows.Forms.Button btnVolver;
    }
}
