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
            btnVolver = new System.Windows.Forms.Button();
            lblSectorA = new System.Windows.Forms.Label();
            btnComprar = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            lblSectorB = new System.Windows.Forms.Label();
            lblSectorC = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            NumPrimero = new System.Windows.Forms.NumericUpDown();
            NumSegundo = new System.Windows.Forms.NumericUpDown();
            NumTercero = new System.Windows.Forms.NumericUpDown();
            lblPrecioA = new System.Windows.Forms.Label();
            lblPrecioB = new System.Windows.Forms.Label();
            lblPrecioC = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)NumPrimero).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumSegundo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumTercero).BeginInit();
            SuspendLayout();
            // 
            // btnVolver
            // 
            btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            btnVolver.Location = new System.Drawing.Point(192, 663);
            btnVolver.Margin = new System.Windows.Forms.Padding(2);
            btnVolver.Name = "btnVolver";
            btnVolver.Size = new System.Drawing.Size(122, 41);
            btnVolver.TabIndex = 1;
            btnVolver.Text = "Volver";
            btnVolver.UseVisualStyleBackColor = true;
            btnVolver.Click += btnVolver_Click_1;
            // 
            // lblSectorA
            // 
            lblSectorA.AutoSize = true;
            lblSectorA.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            lblSectorA.Location = new System.Drawing.Point(30, 198);
            lblSectorA.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblSectorA.Name = "lblSectorA";
            lblSectorA.Size = new System.Drawing.Size(129, 23);
            lblSectorA.TabIndex = 37;
            lblSectorA.Text = "TICKET PREVIA";
            // 
            // btnComprar
            // 
            btnComprar.BackColor = System.Drawing.Color.FromArgb(116, 86, 174);
            btnComprar.Cursor = System.Windows.Forms.Cursors.Hand;
            btnComprar.FlatAppearance.BorderSize = 0;
            btnComprar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnComprar.ForeColor = System.Drawing.Color.White;
            btnComprar.Location = new System.Drawing.Point(128, 588);
            btnComprar.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            btnComprar.Name = "btnComprar";
            btnComprar.Size = new System.Drawing.Size(266, 53);
            btnComprar.TabIndex = 35;
            btnComprar.Text = "COMPRAR";
            btnComprar.UseVisualStyleBackColor = false;
            btnComprar.Click += btnComprar_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(16, 230);
            label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(0, 23);
            label4.TabIndex = 32;
            // 
            // lblSectorB
            // 
            lblSectorB.AutoSize = true;
            lblSectorB.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            lblSectorB.Location = new System.Drawing.Point(30, 313);
            lblSectorB.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblSectorB.Name = "lblSectorB";
            lblSectorB.Size = new System.Drawing.Size(151, 23);
            lblSectorB.TabIndex = 30;
            lblSectorB.Text = "TICKET GENERAL ";
            // 
            // lblSectorC
            // 
            lblSectorC.AutoSize = true;
            lblSectorC.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            lblSectorC.Location = new System.Drawing.Point(30, 431);
            lblSectorC.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblSectorC.Name = "lblSectorC";
            lblSectorC.Size = new System.Drawing.Size(97, 23);
            lblSectorC.TabIndex = 28;
            lblSectorC.Text = "TICKET VIP";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label1.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            label1.Location = new System.Drawing.Point(30, 67);
            label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(391, 60);
            label1.TabIndex = 27;
            label1.Text = "Selecciona tickets";
            // 
            // NumPrimero
            // 
            NumPrimero.Cursor = System.Windows.Forms.Cursors.Hand;
            NumPrimero.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            NumPrimero.Location = new System.Drawing.Point(36, 224);
            NumPrimero.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            NumPrimero.Name = "NumPrimero";
            NumPrimero.Size = new System.Drawing.Size(150, 29);
            NumPrimero.TabIndex = 43;
            // 
            // NumSegundo
            // 
            NumSegundo.Cursor = System.Windows.Forms.Cursors.Hand;
            NumSegundo.Location = new System.Drawing.Point(36, 339);
            NumSegundo.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            NumSegundo.Name = "NumSegundo";
            NumSegundo.Size = new System.Drawing.Size(150, 29);
            NumSegundo.TabIndex = 44;
            // 
            // NumTercero
            // 
            NumTercero.Cursor = System.Windows.Forms.Cursors.Hand;
            NumTercero.Location = new System.Drawing.Point(36, 459);
            NumTercero.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            NumTercero.Name = "NumTercero";
            NumTercero.Size = new System.Drawing.Size(150, 29);
            NumTercero.TabIndex = 45;
            // 
            // lblPrecioA
            // 
            lblPrecioA.AutoSize = true;
            lblPrecioA.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            lblPrecioA.Location = new System.Drawing.Point(217, 226);
            lblPrecioA.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblPrecioA.Name = "lblPrecioA";
            lblPrecioA.Size = new System.Drawing.Size(129, 23);
            lblPrecioA.TabIndex = 46;
            lblPrecioA.Text = "TICKET PREVIA";
            // 
            // lblPrecioB
            // 
            lblPrecioB.AutoSize = true;
            lblPrecioB.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            lblPrecioB.Location = new System.Drawing.Point(217, 341);
            lblPrecioB.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblPrecioB.Name = "lblPrecioB";
            lblPrecioB.Size = new System.Drawing.Size(129, 23);
            lblPrecioB.TabIndex = 47;
            lblPrecioB.Text = "TICKET PREVIA";
            // 
            // lblPrecioC
            // 
            lblPrecioC.AutoSize = true;
            lblPrecioC.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            lblPrecioC.Location = new System.Drawing.Point(216, 459);
            lblPrecioC.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblPrecioC.Name = "lblPrecioC";
            lblPrecioC.Size = new System.Drawing.Size(129, 23);
            lblPrecioC.TabIndex = 48;
            lblPrecioC.Text = "TICKET PREVIA";
            // 
            // FrmCompra
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(514, 731);
            Controls.Add(lblPrecioC);
            Controls.Add(lblPrecioB);
            Controls.Add(lblPrecioA);
            Controls.Add(NumTercero);
            Controls.Add(NumSegundo);
            Controls.Add(NumPrimero);
            Controls.Add(lblSectorA);
            Controls.Add(btnComprar);
            Controls.Add(label4);
            Controls.Add(lblSectorB);
            Controls.Add(lblSectorC);
            Controls.Add(label1);
            Controls.Add(btnVolver);
            Font = new System.Drawing.Font("Nirmala UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            ForeColor = System.Drawing.Color.FromArgb(164, 165, 169);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(2);
            Name = "FrmCompra";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "FrmCompra";
            Load += FrmCompra_Load;
            ((System.ComponentModel.ISupportInitialize)NumPrimero).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumSegundo).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumTercero).EndInit();
            ResumeLayout(false);
            PerformLayout();

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