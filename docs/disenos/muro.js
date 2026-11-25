"use strict";
// El muro de portadas del panel oscuro (PRINCIPIOS-DISENO.md §7). Replica Visual/Componentes/PortadaEvento.cs:
// degradado a 45° de PaletaPortadas[i] a PaletaPortadas[(i + 2) % 6], la inicial al 28 % abajo a la izquierda
// (la cápsula blanca de la categoría se omite: atenuada, es lo más claro de la portada y compite con el
// texto del panel). El índice i es ColorDelNombre: SHA-256(nombre)[0] % 6, calculado de
// antemano para los eventos de la semilla de demostración (no hace falta criptografía en la página).
(function () {
    const eventos = [
        ["Duki en Rosario", "Concierto", 1], ["Clásico Rosarino", "Deportivo", 4], ["Noche de Teatro Independiente", "Teatro", 3],
        ["Orquesta Típica del Litoral", "Concierto", 2], ["Copa Costanera de básquet 3x3", "Deportivo", 4], ["Feria gráfica del Litoral", "Cultural", 3],
        ["Fiesta de los Galpones", "Concierto", 3], ["La casa del río", "Teatro", 5], ["Noches de jazz en el puerto", "Concierto", 2],
        ["La orilla invisible", "Teatro", 2], ["Mapas de la memoria", "Cultural", 4], ["Torneo de vóley de la costanera", "Deportivo", 0],
        ["Milonga de los andenes", "Concierto", 4], ["Bajo la misma luna", "Teatro", 1], ["Taller de fotografía urbana", "Cultural", 2],
        ["Guitarras del litoral", "Concierto", 4], ["Fiesta de la cumbia santafesina", "Concierto", 0], ["Copa de verano 3x3", "Deportivo", 4],
        ["Una noche en el puerto", "Teatro", 5], ["Festival del río", "Concierto", 0], ["Laboratorio de historietas", "Cultural", 3],
        ["Encuentro de vóley mixto", "Deportivo", 1], ["Fiesta de fin de temporada", "Concierto", 5]
    ];
    document.querySelectorAll(".muro").forEach(muro => {
        const cantidad = Number(muro.dataset.cantidad || 27);
        for (let n = 0; n < cantidad; n++) {
            const [nombre, categoria, i] = eventos[(n * 7) % eventos.length];
            const portada = document.createElement("div");
            portada.className = "portada";
            portada.style.background = "linear-gradient(135deg, var(--portada-" + i + "), var(--portada-" + ((i + 2) % 6) + "))";
            const inicial = document.createElement("span");
            inicial.className = "inicial";
            inicial.textContent = Array.from(nombre)[0];
            portada.append(inicial);
            muro.append(portada);
        }
        muro.setAttribute("aria-hidden", "true");
    });
})();
