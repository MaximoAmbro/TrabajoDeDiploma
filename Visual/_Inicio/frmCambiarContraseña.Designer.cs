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
            this.btnVolver = new Visual.Componentes.BotonSecundario();
            this.checkbxShowPass = new System.Windows.Forms.CheckBox();
            this.TxtConfirmpassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TxtPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new Visual.Componentes.EtiquetaTitulo();
            this.btnconfirmar = new Visual.Componentes.BotonPrimario();
            this.txtMail = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtActual = new System.Windows.Forms.TextBox();
            this.lblActual = new System.Windows.Forms.Label();
            this.txtActual.Location = new System.Drawing.Point(24, 160);
            this.txtActual.Size = new System.Drawing.Size(392, 28);
            this.txtActual.Name = "txtActual";
            this.txtActual.TabIndex = 1;
            this.lblActual.AutoSize = true;
            this.lblActual.Name = "lblActual";
            this.lblActual.Size = new System.Drawing.Size(392, 20);
            this.lblActual.Location = new System.Drawing.Point(24, 136);
            this.lblActual.Text = "Contraseña actual";
            this.Controls.Add(this.txtActual);
            this.Controls.Add(this.lblActual);
            this.SuspendLayout();
            // 
            // btnVolver
            // 
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.Location = new System.Drawing.Point(24, 384);
            this.btnVolver.Margin = new System.Windows.Forms.Padding(2);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(136, 40);
            this.btnVolver.TabIndex = 6;
            this.btnVolver.Text = "Volver ";
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            // 
            // checkbxShowPass
            // 
            this.checkbxShowPass.AutoSize = true;
            this.checkbxShowPass.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkbxShowPass.Location = new System.Drawing.Point(24, 328);
            this.checkbxShowPass.Margin = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.checkbxShowPass.Name = "checkbxShowPass";
            this.checkbxShowPass.Size = new System.Drawing.Size(260, 24);
            this.checkbxShowPass.TabIndex = 4;
            this.checkbxShowPass.Text = "Mostrar contraseñas";
            this.checkbxShowPass.CheckedChanged += new System.EventHandler(this.checkbxShowPass_CheckedChanged);
            // 
            // TxtConfirmpassword
            // 
            this.TxtConfirmpassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxtConfirmpassword.Location = new System.Drawing.Point(24, 288);
            this.TxtConfirmpassword.Margin = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.TxtConfirmpassword.Multiline = true;
            this.TxtConfirmpassword.Name = "TxtConfirmpassword";
            this.TxtConfirmpassword.Size = new System.Drawing.Size(392, 28);
            this.TxtConfirmpassword.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 264);
            this.label4.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(392, 20);
            this.label4.TabIndex = 34;
            this.label4.Text = "Confirmar Contraseña";
            // 
            // TxtPassword
            // 
            this.TxtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxtPassword.Location = new System.Drawing.Point(24, 224);
            this.TxtPassword.Margin = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.TxtPassword.Multiline = true;
            this.TxtPassword.Name = "TxtPassword";
            this.TxtPassword.Size = new System.Drawing.Size(392, 28);
            this.TxtPassword.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Cursor = System.Windows.Forms.Cursors.Default;
            this.label3.Location = new System.Drawing.Point(24, 200);
            this.label3.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(392, 20);
            this.label3.TabIndex = 32;
            this.label3.Text = "Nueva Contraseña";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 24);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(392, 32);
            this.label1.TabIndex = 29;
            this.label1.Text = "Nueva contraseña";
            // 
            // btnconfirmar
            // 
            this.btnconfirmar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnconfirmar.Location = new System.Drawing.Point(232, 384);
            this.btnconfirmar.Margin = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.btnconfirmar.Name = "btnconfirmar";
            this.btnconfirmar.Size = new System.Drawing.Size(184, 40);
            this.btnconfirmar.TabIndex = 5;
            this.btnconfirmar.Text = "Confirmar";
            this.btnconfirmar.Click += new System.EventHandler(this.btnconfirmar_Click);
            // 
            // txtMail
            // 
            this.txtMail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMail.Location = new System.Drawing.Point(24, 96);
            this.txtMail.Margin = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.txtMail.Multiline = true;
            this.txtMail.Name = "txtMail";
            this.txtMail.Size = new System.Drawing.Size(392, 28);
            this.txtMail.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 72);
            this.label5.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(392, 20);
            this.label5.TabIndex = 41;
            this.label5.Text = "Mail";
            // 
            // frmCambiarContraseña
            // 
            this.AcceptButton = this.btnconfirmar;
            this.CancelButton = this.btnVolver;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(440, 448);
            this.Controls.Add(this.txtMail);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnconfirmar);
            this.Controls.Add(this.checkbxShowPass);
            this.Controls.Add(this.TxtConfirmpassword);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.TxtPassword);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnVolver);
            this.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmCambiarContraseña";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EventAura · Cambiar contraseña";
            this.Load += new System.EventHandler(this.frmCambiarContraseña_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtActual;
        private System.Windows.Forms.Label lblActual;
        private Visual.Componentes.BotonSecundario btnVolver;
        private System.Windows.Forms.CheckBox checkbxShowPass;
        private System.Windows.Forms.TextBox TxtConfirmpassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TxtPassword;
        private System.Windows.Forms.Label label3;
        private Visual.Componentes.EtiquetaTitulo label1;
        private Visual.Componentes.BotonPrimario btnconfirmar;
        private System.Windows.Forms.TextBox txtMail;
        private System.Windows.Forms.Label label5;
    }
}
