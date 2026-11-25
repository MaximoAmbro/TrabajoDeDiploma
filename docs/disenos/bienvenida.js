"use strict";
// Bienvenida: dos caminos. No pide datos: solo avisa a la aplicación adónde ir.
(function () {
    const puente = window.EventAuraPuente;
    const aviso = document.querySelector("#aviso");

    document.querySelectorAll("[data-destino]").forEach(boton =>
        boton.addEventListener("click", () => puente.enviar({ tipo: "navegar", destino: boton.dataset.destino })));

    // datos: { aviso?: string }  (por ejemplo, el de base no disponible; ver README)
    puente.alDibujar(datos => {
        aviso.textContent = datos.aviso || "";
        aviso.classList.toggle("oculto", !datos.aviso);
        document.querySelectorAll("[data-destino]").forEach(b => b.disabled = !!datos.bloquear);
    });

    puente.simular(mensaje => {
        if (mensaje.tipo === "navegar") {
            const archivo = mensaje.destino === "alta" ? "alta-cuenta.html" : "acceso.html";
            puente.anotarFuera("La aplicación abriría " + archivo + ".");
            setTimeout(() => { window.location.href = archivo; }, 700);
        }
    }, {
        "Normal": {},
        "Base no disponible": {
            bloquear: true,
            aviso: "EventAura no pudo comprobar la base de datos. Instancia: .\\SQLEXPRESS · Base: BddEventAura_Demostracion. Revisá SQL Server y la configuración, y volvé a abrir la aplicación."
        }
    });

    document.querySelector(".camino-principal").focus();
})();
