namespace Visual
{
    partial class frmRegistrarLocal
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
            txtApellido = new System.Windows.Forms.TextBox();
            btnregistrarse = new System.Windows.Forms.Button();
            lblyatengocuen = new System.Windows.Forms.Label();
            txtmail = new System.Windows.Forms.TextBox();
            lblmail = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            TxtPassword = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            txtNombre = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(107, 154);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(72, 20);
            label5.TabIndex = 49;
            label5.Text = "Direccion";
            // 
            // txtApellido
            // 
            txtApellido.BackColor = System.Drawing.Color.FromArgb(230, 231, 233);
            txtApellido.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtApellido.Font = new System.Drawing.Font("MS Gothic", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtApellido.Location = new System.Drawing.Point(107, 190);
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new System.Drawing.Size(252, 27);
            txtApellido.TabIndex = 50;
            // 
            // btnregistrarse
            // 
            btnregistrarse.BackColor = System.Drawing.Color.FromArgb(116, 86, 174);
            btnregistrarse.Cursor = System.Windows.Forms.Cursors.Hand;
            btnregistrarse.FlatAppearance.BorderSize = 0;
            btnregistrarse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnregistrarse.ForeColor = System.Drawing.Color.White;
            btnregistrarse.Location = new System.Drawing.Point(107, 380);
            btnregistrarse.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            btnregistrarse.Name = "btnregistrarse";
            btnregistrarse.Size = new System.Drawing.Size(252, 53);
            btnregistrarse.TabIndex = 47;
            btnregistrarse.Text = "REGISTRARSE";
            btnregistrarse.UseVisualStyleBackColor = false;
            // 
            // lblyatengocuen
            // 
            lblyatengocuen.AutoSize = true;
            lblyatengocuen.Cursor = System.Windows.Forms.Cursors.Hand;
            lblyatengocuen.Font = new System.Drawing.Font("Nirmala UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblyatengocuen.Location = new System.Drawing.Point(207, 464);
            lblyatengocuen.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblyatengocuen.Name = "lblyatengocuen";
            lblyatengocuen.Size = new System.Drawing.Size(53, 20);
            lblyatengocuen.TabIndex = 46;
            lblyatengocuen.Text = "Volver";
            lblyatengocuen.Click += lblyatengocuen_Click;
            // 
            // txtmail
            // 
            txtmail.BackColor = System.Drawing.Color.FromArgb(230, 231, 233);
            txtmail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtmail.Font = new System.Drawing.Font("MS Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtmail.Location = new System.Drawing.Point(107, 270);
            txtmail.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            txtmail.Name = "txtmail";
            txtmail.Size = new System.Drawing.Size(252, 27);
            txtmail.TabIndex = 45;
            // 
            // lblmail
            // 
            lblmail.AutoSize = true;
            lblmail.Location = new System.Drawing.Point(107, 234);
            lblmail.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblmail.Name = "lblmail";
            lblmail.Size = new System.Drawing.Size(49, 20);
            lblmail.TabIndex = 44;
            lblmail.Text = "Altura";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(107, 87);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(64, 20);
            label6.TabIndex = 43;
            label6.Text = "Nombre";
            // 
            // TxtPassword
            // 
            TxtPassword.BackColor = System.Drawing.Color.FromArgb(230, 231, 233);
            TxtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            TxtPassword.Font = new System.Drawing.Font("MS Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            TxtPassword.Location = new System.Drawing.Point(107, 347);
            TxtPassword.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            TxtPassword.Name = "TxtPassword";
            TxtPassword.PasswordChar = '*';
            TxtPassword.Size = new System.Drawing.Size(252, 27);
            TxtPassword.TabIndex = 39;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(107, 312);
            label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(80, 20);
            label3.TabIndex = 38;
            label3.Text = "Capacidad";
            // 
            // txtNombre
            // 
            txtNombre.BackColor = System.Drawing.Color.FromArgb(230, 231, 233);
            txtNombre.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtNombre.Font = new System.Drawing.Font("MS Gothic", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtNombre.Location = new System.Drawing.Point(107, 123);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new System.Drawing.Size(252, 27);
            txtNombre.TabIndex = 48;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label1.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            label1.Location = new System.Drawing.Point(65, 37);
            label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(345, 50);
            label1.TabIndex = 37;
            label1.Text = "REGISTRAR LOCAL";
            // 
            // frmRegistrarLocal
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(459, 529);
            Controls.Add(label5);
            Controls.Add(txtApellido);
            Controls.Add(btnregistrarse);
            Controls.Add(lblyatengocuen);
            Controls.Add(txtmail);
            Controls.Add(lblmail);
            Controls.Add(label6);
            Controls.Add(TxtPassword);
            Controls.Add(label3);
            Controls.Add(txtNombre);
            Controls.Add(label1);
            Name = "frmRegistrarLocal";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "RegistrarLocal";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtApellido;
        private System.Windows.Forms.Button btnregistrarse;
        private System.Windows.Forms.Label lblyatengocuen;
        private System.Windows.Forms.TextBox txtmail;
        private System.Windows.Forms.Label lblmail;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TxtPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label label1;
    }
}