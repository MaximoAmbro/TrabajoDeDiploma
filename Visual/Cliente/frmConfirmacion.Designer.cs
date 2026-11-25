namespace Visual
{
    partial class frmConfirmacion
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
            this.lblTitulo = new Visual.Componentes.EtiquetaTitulo();
            this.lblPedido = new Visual.Componentes.EtiquetaSeccion();
            this.lblDesglose = new System.Windows.Forms.Label();
            this.lblTickets = new System.Windows.Forms.Label();
            this.txtCarpeta = new System.Windows.Forms.TextBox();
            this.lstArchivos = new System.Windows.Forms.ListBox();
            this.pnlArchivos = new System.Windows.Forms.Panel();
            this.pnlArchivos.Name = "pnlArchivos";
            this.pnlArchivos.Location = new System.Drawing.Point(400, 348);
            this.pnlArchivos.Size = new System.Drawing.Size(336, 120);
            this.pnlArchivos.Controls.Add(this.lstArchivos);
            this.lstArchivos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAbrirCarpeta = new Visual.Componentes.BotonSecundario();
            this.btnAceptar = new Visual.Componentes.BotonPrimario();
            this.indicadorPasos = new Visual.Componentes.IndicadorPasos();
            this.indicadorPasos.Name = "indicadorPasos";
            this.indicadorPasos.PasoActual = 4;
            this.indicadorPasos.Location = new System.Drawing.Point(24, 76);
            this.indicadorPasos.Size = new System.Drawing.Size(712, 32);
            this.iconoExito = new Visual.Componentes.IconoExito();
            this.iconoExito.Name = "iconoExito";
            this.iconoExito.Location = new System.Drawing.Point(24, 124);
            this.iconoExito.Size = new System.Drawing.Size(48, 48);
            this.lblTotalConfirmado = new Visual.Componentes.EtiquetaTotal();
            this.lblTotalConfirmado.Name = "lblTotalConfirmado";
            this.lblTotalConfirmado.Location = new System.Drawing.Point(400, 212);
            this.lblTotalConfirmado.Size = new System.Drawing.Size(336, 48);
            this.pnlDetalle = new System.Windows.Forms.Panel();
            this.pnlDetalle.Name = "pnlDetalle";
            this.pnlDetalle.Location = new System.Drawing.Point(24, 212);
            this.pnlDetalle.Size = new System.Drawing.Size(352, 256);
            this.pnlDetalle.AutoScroll = true;
            this.pnlDetalle.Controls.Add(this.lblDesglose);
            this.lblDesglose.AutoSize = false;
            this.lstArchivos.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstArchivos.ItemHeight = 32;
            this.Controls.Add(this.pnlDetalle);
            this.Controls.Add(this.indicadorPasos);
            this.Controls.Add(this.iconoExito);
            this.Controls.Add(this.lblTotalConfirmado);
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Location = new System.Drawing.Point(80, 124);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(656, 32);
            this.lblTitulo.TabIndex = 10;
            this.lblTitulo.Text = "¡Compra confirmada!";
            //
            // lblPedido
            //
            this.lblPedido.AutoSize = true;
            this.lblPedido.Location = new System.Drawing.Point(80, 164);
            this.lblPedido.Name = "lblPedido";
            this.lblPedido.Size = new System.Drawing.Size(400, 24);
            this.lblPedido.TabIndex = 11;
            this.lblPedido.Text = "Pedido";
            //
            // lblDesglose
            //
            this.lblDesglose.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDesglose.Location = new System.Drawing.Point(8, 8);
            this.lblDesglose.Name = "lblDesglose";
            this.lblDesglose.Padding = new System.Windows.Forms.Padding(8);
            this.lblDesglose.Size = new System.Drawing.Size(320, 304);
            this.lblDesglose.TabIndex = 12;
            //
            // lblTickets
            //
            this.lblTickets.Location = new System.Drawing.Point(400, 276);
            this.lblTickets.Name = "lblTickets";
            this.lblTickets.Size = new System.Drawing.Size(336, 24);
            this.lblTickets.TabIndex = 13;
            this.lblTickets.Text = "Entradas generadas en:";
            //
            // txtCarpeta
            //
            this.txtCarpeta.Location = new System.Drawing.Point(400, 308);
            this.txtCarpeta.Name = "txtCarpeta";
            this.txtCarpeta.ReadOnly = true;
            this.txtCarpeta.Size = new System.Drawing.Size(336, 28);
            this.txtCarpeta.TabIndex = 2;
            //
            // lstArchivos
            //
            this.lstArchivos.FormattingEnabled = true;
            this.lstArchivos.ItemHeight = 32;
            this.lstArchivos.IntegralHeight = false;
            this.lstArchivos.Location = new System.Drawing.Point(0, 0);
            this.lstArchivos.Name = "lstArchivos";
            this.lstArchivos.Size = new System.Drawing.Size(336, 120);
            this.lstArchivos.TabIndex = 3;
            //
            // btnAbrirCarpeta
            //
            this.btnAbrirCarpeta.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAbrirCarpeta.Location = new System.Drawing.Point(24, 492);
            this.btnAbrirCarpeta.Name = "btnAbrirCarpeta";
            this.btnAbrirCarpeta.Size = new System.Drawing.Size(176, 40);
            this.btnAbrirCarpeta.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.btnAbrirCarpeta.TabIndex = 1;
            this.btnAbrirCarpeta.Text = "Abrir carpeta";
            this.btnAbrirCarpeta.Click += new System.EventHandler(this.btnAbrirCarpeta_Click);
            //
            // btnAceptar
            //
            this.btnAceptar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAceptar.Location = new System.Drawing.Point(568, 492);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(168, 40);
            this.btnAceptar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.btnAceptar.TabIndex = 0;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            //
            // frmConfirmacion
            //
            this.AcceptButton = this.btnAceptar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(760, 548);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.btnAbrirCarpeta);
            this.Controls.Add(this.pnlArchivos);
            this.Controls.Add(this.txtCarpeta);
            this.Controls.Add(this.lblTickets);

            this.Controls.Add(this.lblPedido);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfirmacion";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Compra confirmada";
            this.Load += new System.EventHandler(this.frmConfirmacion_Load);
            this.barraSuperior = new Visual.Componentes.BarraSuperior();
            this.barraSuperior.Name = "barraSuperior";
            this.barraSuperior.Size = new System.Drawing.Size(760, 60);
            this.barraSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.Controls.Add(this.barraSuperior);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Visual.Componentes.IndicadorPasos indicadorPasos;
        private Visual.Componentes.IconoExito iconoExito;
        private Visual.Componentes.EtiquetaTotal lblTotalConfirmado;
        private System.Windows.Forms.Panel pnlDetalle;

        private Visual.Componentes.EtiquetaTitulo lblTitulo;
        private Visual.Componentes.EtiquetaSeccion lblPedido;
        private System.Windows.Forms.Label lblDesglose;
        private System.Windows.Forms.Label lblTickets;
        private System.Windows.Forms.TextBox txtCarpeta;
        private System.Windows.Forms.ListBox lstArchivos;
        private System.Windows.Forms.Panel pnlArchivos;
        private Visual.Componentes.BotonSecundario btnAbrirCarpeta;
        private Visual.Componentes.BotonPrimario btnAceptar;
        private Visual.Componentes.BarraSuperior barraSuperior;
    }
}
