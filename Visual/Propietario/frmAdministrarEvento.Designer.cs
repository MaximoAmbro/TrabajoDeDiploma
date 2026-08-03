namespace Visual.Vendedor
{
    partial class frmAdministrarEvento
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
            this.btnVolver = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblEvento = new System.Windows.Forms.Label();
            this.dgvSectores = new System.Windows.Forms.DataGridView();
            this.btnAgregarSector = new System.Windows.Forms.Button();
            this.btnEliminarSector = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSectores)).BeginInit();
            this.SuspendLayout();
            // 
            // btnVolver
            // 
            this.btnVolver.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.FlatAppearance.BorderSize = 0;
            this.btnVolver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolver.Font = new System.Drawing.Font("Nirmala UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVolver.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(165)))), ((int)(((byte)(169)))));
            this.btnVolver.Location = new System.Drawing.Point(12, 374);
            this.btnVolver.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(112, 28);
            this.btnVolver.TabIndex = 47;
            this.btnVolver.Text = "Volver";
            this.btnVolver.UseVisualStyleBackColor = false;
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.label1.Location = new System.Drawing.Point(14, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(396, 50);
            this.label1.TabIndex = 38;
            this.label1.Text = "Modificar sectores en";
            // 
            // lblEvento
            // 
            this.lblEvento.AutoSize = true;
            this.lblEvento.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEvento.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.lblEvento.Location = new System.Drawing.Point(420, 15);
            this.lblEvento.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblEvento.Name = "lblEvento";
            this.lblEvento.Size = new System.Drawing.Size(169, 50);
            this.lblEvento.TabIndex = 52;
            this.lblEvento.Text = "[Evento]";
            // 
            // dgvSectores
            // 
            this.dgvSectores.BackgroundColor = System.Drawing.Color.White;
            this.dgvSectores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSectores.GridColor = System.Drawing.Color.White;
            this.dgvSectores.Location = new System.Drawing.Point(290, 146);
            this.dgvSectores.Margin = new System.Windows.Forms.Padding(2);
            this.dgvSectores.Name = "dgvSectores";
            this.dgvSectores.RowHeadersWidth = 51;
            this.dgvSectores.Size = new System.Drawing.Size(490, 257);
            this.dgvSectores.TabIndex = 53;
            // 
            // btnAgregarSector
            // 
            this.btnAgregarSector.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.btnAgregarSector.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAgregarSector.FlatAppearance.BorderSize = 0;
            this.btnAgregarSector.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAgregarSector.Font = new System.Drawing.Font("Nirmala UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAgregarSector.ForeColor = System.Drawing.Color.White;
            this.btnAgregarSector.Location = new System.Drawing.Point(57, 184);
            this.btnAgregarSector.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.btnAgregarSector.Name = "btnAgregarSector";
            this.btnAgregarSector.Size = new System.Drawing.Size(162, 64);
            this.btnAgregarSector.TabIndex = 54;
            this.btnAgregarSector.Text = "Agregar sector";
            this.btnAgregarSector.UseVisualStyleBackColor = false;
            this.btnAgregarSector.Click += new System.EventHandler(this.btnAgregarSector_Click);
            // 
            // btnEliminarSector
            // 
            this.btnEliminarSector.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.btnEliminarSector.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminarSector.FlatAppearance.BorderSize = 0;
            this.btnEliminarSector.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminarSector.Font = new System.Drawing.Font("Nirmala UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminarSector.ForeColor = System.Drawing.Color.White;
            this.btnEliminarSector.Location = new System.Drawing.Point(57, 252);
            this.btnEliminarSector.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.btnEliminarSector.Name = "btnEliminarSector";
            this.btnEliminarSector.Size = new System.Drawing.Size(162, 71);
            this.btnEliminarSector.TabIndex = 55;
            this.btnEliminarSector.Text = "Eliminar sector";
            this.btnEliminarSector.UseVisualStyleBackColor = false;
            this.btnEliminarSector.Click += new System.EventHandler(this.btnEliminarSector_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.label2.Location = new System.Drawing.Point(436, 90);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(169, 50);
            this.label2.TabIndex = 60;
            this.label2.Text = "Sectores";
            // 
            // frmModificarSectoresEventos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(773, 412);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnEliminarSector);
            this.Controls.Add(this.btnAgregarSector);
            this.Controls.Add(this.dgvSectores);
            this.Controls.Add(this.lblEvento);
            this.Controls.Add(this.btnVolver);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmModificarSectoresEventos";
            this.Text = "frmVolver";
            this.Load += new System.EventHandler(this.frmAgregarEventoLocal_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSectores)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtHora;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblEvento;
        private System.Windows.Forms.DataGridView dgvSectores;
        private System.Windows.Forms.Button btnAgregarSector;
        private System.Windows.Forms.Button btnEliminarSector;
        private System.Windows.Forms.Label label2;
    }
}