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
    public sealed class PanelWeb : UserControl
    {
        private const string Origen = "https://eventaura.local/";
        private static Task<CoreWebView2Environment> entorno;
        private WebView2 navegador;
        private Uri pagina;
        private object datos;
        private int version;
        private bool cargada;
        private bool fallida;

        public bool Preparado { get; private set; }
        public bool InicializacionTerminada => Preparado || fallida;

        public Task CapturarAsync(Stream imagen) =>
            navegador.CoreWebView2.CapturePreviewAsync(CoreWebView2CapturePreviewImageFormat.Png, imagen);
        public event Action VistaLista;
        public event Action VistaNoDisponible;
        public event Action<JObject> Mensaje;
        public event Action Escape;

        public PanelWeb()
        {
            Dock = DockStyle.Fill;
            BackColor = Tema.Superficie;
            TabStop = false;
        }

        public async void Cargar(string archivo)
        {
            if (pagina != null || fallida) return;
            pagina = new Uri(Origen + Path.GetFileName(archivo));
            string carpeta = Path.Combine(Path.GetDirectoryName(typeof(PanelWeb).Assembly.Location), "Web");
            try
            {
                if (!File.Exists(Path.Combine(carpeta, Path.GetFileName(archivo))))
                    throw new FileNotFoundException(archivo);
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
                    if (!e.IsSuccess) { Desactivar(); return; }
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
                string respuesta = await navegador.ExecuteScriptAsync("window.eventAuraActualizar(" + json + ")");
                if (IsDisposed || fallida || actual != version) return;
                if (respuesta != "true") { Desactivar(); return; }
                if (!Preparado)
                {
                    Preparado = true;
                    VistaLista?.Invoke();
                }
            }
            catch (Exception error)
            {
                System.Diagnostics.Trace.WriteLine(error);
                Desactivar();
            }
        }

        private void Recibir(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            if (!Preparado || e.Source != pagina.AbsoluteUri) return;
            JObject mensaje;
            try { mensaje = JObject.Parse(e.WebMessageAsJson); }
            catch (JsonException) { return; }
            if (mensaje["tipo"]?.ToString() == "escape") Escape?.Invoke();
            else Mensaje?.Invoke(mensaje);
        }

        private void Desactivar()
        {
            if (IsDisposed || fallida) return;
            fallida = true;
            Preparado = false;
            Hide();
            VistaNoDisponible?.Invoke();
        }
    }
}