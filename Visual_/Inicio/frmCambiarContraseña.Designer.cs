namespace Visual
{
    partial class frmCambiarContraseña
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
            checkbxShowPass = new System.Windows.Forms.CheckBox();
            TxtConfirmpassword = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            TxtPassword = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            btnconfirmar = new System.Windows.Forms.Button();
            txtMail = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // btnVolver
            // 
            btnVolver.BackColor = System.Drawing.Color.White;
            btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            btnVolver.Location = new System.Drawing.Point(5, 561);
            btnVolver.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            btnVolver.Name = "btnVolver";
            btnVolver.Size = new System.Drawing.Size(119, 43);
            btnVolver.TabIndex = 1;
            btnVolver.Text = "Volver ";
            btnVolver.UseVisualStyleBackColor = false;
            btnVolver.Click += btnVolver_Click_1;
            // 
            // checkbxShowPass
            // 
            checkbxShowPass.AutoSize = true;
            checkbxShowPass.Cursor = System.Windows.Forms.Cursors.Hand;
            checkbxShowPass.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            checkbxShowPass.Location = new System.Drawing.Point(246, 491);
            checkbxShowPass.Margin = new System.Windows.Forms.Padding(7, 4, 7, 4);
            checkbxShowPass.Name = "checkbxShowPass";
            checkbxShowPass.Size = new System.Drawing.Size(128, 24);
            checkbxShowPass.TabIndex = 36;
            checkbxShowPass.Text = "Show Password";
            checkbxShowPass.UseVisualStyleBackColor = true;
            checkbxShowPass.CheckedChanged += checkbxShowPass_CheckedChanged;
            // 
            // TxtConfirmpassword
            // 
            TxtConfirmpassword.BackColor = System.Drawing.Color.FromArgb(230, 231, 233);
            TxtConfirmpassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            TxtConfirmpassword.Font = new System.Drawing.Font("MS Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            TxtConfirmpassword.Location = new System.Drawing.Point(38, 350);
            TxtConfirmpassword.Margin = new System.Windows.Forms.Padding(7, 4, 7, 4);
            TxtConfirmpassword.Multiline = true;
            TxtConfirmpassword.Name = "TxtConfirmpassword";
            TxtConfirmpassword.Size = new System.Drawing.Size(336, 43);
            TxtConfirmpassword.TabIndex = 35;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(35, 319);
            label4.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(153, 20);
            label4.TabIndex = 34;
            label4.Text = "Confirmar Contraseña";
            // 
            // TxtPassword
            // 
            TxtPassword.BackColor = System.Drawing.Color.FromArgb(230, 231, 233);
            TxtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            TxtPassword.Font = new System.Drawing.Font("MS Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            TxtPassword.Location = new System.Drawing.Point(38, 251);
            TxtPassword.Margin = new System.Windows.Forms.Padding(7, 4, 7, 4);
            TxtPassword.Multiline = true;
            TxtPassword.Name = "TxtPassword";
            TxtPassword.Size = new System.Drawing.Size(336, 43);
            TxtPassword.TabIndex = 33;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(35, 221);
            label3.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(129, 20);
            label3.TabIndex = 32;
            label3.Text = "Nueva Contraseña";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label1.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            label1.Location = new System.Drawing.Point(18, 77);
            label1.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(405, 50);
            label1.TabIndex = 29;
            label1.Text = "NUEVA CONTRASEÑA";
            // 
            // btnconfirmar
            // 
            btnconfirmar.BackColor = System.Drawing.Color.FromArgb(116, 86, 174);
            btnconfirmar.Cursor = System.Windows.Forms.Cursors.Hand;
            btnconfirmar.FlatAppearance.BorderSize = 0;
            btnconfirmar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnconfirmar.ForeColor = System.Drawing.Color.White;
            btnconfirmar.Location = new System.Drawing.Point(35, 430);
            btnconfirmar.Margin = new System.Windows.Forms.Padding(7, 4, 7, 4);
            btnconfirmar.Name = "btnconfirmar";
            btnconfirmar.Size = new System.Drawing.Size(336, 53);
            btnconfirmar.TabIndex = 40;
            btnconfirmar.Text = "CONFIRMAR";
            btnconfirmar.UseVisualStyleBackColor = false;
            btnconfirmar.Click += btnconfirmar_Click;
            // 
            // txtMail
            // 
            txtMail.BackColor = System.Drawing.Color.FromArgb(230, 231, 233);
            txtMail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtMail.Font = new System.Drawing.Font("MS Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            txtMail.Location = new System.Drawing.Point(38, 174);
            txtMail.Margin = new System.Windows.Forms.Padding(7, 4, 7, 4);
            txtMail.Multiline = true;
            txtMail.Name = "txtMail";
            txtMail.Size = new System.Drawing.Size(336, 43);
            txtMail.TabIndex = 42;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(35, 143);
            label5.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(38, 20);
            label5.TabIndex = 41;
            label5.Text = "Mail";
            // 
            // frmCambiarContraseña
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(434, 611);
            Controls.Add(txtMail);
            Controls.Add(label5);
            Controls.Add(btnconfirmar);
            Controls.Add(checkbxShowPass);
            Controls.Add(TxtConfirmpassword);
            Controls.Add(label4);
            Controls.Add(TxtPassword);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(btnVolver);
            ForeColor = System.Drawing.Color.FromArgb(164, 165, 169);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            Name = "frmCambiarContraseña";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "frmCambiarContraseña";
            Load += frmCambiarContraseña_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.CheckBox checkbxShowPass;
        private System.Windows.Forms.TextBox TxtConfirmpassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TxtPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnconfirmar;
        private System.Windows.Forms.TextBox txtMail;
        private System.Windows.Forms.Label label5;
    }
}