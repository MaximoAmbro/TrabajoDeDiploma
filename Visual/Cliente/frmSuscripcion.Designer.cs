namespace Visual
{
    partial class frmSuscripcion
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
            this.label1 = new System.Windows.Forms.Label();
            this.lblEstado = new System.Windows.Forms.Label();
            this.lblBeneficios = new System.Windows.Forms.Label();
            this.rdbMensual = new System.Windows.Forms.RadioButton();
            this.rdbAnual = new System.Windows.Forms.RadioButton();
            this.btnContratar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnVolver = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.label1.Location = new System.Drawing.Point(64, 30);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(268, 50);
            this.label1.TabIndex = 30;
            this.label1.Text = "Mi suscripción";
            //
            // lblEstado
            //
            this.lblEstado.Font = new System.Drawing.Font("Nirmala UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstado.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.lblEstado.Location = new System.Drawing.Point(30, 96);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(334, 78);
            this.lblEstado.TabIndex = 31;
            this.lblEstado.Text = "Estado";
            this.lblEstado.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblBeneficios
            //
            this.lblBeneficios.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblBeneficios.Location = new System.Drawing.Point(30, 182);
            this.lblBeneficios.Name = "lblBeneficios";
            this.lblBeneficios.Size = new System.Drawing.Size(334, 130);
            this.lblBeneficios.TabIndex = 32;
            this.lblBeneficios.Text = "Beneficios";
            //
            // rdbMensual
            //
            this.rdbMensual.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdbMensual.Location = new System.Drawing.Point(67, 318);
            this.rdbMensual.Name = "rdbMensual";
            this.rdbMensual.Size = new System.Drawing.Size(258, 26);
            this.rdbMensual.TabIndex = 0;
            this.rdbMensual.TabStop = true;
            this.rdbMensual.Text = "Mensual";
            this.rdbMensual.UseVisualStyleBackColor = true;
            //
            // rdbAnual
            //
            this.rdbAnual.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdbAnual.Location = new System.Drawing.Point(67, 348);
            this.rdbAnual.Name = "rdbAnual";
            this.rdbAnual.Size = new System.Drawing.Size(258, 26);
            this.rdbAnual.TabIndex = 1;
            this.rdbAnual.Text = "Anual";
            this.rdbAnual.UseVisualStyleBackColor = true;
            //
            // btnContratar
            //
            this.btnContratar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(86)))), ((int)(((byte)(174)))));
            this.btnContratar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnContratar.ForeColor = System.Drawing.Color.White;
            this.btnContratar.Location = new System.Drawing.Point(67, 386);
            this.btnContratar.Margin = new System.Windows.Forms.Padding(2);
            this.btnContratar.Name = "btnContratar";
            this.btnContratar.Size = new System.Drawing.Size(258, 60);
            this.btnContratar.TabIndex = 2;
            this.btnContratar.Text = "Contratar";
            this.btnContratar.UseVisualStyleBackColor = false;
            this.btnContratar.Click += new System.EventHandler(this.btnContratar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.Location = new System.Drawing.Point(67, 458);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(258, 40);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "Cancelar suscripción";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // btnVolver
            //
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.Location = new System.Drawing.Point(9, 548);
            this.btnVolver.Margin = new System.Windows.Forms.Padding(2);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(113, 36);
            this.btnVolver.TabIndex = 4;
            this.btnVolver.Text = "Volver";
            this.btnVolver.UseVisualStyleBackColor = true;
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            //
            // frmSuscripcion
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(394, 595);
            this.Controls.Add(this.btnVolver);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnContratar);
            this.Controls.Add(this.rdbAnual);
            this.Controls.Add(this.rdbMensual);
            this.Controls.Add(this.lblBeneficios);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Nirmala UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(165)))), ((int)(((byte)(169)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmSuscripcion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmSuscripcion";
            this.Load += new System.EventHandler(this.frmSuscripcion_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.Label lblBeneficios;
        private System.Windows.Forms.RadioButton rdbMensual;
        private System.Windows.Forms.RadioButton rdbAnual;
        private System.Windows.Forms.Button btnContratar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnVolver;
    }
}
