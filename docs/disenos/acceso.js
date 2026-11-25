"use strict";
// Inicio de sesión. La página solo controla que los campos no estén vacíos; la autenticación, el conteo de
// intentos y el bloqueo (RN-04) son de GestorUsuarios.Autenticar. La contraseña viaja una sola vez, en el
// mensaje «iniciarSesion», y la página la borra del campo en cuanto la aplicación responde.
(function () {
    const puente = window.EventAuraPuente;
    const $ = s => document.querySelector(s);
    const formulario = $("#formulario"), mail = $("#mail"), contrasena = $("#contrasena");
    const error = $("#error"), sinBase = $("#sin-base"), entrar = $("#entrar");

    $("#mostrar").addEventListener("change", e => { contrasena.type = e.target.checked ? "text" : "password"; });

    function marcar(campo, texto) {
        const estado = $("#estado-" + campo.id);
        campo.setAttribute("aria-invalid", texto ? "true" : "false");
        estado.textContent = texto || "";
        estado.className = "estado" + (texto ? " mal" : "");
    }
    [mail, contrasena].forEach(c => c.addEventListener("input", () => { marcar(c, ""); error.classList.add("oculto"); }));

    function esperando(si) {
        entrar.disabled = si;
        entrar.textContent = si ? "Entrando…" : "Iniciar sesión";
        formulario.setAttribute("aria-busy", si ? "true" : "false");
    }

    formulario.addEventListener("submit", e => {
        e.preventDefault();
        // Mismos textos que frmInicio hoy.
        if (!mail.value.trim()) { marcar(mail, "Escribí tu correo electrónico para continuar."); mail.focus(); return; }
        if (!contrasena.value) { marcar(contrasena, "Escribí tu contraseña para continuar."); contrasena.focus(); return; }
        esperando(true);
        puente.enviar({ tipo: "iniciarSesion", mail: mail.value.trim(), contrasena: contrasena.value });
    });

    document.querySelectorAll("[data-destino]").forEach(b =>
        b.addEventListener("click", () => puente.enviar({ tipo: "navegar", destino: b.dataset.destino, mail: mail.value.trim() })));

    // datos que manda la aplicación (ver README.md):
    //   { estado: "listo" | "error" | "sinBase", mensaje?: string, mail?: string }
    puente.alDibujar(datos => {
        esperando(false);
        if (datos.mail !== undefined && !mail.value) mail.value = datos.mail;
        const caida = datos.estado === "sinBase";
        formulario.classList.toggle("oculto", caida);
        sinBase.classList.toggle("oculto", !caida);
        sinBase.textContent = caida ? datos.mensaje : "";
        if (datos.estado === "error") {
            contrasena.value = "";
            error.textContent = datos.mensaje;
            error.classList.remove("oculto");
            contrasena.focus();
        } else {
            error.classList.add("oculto");
        }
    });

    // ---- Solo en la maqueta: cómo respondería la aplicación ----
    const cuentas = { "admin@eventaura.com": "Admin1234", "plata@eventaura.com": "Cliente123" };
    puente.simular(m => {
        if (m.tipo === "iniciarSesion") {
            if (cuentas[m.mail.toLowerCase()] === m.contrasena) {
                contrasena.value = "";
                puente.anotarFuera("Sesión iniciada: la aplicación cerraría esta pantalla y abriría el menú del perfil.");
                puente.responder({ estado: "listo" });
            } else {
                puente.responder({ estado: "error", mensaje: "Mail o contraseña incorrectos." });
            }
        } else if (m.tipo === "navegar") {
            const destino = { alta: "alta-cuenta.html", bienvenida: "bienvenida.html", recuperar: "recuperar-clave.html" }[m.destino];
            if (destino) setTimeout(() => { window.location.href = destino; }, 500);
        }
    }, {
        "Listo": { estado: "listo" },
        "Credenciales incorrectas": { estado: "error", mensaje: "Mail o contraseña incorrectos." },
        "Base no disponible": { estado: "sinBase", mensaje: "EventAura no pudo comprobar la base de datos. Instancia: .\\NOEXISTE · Base: BddEventAura. Revisá SQL Server y la configuración, y volvé a abrir la aplicación." }
    });

    mail.focus();
})();
