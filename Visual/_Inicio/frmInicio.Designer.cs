namespace Visual
{
    partial class frmInicio
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
            this.txtMailLogin = new System.Windows.Forms.TextBox();
            this.txtContraseñaLogin = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkbxShowPass = new System.Windows.Forms.CheckBox();
            this.lblolividarcontra = new System.Windows.Forms.Label();
            this.lblcrearcuenta = new System.Windows.Forms.Label();
            this.btninicio = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnModificar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtMailLogin
            // 
            this.txtMailLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(231)))), ((int)(((byte)(233)))));
            this.txtMailLogin.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMailLogin.Font = new System.Drawing.Font("MS Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMailLogin.Location = new System.Drawing.Point(108, 249);
            this.txtMailLogin.Name = "txtMailLogin";
            this.txtMailLogin.Size = new System.Drawing.Size(250, 32);
            this.txtMailLogin.TabIndex = 5;
            this.txtMailLogin.TabStop = false;
            // 
            // txtContraseñaLogin
            // 
            this.txtContraseñaLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(231)))), ((int)(((byte)(233)))));
            this.txtContraseñaLogin.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtContraseñaLogin.Font = new System.Drawing.Font("MS Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtContraseñaLogin.Location = new System.Drawing.Point(108, 336);
            this.txtContraseñaLogin.Name = "txtContraseñaLogin";
            this.txtContraseñaLogin.Size = new System.Drawing.Size(250, 32);
            this.txtContraseñaLogin.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(103, 305);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 28);
            this.label3.TabIndex = 6;
            this.label3.Text = "Contraseña";
            // 
            // checkbxShowPass
            // 
            this.checkbxShowPass.AutoSize = true;
            this.checkbxShowPass.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkbxShowPass.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkbxShowPass.Location = new System.Drawing.Point(228, 407);
            this.checkbxShowPass.Name = "checkbxShowPass";
            this.checkbxShowPass.Size = new System.Drawing.Size(178, 32);
            this.checkbxShowPass.TabIndex = 10;
            this.checkbxShowPass.Text = "Show Password";
            this.checkbxShowPass.UseVisualStyleBackColor = true;
            this.checkbxShowPass.CheckedChanged += new System.EventHandler(this.checkbxShowPass_CheckedChanged);
            // 
            // lblolividarcontra
            // 
            this.lblolividarcontra.AutoSize = true;
            this.lblolividarcontra.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblolividarcontra.Location = new System.Drawing.Point(103, 602);
            this.lblolividarcontra.Name = "lblolividarcontra";
            this.lblolividarcontra.Size = new System.Drawing.Size(255, 28);
            this.lblolividarcontra.TabIndex = 18;
            this.lblolividarcontra.Text = "¿Olvidaste tu contraseña?";
            this.lblolividarcontra.Click += new System.EventHandler(this.lblolividarcontra_Click);
            // 
            // lblcrearcuenta
            // 
            this.lblcrearcuenta.AutoSize = true;
            this.lblcrearcuenta.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblcrearcuenta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.lblcrearcuenta.Location = new System.Drawing.Point(157, 656);
            this.lblcrearcuenta.Name = "lblcrearcuenta";
            this.lblcrearcuenta.Size = new System.Drawing.Size(134, 28);
            this.lblcrearcuenta.TabIndex = 19;
            this.lblcrearcuenta.Text = "Crear Cuenta";
            this.lblcrearcuenta.Click += new System.EventHandler(this.lblcrearcuenta_Click);
            // 
            // btninicio
            // 
            this.btninicio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.btninicio.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btninicio.FlatAppearance.BorderSize = 0;
            this.btninicio.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btninicio.ForeColor = System.Drawing.Color.White;
            this.btninicio.Location = new System.Drawing.Point(108, 508);
            this.btninicio.Name = "btninicio";
            this.btninicio.Size = new System.Drawing.Size(240, 49);
            this.btninicio.TabIndex = 20;
            this.btninicio.Text = "Iniciar sesión";
            this.btninicio.UseVisualStyleBackColor = false;
            this.btninicio.Click += new System.EventHandler(this.btninicio_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(103, 218);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 28);
            this.label5.TabIndex = 21;
            this.label5.Text = "Mail";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.label2.Location = new System.Drawing.Point(108, 93);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(240, 60);
            this.label2.TabIndex = 22;
            this.label2.Text = "EventAura";
            // 
            // btnModificar
            // 
            this.btnModificar.Location = new System.Drawing.Point(494, 572);
            this.btnModificar.Name = "btnModificar";
            this.btnModificar.Size = new System.Drawing.Size(368, 51);
            this.btnModificar.TabIndex = 50;
            this.btnModificar.Text = "Modificar";
            this.btnModificar.UseVisualStyleBackColor = true;
            // 
            // frmInicio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(473, 714);
            this.Controls.Add(this.btnModificar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btninicio);
            this.Controls.Add(this.lblcrearcuenta);
            this.Controls.Add(this.lblolividarcontra);
            this.Controls.Add(this.checkbxShowPass);
            this.Controls.Add(this.txtContraseñaLogin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtMailLogin);
            this.Font = new System.Drawing.Font("Nirmala UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(165)))), ((int)(((byte)(169)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmInicio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmInicio";
            this.Load += new System.EventHandler(this.frmInicio_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtMailLogin;
        private System.Windows.Forms.TextBox txtContraseñaLogin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkbxShowPass;
        private System.Windows.Forms.Label lblolividarcontra;
        private System.Windows.Forms.Label lblcrearcuenta;
        private System.Windows.Forms.Button btninicio;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnModificar;
    }
}