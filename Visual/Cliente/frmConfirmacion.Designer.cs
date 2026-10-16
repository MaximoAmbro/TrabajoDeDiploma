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
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblPedido = new System.Windows.Forms.Label();
            this.lblDesglose = new System.Windows.Forms.Label();
            this.lblTickets = new System.Windows.Forms.Label();
            this.txtCarpeta = new System.Windows.Forms.TextBox();
            this.lstArchivos = new System.Windows.Forms.ListBox();
            this.btnAbrirCarpeta = new System.Windows.Forms.Button();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.lblTitulo.Location = new System.Drawing.Point(20, 14);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(330, 50);
            this.lblTitulo.TabIndex = 10;
            this.lblTitulo.Text = "¡Compra confirmada!";
            //
            // lblPedido
            //
            this.lblPedido.AutoSize = true;
            this.lblPedido.Font = new System.Drawing.Font("Nirmala UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPedido.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblPedido.Location = new System.Drawing.Point(24, 66);
            this.lblPedido.Name = "lblPedido";
            this.lblPedido.Size = new System.Drawing.Size(80, 32);
            this.lblPedido.TabIndex = 11;
            this.lblPedido.Text = "Pedido";
            //
            // lblDesglose
            //
            this.lblDesglose.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDesglose.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDesglose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblDesglose.Location = new System.Drawing.Point(28, 106);
            this.lblDesglose.Name = "lblDesglose";
            this.lblDesglose.Padding = new System.Windows.Forms.Padding(8);
            this.lblDesglose.Size = new System.Drawing.Size(400, 236);
            this.lblDesglose.TabIndex = 12;
            //
            // lblTickets
            //
            this.lblTickets.Location = new System.Drawing.Point(24, 352);
            this.lblTickets.Name = "lblTickets";
            this.lblTickets.Size = new System.Drawing.Size(404, 22);
            this.lblTickets.TabIndex = 13;
            this.lblTickets.Text = "Entradas generadas en:";
            //
            // txtCarpeta
            //
            this.txtCarpeta.Location = new System.Drawing.Point(28, 376);
            this.txtCarpeta.Name = "txtCarpeta";
            this.txtCarpeta.ReadOnly = true;
            this.txtCarpeta.Size = new System.Drawing.Size(400, 26);
            this.txtCarpeta.TabIndex = 2;
            //
            // lstArchivos
            //
            this.lstArchivos.FormattingEnabled = true;
            this.lstArchivos.ItemHeight = 19;
            this.lstArchivos.Location = new System.Drawing.Point(28, 408);
            this.lstArchivos.Name = "lstArchivos";
            this.lstArchivos.Size = new System.Drawing.Size(400, 99);
            this.lstArchivos.TabIndex = 3;
            //
            // btnAbrirCarpeta
            //
            this.btnAbrirCarpeta.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAbrirCarpeta.Location = new System.Drawing.Point(28, 520);
            this.btnAbrirCarpeta.Name = "btnAbrirCarpeta";
            this.btnAbrirCarpeta.Size = new System.Drawing.Size(150, 44);
            this.btnAbrirCarpeta.TabIndex = 1;
            this.btnAbrirCarpeta.Text = "Abrir carpeta";
            this.btnAbrirCarpeta.UseVisualStyleBackColor = true;
            this.btnAbrirCarpeta.Click += new System.EventHandler(this.btnAbrirCarpeta_Click);
            //
            // btnAceptar
            //
            this.btnAceptar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.btnAceptar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAceptar.FlatAppearance.BorderSize = 0;
            this.btnAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAceptar.ForeColor = System.Drawing.Color.White;
            this.btnAceptar.Location = new System.Drawing.Point(248, 520);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(180, 44);
            this.btnAceptar.TabIndex = 0;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = false;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            //
            // frmConfirmacion
            //
            this.AcceptButton = this.btnAceptar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(456, 584);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.btnAbrirCarpeta);
            this.Controls.Add(this.lstArchivos);
            this.Controls.Add(this.txtCarpeta);
            this.Controls.Add(this.lblTickets);
            this.Controls.Add(this.lblDesglose);
            this.Controls.Add(this.lblPedido);
            this.Controls.Add(this.lblTitulo);
            this.Font = new System.Drawing.Font("Nirmala UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(165)))), ((int)(((byte)(169)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfirmacion";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Compra confirmada";
            this.Load += new System.EventHandler(this.frmConfirmacion_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblPedido;
        private System.Windows.Forms.Label lblDesglose;
        private System.Windows.Forms.Label lblTickets;
        private System.Windows.Forms.TextBox txtCarpeta;
        private System.Windows.Forms.ListBox lstArchivos;
        private System.Windows.Forms.Button btnAbrirCarpeta;
        private System.Windows.Forms.Button btnAceptar;
    }
}
