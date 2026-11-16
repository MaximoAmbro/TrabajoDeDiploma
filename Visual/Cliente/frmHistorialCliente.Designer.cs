namespace Visual
{
    partial class frmHistorialCliente
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
            this.dgvCompras = new System.Windows.Forms.DataGridView();
            this.label1 = new Visual.Componentes.EtiquetaTitulo();
            this.lblDetalle = new System.Windows.Forms.Label();
            this.dgvEntradas = new System.Windows.Forms.DataGridView();
            this.btnDevolver = new Visual.Componentes.BotonPrimario();
            this.lblAyuda = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCompras)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntradas)).BeginInit();
            this.SuspendLayout();
            //
            // btnVolver
            //
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.Location = new System.Drawing.Point(24, 448);
            this.btnVolver.Margin = new System.Windows.Forms.Padding(2);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(144, 40);
            this.btnVolver.TabIndex = 3;
            this.btnVolver.Text = "Volver";
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            //
            // dgvCompras
            //
            this.dgvCompras.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCompras.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dgvCompras.Location = new System.Drawing.Point(24, 80);
            this.dgvCompras.Margin = new System.Windows.Forms.Padding(2);
            this.dgvCompras.Name = "dgvCompras";
            this.dgvCompras.RowHeadersWidth = 62;
            this.dgvCompras.Size = new System.Drawing.Size(852, 120);
            this.dgvCompras.TabIndex = 0;
            //
            // label1
            //
            this.label1.AutoSize = false;
            this.label1.Location = new System.Drawing.Point(24, 24);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(852, 32);
            this.label1.TabIndex = 28;
            this.label1.Text = "Historial Compras";
            //
            // lblDetalle
            //
            this.lblDetalle.AutoSize = false;
            this.lblDetalle.Location = new System.Drawing.Point(24, 216);
            this.lblDetalle.Name = "lblDetalle";
            this.lblDetalle.Size = new System.Drawing.Size(852, 24);
            this.lblDetalle.TabIndex = 29;
            this.lblDetalle.Text = "Entradas del pedido";
            //
            // dgvEntradas
            //
            this.dgvEntradas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEntradas.Location = new System.Drawing.Point(24, 248);
            this.dgvEntradas.Margin = new System.Windows.Forms.Padding(2);
            this.dgvEntradas.Name = "dgvEntradas";
            this.dgvEntradas.RowHeadersWidth = 62;
            this.dgvEntradas.Size = new System.Drawing.Size(852, 128);
            this.dgvEntradas.TabIndex = 1;
            //
            // btnDevolver
            //
            this.btnDevolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDevolver.Enabled = false;
            this.btnDevolver.Location = new System.Drawing.Point(680, 448);
            this.btnDevolver.Margin = new System.Windows.Forms.Padding(2);
            this.btnDevolver.Name = "btnDevolver";
            this.btnDevolver.Size = new System.Drawing.Size(196, 40);
            this.btnDevolver.TabIndex = 2;
            this.btnDevolver.Text = "Devolver";
            this.btnDevolver.Click += new System.EventHandler(this.btnDevolver_Click);
            //
            // lblAyuda
            //
            this.lblAyuda.Location = new System.Drawing.Point(24, 392);
            this.lblAyuda.Name = "lblAyuda";
            this.lblAyuda.Size = new System.Drawing.Size(852, 40);
            this.lblAyuda.TabIndex = 30;
            this.lblAyuda.Text = "Con seguro, podés devolver entradas antes del evento.";
            //
            // frmHistorialCliente
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(900, 512);
            this.Controls.Add(this.lblAyuda);
            this.Controls.Add(this.btnDevolver);
            this.Controls.Add(this.dgvEntradas);
            this.Controls.Add(this.lblDetalle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvCompras);
            this.Controls.Add(this.btnVolver);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmHistorialCliente";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EventAura · Mis eventos";
            this.Load += new System.EventHandler(this.frmHistorialCliente_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCompras)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntradas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Visual.Componentes.BotonSecundario btnVolver;
        private System.Windows.Forms.DataGridView dgvCompras;
        private Visual.Componentes.EtiquetaTitulo label1;
        private System.Windows.Forms.Label lblDetalle;
        private System.Windows.Forms.DataGridView dgvEntradas;
        private Visual.Componentes.BotonPrimario btnDevolver;
        private System.Windows.Forms.Label lblAyuda;
    }
}
