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
            this.checkbxShowPass = new System.Windows.Forms.CheckBox();
            this.TxtConfirmpassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TxtPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.label1 = new Visual.Componentes.EtiquetaTitulo();
            this.label6 = new System.Windows.Forms.Label();
            this.lblmail = new System.Windows.Forms.Label();
            this.txtmail = new System.Windows.Forms.TextBox();
            this.lblyatengocuen = new Visual.Componentes.EnlaceTematico();
            this.btnregistrarse = new Visual.Componentes.BotonPrimario();
            this.label5 = new System.Windows.Forms.Label();
            this.txtApellido = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // checkbxShowPass
            // 
            this.checkbxShowPass.AutoSize = true;
            this.checkbxShowPass.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkbxShowPass.Location = new System.Drawing.Point(24, 392);
            this.checkbxShowPass.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.checkbxShowPass.Name = "checkbxShowPass";
            this.checkbxShowPass.Size = new System.Drawing.Size(240, 24);
            this.checkbxShowPass.TabIndex = 23;
            this.checkbxShowPass.Text = "Mostrar contraseña";
            this.checkbxShowPass.CheckedChanged += new System.EventHandler(this.checkbxShowPass_CheckedChanged);
            // 
            // TxtConfirmpassword
            // 
            this.TxtConfirmpassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxtConfirmpassword.Location = new System.Drawing.Point(232, 316);
            this.TxtConfirmpassword.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.TxtConfirmpassword.Name = "TxtConfirmpassword";
            this.TxtConfirmpassword.UseSystemPasswordChar = true;
            this.TxtConfirmpassword.Size = new System.Drawing.Size(184, 28);
            this.TxtConfirmpassword.TabIndex = 22;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(232, 292);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(184, 20);
            this.label4.TabIndex = 21;
            this.label4.Text = "Confirmar Contraseña";
            // 
            // TxtPassword
            // 
            this.TxtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxtPassword.Location = new System.Drawing.Point(24, 316);
            this.TxtPassword.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.TxtPassword.Name = "TxtPassword";
            this.TxtPassword.UseSystemPasswordChar = true;
            this.TxtPassword.Size = new System.Drawing.Size(184, 28);
            this.TxtPassword.TabIndex = 20;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 292);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(184, 20);
            this.label3.TabIndex = 19;
            this.label3.Text = "Contraseña";
            // 
            // txtNombre
            // 
            this.txtNombre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNombre.Location = new System.Drawing.Point(24, 172);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(184, 28);
            this.txtNombre.TabIndex = 34;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 84);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(392, 32);
            this.label1.TabIndex = 16;
            this.label1.Text = "Crear usuario";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(184, 20);
            this.label6.TabIndex = 27;
            this.label6.Text = "Nombre";
            // 
            // lblmail
            // 
            this.lblmail.AutoSize = true;
            this.lblmail.Location = new System.Drawing.Point(24, 220);
            this.lblmail.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblmail.Name = "lblmail";
            this.lblmail.Size = new System.Drawing.Size(392, 20);
            this.lblmail.TabIndex = 29;
            this.lblmail.Text = "Mail";
            // 
            // txtmail
            // 
            this.txtmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtmail.Location = new System.Drawing.Point(24, 244);
            this.txtmail.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.txtmail.Name = "txtmail";
            this.txtmail.Size = new System.Drawing.Size(392, 28);
            this.txtmail.TabIndex = 30;
            // 
            // lblyatengocuen
            // 
            this.lblyatengocuen.AutoSize = false;
            this.lblyatengocuen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblyatengocuen.Location = new System.Drawing.Point(24, 444);
            this.lblyatengocuen.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblyatengocuen.Name = "lblyatengocuen";
            this.lblyatengocuen.Size = new System.Drawing.Size(184, 24);
            this.lblyatengocuen.TabIndex = 31;
            this.lblyatengocuen.Text = "Ya tengo cuenta";
            this.lblyatengocuen.Click += new System.EventHandler(this.lblyatengocuen_Click);
            // 
            // btnregistrarse
            // 
            this.btnregistrarse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnregistrarse.Location = new System.Drawing.Point(232, 436);
            this.btnregistrarse.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.btnregistrarse.Name = "btnregistrarse";
            this.btnregistrarse.Size = new System.Drawing.Size(184, 40);
            this.btnregistrarse.TabIndex = 32;
            this.btnregistrarse.Text = "Registrarse";
            this.btnregistrarse.Click += new System.EventHandler(this.btnregistrarse_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(232, 148);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(184, 20);
            this.label5.TabIndex = 35;
            this.label5.Text = "Apellido";
            // 
            // txtApellido
            // 
            this.txtApellido.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtApellido.Location = new System.Drawing.Point(232, 172);
            this.txtApellido.Name = "txtApellido";
            this.txtApellido.Size = new System.Drawing.Size(184, 28);
            this.txtApellido.TabIndex = 36;
            // 
            // frmAgregarEvento
            // 
            this.AcceptButton = this.btnregistrarse;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1100, 720);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtApellido);
            this.Controls.Add(this.btnregistrarse);
            this.Controls.Add(this.lblyatengocuen);
            this.Controls.Add(this.txtmail);
            this.Controls.Add(this.lblmail);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.checkbxShowPass);
            this.Controls.Add(this.TxtConfirmpassword);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.TxtPassword);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmCrearUsuario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EventAura · Crear cuenta";
            this.Load += new System.EventHandler(this.frmCrearUsuario_Load);
            this.barraSuperior = new Visual.Componentes.BarraSuperior();
            this.barraSuperior.Name = "barraSuperior";
            this.barraSuperior.Size = new System.Drawing.Size(440, 60);
            this.barraSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Controls.Add(this.barraSuperior);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox checkbxShowPass;
        private System.Windows.Forms.TextBox TxtConfirmpassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TxtPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNombre;
        private Visual.Componentes.EtiquetaTitulo label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblmail;
        private System.Windows.Forms.TextBox txtmail;
        private System.Windows.Forms.LinkLabel lblyatengocuen;
        private Visual.Componentes.BotonPrimario btnregistrarse;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtApellido;
        private Visual.Componentes.BarraSuperior barraSuperior;
    }
}
