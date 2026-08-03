namespace Visual
{
    partial class frmCrearUsuario
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
            checkbxShowPass = new System.Windows.Forms.CheckBox();
            TxtConfirmpassword = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            TxtPassword = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            txtNombre = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            lblmail = new System.Windows.Forms.Label();
            txtmail = new System.Windows.Forms.TextBox();
            lblyatengocuen = new System.Windows.Forms.Label();
            btnregistrarse = new System.Windows.Forms.Button();
            label5 = new System.Windows.Forms.Label();
            txtApellido = new System.Windows.Forms.TextBox();
            cbTipoUsuario = new System.Windows.Forms.ComboBox();
            SuspendLayout();
            // 
            // checkbxShowPass
            // 
            checkbxShowPass.AutoSize = true;
            checkbxShowPass.Cursor = System.Windows.Forms.Cursors.Hand;
            checkbxShowPass.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            checkbxShowPass.Location = new System.Drawing.Point(314, 492);
            checkbxShowPass.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            checkbxShowPass.Name = "checkbxShowPass";
            checkbxShowPass.Size = new System.Drawing.Size(131, 23);
            checkbxShowPass.TabIndex = 23;
            checkbxShowPass.Text = "Show Password";
            checkbxShowPass.UseVisualStyleBackColor = true;
            checkbxShowPass.CheckedChanged += checkbxShowPass_CheckedChanged;
            // 
            // TxtConfirmpassword
            // 
            TxtConfirmpassword.BackColor = System.Drawing.Color.FromArgb(230, 231, 233);
            TxtConfirmpassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            TxtConfirmpassword.Font = new System.Drawing.Font("MS Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            TxtConfirmpassword.Location = new System.Drawing.Point(98, 441);
            TxtConfirmpassword.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            TxtConfirmpassword.Name = "TxtConfirmpassword";
            TxtConfirmpassword.Size = new System.Drawing.Size(252, 27);
            TxtConfirmpassword.TabIndex = 22;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(98, 405);
            label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(156, 19);
            label4.TabIndex = 21;
            label4.Text = "Confirmar Contraseña";
            // 
            // TxtPassword
            // 
            TxtPassword.BackColor = System.Drawing.Color.FromArgb(230, 231, 233);
            TxtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            TxtPassword.Font = new System.Drawing.Font("MS Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            TxtPassword.Location = new System.Drawing.Point(98, 347);
            TxtPassword.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            TxtPassword.Name = "TxtPassword";
            TxtPassword.PasswordChar = '*';
            TxtPassword.Size = new System.Drawing.Size(252, 27);
            TxtPassword.TabIndex = 20;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(98, 312);
            label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(84, 19);
            label3.TabIndex = 19;
            label3.Text = "Contraseña";
            // 
            // txtNombre
            // 
            txtNombre.BackColor = System.Drawing.Color.FromArgb(230, 231, 233);
            txtNombre.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtNombre.Font = new System.Drawing.Font("MS Gothic", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtNombre.Location = new System.Drawing.Point(98, 123);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new System.Drawing.Size(252, 27);
            txtNombre.TabIndex = 34;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label1.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            label1.Location = new System.Drawing.Point(5, 30);
            label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(323, 50);
            label1.TabIndex = 16;
            label1.Text = "CREAR USUARIO ";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(98, 87);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(65, 19);
            label6.TabIndex = 27;
            label6.Text = "Nombre";
            // 
            // lblmail
            // 
            lblmail.AutoSize = true;
            lblmail.Location = new System.Drawing.Point(98, 234);
            lblmail.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblmail.Name = "lblmail";
            lblmail.Size = new System.Drawing.Size(38, 19);
            lblmail.TabIndex = 29;
            lblmail.Text = "Mail";
            // 
            // txtmail
            // 
            txtmail.BackColor = System.Drawing.Color.FromArgb(230, 231, 233);
            txtmail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtmail.Font = new System.Drawing.Font("MS Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtmail.Location = new System.Drawing.Point(98, 270);
            txtmail.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            txtmail.Name = "txtmail";
            txtmail.Size = new System.Drawing.Size(252, 27);
            txtmail.TabIndex = 30;
            // 
            // lblyatengocuen
            // 
            lblyatengocuen.AutoSize = true;
            lblyatengocuen.Cursor = System.Windows.Forms.Cursors.Hand;
            lblyatengocuen.Font = new System.Drawing.Font("Nirmala UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblyatengocuen.Location = new System.Drawing.Point(163, 628);
            lblyatengocuen.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblyatengocuen.Name = "lblyatengocuen";
            lblyatengocuen.Size = new System.Drawing.Size(121, 20);
            lblyatengocuen.TabIndex = 31;
            lblyatengocuen.Text = "Ya tengo cuenta";
            lblyatengocuen.Click += lblyatengocuen_Click;
            // 
            // btnregistrarse
            // 
            btnregistrarse.BackColor = System.Drawing.Color.FromArgb(116, 86, 174);
            btnregistrarse.Cursor = System.Windows.Forms.Cursors.Hand;
            btnregistrarse.FlatAppearance.BorderSize = 0;
            btnregistrarse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnregistrarse.ForeColor = System.Drawing.Color.White;
            btnregistrarse.Location = new System.Drawing.Point(98, 545);
            btnregistrarse.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            btnregistrarse.Name = "btnregistrarse";
            btnregistrarse.Size = new System.Drawing.Size(252, 53);
            btnregistrarse.TabIndex = 32;
            btnregistrarse.Text = "REGISTRARSE";
            btnregistrarse.UseVisualStyleBackColor = false;
            btnregistrarse.Click += btnregistrarse_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(98, 154);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(66, 19);
            label5.TabIndex = 35;
            label5.Text = "Apellido";
            // 
            // txtApellido
            // 
            txtApellido.BackColor = System.Drawing.Color.FromArgb(230, 231, 233);
            txtApellido.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtApellido.Font = new System.Drawing.Font("MS Gothic", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtApellido.Location = new System.Drawing.Point(98, 190);
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new System.Drawing.Size(252, 27);
            txtApellido.TabIndex = 36;
            // 
            // cbTipoUsuario
            // 
            cbTipoUsuario.FormattingEnabled = true;
            cbTipoUsuario.Items.AddRange(new object[] { "Cliente", "Propietario" });
            cbTipoUsuario.Location = new System.Drawing.Point(146, 492);
            cbTipoUsuario.Name = "cbTipoUsuario";
            cbTipoUsuario.Size = new System.Drawing.Size(151, 27);
            cbTipoUsuario.TabIndex = 37;
            cbTipoUsuario.Text = "Cliente";
            // 
            // frmCrearUsuario
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(459, 671);
            Controls.Add(cbTipoUsuario);
            Controls.Add(label5);
            Controls.Add(txtApellido);
            Controls.Add(btnregistrarse);
            Controls.Add(lblyatengocuen);
            Controls.Add(txtmail);
            Controls.Add(lblmail);
            Controls.Add(label6);
            Controls.Add(checkbxShowPass);
            Controls.Add(TxtConfirmpassword);
            Controls.Add(label4);
            Controls.Add(TxtPassword);
            Controls.Add(label3);
            Controls.Add(txtNombre);
            Controls.Add(label1);
            Font = new System.Drawing.Font("Nirmala UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            ForeColor = System.Drawing.Color.FromArgb(164, 165, 169);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(2);
            Name = "frmCrearUsuario";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "frmCrearUsuario";
            Load += frmCrearUsuario_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox checkbxShowPass;
        private System.Windows.Forms.TextBox TxtConfirmpassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TxtPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblmail;
        private System.Windows.Forms.TextBox txtmail;
        private System.Windows.Forms.Label lblyatengocuen;
        private System.Windows.Forms.Button btnregistrarse;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtApellido;
        private System.Windows.Forms.ComboBox cbTipoUsuario;
    }
}