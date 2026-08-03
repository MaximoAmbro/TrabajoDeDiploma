namespace Visual.Vendedor
{
    partial class frmAgregarEventoLocal
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
            label6 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            txtNombre = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            lblNombre = new System.Windows.Forms.Label();
            dgvSectores = new System.Windows.Forms.DataGridView();
            btnAgregarSector = new System.Windows.Forms.Button();
            btnEliminarSector = new System.Windows.Forms.Button();
            btnRegistrar = new System.Windows.Forms.Button();
            dtpFechaHora = new System.Windows.Forms.DateTimePicker();
            button1 = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)dgvSectores).BeginInit();
            SuspendLayout();
            // 
            // btnVolver
            // 
            btnVolver.BackColor = System.Drawing.Color.FromArgb(116, 86, 174);
            btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            btnVolver.FlatAppearance.BorderSize = 0;
            btnVolver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnVolver.ForeColor = System.Drawing.Color.White;
            btnVolver.Location = new System.Drawing.Point(36, 390);
            btnVolver.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            btnVolver.Name = "btnVolver";
            btnVolver.Size = new System.Drawing.Size(119, 53);
            btnVolver.TabIndex = 47;
            btnVolver.Text = "VOLVER";
            btnVolver.UseVisualStyleBackColor = false;
            btnVolver.Click += btnVolver_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(37, 100);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(138, 20);
            label6.TabIndex = 44;
            label6.Text = "Nombre del evento";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(37, 165);
            label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(47, 20);
            label3.TabIndex = 39;
            label3.Text = "Fecha";
            // 
            // txtNombre
            // 
            txtNombre.BackColor = System.Drawing.Color.FromArgb(230, 231, 233);
            txtNombre.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtNombre.Font = new System.Drawing.Font("MS Gothic", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtNombre.Location = new System.Drawing.Point(37, 136);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new System.Drawing.Size(252, 27);
            txtNombre.TabIndex = 48;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label1.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            label1.Location = new System.Drawing.Point(23, 38);
            label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(405, 50);
            label1.TabIndex = 38;
            label1.Text = "AGREGAR EVENTO EN";
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblNombre.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            lblNombre.Location = new System.Drawing.Point(425, 38);
            lblNombre.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new System.Drawing.Size(410, 50);
            lblNombre.TabIndex = 52;
            lblNombre.Text = "[NOMBRE DEL LOCAL]";
            // 
            // dgvSectores
            // 
            dgvSectores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSectores.Location = new System.Drawing.Point(312, 136);
            dgvSectores.Name = "dgvSectores";
            dgvSectores.RowHeadersWidth = 51;
            dgvSectores.Size = new System.Drawing.Size(523, 307);
            dgvSectores.TabIndex = 53;
            // 
            // btnAgregarSector
            // 
            btnAgregarSector.BackColor = System.Drawing.Color.FromArgb(116, 86, 174);
            btnAgregarSector.Cursor = System.Windows.Forms.Cursors.Hand;
            btnAgregarSector.FlatAppearance.BorderSize = 0;
            btnAgregarSector.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnAgregarSector.ForeColor = System.Drawing.Color.White;
            btnAgregarSector.Location = new System.Drawing.Point(37, 250);
            btnAgregarSector.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            btnAgregarSector.Name = "btnAgregarSector";
            btnAgregarSector.Size = new System.Drawing.Size(105, 53);
            btnAgregarSector.TabIndex = 54;
            btnAgregarSector.Text = "AGREGAR SECTOR";
            btnAgregarSector.UseVisualStyleBackColor = false;
            // 
            // btnEliminarSector
            // 
            btnEliminarSector.BackColor = System.Drawing.Color.FromArgb(116, 86, 174);
            btnEliminarSector.Cursor = System.Windows.Forms.Cursors.Hand;
            btnEliminarSector.FlatAppearance.BorderSize = 0;
            btnEliminarSector.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnEliminarSector.ForeColor = System.Drawing.Color.White;
            btnEliminarSector.Location = new System.Drawing.Point(37, 309);
            btnEliminarSector.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            btnEliminarSector.Name = "btnEliminarSector";
            btnEliminarSector.Size = new System.Drawing.Size(105, 53);
            btnEliminarSector.TabIndex = 55;
            btnEliminarSector.Text = "ELIMINAR SECTOR";
            btnEliminarSector.UseVisualStyleBackColor = false;
            // 
            // btnRegistrar
            // 
            btnRegistrar.BackColor = System.Drawing.Color.FromArgb(116, 86, 174);
            btnRegistrar.Cursor = System.Windows.Forms.Cursors.Hand;
            btnRegistrar.FlatAppearance.BorderSize = 0;
            btnRegistrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnRegistrar.ForeColor = System.Drawing.Color.White;
            btnRegistrar.Location = new System.Drawing.Point(178, 390);
            btnRegistrar.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            btnRegistrar.Name = "btnRegistrar";
            btnRegistrar.Size = new System.Drawing.Size(111, 53);
            btnRegistrar.TabIndex = 57;
            btnRegistrar.Text = "REGISTRAR";
            btnRegistrar.UseVisualStyleBackColor = false;
            btnRegistrar.Click += btnRegistrar_Click;
            // 
            // dtpFechaHora
            // 
            dtpFechaHora.Location = new System.Drawing.Point(37, 206);
            dtpFechaHora.Name = "dtpFechaHora";
            dtpFechaHora.Size = new System.Drawing.Size(252, 27);
            dtpFechaHora.TabIndex = 58;
            // 
            // button1
            // 
            button1.BackColor = System.Drawing.Color.FromArgb(116, 86, 174);
            button1.Cursor = System.Windows.Forms.Cursors.Hand;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button1.ForeColor = System.Drawing.Color.White;
            button1.Location = new System.Drawing.Point(165, 250);
            button1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(112, 53);
            button1.TabIndex = 59;
            button1.Text = "MODIFICAR SECTOR";
            button1.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label2.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            label2.Location = new System.Drawing.Point(477, 83);
            label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(199, 50);
            label2.TabIndex = 60;
            label2.Text = "SECTORES";
            // 
            // frmAgregarEventoLocal
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(859, 455);
            Controls.Add(label2);
            Controls.Add(button1);
            Controls.Add(dtpFechaHora);
            Controls.Add(btnRegistrar);
            Controls.Add(btnEliminarSector);
            Controls.Add(btnAgregarSector);
            Controls.Add(dgvSectores);
            Controls.Add(lblNombre);
            Controls.Add(btnVolver);
            Controls.Add(label6);
            Controls.Add(label3);
            Controls.Add(txtNombre);
            Controls.Add(label1);
            Name = "frmAgregarEventoLocal";
            Text = "frmVolver";
            Load += frmAgregarEventoLocal_Load;
            ((System.ComponentModel.ISupportInitialize)dgvSectores).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtHora;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.DataGridView dgvSectores;
        private System.Windows.Forms.Button btnAgregarSector;
        private System.Windows.Forms.Button btnEliminarSector;
        private System.Windows.Forms.Button btnRegistrar;
        private System.Windows.Forms.DateTimePicker dtpFechaHora;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
    }
}