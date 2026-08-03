namespace Visual
{
    partial class frmMenuVendedor
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
            lblNombre = new System.Windows.Forms.Label();
            lblvolver = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            btnRegistrarLocal = new System.Windows.Forms.Button();
            btnMisLocales = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblNombre.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            lblNombre.Location = new System.Drawing.Point(255, 69);
            lblNombre.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new System.Drawing.Size(214, 50);
            lblNombre.TabIndex = 38;
            lblNombre.Text = "[USUARIO]";
            lblNombre.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblvolver
            // 
            lblvolver.AutoSize = true;
            lblvolver.Cursor = System.Windows.Forms.Cursors.Hand;
            lblvolver.Font = new System.Drawing.Font("Nirmala UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblvolver.ForeColor = System.Drawing.Color.FromArgb(164, 165, 169);
            lblvolver.Location = new System.Drawing.Point(171, 489);
            lblvolver.Name = "lblvolver";
            lblvolver.Size = new System.Drawing.Size(189, 28);
            lblvolver.TabIndex = 37;
            lblvolver.Text = "VOLVER AL INICIO ";
            lblvolver.Click += lblvolver_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Nirmala UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label1.ForeColor = System.Drawing.Color.FromArgb(116, 86, 174);
            label1.Location = new System.Drawing.Point(29, 69);
            label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(228, 50);
            label1.TabIndex = 36;
            label1.Text = "Bienvenido ";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRegistrarLocal
            // 
            btnRegistrarLocal.BackColor = System.Drawing.Color.FromArgb(116, 86, 174);
            btnRegistrarLocal.Cursor = System.Windows.Forms.Cursors.Hand;
            btnRegistrarLocal.ForeColor = System.Drawing.Color.White;
            btnRegistrarLocal.Location = new System.Drawing.Point(110, 334);
            btnRegistrarLocal.Margin = new System.Windows.Forms.Padding(2);
            btnRegistrarLocal.Name = "btnRegistrarLocal";
            btnRegistrarLocal.Size = new System.Drawing.Size(309, 111);
            btnRegistrarLocal.TabIndex = 35;
            btnRegistrarLocal.Text = "Registrar local";
            btnRegistrarLocal.UseVisualStyleBackColor = false;
            btnRegistrarLocal.Click += btnRegistrarLocal_Click;
            // 
            // btnMisLocales
            // 
            btnMisLocales.BackColor = System.Drawing.Color.FromArgb(116, 86, 174);
            btnMisLocales.Cursor = System.Windows.Forms.Cursors.Hand;
            btnMisLocales.ForeColor = System.Drawing.Color.White;
            btnMisLocales.Location = new System.Drawing.Point(110, 181);
            btnMisLocales.Margin = new System.Windows.Forms.Padding(2);
            btnMisLocales.Name = "btnMisLocales";
            btnMisLocales.Size = new System.Drawing.Size(309, 113);
            btnMisLocales.TabIndex = 34;
            btnMisLocales.Text = "Mis locales";
            btnMisLocales.UseVisualStyleBackColor = false;
            btnMisLocales.Click += btnMisLocales_Click;
            // 
            // frmMenuVendedor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(498, 586);
            Controls.Add(lblNombre);
            Controls.Add(lblvolver);
            Controls.Add(label1);
            Controls.Add(btnRegistrarLocal);
            Controls.Add(btnMisLocales);
            Name = "frmMenuVendedor";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "MenuVendedor";
            Load += frmMenuVendedor_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Label lblvolver;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRegistrarLocal;
        private System.Windows.Forms.Button btnMisLocales;
    }
}