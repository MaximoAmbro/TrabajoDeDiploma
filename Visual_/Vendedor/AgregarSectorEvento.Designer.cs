namespace Visual.Vendedor
{
    partial class AgregarSectorEvento
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
            label5 = new System.Windows.Forms.Label();
            txtPrecio = new System.Windows.Forms.TextBox();
            btnregistrarse = new System.Windows.Forms.Button();
            txtCapacidad = new System.Windows.Forms.TextBox();
            lblmail = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            txtNombre = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            lblyatengocuen = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(104, 150);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(50, 20);
            label5.TabIndex = 50;
            label5.Text = "Precio";
            // 
            // txtPrecio
            // 
            txtPrecio.BackColor = System.Drawing.Color.FromArgb(230, 231, 233);
            txtPrecio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtPrecio.Font = new System.Drawing.Font("MS Gothic", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtPrecio.Location = new System.Drawing.Point(104, 186);
            txtPrecio.Name = "txtPrecio";
            txtPrecio.Size = new System.Drawing.Size(252, 27);
            txtPrecio.TabIndex = 51;
            // 
            // btnregistrarse
            // 
            btnregistrarse.BackColor = System.Drawing.Color.FromArgb(116, 86, 174);
            btnregistrarse.Cursor = System.Windows.Forms.Cursors.Hand;
            btnregistrarse.FlatAppearance.BorderSize = 0;
            btnregistrarse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnregistrarse.ForeColor = System.Drawing.Color.White;
            btnregistrarse.Location = new System.Drawing.Point(104, 299);
            btnregistrarse.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            btnregistrarse.Name = "btnregistrarse";
            btnregistrarse.Size = new System.Drawing.Size(252, 53);
            btnregistrarse.TabIndex = 48;
            btnregistrarse.Text = "REGISTRARSE";
            btnregistrarse.UseVisualStyleBackColor = false;
            // 
            // txtCapacidad
            // 
            txtCapacidad.BackColor = System.Drawing.Color.FromArgb(230, 231, 233);
            txtCapacidad.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtCapacidad.Font = new System.Drawing.Font("MS Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtCapacidad.Location = new System.Drawing.Point(104, 266);
            txtCapacidad.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            txtCapacidad.Name = "txtCapacidad";
            txtCapacidad.Size = new System.Drawing.Size(252, 27);
            txtCapacidad.TabIndex = 46;
            // 
            // lblmail
            // 
            lblmail.AutoSize = true;
            lblmail.Location = new System.Drawing.Point(104, 230);
            lblmail.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblmail.Name = "lblmail";
            lblmail.Size = new System.Drawing.Size(80, 20);
            lblmail.TabIndex = 45;
            lblmail.Text = "Capacidad";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(104, 83);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(64, 20);
            label6.TabIndex = 44;
            label6.Text = "Nombre";
            // 
            // txtNombre
            // 
            txtNombre.BackColor = System.Drawing.Color.FromArgb(230, 231, 233);
            txtNombre.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtNombre.Font = new System.Drawing.Font("MS Gothic", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtNombre.Location = new System.Drawing.Point(104, 119);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new System.Drawing.Size(252, 27);
            txtNombre.TabIndex = 49;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label1.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            label1.Location = new System.Drawing.Point(11, 26);
            label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(367, 50);
            label1.TabIndex = 38;
            label1.Text = "REGISTRAR SECTOR";
            // 
            // lblyatengocuen
            // 
            lblyatengocuen.AutoSize = true;
            lblyatengocuen.Cursor = System.Windows.Forms.Cursors.Hand;
            lblyatengocuen.Font = new System.Drawing.Font("Nirmala UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblyatengocuen.Location = new System.Drawing.Point(209, 368);
            lblyatengocuen.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblyatengocuen.Name = "lblyatengocuen";
            lblyatengocuen.Size = new System.Drawing.Size(53, 20);
            lblyatengocuen.TabIndex = 47;
            lblyatengocuen.Text = "Volver";
            // 
            // AgregarSectorEvento
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(459, 433);
            Controls.Add(label5);
            Controls.Add(txtPrecio);
            Controls.Add(btnregistrarse);
            Controls.Add(lblyatengocuen);
            Controls.Add(txtCapacidad);
            Controls.Add(lblmail);
            Controls.Add(label6);
            Controls.Add(txtNombre);
            Controls.Add(label1);
            Name = "AgregarSectorEvento";
            Text = "AgregarSectorEvento";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPrecio;
        private System.Windows.Forms.Button btnregistrarse;
        private System.Windows.Forms.TextBox txtCapacidad;
        private System.Windows.Forms.Label lblmail;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblyatengocuen;
    }
}