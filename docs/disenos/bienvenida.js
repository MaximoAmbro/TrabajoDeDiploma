"use strict";
// Bienvenida: dos caminos y la recuperación. No pide datos: solo avisa a la aplicación adónde ir.
(function () {
    const puente = window.EventAuraPuente;
    const aviso = document.querySelector("#aviso");
    const archivos = { acceso: "acceso.html", alta: "alta-cuenta.html", recuperar: "recuperar-clave.html" };

    document.querySelectorAll("[data-destino]").forEach(boton =>
        boton.addEventListener("click", () => puente.enviar({ tipo: "navegar", destino: boton.dataset.destino })));

    // datos: { aviso?: string, bloquear?: bool }  (ver README.md)
    puente.alDibujar(datos => {
        aviso.textContent = datos.aviso || "";
        aviso.classList.toggle("oculto", !datos.aviso);
        document.querySelectorAll(".eleccion, [data-destino]").forEach(b => { b.disabled = !!datos.bloquear; });
    });

    puente.simular(m => {
        if (m.tipo === "navegar" && archivos[m.destino]) setTimeout(() => { window.location.href = archivos[m.destino]; }, 500);
    }, {
        "Normal": {},
        "Base no disponible": {
            bloquear: true,
            aviso: "EventAura no pudo comprobar la base de datos. Instancia: .\\SQLEXPRESS · Base: BddEventAura_Demostracion. Revisá SQL Server y la configuración, y volvé a abrir la aplicación."
        }
    });
})();
