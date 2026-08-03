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
            this.lblSectorA = new System.Windows.Forms.Label();
            this.btnComprar = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lblSectorB = new System.Windows.Forms.Label();
            this.lblSectorC = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.NumPrimero = new System.Windows.Forms.NumericUpDown();
            this.NumSegundo = new System.Windows.Forms.NumericUpDown();
            this.NumTercero = new System.Windows.Forms.NumericUpDown();
            this.lblPrecioA = new System.Windows.Forms.Label();
            this.lblPrecioB = new System.Windows.Forms.Label();
            this.lblPrecioC = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.NumPrimero)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumSegundo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumTercero)).BeginInit();
            this.SuspendLayout();
            // 
            // btnVolver
            // 
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.Location = new System.Drawing.Point(9, 552);
            this.btnVolver.Margin = new System.Windows.Forms.Padding(2);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(102, 34);
            this.btnVolver.TabIndex = 1;
            this.btnVolver.Text = "Volver";
            this.btnVolver.UseVisualStyleBackColor = true;
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            // 
            // lblSectorA
            // 
            this.lblSectorA.AutoSize = true;
            this.lblSectorA.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.lblSectorA.Location = new System.Drawing.Point(48, 166);
            this.lblSectorA.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSectorA.Name = "lblSectorA";
            this.lblSectorA.Size = new System.Drawing.Size(96, 19);
            this.lblSectorA.TabIndex = 37;
            this.lblSectorA.Text = "Ticket previa";
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
            // lblSectorB
            // 
            this.lblSectorB.AutoSize = true;
            this.lblSectorB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.lblSectorB.Location = new System.Drawing.Point(48, 262);
            this.lblSectorB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSectorB.Name = "lblSectorB";
            this.lblSectorB.Size = new System.Drawing.Size(104, 19);
            this.lblSectorB.TabIndex = 30;
            this.lblSectorB.Text = "Ticket general";
            // 
            // lblSectorC
            // 
            this.lblSectorC.AutoSize = true;
            this.lblSectorC.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.lblSectorC.Location = new System.Drawing.Point(48, 360);
            this.lblSectorC.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSectorC.Name = "lblSectorC";
            this.lblSectorC.Size = new System.Drawing.Size(74, 19);
            this.lblSectorC.TabIndex = 28;
            this.lblSectorC.Text = "Ticket vip";
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
            // NumPrimero
            // 
            this.NumPrimero.Cursor = System.Windows.Forms.Cursors.Hand;
            this.NumPrimero.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.NumPrimero.Location = new System.Drawing.Point(53, 188);
            this.NumPrimero.Margin = new System.Windows.Forms.Padding(2);
            this.NumPrimero.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.NumPrimero.Name = "NumPrimero";
            this.NumPrimero.Size = new System.Drawing.Size(125, 26);
            this.NumPrimero.TabIndex = 43;
            // 
            // NumSegundo
            // 
            this.NumSegundo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.NumSegundo.Location = new System.Drawing.Point(53, 283);
            this.NumSegundo.Margin = new System.Windows.Forms.Padding(2);
            this.NumSegundo.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.NumSegundo.Name = "NumSegundo";
            this.NumSegundo.Size = new System.Drawing.Size(125, 26);
            this.NumSegundo.TabIndex = 44;
            // 
            // NumTercero
            // 
            this.NumTercero.Cursor = System.Windows.Forms.Cursors.Hand;
            this.NumTercero.Location = new System.Drawing.Point(53, 383);
            this.NumTercero.Margin = new System.Windows.Forms.Padding(2);
            this.NumTercero.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.NumTercero.Name = "NumTercero";
            this.NumTercero.Size = new System.Drawing.Size(125, 26);
            this.NumTercero.TabIndex = 45;
            // 
            // lblPrecioA
            // 
            this.lblPrecioA.AutoSize = true;
            this.lblPrecioA.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.lblPrecioA.Location = new System.Drawing.Point(204, 189);
            this.lblPrecioA.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPrecioA.Name = "lblPrecioA";
            this.lblPrecioA.Size = new System.Drawing.Size(100, 19);
            this.lblPrecioA.TabIndex = 46;
            this.lblPrecioA.Text = "Ticket previa ";
            // 
            // lblPrecioB
            // 
            this.lblPrecioB.AutoSize = true;
            this.lblPrecioB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.lblPrecioB.Location = new System.Drawing.Point(204, 285);
            this.lblPrecioB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPrecioB.Name = "lblPrecioB";
            this.lblPrecioB.Size = new System.Drawing.Size(96, 19);
            this.lblPrecioB.TabIndex = 47;
            this.lblPrecioB.Text = "Ticket previa";
            // 
            // lblPrecioC
            // 
            this.lblPrecioC.AutoSize = true;
            this.lblPrecioC.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.lblPrecioC.Location = new System.Drawing.Point(203, 383);
            this.lblPrecioC.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPrecioC.Name = "lblPrecioC";
            this.lblPrecioC.Size = new System.Drawing.Size(96, 19);
            this.lblPrecioC.TabIndex = 48;
            this.lblPrecioC.Text = "Ticket previa";
            // 
            // FrmCompraCliente
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(394, 595);
            this.Controls.Add(this.lblPrecioC);
            this.Controls.Add(this.lblPrecioB);
            this.Controls.Add(this.lblPrecioA);
            this.Controls.Add(this.NumTercero);
            this.Controls.Add(this.NumSegundo);
            this.Controls.Add(this.NumPrimero);
            this.Controls.Add(this.lblSectorA);
            this.Controls.Add(this.btnComprar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblSectorB);
            this.Controls.Add(this.lblSectorC);
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
            ((System.ComponentModel.ISupportInitialize)(this.NumPrimero)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumSegundo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumTercero)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Label lblSectorA;
        private System.Windows.Forms.Button btnComprar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblSectorB;
        private System.Windows.Forms.Label lblSectorC;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown NumPrimero;
        private System.Windows.Forms.NumericUpDown NumSegundo;
        private System.Windows.Forms.NumericUpDown NumTercero;
        private System.Windows.Forms.Label lblPrecioA;
        private System.Windows.Forms.Label lblPrecioB;
        private System.Windows.Forms.Label lblPrecioC;
    }
}