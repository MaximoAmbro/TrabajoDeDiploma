"use strict";
(function () {
    const puente = window.EventAuraPuente;
    const V = window.EventAuraValidaciones;
    const $ = s => document.querySelector(s);
    const parametros = { longitudMinima: 8, fortalezaMinima: 4 };
    const ids = ["actual", "nueva", "confirmar"];

    function marcar(id, mensaje) {
        const campo = $("#" + id), estado = $("#estado-" + id);
        campo.setAttribute("aria-invalid", mensaje ? "true" : "false");
        estado.className = "estado" + (mensaje ? " mal" : "");
        estado.textContent = mensaje || "";
    }
    function fortaleza() {
        const clave = $("#nueva").value;
        const puntaje = V.puntajeFortaleza(clave, parametros.longitudMinima);
        $("#fortaleza").dataset.nivel = puntaje < parametros.fortalezaMinima ? "bajo" : "ok";
        document.querySelectorAll("#fortaleza .barras span").forEach((barra, i) => barra.classList.toggle("lleno", i < puntaje));
        let texto = "Fortaleza: " + V.nivelFortaleza(puntaje, parametros.fortalezaMinima) + " (" + puntaje + "/5; mínimo " + parametros.fortalezaMinima + ")";
        if (clave && !V.contrasenaCumpleMinimo(clave, parametros.longitudMinima)) texto += ". Necesita al menos " + parametros.longitudMinima + " caracteres, con una letra y un número.";
        $("#fortaleza-texto").textContent = texto;
        return !!clave && V.contrasenaCumpleMinimo(clave, parametros.longitudMinima) && puntaje >= parametros.fortalezaMinima;
    }
    $("#nueva").addEventListener("input", () => { fortaleza(); if ($("#confirmar").value) validarConfirmacion(); });
    function validarConfirmacion() {
        if (!$("#confirmar").value) { marcar("confirmar", "Repetí la contraseña nueva."); return false; }
        if ($("#confirmar").value !== $("#nueva").value) { marcar("confirmar", "Las contraseñas no coinciden. Revisá lo que escribiste."); return false; }
        marcar("confirmar", ""); return true;
    }
    $("#confirmar").addEventListener("input", validarConfirmacion);
    ids.forEach(id => $("#" + id).addEventListener("input", () => { marcar(id, ""); $("#error").classList.add("oculto"); $("#exito").classList.add("oculto"); }));
    $("#volver").addEventListener("click", () => puente.enviar({ tipo: "navegar" }));
    $("#formulario").addEventListener("submit", e => {
        e.preventDefault();
        let valido = true;
        if (!$("#actual").value) { marcar("actual", "Ingresá la contraseña actual."); valido = false; }
        if (!fortaleza()) { marcar("nueva", "La contraseña no alcanza la fortaleza mínima."); valido = false; }
        if (!validarConfirmacion()) valido = false;
        if (!valido) return;
        $("#confirmar-cambio").disabled = true;
        puente.enviar({ tipo: "cambiarContrasena", actual: $("#actual").value, nueva: $("#nueva").value });
    });
    puente.alDibujar(datos => {
        if (datos.mail) $("#mail").value = datos.mail;
        if (datos.parametros) Object.assign(parametros, datos.parametros);
        fortaleza();
        $("#confirmar-cambio").disabled = false;
        if (datos.error) {
            if (datos.error.campo && $("#estado-" + datos.error.campo)) marcar(datos.error.campo, datos.error.mensaje);
            else { $("#error").textContent = datos.error.mensaje; $("#error").classList.remove("oculto"); }
        }
        if (datos.estado === "ok") {
            ids.forEach(id => { $("#" + id).value = ""; marcar(id, ""); });
            $("#exito").textContent = datos.mensaje;
            $("#exito").classList.remove("oculto");
            fortaleza();
        }
    });
    puente.simular(() => {}, {}, { mail: "cliente@eventaura.com" });
})();
