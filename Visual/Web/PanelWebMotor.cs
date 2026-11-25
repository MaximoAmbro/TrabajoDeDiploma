using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Visual.Web
{
    // Esta clase se carga por reflexión desde PanelWeb. Sus dependencias opcionales quedan
    // detrás de una frontera que permite construir el respaldo aunque falten las DLL.
    public sealed class PanelWebMotor : UserControl
    {
        private const string Origen = "https://eventaura.local/";
        private static Task<CoreWebView2Environment> entorno;
        private WebView2 navegador;
        private Uri pagina;
        private object datos;
        private int version;
        private bool cargada;
        private bool fallida;

        public event Action<object> VistaLista;
        public event Action<object> VistaNoDisponible;
        public event Action<object> Mensaje;
        public event Action<object> Escape;

        public PanelWebMotor() { BackColor = Tema.Superficie; }

        public Task CapturarAsync(Stream imagen) =>
            navegador.CoreWebView2.CapturePreviewAsync(CoreWebView2CapturePreviewImageFormat.Png, imagen);

        public void EnfocarVista() => navegador?.Focus();

        public Task<string> EjecutarScriptAsync(string script) => navegador.ExecuteScriptAsync(script);

        public async void Cargar(string archivo)
        {
            if (pagina != null || fallida) return;
            pagina = new Uri(Origen + Path.GetFileName(archivo));
            string carpeta = Path.Combine(Path.GetDirectoryName(typeof(PanelWebMotor).Assembly.Location), "Web");
            try
            {
                if (!File.Exists(Path.Combine(carpeta, Path.GetFileName(archivo)))) throw new FileNotFoundException(archivo);
                navegador = new WebView2 { Dock = DockStyle.Fill };
                Controls.Add(navegador);
                if (entorno == null || entorno.IsFaulted)
                    entorno = CoreWebView2Environment.CreateAsync(null,
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EventAura", "WebView2"),
                        new CoreWebView2EnvironmentOptions("--disable-background-networking --disable-component-update --disable-sync --no-pings"));
                await navegador.EnsureCoreWebView2Async(await entorno);
                if (IsDisposed || fallida) return;
                var core = navegador.CoreWebView2;
                core.SetVirtualHostNameToFolderMapping("eventaura.local", carpeta, CoreWebView2HostResourceAccessKind.Deny);
                core.Settings.AreHostObjectsAllowed = false;
                core.Settings.AreDevToolsEnabled = false;
                core.Settings.AreDefaultContextMenusEnabled = false;
                core.Settings.AreBrowserAcceleratorKeysEnabled = false;
                core.NavigationStarting += (s, e) => e.Cancel = e.Uri != pagina.AbsoluteUri;
                core.FrameNavigationStarting += (s, e) => e.Cancel = true;
                core.NewWindowRequested += (s, e) => e.Handled = true;
                core.PermissionRequested += (s, e) => e.State = CoreWebView2PermissionState.Deny;
                core.DownloadStarting += (s, e) => e.Cancel = true;
                core.ProcessFailed += (s, e) => Desactivar();
                core.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
                core.WebResourceRequested += (s, e) =>
                {
                    if (!e.Request.Uri.StartsWith(Origen, StringComparison.Ordinal))
                        e.Response = core.Environment.CreateWebResourceResponse(null, 403, "Forbidden", "");
                };
                core.WebMessageReceived += Recibir;
                core.NavigationCompleted += async (s, e) =>
                {
                    if (IsDisposed || fallida) return;
                    if (!e.IsSuccess)
                    {
                        System.Diagnostics.Trace.WriteLine("WebView2 navigation failed: " + e.WebErrorStatus);
                        Desactivar(); return;
                    }
                    cargada = true;
                    await Renderizar();
                };
                core.Navigate(pagina.AbsoluteUri);
            }
            catch (Exception error)
            {
                System.Diagnostics.Trace.WriteLine(error);
                Desactivar();
            }
        }

        public void Actualizar(object modelo)
        {
            datos = modelo;
            version++;
            if (cargada) _ = Renderizar();
        }

        private async Task Renderizar()
        {
            if (IsDisposed || fallida) return;
            int actual = version;
            try
            {
                string json = JsonConvert.SerializeObject(new { tema = Tema.VariablesWeb(), datos });
                string respuesta = await navegador.ExecuteScriptAsync("(()=>{try{return window.eventAuraActualizar(" + json + ");}catch(e){return 'EVENTAURA_ERROR:'+String(e&&e.message||e);}})()");
                if (IsDisposed || fallida || actual != version) return;
                if (respuesta != "true")
                {
                    System.Diagnostics.Trace.WriteLine("WebView2 did not render data: " + (respuesta ?? "<null>"));
                    Desactivar(); return;
                }
                VistaLista?.Invoke(null);
            }
            catch (Exception error)
            {
                System.Diagnostics.Trace.WriteLine(error);
                Desactivar();
            }
        }

        private void Recibir(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            if (e.Source != pagina.AbsoluteUri) return;
            JObject mensaje;
            try { mensaje = JObject.Parse(e.WebMessageAsJson); }
            catch (JsonException) { return; }
            if (mensaje["tipo"]?.ToString() == "escape") Escape?.Invoke(null);
            else Mensaje?.Invoke(mensaje.ToString(Formatting.None));
        }

        private void Desactivar()
        {
            if (IsDisposed || fallida) return;
            fallida = true;
            Hide();
            VistaNoDisponible?.Invoke(null);
        }
    }
}
