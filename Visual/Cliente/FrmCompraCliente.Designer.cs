namespace Visual
{
    partial class FrmCompraCliente
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
            this.btnVolver = new System.Windows.Forms.Button();
            this.btnComprar = new System.Windows.Forms.Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.dgvSectores)).BeginInit();
            this.pnlResumen.SuspendLayout();
            this.SuspendLayout();
            //
            // btnVolver
            //
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.Location = new System.Drawing.Point(9, 552);
            this.btnVolver.Margin = new System.Windows.Forms.Padding(2);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(150, 34);
            this.btnVolver.TabIndex = 1;
            this.btnVolver.Text = "Volver al listado";
            this.btnVolver.UseVisualStyleBackColor = true;
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            //
            // btnComprar
            //
            this.btnComprar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.btnComprar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnComprar.FlatAppearance.BorderSize = 0;
            this.btnComprar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnComprar.ForeColor = System.Drawing.Color.White;
            this.btnComprar.Location = new System.Drawing.Point(82, 466);
            this.btnComprar.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.btnComprar.Name = "btnComprar";
            this.btnComprar.Size = new System.Drawing.Size(222, 44);
            this.btnComprar.TabIndex = 35;
            this.btnComprar.Text = "Comprar";
            this.btnComprar.UseVisualStyleBackColor = false;
            this.btnComprar.Click += new System.EventHandler(this.btnComprar_Click);
            //
            // label4
            //
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 192);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 19);
            this.label4.TabIndex = 32;
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.label1.Location = new System.Drawing.Point(33, 68);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(329, 50);
            this.label1.TabIndex = 27;
            this.label1.Text = "Selecciona tickets";
            //
            // lblNivel
            //
            this.lblNivel.AutoSize = true;
            this.lblNivel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.lblNivel.Location = new System.Drawing.Point(17, 122);
            this.lblNivel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNivel.Name = "lblNivel";
            this.lblNivel.Size = new System.Drawing.Size(40, 19);
            this.lblNivel.TabIndex = 49;
            this.lblNivel.Text = "Nivel";
            //
            // lnkNiveles
            //
            this.lnkNiveles.AutoSize = true;
            this.lnkNiveles.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.lnkNiveles.Location = new System.Drawing.Point(230, 122);
            this.lnkNiveles.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnkNiveles.Name = "lnkNiveles";
            this.lnkNiveles.Size = new System.Drawing.Size(147, 19);
            this.lnkNiveles.TabIndex = 50;
            this.lnkNiveles.TabStop = true;
            this.lnkNiveles.Text = "¿Cómo funcionan los niveles?";
            this.lnkNiveles.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkNiveles_LinkClicked);
            //
            // dgvSectores
            //
            this.dgvSectores.Location = new System.Drawing.Point(17, 148);
            this.dgvSectores.Margin = new System.Windows.Forms.Padding(2);
            this.dgvSectores.Name = "dgvSectores";
            this.dgvSectores.Size = new System.Drawing.Size(360, 170);
            this.dgvSectores.TabIndex = 36;
            //
            // chkHospedaje
            //
            this.chkHospedaje.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkHospedaje.Location = new System.Drawing.Point(17, 326);
            this.chkHospedaje.Name = "chkHospedaje";
            this.chkHospedaje.Size = new System.Drawing.Size(175, 23);
            this.chkHospedaje.TabIndex = 37;
            this.chkHospedaje.Text = "Hospedaje";
            this.chkHospedaje.UseVisualStyleBackColor = true;
            //
            // chkTraslado
            //
            this.chkTraslado.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkTraslado.Location = new System.Drawing.Point(200, 326);
            this.chkTraslado.Name = "chkTraslado";
            this.chkTraslado.Size = new System.Drawing.Size(175, 23);
            this.chkTraslado.TabIndex = 38;
            this.chkTraslado.Text = "Traslado";
            this.chkTraslado.UseVisualStyleBackColor = true;
            //
            // chkMerch
            //
            this.chkMerch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkMerch.Location = new System.Drawing.Point(17, 352);
            this.chkMerch.Name = "chkMerch";
            this.chkMerch.Size = new System.Drawing.Size(175, 23);
            this.chkMerch.TabIndex = 39;
            this.chkMerch.Text = "Merchandising";
            this.chkMerch.UseVisualStyleBackColor = true;
            //
            // chkVIP
            //
            this.chkVIP.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkVIP.Location = new System.Drawing.Point(200, 352);
            this.chkVIP.Name = "chkVIP";
            this.chkVIP.Size = new System.Drawing.Size(175, 23);
            this.chkVIP.TabIndex = 40;
            this.chkVIP.Text = "Experiencia VIP";
            this.chkVIP.UseVisualStyleBackColor = true;
            //
            // chkSeguro
            //
            this.chkSeguro.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSeguro.Location = new System.Drawing.Point(17, 384);
            this.chkSeguro.Name = "chkSeguro";
            this.chkSeguro.Size = new System.Drawing.Size(360, 23);
            this.chkSeguro.TabIndex = 41;
            this.chkSeguro.Text = "Seguro de entrada (devolución hasta 48 h antes)";
            this.chkSeguro.UseVisualStyleBackColor = true;
            //
            // pnlResumen
            //
            this.pnlResumen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlResumen.Controls.Add(this.lblResumen);
            this.pnlResumen.Location = new System.Drawing.Point(395, 68);
            this.pnlResumen.Name = "pnlResumen";
            this.pnlResumen.Size = new System.Drawing.Size(350, 442);
            this.pnlResumen.TabIndex = 42;
            //
            // lblResumen
            //
            this.lblResumen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblResumen.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResumen.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblResumen.Location = new System.Drawing.Point(0, 0);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Padding = new System.Windows.Forms.Padding(8);
            this.lblResumen.Size = new System.Drawing.Size(348, 440);
            this.lblResumen.TabIndex = 0;
            //
            // FrmCompraCliente
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(760, 595);
            this.Controls.Add(this.pnlResumen);
            this.Controls.Add(this.chkSeguro);
            this.Controls.Add(this.chkVIP);
            this.Controls.Add(this.chkMerch);
            this.Controls.Add(this.chkTraslado);
            this.Controls.Add(this.chkHospedaje);
            this.Controls.Add(this.dgvSectores);
            this.Controls.Add(this.lnkNiveles);
            this.Controls.Add(this.lblNivel);
            this.Controls.Add(this.btnComprar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnVolver);
            this.Font = new System.Drawing.Font("Nirmala UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(165)))), ((int)(((byte)(169)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FrmCompraCliente";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmCompra";
            this.Load += new System.EventHandler(this.FrmCompraCliente_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSectores)).EndInit();
            this.pnlResumen.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Button btnComprar;
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
    }
}
