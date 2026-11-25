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
            this.lblTitulo = new Visual.Componentes.EtiquetaTitulo();
            this.lblTemporizador = new Visual.Componentes.EtiquetaSeccion();
            this.lblSeguro = new System.Windows.Forms.Label();
            this.lblTotal = new Visual.Componentes.EtiquetaSeccion();
            this.lblTitular = new System.Windows.Forms.Label();
            this.txtTitular = new System.Windows.Forms.TextBox();
            this.lblNumero = new System.Windows.Forms.Label();
            this.txtNumero = new System.Windows.Forms.TextBox();
            this.lblVencimiento = new System.Windows.Forms.Label();
            this.txtVencimiento = new System.Windows.Forms.TextBox();
            this.lblCvv = new System.Windows.Forms.Label();
            this.txtCvv = new System.Windows.Forms.TextBox();
            this.lblEstado = new System.Windows.Forms.Label();
            this.btnPagar = new Visual.Componentes.BotonPrimario();
            this.btnCancelar = new Visual.Componentes.BotonSecundario();
            this.tmrReserva = new System.Windows.Forms.Timer(this.components);
            this.indicadorPasos = new Visual.Componentes.IndicadorPasos();
            this.indicadorPasos.Name = "indicadorPasos";
            this.indicadorPasos.PasoActual = 3;
            this.indicadorPasos.Location = new System.Drawing.Point(24, 68);
            this.indicadorPasos.Size = new System.Drawing.Size(392, 24);
            this.Controls.Add(this.indicadorPasos);
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Location = new System.Drawing.Point(24, 108);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(184, 32);
            this.lblTitulo.TabIndex = 20;
            this.lblTitulo.Text = "Pago";
            //
            // lblTemporizador
            //
            this.lblTemporizador.Location = new System.Drawing.Point(232, 108);
            this.lblTemporizador.Name = "lblTemporizador";
            this.lblTemporizador.Size = new System.Drawing.Size(184, 32);
            this.lblTemporizador.TabIndex = 21;
            this.lblTemporizador.Text = "Reserva: 10:00";
            this.lblTemporizador.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblSeguro
            //
            this.lblSeguro.AutoSize = true;
            this.lblSeguro.Location = new System.Drawing.Point(24, 148);
            this.lblSeguro.Name = "lblSeguro";
            this.lblSeguro.Size = new System.Drawing.Size(392, 32);
            this.lblSeguro.TabIndex = 22;
            this.lblSeguro.Text = "Conexión segura (HTTPS simulado). La tarjeta se tokeniza.";
            //
            // lblTotal
            //
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(24, 188);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(392, 24);
            this.lblTotal.TabIndex = 23;
            this.lblTotal.Text = "Total a pagar";
            //
            // lblTitular
            //
            this.lblTitular.AutoSize = true;
            this.lblTitular.Location = new System.Drawing.Point(24, 220);
            this.lblTitular.Name = "lblTitular";
            this.lblTitular.Size = new System.Drawing.Size(392, 20);
            this.lblTitular.TabIndex = 24;
            this.lblTitular.Text = "Titular (como figura en la tarjeta)";
            //
            // txtTitular
            //
            this.txtTitular.Location = new System.Drawing.Point(24, 244);
            this.txtTitular.MaxLength = 60;
            this.txtTitular.Name = "txtTitular";
            this.txtTitular.Size = new System.Drawing.Size(392, 28);
            this.txtTitular.TabIndex = 0;
            //
            // lblNumero
            //
            this.lblNumero.AutoSize = true;
            this.lblNumero.Location = new System.Drawing.Point(24, 284);
            this.lblNumero.Name = "lblNumero";
            this.lblNumero.Size = new System.Drawing.Size(392, 20);
            this.lblNumero.TabIndex = 25;
            this.lblNumero.Text = "Número de tarjeta";
            //
            // txtNumero
            //
            this.txtNumero.Location = new System.Drawing.Point(24, 308);
            this.txtNumero.MaxLength = 23;
            this.txtNumero.Name = "txtNumero";
            this.txtNumero.Size = new System.Drawing.Size(392, 28);
            this.txtNumero.TabIndex = 1;
            //
            // lblVencimiento
            //
            this.lblVencimiento.AutoSize = true;
            this.lblVencimiento.Location = new System.Drawing.Point(24, 348);
            this.lblVencimiento.Name = "lblVencimiento";
            this.lblVencimiento.Size = new System.Drawing.Size(184, 20);
            this.lblVencimiento.TabIndex = 26;
            this.lblVencimiento.Text = "Vencimiento (MM/AA)";
            //
            // txtVencimiento
            //
            this.txtVencimiento.Location = new System.Drawing.Point(24, 372);
            this.txtVencimiento.MaxLength = 5;
            this.txtVencimiento.Name = "txtVencimiento";
            this.txtVencimiento.Size = new System.Drawing.Size(184, 28);
            this.txtVencimiento.TabIndex = 2;
            //
            // lblCvv
            //
            this.lblCvv.AutoSize = true;
            this.lblCvv.Location = new System.Drawing.Point(232, 348);
            this.lblCvv.Name = "lblCvv";
            this.lblCvv.Size = new System.Drawing.Size(184, 20);
            this.lblCvv.TabIndex = 27;
            this.lblCvv.Text = "Código de seguridad";
            //
            // txtCvv
            //
            this.txtCvv.Location = new System.Drawing.Point(232, 372);
            this.txtCvv.MaxLength = 4;
            this.txtCvv.Name = "txtCvv";
            this.txtCvv.PasswordChar = '•';
            this.txtCvv.Size = new System.Drawing.Size(184, 28);
            this.txtCvv.TabIndex = 3;
            //
            // lblEstado
            //
            this.lblEstado.Location = new System.Drawing.Point(24, 404);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(392, 40);
            this.lblEstado.TabIndex = 28;
            //
            // btnPagar
            //
            this.btnPagar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPagar.Location = new System.Drawing.Point(232, 452);
            this.btnPagar.Name = "btnPagar";
            this.btnPagar.Size = new System.Drawing.Size(184, 40);
            this.btnPagar.TabIndex = 4;
            this.btnPagar.Text = "Pagar";
            this.btnPagar.Click += new System.EventHandler(this.btnPagar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.Location = new System.Drawing.Point(24, 452);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(184, 40);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "Cancelar pago";
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(440, 516);
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPago";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pago seguro";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPago_FormClosing);
            this.Load += new System.EventHandler(this.frmPago_Load);
            this.barraSuperior = new Visual.Componentes.BarraSuperior();
            this.barraSuperior.Name = "barraSuperior";
            this.barraSuperior.Size = new System.Drawing.Size(440, 60);
            this.barraSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Controls.Add(this.barraSuperior);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Visual.Componentes.IndicadorPasos indicadorPasos;

        private Visual.Componentes.EtiquetaTitulo lblTitulo;
        private Visual.Componentes.EtiquetaSeccion lblTemporizador;
        private System.Windows.Forms.Label lblSeguro;
        private Visual.Componentes.EtiquetaSeccion lblTotal;
        private System.Windows.Forms.Label lblTitular;
        private System.Windows.Forms.TextBox txtTitular;
        private System.Windows.Forms.Label lblNumero;
        private System.Windows.Forms.TextBox txtNumero;
        private System.Windows.Forms.Label lblVencimiento;
        private System.Windows.Forms.TextBox txtVencimiento;
        private System.Windows.Forms.Label lblCvv;
        private System.Windows.Forms.TextBox txtCvv;
        private System.Windows.Forms.Label lblEstado;
        private Visual.Componentes.BotonPrimario btnPagar;
        private Visual.Componentes.BotonSecundario btnCancelar;
        private System.Windows.Forms.Timer tmrReserva;
        private Visual.Componentes.BarraSuperior barraSuperior;
    }
}
