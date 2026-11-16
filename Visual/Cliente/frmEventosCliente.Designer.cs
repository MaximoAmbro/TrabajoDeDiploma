namespace Visual
{
    partial class frmEventosCliente
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
            this.btnVolver = new Visual.Componentes.BotonSecundario();
            this.label2 = new Visual.Componentes.EtiquetaTitulo();
            this.dgveventos = new System.Windows.Forms.DataGridView();
            this.btnComprar = new Visual.Componentes.BotonPrimario();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.cmbCategoria = new System.Windows.Forms.ComboBox();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.btnBuscar = new Visual.Componentes.BotonPrimario();
            this.btnLimpiar = new Visual.Componentes.BotonSecundario();
            ((System.ComponentModel.ISupportInitialize)(this.dgveventos)).BeginInit();
            this.indicadorPasos = new Visual.Componentes.IndicadorPasos();
            this.indicadorPasos.Name = "indicadorPasos";
            this.indicadorPasos.PasoActual = 1;
            this.indicadorPasos.Location = new System.Drawing.Point(24, 76);
            this.indicadorPasos.Size = new System.Drawing.Size(792, 32);
            this.Controls.Add(this.indicadorPasos);
            this.SuspendLayout();
            // 
            // btnVolver
            // 
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.Location = new System.Drawing.Point(24, 484);
            this.btnVolver.Margin = new System.Windows.Forms.Padding(2);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(144, 40);
            this.btnVolver.TabIndex = 1;
            this.btnVolver.Text = "Volver";
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 124);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(792, 32);
            this.label2.TabIndex = 28;
            this.label2.Text = "Eventos";
            // 
            // dgveventos
            // 
            this.dgveventos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgveventos.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgveventos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgveventos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgveventos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dgveventos.Location = new System.Drawing.Point(24, 236);
            this.dgveventos.Name = "dgveventos";
            this.dgveventos.RowHeadersWidth = 51;
            this.dgveventos.Size = new System.Drawing.Size(792, 224);
            this.dgveventos.TabIndex = 29;
            // 
            // btnComprar
            // 
            this.btnComprar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnComprar.Location = new System.Drawing.Point(648, 484);
            this.btnComprar.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.btnComprar.Name = "btnComprar";
            this.btnComprar.Size = new System.Drawing.Size(168, 40);
            this.btnComprar.TabIndex = 36;
            this.btnComprar.Text = "Comprar";
            this.btnComprar.Click += new System.EventHandler(this.btnComprar_Click);
            //
            // txtBuscar
            //
            this.txtBuscar.Location = new System.Drawing.Point(24, 180);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(248, 28);
            this.txtBuscar.TabIndex = 2;
            this.txtBuscar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBuscar_KeyDown);
            //
            // cmbCategoria
            //
            this.cmbCategoria.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmbCategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategoria.FormattingEnabled = true;
            this.cmbCategoria.Location = new System.Drawing.Point(280, 180);
            this.cmbCategoria.Name = "cmbCategoria";
            this.cmbCategoria.Size = new System.Drawing.Size(128, 28);
            this.cmbCategoria.TabIndex = 3;
            //
            // dtpDesde
            //
            this.dtpDesde.Checked = false;
            this.dtpDesde.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(416, 180);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.ShowCheckBox = true;
            this.dtpDesde.Size = new System.Drawing.Size(144, 28);
            this.dtpDesde.TabIndex = 4;
            //
            // btnBuscar
            //
            this.btnBuscar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBuscar.Location = new System.Drawing.Point(568, 180);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(112, 32);
            this.btnBuscar.TabIndex = 5;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            //
            // btnLimpiar
            //
            this.btnLimpiar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLimpiar.Location = new System.Drawing.Point(688, 180);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(120, 32);
            this.btnLimpiar.TabIndex = 6;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            //
            // frmEventosCliente
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(840, 548);
            this.Controls.Add(this.btnComprar);
            this.Controls.Add(this.dgveventos);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnVolver);
            this.Controls.Add(this.txtBuscar);
            this.Controls.Add(this.cmbCategoria);
            this.Controls.Add(this.dtpDesde);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.btnLimpiar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmEventosCliente";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EventAura · Eventos disponibles";
            this.Load += new System.EventHandler(this.frmEventosCliente_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgveventos)).EndInit();
            this.barraSuperior = new Visual.Componentes.BarraSuperior();
            this.barraSuperior.Name = "barraSuperior";
            this.barraSuperior.Size = new System.Drawing.Size(840, 60);
            this.barraSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Controls.Add(this.barraSuperior);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Visual.Componentes.IndicadorPasos indicadorPasos;

        private Visual.Componentes.BotonSecundario btnVolver;
        private Visual.Componentes.EtiquetaTitulo label2;
        private System.Windows.Forms.DataGridView dgveventos;
        private Visual.Componentes.BotonPrimario btnComprar;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.ComboBox cmbCategoria;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private Visual.Componentes.BotonPrimario btnBuscar;
        private Visual.Componentes.BotonSecundario btnLimpiar;
        private Visual.Componentes.BarraSuperior barraSuperior;
    }
}