namespace Visual.Vendedor
{
    partial class frmAdministrarEvento
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
            this.btnVolver = new Visual.Componentes.BotonSecundario();
            this.label1 = new Visual.Componentes.EtiquetaTitulo();
            this.lblEvento = new Visual.Componentes.EtiquetaSeccion();
            this.dgvSectores = new System.Windows.Forms.DataGridView();
            this.btnAgregarSector = new Visual.Componentes.BotonPrimario();
            this.btnEliminarSector = new Visual.Componentes.BotonPrimario();
            this.label2 = new Visual.Componentes.EtiquetaSeccion();
            this.lblPreventa = new Visual.Componentes.EtiquetaRotulo();
            this.dtpFinPreventa = new System.Windows.Forms.DateTimePicker();
            this.btnGuardarPreventa = new Visual.Componentes.BotonPrimario();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSectores)).BeginInit();
            this.SuspendLayout();
            // 
            // btnVolver
            // 
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.Location = new System.Drawing.Point(24, 384);
            this.btnVolver.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(136, 40);
            this.btnVolver.TabIndex = 47;
            this.btnVolver.Text = "Volver";
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 24);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(792, 32);
            this.label1.TabIndex = 38;
            this.label1.Text = "Administrar evento";
            // 
            // lblEvento
            // 
            this.lblEvento.AutoSize = false;
            this.lblEvento.Location = new System.Drawing.Point(24, 64);
            this.lblEvento.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblEvento.Name = "lblEvento";
            this.lblEvento.Size = new System.Drawing.Size(792, 24);
            this.lblEvento.TabIndex = 52;
            this.lblEvento.Text = "[Evento]";
            // 
            // dgvSectores
            // 
            this.dgvSectores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSectores.Location = new System.Drawing.Point(24, 216);
            this.dgvSectores.Margin = new System.Windows.Forms.Padding(2);
            this.dgvSectores.Name = "dgvSectores";
            this.dgvSectores.RowHeadersWidth = 51;
            this.dgvSectores.Size = new System.Drawing.Size(792, 136);
            this.dgvSectores.TabIndex = 53;
            // 
            // btnAgregarSector
            // 
            this.btnAgregarSector.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAgregarSector.Location = new System.Drawing.Point(464, 384);
            this.btnAgregarSector.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.btnAgregarSector.Name = "btnAgregarSector";
            this.btnAgregarSector.Size = new System.Drawing.Size(168, 40);
            this.btnAgregarSector.TabIndex = 54;
            this.btnAgregarSector.Text = "Agregar sector";
            this.btnAgregarSector.Click += new System.EventHandler(this.btnAgregarSector_Click);
            // 
            // btnEliminarSector
            // 
            this.btnEliminarSector.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminarSector.Location = new System.Drawing.Point(648, 384);
            this.btnEliminarSector.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.btnEliminarSector.Name = "btnEliminarSector";
            this.btnEliminarSector.Size = new System.Drawing.Size(168, 40);
            this.btnEliminarSector.TabIndex = 55;
            this.btnEliminarSector.Text = "Eliminar sector";
            this.btnEliminarSector.Click += new System.EventHandler(this.btnEliminarSector_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = false;
            this.label2.Location = new System.Drawing.Point(24, 184);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(792, 24);
            this.label2.TabIndex = 60;
            this.label2.Text = "Sectores";
            //
            // lblPreventa
            //
            this.lblPreventa.AutoSize = true;
            this.lblPreventa.Location = new System.Drawing.Point(24, 112);
            this.lblPreventa.Name = "lblPreventa";
            this.lblPreventa.Size = new System.Drawing.Size(352, 20);
            this.lblPreventa.TabIndex = 61;
            this.lblPreventa.Text = "Preventa para suscriptores hasta";
            //
            // dtpFinPreventa
            //
            this.dtpFinPreventa.Checked = false;
            this.dtpFinPreventa.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dtpFinPreventa.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dtpFinPreventa.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFinPreventa.Location = new System.Drawing.Point(24, 136);
            this.dtpFinPreventa.Name = "dtpFinPreventa";
            this.dtpFinPreventa.ShowCheckBox = true;
            this.dtpFinPreventa.Size = new System.Drawing.Size(352, 28);
            this.dtpFinPreventa.TabIndex = 56;
            //
            // btnGuardarPreventa
            //
            this.btnGuardarPreventa.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardarPreventa.Location = new System.Drawing.Point(400, 128);
            this.btnGuardarPreventa.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.btnGuardarPreventa.Name = "btnGuardarPreventa";
            this.btnGuardarPreventa.Size = new System.Drawing.Size(208, 40);
            this.btnGuardarPreventa.TabIndex = 57;
            this.btnGuardarPreventa.Text = "Guardar preventa";
            this.btnGuardarPreventa.Click += new System.EventHandler(this.btnGuardarPreventa_Click);
            // 
            // frmModificarSectoresEventos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(840, 448);
            this.Controls.Add(this.btnGuardarPreventa);
            this.Controls.Add(this.dtpFinPreventa);
            this.Controls.Add(this.lblPreventa);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnEliminarSector);
            this.Controls.Add(this.btnAgregarSector);
            this.Controls.Add(this.dgvSectores);
            this.Controls.Add(this.lblEvento);
            this.Controls.Add(this.btnVolver);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmModificarSectoresEventos";
            this.Text = "EventAura · Administrar evento";
            this.Load += new System.EventHandler(this.frmAgregarEventoLocal_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSectores)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Visual.Componentes.BotonSecundario btnVolver;
        private Visual.Componentes.EtiquetaTitulo label1;
        private Visual.Componentes.EtiquetaSeccion lblEvento;
        private System.Windows.Forms.DataGridView dgvSectores;
        private Visual.Componentes.BotonPrimario btnAgregarSector;
        private Visual.Componentes.BotonPrimario btnEliminarSector;
        private Visual.Componentes.EtiquetaSeccion label2;
        private Visual.Componentes.EtiquetaRotulo lblPreventa;
        private System.Windows.Forms.DateTimePicker dtpFinPreventa;
        private Visual.Componentes.BotonPrimario btnGuardarPreventa;
    }
}