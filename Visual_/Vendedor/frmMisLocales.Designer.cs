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
            dgvLocales = new System.Windows.Forms.DataGridView();
            label2 = new System.Windows.Forms.Label();
            btnVolver = new System.Windows.Forms.Button();
            btnEventosLocal = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)dgvLocales).BeginInit();
            SuspendLayout();
            // 
            // dgvLocales
            // 
            dgvLocales.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLocales.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dgvLocales.BackgroundColor = System.Drawing.SystemColors.HighlightText;
            dgvLocales.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dgvLocales.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLocales.Cursor = System.Windows.Forms.Cursors.Hand;
            dgvLocales.Location = new System.Drawing.Point(8, 109);
            dgvLocales.Name = "dgvLocales";
            dgvLocales.RowHeadersWidth = 51;
            dgvLocales.Size = new System.Drawing.Size(876, 233);
            dgvLocales.TabIndex = 39;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Nirmala UI", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label2.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            label2.Location = new System.Drawing.Point(249, 10);
            label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(222, 62);
            label2.TabIndex = 38;
            label2.Text = "LOCALES";
            // 
            // btnVolver
            // 
            btnVolver.BackColor = System.Drawing.Color.White;
            btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            btnVolver.ForeColor = System.Drawing.Color.FromArgb(164, 165, 169);
            btnVolver.Location = new System.Drawing.Point(326, 395);
            btnVolver.Margin = new System.Windows.Forms.Padding(2);
            btnVolver.Name = "btnVolver";
            btnVolver.Size = new System.Drawing.Size(121, 33);
            btnVolver.TabIndex = 37;
            btnVolver.Text = "Volver";
            btnVolver.UseVisualStyleBackColor = false;
            btnVolver.Click += btnVolver_Click;
            // 
            // btnEventosLocal
            // 
            btnEventosLocal.BackColor = System.Drawing.Color.FromArgb(116, 86, 174);
            btnEventosLocal.Cursor = System.Windows.Forms.Cursors.Hand;
            btnEventosLocal.FlatAppearance.BorderSize = 0;
            btnEventosLocal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnEventosLocal.ForeColor = System.Drawing.Color.White;
            btnEventosLocal.Location = new System.Drawing.Point(279, 348);
            btnEventosLocal.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            btnEventosLocal.Name = "btnEventosLocal";
            btnEventosLocal.Size = new System.Drawing.Size(215, 42);
            btnEventosLocal.TabIndex = 41;
            btnEventosLocal.Text = "Ver eventos";
            btnEventosLocal.UseVisualStyleBackColor = false;
            btnEventosLocal.Click += btnEventosLocal_Click;
            // 
            // frmMisLocales
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(893, 438);
            Controls.Add(btnEventosLocal);
            Controls.Add(dgvLocales);
            Controls.Add(label2);
            Controls.Add(btnVolver);
            Name = "frmMisLocales";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "MisLocales";
            Load += frmMisLocales_Load;
            ((System.ComponentModel.ISupportInitialize)dgvLocales).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvLocales;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Button btnEventosLocal;
    }
}