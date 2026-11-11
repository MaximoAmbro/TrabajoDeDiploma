using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Datos;
using Entidades;
using Microsoft.Data.SqlClient;
using Negocio;

namespace Pruebas
{
    // Investigación con Program.Main original y Application.Run. Solo los controles de la UI
    // crean/modifican datos. Las consultas propias son SELECT y no se usa Fake.
    internal static class InvestigacionPreventaReal
    {
        [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr h, int modo);
        [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr h);
        private static readonly List<string> Log = new List<string>();
        private static string carpeta, cadena, nombreLocal, nombreEvento, mail, clave;
        private static int paso, eventoId, usuarioId, sectorId, stockAntes;
        private static bool ocupado, terminado;
        private static Dictionary<string, decimal> antes;

        [STAThread]
        private static int Main(string[] args)
        {
            carpeta = args[0];
            Directory.CreateDirectory(carpeta);
            cadena = ConfigurationManager.ConnectionStrings[BaseDePrueba.NombreCadena].ConnectionString;
            string catalogo = new SqlConnectionStringBuilder(cadena).InitialCatalog;
            if (!catalogo.EndsWith("_Pruebas", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Solo base de pruebas.");
            string marca = Guid.NewGuid().ToString("N").Substring(0, 10);
            nombreLocal = "UI Local " + marca;
            nombreEvento = "UI Preventa " + marca;
            mail = "ui" + marca + "@pruebas.eventaura.com";
            clave = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Guid.NewGuid().ToByteArray()[0].ToString();
            Log.Add("Base=" + catalogo);
            Log.Add("Local=" + nombreLocal + "; Evento=" + nombreEvento + "; Cliente=" + mail);
            using (var timer = new Timer { Interval = 150 })
            {
                timer.Tick += (s, e) =>
                {
                    if (ocupado || terminado) return;
                    ocupado = true;
                    try { EjecutarPaso(); }
                    catch (Exception ex) { Log.Add("INCONCLUSO: " + ex.GetBaseException().Message); terminado = true; }
                    finally
                    {
                        ocupado = false;
                        File.WriteAllLines(Path.Combine(carpeta, "recorrido.txt"), Log);
                        if (terminado) { timer.Stop(); Application.Exit(); }
                    }
                };
                // Program.Main debe instalar su manejador antes de crear la ventana del timer.
                EventHandler iniciar = null;
                iniciar = (s, e) => { Application.Idle -= iniciar; timer.Start(); };
                Application.Idle += iniciar;
                typeof(Visual.frmInicio).Assembly.GetType("Visual.Program").GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic)
                    .Invoke(null, null);
            }
            return Log.Any(l => l.StartsWith("Categoria=")) ? 0 : 1;
        }

        private static T Visible<T>() where T : Form => Application.OpenForms.OfType<T>().Last(f => f.Visible);
        private static T Campo<T>(Control padre, string nombre) where T : Control => (T)padre.Controls.Find(nombre, true).Single();
        private static void Texto(Form f, string nombre, string valor) => Campo<TextBox>(f, nombre).Text = valor;
        private static void Click(Form f, string nombre)
        {
            Control c = Campo<Control>(f, nombre);
            if (c is Button) ((Button)c).PerformClick();
            else typeof(Control).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(c, new object[] { EventArgs.Empty });
        }
        private static void Aviso(Action accion, string texto, string titulo = "") => DialogosPrueba.Comprobar(accion,
            new DialogoEsperado { Titulo = titulo, Texto = texto });

        private static void EjecutarPaso()
        {
            Log.Add("Paso=" + paso + "; MessageLoop=" + Application.MessageLoop);
            switch (paso++)
            {
                case 0:
                    var login = Visible<Visual.frmInicio>();
                    ShowWindow(login.Handle, 5);
                    Usuario admin = RepositorioUsuarios.Instance.ObtenerPorMail("admin@eventaura.com");
                    if (Seguridad.EsHash(admin.Contraseña)) throw new InvalidOperationException("Requiere la semilla aislada sin autenticar previamente al administrador.");
                    Texto(login, "txtMailLogin", admin.Mail); Texto(login, "txtContraseñaLogin", admin.Contraseña);
                    Click(login, "btninicio"); break;
                case 1: Click(Visible<Visual.frmMenuPropietario>(), "btnRegistrarLocal"); break;
                case 2:
                    var local = Visible<Visual.frmRegistrarLocal>();
                    Texto(local, "txtNombre", nombreLocal); Texto(local, "txtDireccion", "Mitre");
                    Texto(local, "txtAltura", "123"); Texto(local, "txtCapacidad", "10");
                    Aviso(() => Click(local, "btnregistrarse"), "Local registrado correctamente"); break;
                case 3: Click(Visible<Visual.frmMenuPropietario>(), "btnMisLocales"); break;
                case 4:
                    var locales = Visible<Visual.frmMisLocales>();
                    DataGridView grillaLocales = Campo<DataGridView>(locales, "dgvLocales");
                    grillaLocales.CurrentCell = grillaLocales.Rows.Cast<DataGridViewRow>()
                        .Single(r => Convert.ToString(r.Cells["Nombre"].Value) == nombreLocal).Cells["Nombre"];
                    Click(locales, "btnEventosLocal"); break;
                case 5: Click(Visible<Visual.Vendedor.frmEventosLocal>(), "btnAgregarEvento"); break;
                case 6:
                    var alta = Visible<Visual.Propietario.frmAgregarEvento>();
                    Texto(alta, "txtNombre", nombreEvento);
                    Campo<ComboBox>(alta, "cmbCategoria").SelectedIndex = 0;
                    Campo<DateTimePicker>(alta, "dtEvento").Value = DateTime.Today.AddDays(30).AddHours(21);
                    Campo<DateTimePicker>(alta, "dtpFinPreventa").Value = DateTime.Today.AddDays(1).AddHours(20);
                    Campo<DateTimePicker>(alta, "dtpFinPreventa").Checked = true;
                    Aviso(() => Click(alta, "btnRegistrar"), "Evento guardado como borrador."); break;
                case 7:
                    var listado = Visible<Visual.Vendedor.frmEventosLocal>();
                    var fila = Campo<DataGridView>(listado, "dgvEventos").Rows.Cast<DataGridViewRow>()
                        .Single(r => Convert.ToString(r.Cells["Nombre"].Value) == nombreEvento);
                    eventoId = Convert.ToInt32(fila.Cells["ID"].Value);
                    Campo<DataGridView>(listado, "dgvEventos").CurrentCell = fila.Cells["Nombre"];
                    Click(listado, "btnAdministrarEvento"); break;
                case 8: Click(Visible<Visual.Vendedor.frmAdministrarEvento>(), "btnAgregarSector"); break;
                case 9:
                    var sector = Visible<Visual.Vendedor.frmAgregarSector>();
                    Texto(sector, "txtNombre", "General"); Texto(sector, "txtCapacidad", "3"); Texto(sector, "txtPrecio", "1000");
                    Aviso(() => Click(sector, "btnregistrarse"), "Sector agregado correctamente.");
                    sectorId = GestorEventos.Instance.Sectores(eventoId).Single().Id; break;
                case 10: Click(Visible<Visual.Vendedor.frmAgregarSector>(), "lblVolver"); break;
                case 11: Click(Visible<Visual.Vendedor.frmAdministrarEvento>(), "btnVolver"); break;
                case 12: Click(Visible<Visual.Vendedor.frmEventosLocal>(), "btnVolver"); break;
                case 13: Click(Visible<Visual.frmMisLocales>(), "btnVolver"); break;
                case 14: Click(Visible<Visual.frmMenuPropietario>(), "lblvolver"); break;
                case 15: Click(Visible<Visual.frmInicio>(), "lblcrearcuenta"); break;
                case 16:
                    var registro = Visible<Visual.frmCrearUsuario>();
                    Texto(registro, "txtNombre", "Investigación"); Texto(registro, "txtApellido", "Preventa"); Texto(registro, "txtmail", mail);
                    Texto(registro, "TxtPassword", clave); Texto(registro, "TxtConfirmpassword", clave);
                    Aviso(() => Click(registro, "btnregistrarse"), "Usuario creado con exito.");
                    usuarioId = RepositorioUsuarios.Instance.ObtenerPorMail(mail).Id; break;
                case 17:
                    var acceso = Visible<Visual.frmInicio>();
                    Texto(acceso, "txtMailLogin", mail); Texto(acceso, "txtContraseñaLogin", clave); Click(acceso, "btninicio"); break;
                case 18: Click(Visible<Visual.frmMenuCliente>(), "btnEventos"); break;
                case 19:
                    var eventos = Visible<Visual.frmEventosCliente>();
                    var grilla = Campo<DataGridView>(eventos, "dgveventos");
                    var filaEvento = grilla.Rows.Cast<DataGridViewRow>().Single(r => Convert.ToInt32(r.Cells["ID"].Value) == eventoId);
                    grilla.CurrentCell = filaEvento.Cells["Nombre"];
                    antes = Estado("antes"); stockAntes = GestorPaquetes.Instance.DisponiblesDe(sectorId);
                    Aviso(() => Click(eventos, "btnComprar"), "a las 20:00", "No se puede armar el paquete"); break;
                case 20:
                    var despues = Estado("despues");
                    foreach (Form ventana in Application.OpenForms.Cast<Form>().Where(f => f.Visible))
                    {
                        Log.Add("Visible=" + ventana.GetType().FullName + "; titulo=" + ventana.Text);
                        Capturar(ventana, Path.Combine(carpeta, ventana.GetType().Name + ".png"));
                    }
                    bool visible = Application.OpenForms.OfType<Visual.FrmCompraCliente>().Any(f => f.Visible);
                    foreach (var compra in Application.OpenForms.OfType<Visual.FrmCompraCliente>().Where(f => f.Visible))
                        Log.Add("CompraVisible: filasSectores=" + Campo<DataGridView>(compra, "dgvSectores").Rows.Count +
                            "; tituloEvento=" + Campo<Label>(compra, "label1").Text);
                    bool datos = despues.Any(p => p.Key != "Disponibles" && p.Value > antes[p.Key]) || despues["Disponibles"] != stockAntes;
                    Log.Add("Categoria=" + (datos ? "C" : visible ? "B" : "A"));
                    Log.Add("EventoID=" + eventoId + "; UsuarioID=" + usuarioId + "; SectorID=" + sectorId);
                    terminado = true; break;
            }
        }

        private static Dictionary<string, decimal> Estado(string fase)
        {
            var resultado = new Dictionary<string, decimal>();
            string compras = "SELECT ID FROM Compras WHERE ID_Usuario=@U OR ID IN (SELECT ID_Compra FROM Entradas WHERE ID_Evento=@E)";
            resultado["Compras"] = Leer("SELECT COUNT(*) FROM Compras WHERE ID IN (" + compras + ")");
            resultado["Entradas"] = Leer("SELECT COUNT(*) FROM Entradas WHERE ID_Evento=@E OR ID_Usuario=@U");
            resultado["QRConfirmados"] = Leer("SELECT COUNT(*) FROM Entradas WHERE (ID_Evento=@E OR ID_Usuario=@U) AND Estado='Confirmada' AND CodigoQR IS NOT NULL");
            resultado["ReservasVigentes"] = Leer("SELECT COUNT(*) FROM Compras WHERE ID IN (" + compras + ") AND ReservaVence > GETDATE()");
            resultado["Pagos"] = Leer("SELECT COUNT(*) FROM Pagos WHERE ID_Compra IN (" + compras + ")");
            resultado["TotalPagado"] = Leer("SELECT COALESCE(SUM(TotalPagado),0) FROM Compras WHERE ID IN (" + compras + ")");
            resultado["Seguros"] = Leer("SELECT COUNT(*) FROM Seguros WHERE ID_Entrada IN (SELECT ID FROM Entradas WHERE ID_Evento=@E OR ID_Usuario=@U)");
            resultado["Fidelizacion"] = GestorFidelizacion.Instance.ComprasEnVentana(usuarioId, DateTime.Now);
            resultado["Disponibles"] = GestorPaquetes.Instance.DisponiblesDe(sectorId);
            foreach (var par in resultado) Log.Add(fase + "." + par.Key + "=" + par.Value);
            return resultado;
        }

        private static decimal Leer(string select)
        {
            using (var conexion = new SqlConnection(cadena))
            using (var comando = new SqlCommand(select, conexion))
            {
                comando.Parameters.AddWithValue("@U", usuarioId); comando.Parameters.AddWithValue("@E", eventoId);
                conexion.Open(); return Convert.ToDecimal(comando.ExecuteScalar());
            }
        }

        private static void Capturar(Form f, string ruta)
        {
            ShowWindow(f.Handle, 5); SetForegroundWindow(f.Handle); f.Refresh(); Application.DoEvents();
            System.Threading.Thread.Sleep(100);
            using (var imagen = new Bitmap(f.Width, f.Height))
            using (Graphics g = Graphics.FromImage(imagen))
            {
                g.CopyFromScreen(f.Location, Point.Empty, imagen.Size); imagen.Save(ruta);
            }
        }
    }
}
