namespace Visual
{
    partial class frmPago
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
            this.components = new System.ComponentModel.Container();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblTemporizador = new System.Windows.Forms.Label();
            this.lblSeguro = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblTitular = new System.Windows.Forms.Label();
            this.txtTitular = new System.Windows.Forms.TextBox();
            this.lblNumero = new System.Windows.Forms.Label();
            this.txtNumero = new System.Windows.Forms.TextBox();
            this.lblVencimiento = new System.Windows.Forms.Label();
            this.txtVencimiento = new System.Windows.Forms.TextBox();
            this.lblCvv = new System.Windows.Forms.Label();
            this.txtCvv = new System.Windows.Forms.TextBox();
            this.lblEstado = new System.Windows.Forms.Label();
            this.btnPagar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.tmrReserva = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.lblTitulo.Location = new System.Drawing.Point(20, 14);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(98, 50);
            this.lblTitulo.TabIndex = 20;
            this.lblTitulo.Text = "Pago";
            //
            // lblTemporizador
            //
            this.lblTemporizador.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTemporizador.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.lblTemporizador.Location = new System.Drawing.Point(228, 24);
            this.lblTemporizador.Name = "lblTemporizador";
            this.lblTemporizador.Size = new System.Drawing.Size(200, 30);
            this.lblTemporizador.TabIndex = 21;
            this.lblTemporizador.Text = "Reserva: 10:00";
            this.lblTemporizador.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblSeguro
            //
            this.lblSeguro.AutoSize = true;
            this.lblSeguro.Location = new System.Drawing.Point(24, 66);
            this.lblSeguro.Name = "lblSeguro";
            this.lblSeguro.Size = new System.Drawing.Size(360, 19);
            this.lblSeguro.TabIndex = 22;
            this.lblSeguro.Text = "Conexión segura (HTTPS simulado). La tarjeta se tokeniza.";
            //
            // lblTotal
            //
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Nirmala UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblTotal.Location = new System.Drawing.Point(22, 96);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(130, 28);
            this.lblTotal.TabIndex = 23;
            this.lblTotal.Text = "Total a pagar";
            //
            // lblTitular
            //
            this.lblTitular.AutoSize = true;
            this.lblTitular.Location = new System.Drawing.Point(24, 140);
            this.lblTitular.Name = "lblTitular";
            this.lblTitular.Size = new System.Drawing.Size(220, 19);
            this.lblTitular.TabIndex = 24;
            this.lblTitular.Text = "Titular (como figura en la tarjeta)";
            //
            // txtTitular
            //
            this.txtTitular.Location = new System.Drawing.Point(28, 162);
            this.txtTitular.MaxLength = 60;
            this.txtTitular.Name = "txtTitular";
            this.txtTitular.Size = new System.Drawing.Size(400, 26);
            this.txtTitular.TabIndex = 0;
            //
            // lblNumero
            //
            this.lblNumero.AutoSize = true;
            this.lblNumero.Location = new System.Drawing.Point(24, 198);
            this.lblNumero.Name = "lblNumero";
            this.lblNumero.Size = new System.Drawing.Size(124, 19);
            this.lblNumero.TabIndex = 25;
            this.lblNumero.Text = "Número de tarjeta";
            //
            // txtNumero
            //
            this.txtNumero.Location = new System.Drawing.Point(28, 220);
            this.txtNumero.MaxLength = 23;
            this.txtNumero.Name = "txtNumero";
            this.txtNumero.Size = new System.Drawing.Size(400, 26);
            this.txtNumero.TabIndex = 1;
            //
            // lblVencimiento
            //
            this.lblVencimiento.AutoSize = true;
            this.lblVencimiento.Location = new System.Drawing.Point(24, 256);
            this.lblVencimiento.Name = "lblVencimiento";
            this.lblVencimiento.Size = new System.Drawing.Size(146, 19);
            this.lblVencimiento.TabIndex = 26;
            this.lblVencimiento.Text = "Vencimiento (MM/AA)";
            //
            // txtVencimiento
            //
            this.txtVencimiento.Location = new System.Drawing.Point(28, 278);
            this.txtVencimiento.MaxLength = 5;
            this.txtVencimiento.Name = "txtVencimiento";
            this.txtVencimiento.Size = new System.Drawing.Size(120, 26);
            this.txtVencimiento.TabIndex = 2;
            //
            // lblCvv
            //
            this.lblCvv.AutoSize = true;
            this.lblCvv.Location = new System.Drawing.Point(196, 256);
            this.lblCvv.Name = "lblCvv";
            this.lblCvv.Size = new System.Drawing.Size(140, 19);
            this.lblCvv.TabIndex = 27;
            this.lblCvv.Text = "Código de seguridad";
            //
            // txtCvv
            //
            this.txtCvv.Location = new System.Drawing.Point(200, 278);
            this.txtCvv.MaxLength = 4;
            this.txtCvv.Name = "txtCvv";
            this.txtCvv.PasswordChar = '•';
            this.txtCvv.Size = new System.Drawing.Size(90, 26);
            this.txtCvv.TabIndex = 3;
            //
            // lblEstado
            //
            this.lblEstado.ForeColor = System.Drawing.Color.Firebrick;
            this.lblEstado.Location = new System.Drawing.Point(24, 316);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(408, 100);
            this.lblEstado.TabIndex = 28;
            //
            // btnPagar
            //
            this.btnPagar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.btnPagar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPagar.FlatAppearance.BorderSize = 0;
            this.btnPagar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPagar.ForeColor = System.Drawing.Color.White;
            this.btnPagar.Location = new System.Drawing.Point(248, 424);
            this.btnPagar.Name = "btnPagar";
            this.btnPagar.Size = new System.Drawing.Size(180, 44);
            this.btnPagar.TabIndex = 4;
            this.btnPagar.Text = "Pagar";
            this.btnPagar.UseVisualStyleBackColor = false;
            this.btnPagar.Click += new System.EventHandler(this.btnPagar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.Location = new System.Drawing.Point(28, 424);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(150, 44);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "Cancelar pago";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // tmrReserva
            //
            this.tmrReserva.Interval = 1000;
            this.tmrReserva.Tick += new System.EventHandler(this.tmrReserva_Tick);
            //
            // frmPago
            //
            this.AcceptButton = this.btnPagar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(456, 488);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnPagar);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.txtCvv);
            this.Controls.Add(this.lblCvv);
            this.Controls.Add(this.txtVencimiento);
            this.Controls.Add(this.lblVencimiento);
            this.Controls.Add(this.txtNumero);
            this.Controls.Add(this.lblNumero);
            this.Controls.Add(this.txtTitular);
            this.Controls.Add(this.lblTitular);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.lblSeguro);
            this.Controls.Add(this.lblTemporizador);
            this.Controls.Add(this.lblTitulo);
            this.Font = new System.Drawing.Font("Nirmala UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(165)))), ((int)(((byte)(169)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPago";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pago seguro";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPago_FormClosing);
            this.Load += new System.EventHandler(this.frmPago_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblTemporizador;
        private System.Windows.Forms.Label lblSeguro;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblTitular;
        private System.Windows.Forms.TextBox txtTitular;
        private System.Windows.Forms.Label lblNumero;
        private System.Windows.Forms.TextBox txtNumero;
        private System.Windows.Forms.Label lblVencimiento;
        private System.Windows.Forms.TextBox txtVencimiento;
        private System.Windows.Forms.Label lblCvv;
        private System.Windows.Forms.TextBox txtCvv;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.Button btnPagar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Timer tmrReserva;
    }
}
