"use strict";
(function () {
    const puente = window.EventAuraPuente;
    const $ = s => document.querySelector(s);
    let seleccion = "Mensual";
    let accionPendiente = "contratar";
    function actualizar(datos) {
        if (datos.mensual) $("#precio-mensual").textContent = datos.mensual;
        if (datos.anual) $("#precio-anual").textContent = datos.anual;
        if (datos.ahorro) $("#ahorro").textContent = datos.ahorro;
        const activa = !!datos.activa;
        $("#estado").textContent = datos.estado || "No tenés una suscripción activa.";
        $("#estado").dataset.activa = String(activa);
        $("#cancelar").classList.toggle("oculto", !activa);
        $("#contratar").classList.toggle("oculto", activa);
        document.querySelectorAll('input[name="plan"]').forEach(radio => {
            radio.disabled = activa;
            radio.checked = activa ? radio.value === datos.plan : radio.value === seleccion;
        });
        $("#error").classList.toggle("oculto", !datos.error);
        if (datos.error) $("#error").textContent = datos.error;
    }
    document.querySelectorAll('input[name="plan"]').forEach(radio => radio.addEventListener("change", () => { seleccion = radio.value; }));
    function abrirConfirmacion(accion) {
        accionPendiente = accion;
        const dialogo = $("#confirmacion");
        const titulo = $("#confirmacion-titulo");
        const detalle = $("#confirmacion-detalle");
        const aceptar = $("#aceptar-confirmacion");
        detalle.replaceChildren();
        if (accion === "contratar") {
            const precio = seleccion === "Anual" ? $("#precio-anual").textContent : $("#precio-mensual").textContent;
            titulo.textContent = "Confirmar suscripción";
            detalle.append(document.createTextNode("Plan: " + seleccion + " · Precio: " + precio + " · Vigencia: " + (seleccion === "Anual" ? "12 meses" : "1 mes")));
            aceptar.textContent = "Confirmar suscripción";
        } else {
            titulo.textContent = "Cancelar suscripción";
            detalle.append(document.createTextNode("¿Cancelar la suscripción? Los beneficios se pierden desde ahora."));
            aceptar.textContent = "Confirmar cancelación";
        }
        dialogo.showModal();
        aceptar.focus();
    }
    $("#contratar").addEventListener("click", () => abrirConfirmacion("contratar"));
    $("#cancelar").addEventListener("click", () => abrirConfirmacion("cancelar"));
    $("#volver-confirmacion").addEventListener("click", () => $("#confirmacion").close());
    $("#form-confirmacion").addEventListener("submit", e => {
        e.preventDefault();
        $("#confirmacion").close();
        puente.enviar({ tipo: accionPendiente === "contratar" ? "contratarConfirmado" : "cancelarConfirmado", plan: seleccion });
    });
    document.addEventListener("keydown", e => {
        if (e.key === "Enter" && !e.defaultPrevented && e.target.closest("#planes")) {
            e.preventDefault();
            if (!$("#contratar").classList.contains("oculto")) $("#contratar").click();
        }
    });
    puente.alDibujar(actualizar);
    puente.simular(() => {}, {}, {});
})();
