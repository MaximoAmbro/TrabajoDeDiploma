"use strict";
const lineas = document.querySelector("#lineas");
const total = document.querySelector("#total");
const ayuda = document.querySelector("#ayuda");
window.eventAuraDibujar = comprobante => {
    lineas.replaceChildren();
    if (!comprobante) return;
    let grupo = lineas;
    comprobante.filas.forEach(fila => {
        if (fila.seccion) {
            grupo = document.createElement("section");
            grupo.className = "grupo";
            const titulo = document.createElement("h2");
            titulo.textContent = fila.concepto;
            grupo.append(titulo);
            lineas.append(grupo);
            return;
        }
        const renglon = document.createElement("div");
        renglon.className = "fila";
        const concepto = document.createElement("span");
        concepto.textContent = fila.concepto;
        const importe = document.createElement("span");
        importe.className = "importe";
        importe.textContent = fila.importe || "";
        renglon.append(concepto, importe);
        grupo.append(renglon);
    });
    total.textContent = comprobante.total;
    ayuda.textContent = comprobante.ayuda;
};