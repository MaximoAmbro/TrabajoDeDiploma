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
            btnEliminarEvento = new System.Windows.Forms.Button();
            dgvEventos = new System.Windows.Forms.DataGridView();
            label2 = new System.Windows.Forms.Label();
            btnVolver = new System.Windows.Forms.Button();
            lblNombre = new System.Windows.Forms.Label();
            btnAgregarEvento = new System.Windows.Forms.Button();
            btnModificarEvento = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)dgvEventos).BeginInit();
            SuspendLayout();
            // 
            // btnEliminarEvento
            // 
            btnEliminarEvento.BackColor = System.Drawing.Color.FromArgb(116, 86, 174);
            btnEliminarEvento.Cursor = System.Windows.Forms.Cursors.Hand;
            btnEliminarEvento.FlatAppearance.BorderSize = 0;
            btnEliminarEvento.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnEliminarEvento.ForeColor = System.Drawing.Color.White;
            btnEliminarEvento.Location = new System.Drawing.Point(50, 354);
            btnEliminarEvento.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            btnEliminarEvento.Name = "btnEliminarEvento";
            btnEliminarEvento.Size = new System.Drawing.Size(215, 42);
            btnEliminarEvento.TabIndex = 45;
            btnEliminarEvento.Text = "Eliminar evento";
            btnEliminarEvento.UseVisualStyleBackColor = false;
            btnEliminarEvento.Click += btnEliminarEvento_Click;
            // 
            // dgvEventos
            // 
            dgvEventos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dgvEventos.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dgvEventos.BackgroundColor = System.Drawing.SystemColors.HighlightText;
            dgvEventos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dgvEventos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEventos.Cursor = System.Windows.Forms.Cursors.Hand;
            dgvEventos.Location = new System.Drawing.Point(12, 115);
            dgvEventos.Name = "dgvEventos";
            dgvEventos.RowHeadersWidth = 51;
            dgvEventos.Size = new System.Drawing.Size(876, 233);
            dgvEventos.TabIndex = 44;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Nirmala UI", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label2.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            label2.Location = new System.Drawing.Point(32, 9);
            label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(233, 62);
            label2.TabIndex = 43;
            label2.Text = "EVENTOS";
            // 
            // btnVolver
            // 
            btnVolver.BackColor = System.Drawing.Color.White;
            btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            btnVolver.ForeColor = System.Drawing.Color.FromArgb(164, 165, 169);
            btnVolver.Location = new System.Drawing.Point(394, 401);
            btnVolver.Margin = new System.Windows.Forms.Padding(2);
            btnVolver.Name = "btnVolver";
            btnVolver.Size = new System.Drawing.Size(121, 33);
            btnVolver.TabIndex = 42;
            btnVolver.Text = "Volver";
            btnVolver.UseVisualStyleBackColor = false;
            btnVolver.Click += btnVolver_Click;
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Font = new System.Drawing.Font("Nirmala UI", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblNombre.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            lblNombre.Location = new System.Drawing.Point(294, 9);
            lblNombre.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new System.Drawing.Size(358, 62);
            lblNombre.TabIndex = 46;
            lblNombre.Text = "(Nombre local)";
            // 
            // btnAgregarEvento
            // 
            btnAgregarEvento.BackColor = System.Drawing.Color.FromArgb(116, 86, 174);
            btnAgregarEvento.Cursor = System.Windows.Forms.Cursors.Hand;
            btnAgregarEvento.FlatAppearance.BorderSize = 0;
            btnAgregarEvento.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnAgregarEvento.ForeColor = System.Drawing.Color.White;
            btnAgregarEvento.Location = new System.Drawing.Point(348, 354);
            btnAgregarEvento.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            btnAgregarEvento.Name = "btnAgregarEvento";
            btnAgregarEvento.Size = new System.Drawing.Size(215, 42);
            btnAgregarEvento.TabIndex = 47;
            btnAgregarEvento.Text = "Agregar evento";
            btnAgregarEvento.UseVisualStyleBackColor = false;
            btnAgregarEvento.Click += btnAgregarEvento_Click;
            // 
            // btnModificarEvento
            // 
            btnModificarEvento.BackColor = System.Drawing.Color.FromArgb(116, 86, 174);
            btnModificarEvento.Cursor = System.Windows.Forms.Cursors.Hand;
            btnModificarEvento.FlatAppearance.BorderSize = 0;
            btnModificarEvento.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnModificarEvento.ForeColor = System.Drawing.Color.White;
            btnModificarEvento.Location = new System.Drawing.Point(622, 354);
            btnModificarEvento.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            btnModificarEvento.Name = "btnModificarEvento";
            btnModificarEvento.Size = new System.Drawing.Size(215, 42);
            btnModificarEvento.TabIndex = 48;
            btnModificarEvento.Text = "Modificar evento";
            btnModificarEvento.UseVisualStyleBackColor = false;
            // 
            // frmEventosLocal
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(893, 438);
            Controls.Add(btnModificarEvento);
            Controls.Add(btnAgregarEvento);
            Controls.Add(lblNombre);
            Controls.Add(btnEliminarEvento);
            Controls.Add(dgvEventos);
            Controls.Add(label2);
            Controls.Add(btnVolver);
            Name = "frmEventosLocal";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "frmRegistrarEvento";
            Load += frmRegistrarEvento_Load;
            ((System.ComponentModel.ISupportInitialize)dgvEventos).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnEliminarEvento;
        private System.Windows.Forms.DataGridView dgvEventos;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Button btnAgregarEvento;
        private System.Windows.Forms.Button btnModificarEvento;
    }
}