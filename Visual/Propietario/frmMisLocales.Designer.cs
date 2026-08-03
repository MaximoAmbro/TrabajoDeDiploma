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
            this.label2 = new System.Windows.Forms.Label();
            this.btnVolver = new System.Windows.Forms.Button();
            this.btnEventosLocal = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLocales)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvLocales
            // 
            this.dgvLocales.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvLocales.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvLocales.BackgroundColor = System.Drawing.SystemColors.HighlightText;
            this.dgvLocales.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvLocales.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLocales.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dgvLocales.Location = new System.Drawing.Point(24, 112);
            this.dgvLocales.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvLocales.Name = "dgvLocales";
            this.dgvLocales.RowHeadersWidth = 51;
            this.dgvLocales.Size = new System.Drawing.Size(986, 254);
            this.dgvLocales.TabIndex = 39;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Nirmala UI", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.label2.Location = new System.Drawing.Point(322, 20);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(223, 74);
            this.label2.TabIndex = 38;
            this.label2.Text = "Locales";
            // 
            // btnVolver
            // 
            this.btnVolver.BackColor = System.Drawing.Color.White;
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.Font = new System.Drawing.Font("Nirmala UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVolver.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(165)))), ((int)(((byte)(169)))));
            this.btnVolver.Location = new System.Drawing.Point(11, 452);
            this.btnVolver.Margin = new System.Windows.Forms.Padding(2);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(129, 31);
            this.btnVolver.TabIndex = 37;
            this.btnVolver.Text = "Volver";
            this.btnVolver.UseVisualStyleBackColor = false;
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            // 
            // btnEventosLocal
            // 
            this.btnEventosLocal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.btnEventosLocal.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEventosLocal.FlatAppearance.BorderSize = 0;
            this.btnEventosLocal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEventosLocal.Font = new System.Drawing.Font("Nirmala UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEventosLocal.ForeColor = System.Drawing.Color.White;
            this.btnEventosLocal.Location = new System.Drawing.Point(319, 400);
            this.btnEventosLocal.Margin = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.btnEventosLocal.Name = "btnEventosLocal";
            this.btnEventosLocal.Size = new System.Drawing.Size(242, 48);
            this.btnEventosLocal.TabIndex = 41;
            this.btnEventosLocal.Text = "Ver eventos";
            this.btnEventosLocal.UseVisualStyleBackColor = false;
            this.btnEventosLocal.Click += new System.EventHandler(this.btnEventosLocal_Click);
            // 
            // frmMisLocales
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(928, 494);
            this.Controls.Add(this.btnEventosLocal);
            this.Controls.Add(this.dgvLocales);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnVolver);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmMisLocales";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MisLocales";
            this.Load += new System.EventHandler(this.frmMisLocales_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLocales)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvLocales;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Button btnEventosLocal;
    }
}