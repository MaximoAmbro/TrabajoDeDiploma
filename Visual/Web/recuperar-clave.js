"use strict";
// Recuperación de contraseña. Mismo flujo que hoy (GestorCodigosCorreo.SolicitarRecuperacion y
// RecuperarContrasena): la respuesta al pedir el código es idéntica exista o no la cuenta, cada código vence,
// se usa una vez y pedir otro invalida el anterior.
(function () {
    const puente = window.EventAuraPuente;
    const V = window.EventAuraValidaciones;
    const $ = s => document.querySelector(s);
    const parametros = { longitudMinima: 8, fortalezaMinima: 4 };

    function mostrarPaso(paso) {
        document.querySelectorAll("[data-paso]").forEach(p => p.classList.toggle("oculto", p.dataset.paso !== paso));
        $("#bajada").textContent = paso === "mail" ? "Escribí el correo de tu cuenta y te enviamos un código."
            : paso === "codigo" ? "Si la cuenta existe, enviamos un código. Para continuar, abrí fuera de EventAura el correo simulado de esa dirección."
            : "Listo.";
        $("#bajada").setAttribute("role", paso === "codigo" ? "status" : "");
        const foco = document.querySelector('[data-paso="' + paso + '"] input, [data-paso="' + paso + '"] .boton');
        if (foco) foco.focus({ preventScroll: true });
    }
    function marcar(id, tipo, texto) {
        const linea = $("#estado-" + id), campo = document.getElementById(id);
        if (campo) campo.setAttribute("aria-invalid", tipo === "mal" ? "true" : "false");
        if (linea) { linea.className = "estado" + (tipo ? " " + tipo : ""); linea.textContent = texto || ""; }
    }
    function ocupado(boton, si, rotulo) {
        if (!boton.dataset.rotulo) boton.dataset.rotulo = boton.textContent;
        boton.disabled = si; boton.textContent = si ? rotulo : boton.dataset.rotulo;
    }
    function fortaleza() {
        const pwd = $("#contrasena").value;
        const puntaje = V.puntajeFortaleza(pwd, parametros.longitudMinima);
        $("#fortaleza").dataset.nivel = puntaje < parametros.fortalezaMinima ? "bajo" : "ok";
        document.querySelectorAll("#fortaleza .barras span").forEach((b, i) => b.classList.toggle("lleno", i < puntaje));
        let linea = "Fortaleza: " + V.nivelFortaleza(puntaje, parametros.fortalezaMinima) + " (" + puntaje + "/5; mínimo " + parametros.fortalezaMinima + ")";
        if (pwd && !V.contrasenaCumpleMinimo(pwd, parametros.longitudMinima)) linea += ". Necesita al menos " + parametros.longitudMinima + " caracteres, con una letra y un número.";
        $("#fortaleza-texto").textContent = linea;
        return V.contrasenaCumpleMinimo(pwd, parametros.longitudMinima) && puntaje >= parametros.fortalezaMinima;
    }
    $("#contrasena").addEventListener("input", () => { fortaleza(); if ($("#confirmar").value) coinciden(); });
    function coinciden() {
        if (!$("#confirmar").value) { marcar("confirmar", "mal", "Repetí la contraseña."); return false; }
        if ($("#confirmar").value !== $("#contrasena").value) { marcar("confirmar", "mal", "Las contraseñas no coinciden. Revisá lo que escribiste."); return false; }
        marcar("confirmar", "ok", "Coinciden."); return true;
    }
    $("#confirmar").addEventListener("input", coinciden);
    $("#mostrar").addEventListener("change", e => ["contrasena", "confirmar"].forEach(id => { document.getElementById(id).type = e.target.checked ? "text" : "password"; }));
    $("#mail").addEventListener("input", () => marcar("mail", "", ""));
    $("#codigo").addEventListener("input", () => marcar("codigo", "", ""));

    $("#form-mail").addEventListener("submit", e => {
        e.preventDefault();
        if (!$("#mail").value.trim()) { marcar("mail", "mal", "Escribí tu correo electrónico."); $("#mail").focus(); return; }
        ocupado($("#enviar"), true, "Enviando…");
        puente.enviar({ tipo: "enviarCodigoRecuperacion", mail: $("#mail").value.trim() });
    });
    $("#otro-codigo").addEventListener("click", () => {
        ocupado($("#otro-codigo"), true, "Enviando…");
        puente.enviar({ tipo: "enviarCodigoRecuperacion", mail: $("#mail").value.trim() });
    });
    $("#form-codigo").addEventListener("submit", e => {
        e.preventDefault();
        let ok = true;
        if (!$("#codigo").value.trim()) { marcar("codigo", "mal", "Escribí el código que recibiste."); ok = false; }
        if (!fortaleza()) { $("#contrasena").setAttribute("aria-invalid", "true"); ok = false; }
        if (!coinciden()) ok = false;
        if (!ok) return;
        ocupado($("#restablecer"), true, "Guardando…");
        puente.enviar({ tipo: "restablecer", mail: $("#mail").value.trim(), codigo: $("#codigo").value.trim(), contrasena: $("#contrasena").value });
    });
    document.querySelectorAll("[data-destino]").forEach(b =>
        b.addEventListener("click", () => puente.enviar({ tipo: "navegar", destino: b.dataset.destino })));

    // datos: { paso: "mail" | "codigo" | "listo", parametros?, mail?, error?: { campo?, mensaje } }
    puente.alDibujar(datos => {
        ["enviar", "otro-codigo", "restablecer"].forEach(id => ocupado(document.getElementById(id), false));
        if (datos.parametros) { Object.assign(parametros, datos.parametros); fortaleza(); }
        if (datos.mail && !$("#mail").value) $("#mail").value = datos.mail;
        if (datos.paso) mostrarPaso(datos.paso);
        if (datos.paso === "codigo" || datos.paso === "listo") { $("#contrasena").value = ""; $("#confirmar").value = ""; fortaleza(); }
        $("#error").classList.add("oculto");
        if (datos.error) {
            if (datos.error.campo && $("#estado-" + datos.error.campo)) marcar(datos.error.campo, "mal", datos.error.mensaje);
            else { $("#error").textContent = datos.error.mensaje; $("#error").classList.remove("oculto"); }
        }
    });

    // ---- Solo en la maqueta ----
    let codigo = null;
    puente.simular(m => {
        if (m.tipo === "enviarCodigoRecuperacion") {
            codigo = String(Math.floor(100000 + Math.random() * 900000));
            puente.anotarFuera("Correo simulado para " + m.mail + " · Código: " + codigo + " (si la cuenta existe; la pantalla no lo dice)");
            puente.responder({ paso: "codigo" });
        } else if (m.tipo === "restablecer") {
            puente.responder(m.codigo === codigo ? { paso: "listo" }
                : { paso: "codigo", error: { campo: "codigo", mensaje: "El código no es válido o venció. Solicitá uno nuevo." } });
        } else if (m.tipo === "navegar" && m.destino === "acceso") {
            setTimeout(() => { window.location.href = "acceso.html"; }, 500);
        }
    }, {
        "Paso: código": { paso: "codigo" },
        "Código vencido": { paso: "codigo", error: { campo: "codigo", mensaje: "El código no es válido o venció. Solicitá uno nuevo." } },
        "Listo": { paso: "listo" }
    }, { paso: "mail", parametros: { longitudMinima: 8, fortalezaMinima: 4 } });

    // Solo en la maqueta: #codigo o #listo abren ese paso.
    if (!puente.enAplicacion && location.hash) puente.responder({ paso: location.hash.slice(1) });
})();
