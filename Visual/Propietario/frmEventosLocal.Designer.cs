namespace Visual.Vendedor
{
    partial class frmEventosLocal
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
            this.btnEliminarEvento = new Visual.Componentes.BotonPrimario();
            this.dgvEventos = new System.Windows.Forms.DataGridView();
            this.label2 = new Visual.Componentes.EtiquetaTitulo();
            this.btnVolver = new Visual.Componentes.BotonSecundario();
            this.lblNombre = new Visual.Componentes.EtiquetaTitulo();
            this.btnAdministrarEvento = new Visual.Componentes.BotonPrimario();
            this.btnAgregarEvento = new Visual.Componentes.BotonPrimario();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEventos)).BeginInit();
            this.SuspendLayout();
            // 
            // btnEliminarEvento
            // 
            this.btnEliminarEvento.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminarEvento.Location = new System.Drawing.Point(16, 306);
            this.btnEliminarEvento.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.btnEliminarEvento.Name = "btnEliminarEvento";
            this.btnEliminarEvento.Size = new System.Drawing.Size(202, 35);
            this.btnEliminarEvento.TabIndex = 45;
            this.btnEliminarEvento.Text = "Eliminar evento";
            this.btnEliminarEvento.Click += new System.EventHandler(this.btnEliminarEvento_Click);
            // 
            // dgvEventos
            // 
            this.dgvEventos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvEventos.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvEventos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvEventos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEventos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dgvEventos.Location = new System.Drawing.Point(23, 71);
            this.dgvEventos.Margin = new System.Windows.Forms.Padding(2);
            this.dgvEventos.Name = "dgvEventos";
            this.dgvEventos.RowHeadersWidth = 51;
            this.dgvEventos.Size = new System.Drawing.Size(822, 193);
            this.dgvEventos.TabIndex = 44;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(85, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(198, 62);
            this.label2.TabIndex = 43;
            this.label2.Text = "Eventos";
            // 
            // btnVolver
            // 
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.Location = new System.Drawing.Point(9, 375);
            this.btnVolver.Margin = new System.Windows.Forms.Padding(2);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(113, 28);
            this.btnVolver.TabIndex = 42;
            this.btnVolver.Text = "Volver";
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(292, 0);
            this.lblNombre.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(358, 62);
            this.lblNombre.TabIndex = 46;
            this.lblNombre.Text = "(Nombre local)";
            // 
            // btnAdministrarEvento
            // 
            this.btnAdministrarEvento.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdministrarEvento.Location = new System.Drawing.Point(290, 306);
            this.btnAdministrarEvento.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.btnAdministrarEvento.Name = "btnAdministrarEvento";
            this.btnAdministrarEvento.Size = new System.Drawing.Size(208, 35);
            this.btnAdministrarEvento.TabIndex = 47;
            this.btnAdministrarEvento.Text = "Administrar Evento";
            this.btnAdministrarEvento.Click += new System.EventHandler(this.btnAdministrarEvento_Click);
            // 
            // btnAgregarEvento
            // 
            this.btnAgregarEvento.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAgregarEvento.Location = new System.Drawing.Point(552, 306);
            this.btnAgregarEvento.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.btnAgregarEvento.Name = "btnAgregarEvento";
            this.btnAgregarEvento.Size = new System.Drawing.Size(202, 35);
            this.btnAgregarEvento.TabIndex = 48;
            this.btnAgregarEvento.Text = "Agregar Evento";
            this.btnAgregarEvento.Click += new System.EventHandler(this.btnAgregarEvento_Click);
            // 
            // frmEventosLocal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(773, 412);
            this.Controls.Add(this.btnAgregarEvento);
            this.Controls.Add(this.btnAdministrarEvento);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.btnEliminarEvento);
            this.Controls.Add(this.dgvEventos);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnVolver);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmEventosLocal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EventAura · Eventos del local";
            this.Load += new System.EventHandler(this.frmEventosLocal_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEventos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Visual.Componentes.BotonPrimario btnEliminarEvento;
        private System.Windows.Forms.DataGridView dgvEventos;
        private Visual.Componentes.EtiquetaTitulo label2;
        private Visual.Componentes.BotonSecundario btnVolver;
        private Visual.Componentes.EtiquetaTitulo lblNombre;
        private Visual.Componentes.BotonPrimario btnAdministrarEvento;
        private Visual.Componentes.BotonPrimario btnAgregarEvento;
    }
}