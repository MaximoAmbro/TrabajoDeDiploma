namespace Visual.Componentes
{
    partial class InicioCliente
    {
        private EtiquetaSeccion nivel,tituloProximas,tituloAvisos;
        private EtiquetaAyuda siguiente,proximas,avisos,ultimo,icono;
        private BotonPrimario verEventos;
        private void InitializeComponent()
        {
            this.SuspendLayout();
            nivel=new EtiquetaSeccion {Name="nivelInicio",Location=new System.Drawing.Point(16,12),Size=new System.Drawing.Size(440,32)};
            siguiente=new EtiquetaAyuda {Name="siguienteNivel",Location=new System.Drawing.Point(16,48),Size=new System.Drawing.Size(440,48)};
            tituloProximas=new EtiquetaSeccion {Text="Tus próximas entradas",Location=new System.Drawing.Point(16,96),Size=new System.Drawing.Size(440,28)};
            icono=new EtiquetaAyuda {Text="\uE8A5",Location=new System.Drawing.Point(16,128),Size=new System.Drawing.Size(48,48)};
            proximas=new EtiquetaAyuda {Name="proximasEntradas",Location=new System.Drawing.Point(72,128),Size=new System.Drawing.Size(384,80)};
            verEventos=new BotonPrimario {Name="verEventosInicio",Text="Ver eventos",Location=new System.Drawing.Point(16,216),Size=new System.Drawing.Size(240,36)};
            tituloAvisos=new EtiquetaSeccion {Text="Tu lista de espera",Location=new System.Drawing.Point(16,268),Size=new System.Drawing.Size(440,28)};
            avisos=new EtiquetaAyuda {Name="avisosInicio",Location=new System.Drawing.Point(16,304),Size=new System.Drawing.Size(440,56)};
            ultimo=new EtiquetaAyuda {Name="ultimoInicio",Location=new System.Drawing.Point(16,368),Size=new System.Drawing.Size(440,28)};
            AutoScaleDimensions=new System.Drawing.SizeF(96F,96F);AutoScaleMode=System.Windows.Forms.AutoScaleMode.Dpi;Size=new System.Drawing.Size(480,492);AutoScroll=true;
            Controls.Add(nivel);Controls.Add(siguiente);Controls.Add(tituloProximas);Controls.Add(icono);Controls.Add(proximas);Controls.Add(verEventos);Controls.Add(tituloAvisos);Controls.Add(avisos);Controls.Add(ultimo);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
