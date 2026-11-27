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
        [ThreadStatic]
        private static Task<CoreWebView2Environment> entorno;
        [ThreadStatic]
        private static string perfilWebPrueba;
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
                {
                    string perfilPruebas = Environment.GetEnvironmentVariable("EVENTAURA_WEBVIEW2_USER_DATA");
                    string carpetaDatos;
                    if (string.IsNullOrWhiteSpace(perfilPruebas))
                        carpetaDatos = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EventAura", "WebView2");
                    else
                    {
                        if (perfilWebPrueba == null)
                            perfilWebPrueba = Path.Combine(perfilPruebas, Guid.NewGuid().ToString("N"));
                        carpetaDatos = perfilWebPrueba;
                    }
                    entorno = CoreWebView2Environment.CreateAsync(null, carpetaDatos,
                        new CoreWebView2EnvironmentOptions("--disable-background-networking --disable-component-update --disable-sync --no-pings"));
                }
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
                core.ProcessFailed += (s, e) => Desactivar(
                    new InvalidOperationException("WebView2 process failed: " + e.ProcessFailedKind));
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
                        Desactivar(new InvalidOperationException("WebView2 navigation failed: " + e.WebErrorStatus));
                        return;
                    }
                    cargada = true;
                    await Renderizar();
                };
                core.Navigate(pagina.AbsoluteUri);
            }
            catch (Exception error)
            {
                Desactivar(error);
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
                    Desactivar(new InvalidOperationException("WebView2 did not render data: " + (respuesta ?? "<null>")));
                    return;
                }
                VistaLista?.Invoke(null);
            }
            catch (Exception error)
            {
                Desactivar(error);
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

        private void Desactivar(Exception motivo = null)
        {
            if (IsDisposed || fallida) return;
            if (motivo != null) ManejadorErrores.Registrar(motivo);
            fallida = true;
            Hide();
            VistaNoDisponible?.Invoke(null);
        }
    }
}
