namespace Visual
{
    partial class frmMisLocales
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
            this.dgvLocales = new System.Windows.Forms.DataGridView();
            this.label2 = new Visual.Componentes.EtiquetaTitulo();
            this.btnVolver = new Visual.Componentes.BotonSecundario();
            this.btnEventosLocal = new Visual.Componentes.BotonPrimario();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLocales)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvLocales
            // 
            this.dgvLocales.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvLocales.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvLocales.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvLocales.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLocales.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dgvLocales.Location = new System.Drawing.Point(24, 148);
            this.dgvLocales.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvLocales.Name = "dgvLocales";
            this.dgvLocales.RowHeadersWidth = 51;
            this.dgvLocales.Size = new System.Drawing.Size(792, 264);
            this.dgvLocales.TabIndex = 39;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 84);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(792, 32);
            this.label2.TabIndex = 38;
            this.label2.Text = "Locales";
            // 
            // btnVolver
            // 
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.Location = new System.Drawing.Point(24, 444);
            this.btnVolver.Margin = new System.Windows.Forms.Padding(2);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(144, 40);
            this.btnVolver.TabIndex = 37;
            this.btnVolver.Text = "Volver";
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            // 
            // btnEventosLocal
            // 
            this.btnEventosLocal.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEventosLocal.Location = new System.Drawing.Point(624, 444);
            this.btnEventosLocal.Margin = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.btnEventosLocal.Name = "btnEventosLocal";
            this.btnEventosLocal.Size = new System.Drawing.Size(192, 40);
            this.btnEventosLocal.TabIndex = 41;
            this.btnEventosLocal.Text = "Ver eventos";
            this.btnEventosLocal.Click += new System.EventHandler(this.btnEventosLocal_Click);
            // 
            // frmMisLocales
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(840, 508);
            this.Controls.Add(this.btnEventosLocal);
            this.Controls.Add(this.dgvLocales);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnVolver);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmMisLocales";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EventAura · Mis locales";
            this.Load += new System.EventHandler(this.frmMisLocales_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLocales)).EndInit();
            this.barraSuperior = new Visual.Componentes.BarraSuperior();
            this.barraSuperior.Name = "barraSuperior";
            this.barraSuperior.Size = new System.Drawing.Size(840, 60);
            this.barraSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Controls.Add(this.barraSuperior);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvLocales;
        private Visual.Componentes.EtiquetaTitulo label2;
        private Visual.Componentes.BotonSecundario btnVolver;
        private Visual.Componentes.BotonPrimario btnEventosLocal;
        private Visual.Componentes.BarraSuperior barraSuperior;
    }
}