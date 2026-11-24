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
            this.lblolividarcontra = new Visual.Componentes.EnlaceTematico();
            this.lblcrearcuenta = new Visual.Componentes.EnlaceTematico();
            this.btninicio = new Visual.Componentes.BotonPrimario();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new Visual.Componentes.EtiquetaTitulo();
            this.SuspendLayout();
            // 
            // txtMailLogin
            // 
            this.txtMailLogin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMailLogin.Location = new System.Drawing.Point(64, 172);
            this.txtMailLogin.Name = "txtMailLogin";
            this.txtMailLogin.Size = new System.Drawing.Size(352, 28);
            this.txtMailLogin.TabIndex = 0;
            this.txtMailLogin.TabStop = true;
            // 
            // txtContraseñaLogin
            // 
            this.txtContraseñaLogin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtContraseñaLogin.Location = new System.Drawing.Point(64, 244);
            this.txtContraseñaLogin.Name = "txtContraseñaLogin";
            this.txtContraseñaLogin.Size = new System.Drawing.Size(352, 28);
            this.txtContraseñaLogin.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(64, 220);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(352, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Contraseña";
            // 
            // checkbxShowPass
            // 
            this.checkbxShowPass.AutoSize = true;
            this.checkbxShowPass.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkbxShowPass.Location = new System.Drawing.Point(64, 292);
            this.checkbxShowPass.Name = "checkbxShowPass";
            this.checkbxShowPass.Size = new System.Drawing.Size(240, 24);
            this.checkbxShowPass.TabIndex = 2;
            this.checkbxShowPass.Text = "Mostrar contraseña";
            this.checkbxShowPass.CheckedChanged += new System.EventHandler(this.checkbxShowPass_CheckedChanged);
            // 
            // lblolividarcontra
            // 
            this.lblolividarcontra.AutoSize = false;
            this.lblolividarcontra.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblolividarcontra.Location = new System.Drawing.Point(256, 412);
            this.lblolividarcontra.Name = "lblolividarcontra";
            this.lblolividarcontra.Size = new System.Drawing.Size(160, 24);
            this.lblolividarcontra.TabIndex = 5;
            this.lblolividarcontra.Text = "Recuperar contraseña";
            this.lblolividarcontra.Click += new System.EventHandler(this.lblolividarcontra_Click);
            // 
            // lblcrearcuenta
            // 
            this.lblcrearcuenta.AutoSize = false;
            this.lblcrearcuenta.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblcrearcuenta.Location = new System.Drawing.Point(64, 412);
            this.lblcrearcuenta.Name = "lblcrearcuenta";
            this.lblcrearcuenta.Size = new System.Drawing.Size(160, 24);
            this.lblcrearcuenta.TabIndex = 4;
            this.lblcrearcuenta.Text = "Crear cuenta";
            this.lblcrearcuenta.Click += new System.EventHandler(this.lblcrearcuenta_Click);
            // 
            // btninicio
            // 
            this.btninicio.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btninicio.Location = new System.Drawing.Point(64, 348);
            this.btninicio.Name = "btninicio";
            this.btninicio.Size = new System.Drawing.Size(352, 40);
            this.btninicio.TabIndex = 3;
            this.btninicio.Text = "Iniciar sesión";
            this.btninicio.Click += new System.EventHandler(this.btninicio_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(64, 148);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(352, 20);
            this.label5.TabIndex = 21;
            this.label5.Text = "Mail";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(64, 84);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(352, 32);
            this.label2.TabIndex = 22;
            this.label2.Text = "Iniciar sesión";
            // 
            // btnModificar
            // 
            // 
            // frmInicio
            // 
            this.AcceptButton = this.btninicio;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(480, 484);

            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btninicio);
            this.Controls.Add(this.lblcrearcuenta);
            this.Controls.Add(this.lblolividarcontra);
            this.Controls.Add(this.checkbxShowPass);
            this.Controls.Add(this.txtContraseñaLogin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtMailLogin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmInicio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EventAura · Iniciar sesión";
            this.Load += new System.EventHandler(this.frmInicio_Load);
            this.barraSuperior = new Visual.Componentes.BarraSuperior();
            this.barraSuperior.Name = "barraSuperior";
            this.barraSuperior.Size = new System.Drawing.Size(400, 60);
            this.barraSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Controls.Add(this.barraSuperior);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtMailLogin;
        private System.Windows.Forms.TextBox txtContraseñaLogin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkbxShowPass;
        private System.Windows.Forms.LinkLabel lblolividarcontra;
        private System.Windows.Forms.LinkLabel lblcrearcuenta;
        private Visual.Componentes.BotonPrimario btninicio;
        private System.Windows.Forms.Label label5;
        private Visual.Componentes.EtiquetaTitulo label2;
        private Visual.Componentes.BarraSuperior barraSuperior;
    }
}
