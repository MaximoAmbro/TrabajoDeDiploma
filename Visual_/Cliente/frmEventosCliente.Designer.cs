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
            btnVolver = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            dgveventos = new System.Windows.Forms.DataGridView();
            btnComprar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)dgveventos).BeginInit();
            SuspendLayout();
            // 
            // btnVolver
            // 
            btnVolver.BackColor = System.Drawing.Color.White;
            btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            btnVolver.ForeColor = System.Drawing.Color.FromArgb(164, 165, 169);
            btnVolver.Location = new System.Drawing.Point(385, 394);
            btnVolver.Margin = new System.Windows.Forms.Padding(2);
            btnVolver.Name = "btnVolver";
            btnVolver.Size = new System.Drawing.Size(121, 33);
            btnVolver.TabIndex = 1;
            btnVolver.Text = "Volver";
            btnVolver.UseVisualStyleBackColor = false;
            btnVolver.Click += btnVolver_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Nirmala UI", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label2.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            label2.Location = new System.Drawing.Point(308, 9);
            label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(276, 74);
            label2.TabIndex = 28;
            label2.Text = "EVENTOS";
            // 
            // dgveventos
            // 
            dgveventos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dgveventos.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dgveventos.BackgroundColor = System.Drawing.SystemColors.HighlightText;
            dgveventos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dgveventos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgveventos.Cursor = System.Windows.Forms.Cursors.Hand;
            dgveventos.Location = new System.Drawing.Point(67, 108);
            dgveventos.Name = "dgveventos";
            dgveventos.RowHeadersWidth = 51;
            dgveventos.Size = new System.Drawing.Size(876, 233);
            dgveventos.TabIndex = 29;
            // 
            // btnComprar
            // 
            btnComprar.BackColor = System.Drawing.Color.FromArgb(116, 86, 174);
            btnComprar.Cursor = System.Windows.Forms.Cursors.Hand;
            btnComprar.FlatAppearance.BorderSize = 0;
            btnComprar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnComprar.ForeColor = System.Drawing.Color.White;
            btnComprar.Location = new System.Drawing.Point(342, 347);
            btnComprar.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            btnComprar.Name = "btnComprar";
            btnComprar.Size = new System.Drawing.Size(215, 42);
            btnComprar.TabIndex = 36;
            btnComprar.Text = "COMPRAR";
            btnComprar.UseVisualStyleBackColor = false;
            btnComprar.Click += btnComprar_Click_1;
            // 
            // frmEventos
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(893, 438);
            Controls.Add(btnComprar);
            Controls.Add(dgveventos);
            Controls.Add(label2);
            Controls.Add(btnVolver);
            Font = new System.Drawing.Font("Nirmala UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            ForeColor = System.Drawing.Color.FromArgb(164, 165, 169);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(2);
            Name = "frmEventos";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "frmEventos";
            Load += frmEventos_Load;
            ((System.ComponentModel.ISupportInitialize)dgveventos).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dgveventos;
        private System.Windows.Forms.Button btnComprar;
    }
}