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
            this.label1 = new System.Windows.Forms.Label();
            this.lblNivel = new System.Windows.Forms.Label();
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
            this.lblTituloEntradas = new System.Windows.Forms.Label();
            this.lblTituloServicios = new System.Windows.Forms.Label();
            this.lblTituloResumen = new System.Windows.Forms.Label();
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
            this.btnVolver.Size = Tema.Compra.BotonVolver;
            this.btnVolver.Text = "Volver al listado";
            this.btnVolver.TabIndex = 0;
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            //
            // btnComprar
            //
            this.btnComprar.Name = "btnComprar";
            this.btnComprar.Size = Tema.Compra.BotonComprar;
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
            this.label1.Font = Tema.Titulo;
            this.label1.BackColor = Tema.Superficie;
            this.label1.ForeColor = Tema.Texto;
            this.label1.Location = Tema.Compra.Titulo;
            this.label1.Size = Tema.Compra.NombreEvento;
            this.label1.UseMnemonic = false;
            this.label1.Text = "Seleccioná tus entradas";
            //
            // lblNivel
            //
            this.lblNivel.Name = "lblNivel";
            this.lblNivel.AutoSize = true;
            this.lblNivel.Font = Tema.Ayuda;
            this.lblNivel.BackColor = Tema.Superficie;
            this.lblNivel.ForeColor = Tema.TextoSecundario;
            this.lblNivel.Location = Tema.Compra.Nivel;
            this.lblNivel.Text = "Nivel";
            //
            // lnkNiveles
            //
            this.lnkNiveles.Name = "lnkNiveles";
            this.lnkNiveles.AutoSize = true;
            this.lnkNiveles.Font = Tema.Ayuda;
            this.lnkNiveles.BackColor = Tema.Superficie;
            this.lnkNiveles.ForeColor = Tema.TextoSecundario;
            this.lnkNiveles.Location = Tema.Compra.Enlace;
            this.lnkNiveles.Text = "¿Cómo funcionan los niveles?";
            this.lnkNiveles.LinkColor = Tema.Primario;
            this.lnkNiveles.ActiveLinkColor = Tema.PrimarioPresionado;
            this.lnkNiveles.VisitedLinkColor = Tema.Primario;
            this.lnkNiveles.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkNiveles.TabIndex = 0;
            this.lnkNiveles.TabStop = true;
            this.lnkNiveles.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkNiveles_LinkClicked);
            //
            // dgvSectores
            //
            this.dgvSectores.Name = "dgvSectores";
            this.dgvSectores.Location = Tema.Compra.Grilla;
            this.dgvSectores.Size = Tema.Compra.Sectores;
            this.dgvSectores.TabIndex = 0;
            this.dgvSectores.StandardTab = true;
            //
            // chkHospedaje
            //
            this.chkHospedaje.Name = "chkHospedaje";
            this.chkHospedaje.Font = Tema.Cuerpo;
            this.chkHospedaje.ForeColor = Tema.Texto;
            this.chkHospedaje.Location = Tema.Compra.Hospedaje;
            this.chkHospedaje.Size = Tema.Compra.Casilla;
            this.chkHospedaje.TabIndex = 0;
            this.chkHospedaje.Text = "Hospedaje";
            //
            // chkTraslado
            //
            this.chkTraslado.Name = "chkTraslado";
            this.chkTraslado.Font = Tema.Cuerpo;
            this.chkTraslado.ForeColor = Tema.Texto;
            this.chkTraslado.Location = Tema.Compra.Traslado;
            this.chkTraslado.Size = Tema.Compra.Casilla;
            this.chkTraslado.TabIndex = 1;
            this.chkTraslado.Text = "Traslado";
            //
            // chkMerch
            //
            this.chkMerch.Name = "chkMerch";
            this.chkMerch.Font = Tema.Cuerpo;
            this.chkMerch.ForeColor = Tema.Texto;
            this.chkMerch.Location = Tema.Compra.Merch;
            this.chkMerch.Size = Tema.Compra.Casilla;
            this.chkMerch.TabIndex = 2;
            this.chkMerch.Text = "Merchandising";
            //
            // chkVIP
            //
            this.chkVIP.Name = "chkVIP";
            this.chkVIP.Font = Tema.Cuerpo;
            this.chkVIP.ForeColor = Tema.Texto;
            this.chkVIP.Location = Tema.Compra.Vip;
            this.chkVIP.Size = Tema.Compra.Casilla;
            this.chkVIP.TabIndex = 3;
            this.chkVIP.Text = "Experiencia VIP";
            //
            // chkSeguro
            //
            this.chkSeguro.Name = "chkSeguro";
            this.chkSeguro.Font = Tema.Cuerpo;
            this.chkSeguro.ForeColor = Tema.Texto;
            this.chkSeguro.Location = Tema.Compra.Seguro;
            this.chkSeguro.Size = Tema.Compra.CasillaSeguro;
            this.chkSeguro.TabIndex = 4;
            this.chkSeguro.Text = "Seguro de entrada (devolución hasta 48 h antes)";
            //
            // pnlResumen
            //
            this.pnlResumen.Name = "pnlResumen";
            this.pnlResumen.BackColor = Tema.Superficie;
            this.pnlResumen.Location = Tema.Compra.Resumen;
            this.pnlResumen.Size = Tema.Compra.TarjetaResumen;
            this.pnlResumen.TabIndex = 2;
            this.pnlResumen.AutoScroll = true;
            this.pnlResumen.Controls.Add(this.lblResumen);
            this.pnlResumen.Controls.Add(this.lblTituloResumen);
            //
            // lblResumen
            //
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.AutoSize = true;
            this.lblResumen.Font = Tema.Comprobante;
            this.lblResumen.ForeColor = Tema.Texto;
            this.lblResumen.Location = Tema.Compra.Grilla;
            this.lblResumen.Padding = new System.Windows.Forms.Padding(Tema.Espacio.S);
            this.lblResumen.UseMnemonic = false;
            //
            // lblAgotado
            //
            this.lblAgotado.Name = "lblAgotado";
            this.lblAgotado.Font = Tema.Cuerpo;
            this.lblAgotado.ForeColor = Tema.Texto;
            this.lblAgotado.Location = Tema.Compra.Agotado;
            this.lblAgotado.Size = Tema.Compra.AvisoAgotado;
            this.lblAgotado.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAgotado.Visible = false;
            this.lblAgotado.Text = "Entradas agotadas.\r\nAnotate en la lista de espera y te vamos a notificar si se libera un lugar.";
            //
            // btnListaEspera
            //
            this.btnListaEspera.Name = "btnListaEspera";
            this.btnListaEspera.Size = Tema.Compra.BotonEspera;
            this.btnListaEspera.Text = "Anotarme en la lista de espera";
            this.btnListaEspera.TabIndex = 1;
            this.btnListaEspera.Location = Tema.Compra.ListaEspera;
            this.btnListaEspera.Visible = false;
            this.btnListaEspera.Click += new System.EventHandler(this.btnListaEspera_Click);
            //
            // pnlEncabezadoCompra
            //
            this.pnlEncabezadoCompra.Name = "pnlEncabezadoCompra";
            this.pnlEncabezadoCompra.BackColor = Tema.Superficie;
            this.pnlEncabezadoCompra.Location = System.Drawing.Point.Empty;
            this.pnlEncabezadoCompra.Width = Tema.Compra.Ventana.Width;
            this.pnlEncabezadoCompra.Height = Tema.AltoEncabezadoVentana;
            this.pnlEncabezadoCompra.TabIndex = 0;
            this.pnlContenidoCompra.Controls.Add(this.label1);
            this.pnlContenidoCompra.Controls.Add(this.lblNivel);
            this.pnlContenidoCompra.Controls.Add(this.lnkNiveles);
            this.pnlContenidoCompra.Controls.Add(this.label4);
            //
            // pnlContenidoCompra
            //
            this.pnlContenidoCompra.Name = "pnlContenidoCompra";
            this.pnlContenidoCompra.BackColor = Tema.Superficie;
            this.pnlContenidoCompra.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContenidoCompra.AutoScroll = true;
            this.pnlContenidoCompra.BackColor = Tema.Fondo;
            this.pnlContenidoCompra.AutoScrollMargin = new System.Drawing.Size(Tema.Espacio.XL, Tema.Espacio.XL);
            this.pnlContenidoCompra.TabIndex = 1;
            this.pnlContenidoCompra.Controls.Add(this.pnlTarjetaEntradas);
            this.pnlContenidoCompra.Controls.Add(this.pnlTarjetaServicios);
            this.pnlContenidoCompra.Controls.Add(this.pnlResumen);
            //
            // pnlAccionesCompra
            //
            this.pnlAccionesCompra.Name = "pnlAccionesCompra";
            this.pnlAccionesCompra.BackColor = Tema.Superficie;
            this.pnlAccionesCompra.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAccionesCompra.Height = Tema.AltoPie;
            this.pnlAccionesCompra.Padding = new System.Windows.Forms.Padding(Tema.Espacio.XL, Tema.Espacio.M, Tema.Espacio.XL, Tema.Espacio.M);
            this.pnlAccionesCompra.TabIndex = 2;
            this.pnlAccionesCompra.Controls.Add(this.btnVolver);
            this.pnlAccionesCompra.Controls.Add(this.btnComprar);
            //
            // pnlTarjetaEntradas
            //
            this.pnlTarjetaEntradas.Name = "pnlTarjetaEntradas";
            this.pnlTarjetaEntradas.BackColor = Tema.Superficie;
            this.pnlTarjetaEntradas.Location = Tema.Compra.Entradas;
            this.pnlTarjetaEntradas.Size = Tema.Compra.TarjetaEntradas;
            this.pnlTarjetaEntradas.TabIndex = 0;
            this.pnlTarjetaEntradas.Controls.Add(this.lblTituloEntradas);
            this.pnlTarjetaEntradas.Controls.Add(this.dgvSectores);
            this.pnlTarjetaEntradas.Controls.Add(this.lblAgotado);
            this.pnlTarjetaEntradas.Controls.Add(this.btnListaEspera);
            //
            // pnlTarjetaServicios
            //
            this.pnlTarjetaServicios.Name = "pnlTarjetaServicios";
            this.pnlTarjetaServicios.BackColor = Tema.Superficie;
            this.pnlTarjetaServicios.Location = Tema.Compra.Servicios;
            this.pnlTarjetaServicios.Size = Tema.Compra.TarjetaServicios;
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
            this.lblTituloEntradas.Font = Tema.Seccion;
            this.lblTituloEntradas.ForeColor = Tema.Texto;
            this.lblTituloEntradas.Location = Tema.Compra.TituloTarjeta;
            this.lblTituloEntradas.Text = "Entradas";
            //
            // lblTituloServicios
            //
            this.lblTituloServicios.Name = "lblTituloServicios";
            this.lblTituloServicios.AutoSize = true;
            this.lblTituloServicios.Font = Tema.Seccion;
            this.lblTituloServicios.ForeColor = Tema.Texto;
            this.lblTituloServicios.Location = Tema.Compra.TituloTarjeta;
            this.lblTituloServicios.Text = "Servicios y seguro";
            //
            // lblTituloResumen
            //
            this.lblTituloResumen.Name = "lblTituloResumen";
            this.lblTituloResumen.AutoSize = true;
            this.lblTituloResumen.Font = Tema.Seccion;
            this.lblTituloResumen.ForeColor = Tema.Texto;
            this.lblTituloResumen.Location = Tema.Compra.TituloTarjeta;
            this.lblTituloResumen.Text = "Resumen";
            //
            // FrmCompraCliente
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = Tema.Fondo;
            this.ClientSize = new System.Drawing.Size(880, 544);
            this.Font = Tema.Cuerpo;
            this.ForeColor = Tema.Texto;
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblNivel;
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
        private System.Windows.Forms.Label lblTituloEntradas;
        private System.Windows.Forms.Label lblTituloServicios;
        private System.Windows.Forms.Label lblTituloResumen;
    }
}
