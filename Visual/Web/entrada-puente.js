"use strict";
// Puente entre la página y la pantalla de Windows Forms que la aloja (Visual/Web/PanelWeb.cs).
// Sigue el mismo contrato que tema.js del catálogo:
//   - la pantalla llama a window.eventAuraActualizar({ tema, datos }) y espera que devuelva true;
//   - la página le habla a la pantalla con window.chrome.webview.postMessage({ tipo, ... });
//   - Escape se manda como { tipo: "escape" }.
// Abierta en un navegador, sin WebView2, la página usa un host SIMULADO: el panel «Maqueta» muestra cada
// mensaje que se mandaría y responde como respondería la aplicación. Ese panel no existe dentro de WebView2.
(function () {
    const webview = window.chrome && window.chrome.webview;
    const enAplicacion = !!webview;
    let simulador = null;          // lo define cada página: (mensaje) => void
    let dibujar = () => {};
    document.querySelectorAll(".ojo-contrasena").forEach(boton => {
        const campo = document.getElementById(boton.getAttribute("aria-controls"));
        if (!campo) return;
        boton.addEventListener("click", () => {
            const visible = campo.type === "password";
            campo.type = visible ? "text" : "password";
            boton.setAttribute("aria-pressed", String(visible));
            boton.setAttribute("aria-label", visible ? "Ocultar contraseña" : "Mostrar contraseña");
            boton.dataset.visible = String(visible);
        });
    });

    function aplicarTema(tema) {
        Object.entries(tema || {}).forEach(([nombre, valor]) =>
            document.documentElement.style.setProperty("--" + nombre, valor));
    }

    window.eventAuraActualizar = ({ tema, datos }) => {
        aplicarTema(tema);
        dibujar(datos || {});
        return true;
    };

    function ocultarSecretos(mensaje) {
        return JSON.parse(JSON.stringify(mensaje, (clave, valor) => {
            if (/contrasena|codigo/i.test(clave) && typeof valor === "string") return "•".repeat(Math.min(valor.length, 8));
            if (clave === "base64" && typeof valor === "string") return "(" + Math.round(valor.length * 3 / 4 / 1024) + " KB en base64)";
            return valor;
        }));
    }

    function enviar(mensaje) {
        if (enAplicacion) { webview.postMessage(mensaje); return; }
        registrar("página → aplicación", ocultarSecretos(mensaje));
        if (simulador) setTimeout(() => simulador(mensaje), 450);
    }

    window.addEventListener("keydown", e => {
        if (e.key !== "Escape" || e.defaultPrevented) return;
        e.preventDefault();
        enviar({ tipo: "escape" });
    });

    // ---- Panel de la maqueta (solo fuera de la aplicación) ----
    let lista = null;
    function registrar(direccion, contenido) {
        if (!lista) return;
        const fila = document.createElement("li");
        const hora = new Date().toLocaleTimeString("es-AR");
        fila.innerHTML = "";
        const cabeza = document.createElement("span");
        cabeza.className = "maqueta-direccion";
        cabeza.textContent = hora + " · " + direccion;
        const cuerpo = document.createElement("code");
        cuerpo.textContent = typeof contenido === "string" ? contenido : JSON.stringify(contenido);
        fila.append(cabeza, cuerpo);
        lista.prepend(fila);
    }

    function crearPanel(estados) {
        const panel = document.createElement("aside");
        panel.className = "maqueta";
        panel.setAttribute("aria-label", "Panel de la maqueta");
        panel.innerHTML =
            '<details><summary>Maqueta · aplicación simulada</summary>' +
            '<p class="maqueta-nota">Este panel solo existe al abrir el archivo en un navegador. Dentro de EventAura los mensajes van a la pantalla de Windows Forms.</p>' +
            '<div class="maqueta-estados"></div>' +
            '<p class="maqueta-titulo">Mensajes</p><ol class="maqueta-mensajes"></ol></details>';
        document.body.append(panel);
        lista = panel.querySelector(".maqueta-mensajes");
        const zona = panel.querySelector(".maqueta-estados");
        Object.entries(estados || {}).forEach(([nombre, datos]) => {
            const boton = document.createElement("button");
            boton.type = "button";
            boton.textContent = nombre;
            boton.addEventListener("click", () => responder(typeof datos === "function" ? datos() : datos));
            zona.append(boton);
        });
        if (!zona.children.length) zona.remove();
    }

    // La aplicación simulada «responde» igual que lo haría la pantalla real: llamando a eventAuraActualizar.
    function responder(datos) {
        registrar("aplicación → página", datos);
        window.eventAuraActualizar({ tema: {}, datos });
    }

    window.EventAuraPuente = {
        enAplicacion,
        enviar,
        alDibujar(funcion) { dibujar = funcion; },
        // Solo para la maqueta: cómo respondería la aplicación y qué estados se pueden previsualizar.
        simular(funcion, estados, datosIniciales) {
            if (enAplicacion) return;
            simulador = funcion;
            crearPanel(estados);
            if (datosIniciales) responder(datosIniciales);
        },
        responder,
        // Para que la maqueta muestre lo que en la realidad está FUERA de la aplicación (la bandeja simulada).
        anotarFuera(texto) { registrar("fuera de EventAura (bandeja simulada)", texto); }
    };
})();
