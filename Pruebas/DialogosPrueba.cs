using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pruebas
{
    // Responde únicamente a los MessageBox del hilo de esta prueba. No usa el teclado
    // global ni busca ventanas de otras aplicaciones.
    internal sealed class DialogoEsperado
    {
        public string Titulo;
        public string Texto;
        public DialogResult Predeterminado = DialogResult.OK;
        public DialogResult Respuesta = DialogResult.OK;
    }

    internal static class DialogosPrueba
    {
        private delegate bool EnumerarVentana(IntPtr ventana, IntPtr parametro);
        [DllImport("user32.dll")] private static extern bool EnumThreadWindows(uint hilo, EnumerarVentana enumerar, IntPtr parametro);
        [DllImport("user32.dll")] private static extern bool EnumChildWindows(IntPtr padre, EnumerarVentana enumerar, IntPtr parametro);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetClassName(IntPtr ventana, StringBuilder texto, int longitud);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetWindowText(IntPtr ventana, StringBuilder texto, int longitud);
        [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr ventana);
        [DllImport("user32.dll")] private static extern bool IsWindowEnabled(IntPtr ventana);
        [DllImport("user32.dll")] private static extern IntPtr GetDlgItem(IntPtr ventana, int identificador);
        [DllImport("user32.dll")] private static extern IntPtr SendMessage(IntPtr ventana, uint mensaje, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")] private static extern bool PostMessage(IntPtr ventana, uint mensaje, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();

        public static void Comprobar(Action accion, params DialogoEsperado[] esperados)
        {
            var pendientes = new Queue<DialogoEsperado>(esperados);
            var errores = new List<string>();
            var respondidos = new HashSet<IntPtr>();
            using (var timer = new Timer { Interval = 30 })
            {
                timer.Tick += (s, e) =>
                {
                    EnumThreadWindows(GetCurrentThreadId(), (ventana, parametro) =>
                    {
                        if (!IsWindowVisible(ventana) || Clase(ventana) != "#32770" || respondidos.Contains(ventana)) return true;
                        var siguiente = pendientes.Count > 0 ? pendientes.Peek() : null;
                        IntPtr boton = GetDlgItem(ventana, (int)(siguiente?.Respuesta ?? DialogResult.Cancel));
                        // Windows puede identificar el único botón Aceptar como IDCANCEL para
                        // permitir cerrar con Escape. No confundirlo con un diálogo OK/Cancelar.
                        bool aceptarComoCancelar = false;
                        if (boton == IntPtr.Zero && siguiente?.Respuesta == DialogResult.OK)
                        {
                            var botones = new List<IntPtr>();
                            EnumChildWindows(ventana, (hijo, p) => { if (Clase(hijo) == "Button") botones.Add(hijo); return true; }, IntPtr.Zero);
                            IntPtr cancelar = GetDlgItem(ventana, (int)DialogResult.Cancel);
                            aceptarComoCancelar = botones.Count == 1 && botones[0] == cancelar;
                            if (aceptarComoCancelar) boton = cancelar;
                        }
                        if (boton == IntPtr.Zero)
                        {
                            foreach (DialogResult alternativa in new[] { DialogResult.Cancel, DialogResult.No, DialogResult.OK })
                            {
                                boton = GetDlgItem(ventana, (int)alternativa);
                                if (boton != IntPtr.Zero) break;
                            }
                        }
                        if (boton == IntPtr.Zero || !IsWindowVisible(boton) || !IsWindowEnabled(boton)) return true;
                        respondidos.Add(ventana);
                        string titulo = Texto(ventana);
                        var fragmentos = new List<string>();
                        EnumChildWindows(ventana, (hijo, p) => { if (Clase(hijo) == "Static") fragmentos.Add(Texto(hijo)); return true; }, IntPtr.Zero);
                        string texto = string.Join(" ", fragmentos);
                        int defecto = SendMessage(ventana, 0x400, IntPtr.Zero, IntPtr.Zero).ToInt32() & 0xffff; // DM_GETDEFID
                        if (aceptarComoCancelar && defecto == (int)DialogResult.Cancel) defecto = (int)DialogResult.OK;
                        DialogoEsperado esperado = pendientes.Count > 0 ? pendientes.Dequeue() : null;
                        if (esperado == null) errores.Add("Diálogo inesperado: " + titulo + " / " + texto);
                        else
                        {
                            if (titulo != esperado.Titulo) errores.Add($"Título: '{titulo}', esperado '{esperado.Titulo}'.");
                            if (!texto.Contains(esperado.Texto)) errores.Add($"Texto: '{texto}', esperado '{esperado.Texto}'.");
                            if (defecto != (int)esperado.Predeterminado) errores.Add($"Botón por defecto de '{titulo}': {defecto}, esperado {(int)esperado.Predeterminado}.");
                        }
                        PostMessage(boton, 0xF5, IntPtr.Zero, IntPtr.Zero); // BM_CLICK, botón del propio diálogo
                        return true;
                    }, IntPtr.Zero);
                };
                timer.Start();
                accion();
                timer.Stop();
            }
            Assert.AreEqual(0, pendientes.Count, "No aparecieron todos los diálogos esperados.");
            Assert.AreEqual(0, errores.Count, string.Join(Environment.NewLine, errores));
        }

        private static string Clase(IntPtr ventana) { var texto = new StringBuilder(256); GetClassName(ventana, texto, texto.Capacity); return texto.ToString(); }
        private static string Texto(IntPtr ventana) { var texto = new StringBuilder(4096); GetWindowText(ventana, texto, texto.Capacity); return texto.ToString(); }
    }
}
