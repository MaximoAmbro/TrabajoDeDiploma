"use strict";
(function () {
    const puente = window.EventAuraPuente;
    const $ = s => document.querySelector(s);
    let seleccion = "Mensual";
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
    $("#contratar").addEventListener("click", () => puente.enviar({ tipo: "contratar", plan: seleccion }));
    $("#cancelar").addEventListener("click", () => puente.enviar({ tipo: "cancelar" }));
    document.addEventListener("keydown", e => {
        if (e.key === "Enter" && !e.defaultPrevented && e.target.closest("#planes")) {
            e.preventDefault();
            if (!$("#contratar").classList.contains("oculto")) $("#contratar").click();
        }
    });
    puente.alDibujar(actualizar);
    puente.simular(() => {}, {}, {});
})();
