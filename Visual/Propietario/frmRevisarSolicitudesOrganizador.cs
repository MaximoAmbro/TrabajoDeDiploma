using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Visual
{
    public sealed class frmRevisarSolicitudesOrganizador : FormularioEventAura
    {
        private readonly ListBox pendientes = new ListBox { Location=new Point(20,70), Size=new Size(300,500) };
        private readonly Label detalle = new Label { Location=new Point(340,70), Size=new Size(430,210), AutoEllipsis=true };
        private readonly PictureBox documento = new PictureBox { Location=new Point(790,70), Size=new Size(360,260), SizeMode=PictureBoxSizeMode.Zoom, BorderStyle=BorderStyle.FixedSingle };
        private readonly TextBox motivo = new TextBox { Location=new Point(340,410), Size=new Size(810,80), Multiline=true, MaxLength=200 };
        private readonly Button aprobar = new Componentes.BotonPrimario { Text="Aprobar y crear local", Location=new Point(340,520), Size=new Size(230,44) };
        private readonly Button rechazar = new Componentes.BotonPeligro { Text="Rechazar", Location=new Point(585,520), Size=new Size(150,44) };
        public frmRevisarSolicitudesOrganizador()
        {
            GestorSesion.Instance.RequerirAdministrador();
            Text="Solicitudes de Organizador"; StartPosition=FormStartPosition.CenterParent; ClientSize=new Size(1180,620); MinimumSize=new Size(1180,620);
            Controls.Add(new Componentes.EtiquetaTitulo { Text="Revisión de solicitudes", Location=new Point(20,18), Size=new Size(500,40) });
            Controls.AddRange(new Control[]{pendientes,detalle,documento,motivo,aprobar,rechazar});
            Controls.Add(new Componentes.EtiquetaAyuda { Text="Escribí un motivo antes de aprobar o rechazar.", Location=new Point(340,380), Size=new Size(600,28) });
            pendientes.SelectedIndexChanged+=(s,e)=>MostrarSeleccion();
            aprobar.Click+=(s,e)=>Revisar(true); rechazar.Click+=(s,e)=>Revisar(false);
            Load+=(s,e)=>Cargar();
        }
        private void Cargar()
        {
            pendientes.DataSource=null; pendientes.DataSource=GestorSolicitudesOrganizador.Instance.Pendientes();
            pendientes.DisplayMember="NombreLocal"; MostrarSeleccion();
        }
        private SolicitudOrganizador Actual => pendientes.SelectedItem as SolicitudOrganizador;
        private void MostrarSeleccion()
        {
            var s=Actual; documento.Image=null;
            if(s==null){detalle.Text="No hay solicitudes pendientes.";return;}
            detalle.Text="Responsable: "+s.Responsable+Environment.NewLine+"CUIT: "+s.Cuit+Environment.NewLine+"Teléfono: "+s.Telefono+Environment.NewLine+
                "Local: "+s.NombreLocal+Environment.NewLine+"Dirección: "+s.Direccion+Environment.NewLine+"Capacidad: "+s.Capacidad+Environment.NewLine+"CBU / alias: "+s.CbuAlias+Environment.NewLine+"Documentación adjunta: imagen validada.";
            using(var stream=new MemoryStream(s.Documentacion)) using(var imagen=Image.FromStream(stream)) documento.Image=new Bitmap(imagen);
        }
        private void Revisar(bool aprobarSolicitud)
        {
            var s=Actual; if(s==null)return;
            try
            {
                if(aprobarSolicitud)GestorSolicitudesOrganizador.Instance.Aprobar(s.Id,motivo.Text);
                else GestorSolicitudesOrganizador.Instance.Rechazar(s.Id,motivo.Text);
                MessageBox.Show(this, aprobarSolicitud?"Solicitud aprobada y local creado.":"Solicitud rechazada con el motivo registrado.","Revisión guardada",MessageBoxButtons.OK,MessageBoxIcon.Information);
                Cargar();
            }
            catch(Exception ex){MessageBox.Show(this,ex.Message,"No pudimos guardar la revisión",MessageBoxButtons.OK,MessageBoxIcon.Warning);}
        }
    }
}
