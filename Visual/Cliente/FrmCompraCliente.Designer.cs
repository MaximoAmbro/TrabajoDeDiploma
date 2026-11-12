namespace Visual
{
    partial class FrmCompraCliente
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.btnVolver = new Visual.Componentes.BotonSecundario();
            this.btnComprar = new Visual.Componentes.BotonPrimario();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new Visual.Componentes.EtiquetaTitulo();
            this.lblNivel = new Visual.Componentes.EtiquetaAyuda();
            this.lnkNiveles = new System.Windows.Forms.LinkLabel();
            this.dgvSectores = new System.Windows.Forms.DataGridView();
            this.chkHospedaje = new System.Windows.Forms.CheckBox();
            this.chkTraslado = new System.Windows.Forms.CheckBox();
            this.chkMerch = new System.Windows.Forms.CheckBox();
            this.chkVIP = new System.Windows.Forms.CheckBox();
            this.chkSeguro = new System.Windows.Forms.CheckBox();
            this.pnlResumen = new System.Windows.Forms.Panel();
            this.lblResumen = new System.Windows.Forms.Label();
            this.lblAgotado = new System.Windows.Forms.Label();
            this.btnListaEspera = new Visual.Componentes.BotonSecundario();
            this.pnlEncabezadoCompra = new System.Windows.Forms.Panel();
            this.pnlContenidoCompra = new System.Windows.Forms.Panel();
            this.pnlAccionesCompra = new System.Windows.Forms.Panel();
            this.pnlTarjetaEntradas = new System.Windows.Forms.Panel();
            this.pnlTarjetaServicios = new System.Windows.Forms.Panel();
            this.lblTituloEntradas = new Visual.Componentes.EtiquetaSeccion();
            this.lblTituloServicios = new Visual.Componentes.EtiquetaSeccion();
            this.lblTituloResumen = new Visual.Componentes.EtiquetaSeccion();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSectores)).BeginInit();
            this.pnlEncabezadoCompra.SuspendLayout();
            this.pnlContenidoCompra.SuspendLayout();
            this.pnlAccionesCompra.SuspendLayout();
            this.pnlTarjetaEntradas.SuspendLayout();
            this.pnlTarjetaServicios.SuspendLayout();
            this.pnlResumen.SuspendLayout();
            this.SuspendLayout();
            //
            // btnVolver
            //
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(168, 32);
            this.btnVolver.Text = "Volver al listado";
            this.btnVolver.TabIndex = 0;
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            //
            // btnComprar
            //
            this.btnComprar.Name = "btnComprar";
            this.btnComprar.Size = new System.Drawing.Size(160, 40);
            this.btnComprar.Text = "Comprar";
            this.btnComprar.TabIndex = 1;
            this.btnComprar.Click += new System.EventHandler(this.btnComprar_Click);
            //
            // label4
            //
            this.label4.Name = "label4";
            this.label4.AutoSize = true;
            this.label4.Visible = false;
            this.label4.Text = "";
            //
            // label1
            //
            this.label1.Name = "label1";
            this.label1.Location = new System.Drawing.Point(24, 8);
            this.label1.Size = new System.Drawing.Size(832, 40);
            this.label1.UseMnemonic = false;
            this.label1.Text = "Seleccioná tus entradas";
            //
            // lblNivel
            //
            this.lblNivel.Name = "lblNivel";
            this.lblNivel.AutoSize = true;
            this.lblNivel.Location = new System.Drawing.Point(24, 48);
            this.lblNivel.Text = "Nivel";
            //
            // lnkNiveles
            //
            this.lnkNiveles.Name = "lnkNiveles";
            this.lnkNiveles.AutoSize = true;
            this.lnkNiveles.Location = new System.Drawing.Point(320, 48);
            this.lnkNiveles.Text = "¿Cómo funcionan los niveles?";
            this.lnkNiveles.TabIndex = 0;
            this.lnkNiveles.TabStop = true;
            this.lnkNiveles.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkNiveles_LinkClicked);
            //
            // dgvSectores
            //
            this.dgvSectores.Name = "dgvSectores";
            this.dgvSectores.Location = new System.Drawing.Point(16, 56);
            this.dgvSectores.Size = new System.Drawing.Size(368, 152);
            this.dgvSectores.TabIndex = 0;
            this.dgvSectores.StandardTab = true;
            //
            // chkHospedaje
            //
            this.chkHospedaje.Name = "chkHospedaje";
            this.chkHospedaje.Location = new System.Drawing.Point(16, 56);
            this.chkHospedaje.Size = new System.Drawing.Size(368, 24);
            this.chkHospedaje.TabIndex = 0;
            this.chkHospedaje.Text = "Hospedaje";
            //
            // chkTraslado
            //
            this.chkTraslado.Name = "chkTraslado";
            this.chkTraslado.Location = new System.Drawing.Point(16, 88);
            this.chkTraslado.Size = new System.Drawing.Size(368, 24);
            this.chkTraslado.TabIndex = 1;
            this.chkTraslado.Text = "Traslado";
            //
            // chkMerch
            //
            this.chkMerch.Name = "chkMerch";
            this.chkMerch.Location = new System.Drawing.Point(16, 120);
            this.chkMerch.Size = new System.Drawing.Size(368, 24);
            this.chkMerch.TabIndex = 2;
            this.chkMerch.Text = "Merchandising";
            //
            // chkVIP
            //
            this.chkVIP.Name = "chkVIP";
            this.chkVIP.Location = new System.Drawing.Point(16, 152);
            this.chkVIP.Size = new System.Drawing.Size(368, 24);
            this.chkVIP.TabIndex = 3;
            this.chkVIP.Text = "Experiencia VIP";
            //
            // chkSeguro
            //
            this.chkSeguro.Name = "chkSeguro";
            this.chkSeguro.Location = new System.Drawing.Point(16, 192);
            this.chkSeguro.Size = new System.Drawing.Size(368, 40);
            this.chkSeguro.TabIndex = 4;
            this.chkSeguro.Text = "Seguro de entrada (devolución hasta 48 h antes)";
            //
            // pnlResumen
            //
            this.pnlResumen.Name = "pnlResumen";
            this.pnlResumen.Location = new System.Drawing.Point(448, 24);
            this.pnlResumen.Size = new System.Drawing.Size(408, 496);
            this.pnlResumen.TabIndex = 2;
            this.pnlResumen.AutoScroll = true;
            this.pnlResumen.Controls.Add(this.lblResumen);
            this.pnlResumen.Controls.Add(this.lblTituloResumen);
            //
            // lblResumen
            //
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.AutoSize = true;
            this.lblResumen.Location = new System.Drawing.Point(16, 56);
            this.lblResumen.Padding = new System.Windows.Forms.Padding(8);
            this.lblResumen.UseMnemonic = false;
            //
            // lblAgotado
            //
            this.lblAgotado.Name = "lblAgotado";
            this.lblAgotado.Location = new System.Drawing.Point(16, 56);
            this.lblAgotado.Size = new System.Drawing.Size(368, 80);
            this.lblAgotado.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAgotado.Visible = false;
            this.lblAgotado.Text = "Entradas agotadas.\r\nAnotate en la lista de espera y te vamos a notificar si se libera un lugar.";
            //
            // btnListaEspera
            //
            this.btnListaEspera.Name = "btnListaEspera";
            this.btnListaEspera.Size = new System.Drawing.Size(320, 32);
            this.btnListaEspera.Text = "Anotarme en la lista de espera";
            this.btnListaEspera.TabIndex = 1;
            this.btnListaEspera.Location = new System.Drawing.Point(40, 152);
            this.btnListaEspera.Visible = false;
            this.btnListaEspera.Click += new System.EventHandler(this.btnListaEspera_Click);
            //
            // pnlEncabezadoCompra
            //
            this.pnlEncabezadoCompra.Name = "pnlEncabezadoCompra";
            this.pnlEncabezadoCompra.Location = new System.Drawing.Point(0, 0);
            this.pnlEncabezadoCompra.Width = 880;
            this.pnlEncabezadoCompra.Height = 72;
            this.pnlEncabezadoCompra.TabIndex = 0;
            this.pnlContenidoCompra.Controls.Add(this.label1);
            this.pnlContenidoCompra.Controls.Add(this.lblNivel);
            this.pnlContenidoCompra.Controls.Add(this.lnkNiveles);
            this.pnlContenidoCompra.Controls.Add(this.label4);
            //
            // pnlContenidoCompra
            //
            this.pnlContenidoCompra.Name = "pnlContenidoCompra";
            this.pnlContenidoCompra.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContenidoCompra.AutoScroll = true;
            this.pnlContenidoCompra.AutoScrollMargin = new System.Drawing.Size(24, 24);
            this.pnlContenidoCompra.TabIndex = 1;
            this.pnlContenidoCompra.Controls.Add(this.pnlTarjetaEntradas);
            this.pnlContenidoCompra.Controls.Add(this.pnlTarjetaServicios);
            this.pnlContenidoCompra.Controls.Add(this.pnlResumen);
            //
            // pnlAccionesCompra
            //
            this.pnlAccionesCompra.Name = "pnlAccionesCompra";
            this.pnlAccionesCompra.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAccionesCompra.Height = 64;
            this.pnlAccionesCompra.Padding = new System.Windows.Forms.Padding(24, 16, 24, 16);
            this.pnlAccionesCompra.TabIndex = 2;
            this.pnlAccionesCompra.Controls.Add(this.btnVolver);
            this.pnlAccionesCompra.Controls.Add(this.btnComprar);
            //
            // pnlTarjetaEntradas
            //
            this.pnlTarjetaEntradas.Name = "pnlTarjetaEntradas";
            this.pnlTarjetaEntradas.Location = new System.Drawing.Point(24, 24);
            this.pnlTarjetaEntradas.Size = new System.Drawing.Size(400, 224);
            this.pnlTarjetaEntradas.TabIndex = 0;
            this.pnlTarjetaEntradas.Controls.Add(this.lblTituloEntradas);
            this.pnlTarjetaEntradas.Controls.Add(this.dgvSectores);
            this.pnlTarjetaEntradas.Controls.Add(this.lblAgotado);
            this.pnlTarjetaEntradas.Controls.Add(this.btnListaEspera);
            //
            // pnlTarjetaServicios
            //
            this.pnlTarjetaServicios.Name = "pnlTarjetaServicios";
            this.pnlTarjetaServicios.Location = new System.Drawing.Point(24, 272);
            this.pnlTarjetaServicios.Size = new System.Drawing.Size(400, 248);
            this.pnlTarjetaServicios.TabIndex = 1;
            this.pnlTarjetaServicios.Controls.Add(this.lblTituloServicios);
            this.pnlTarjetaServicios.Controls.Add(this.chkHospedaje);
            this.pnlTarjetaServicios.Controls.Add(this.chkTraslado);
            this.pnlTarjetaServicios.Controls.Add(this.chkMerch);
            this.pnlTarjetaServicios.Controls.Add(this.chkVIP);
            this.pnlTarjetaServicios.Controls.Add(this.chkSeguro);
            //
            // lblTituloEntradas
            //
            this.lblTituloEntradas.Name = "lblTituloEntradas";
            this.lblTituloEntradas.AutoSize = true;
            this.lblTituloEntradas.Location = new System.Drawing.Point(16, 16);
            this.lblTituloEntradas.Text = "Entradas";
            //
            // lblTituloServicios
            //
            this.lblTituloServicios.Name = "lblTituloServicios";
            this.lblTituloServicios.AutoSize = true;
            this.lblTituloServicios.Location = new System.Drawing.Point(16, 16);
            this.lblTituloServicios.Text = "Servicios y seguro";
            //
            // lblTituloResumen
            //
            this.lblTituloResumen.Name = "lblTituloResumen";
            this.lblTituloResumen.AutoSize = true;
            this.lblTituloResumen.Location = new System.Drawing.Point(16, 16);
            this.lblTituloResumen.Text = "Resumen";
            //
            // FrmCompraCliente
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(880, 544);
            this.CancelButton = this.btnVolver;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmCompraCliente";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EventAura · Armar paquete";
            this.Load += new System.EventHandler(this.FrmCompraCliente_Load);
            this.Controls.Add(this.pnlContenidoCompra);
            this.Controls.Add(this.pnlAccionesCompra);
            this.pnlContenidoCompra.Controls.Add(this.pnlEncabezadoCompra);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSectores)).EndInit();
            this.pnlEncabezadoCompra.ResumeLayout(false);
            this.pnlEncabezadoCompra.PerformLayout();
            this.pnlContenidoCompra.ResumeLayout(false);
            this.pnlContenidoCompra.PerformLayout();
            this.pnlAccionesCompra.ResumeLayout(false);
            this.pnlAccionesCompra.PerformLayout();
            this.pnlTarjetaEntradas.ResumeLayout(false);
            this.pnlTarjetaEntradas.PerformLayout();
            this.pnlTarjetaServicios.ResumeLayout(false);
            this.pnlTarjetaServicios.PerformLayout();
            this.pnlResumen.ResumeLayout(false);
            this.pnlResumen.PerformLayout();
            this.ResumeLayout(false);
        }
        #endregion

        private Visual.Componentes.BotonSecundario btnVolver;
        private Visual.Componentes.BotonPrimario btnComprar;
        private System.Windows.Forms.Label label4;
        private Visual.Componentes.EtiquetaTitulo label1;
        private Visual.Componentes.EtiquetaAyuda lblNivel;
        private System.Windows.Forms.LinkLabel lnkNiveles;
        private System.Windows.Forms.DataGridView dgvSectores;
        private System.Windows.Forms.CheckBox chkHospedaje;
        private System.Windows.Forms.CheckBox chkTraslado;
        private System.Windows.Forms.CheckBox chkMerch;
        private System.Windows.Forms.CheckBox chkVIP;
        private System.Windows.Forms.CheckBox chkSeguro;
        private System.Windows.Forms.Panel pnlResumen;
        private System.Windows.Forms.Label lblResumen;
        private System.Windows.Forms.Label lblAgotado;
        private Visual.Componentes.BotonSecundario btnListaEspera;
        private System.Windows.Forms.Panel pnlEncabezadoCompra;
        private System.Windows.Forms.Panel pnlContenidoCompra;
        private System.Windows.Forms.Panel pnlAccionesCompra;
        private System.Windows.Forms.Panel pnlTarjetaEntradas;
        private System.Windows.Forms.Panel pnlTarjetaServicios;
        private Visual.Componentes.EtiquetaSeccion lblTituloEntradas;
        private Visual.Componentes.EtiquetaSeccion lblTituloServicios;
        private Visual.Componentes.EtiquetaSeccion lblTituloResumen;
    }
}
