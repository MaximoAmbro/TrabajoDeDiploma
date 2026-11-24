"use strict";
const catalogo = document.querySelector("#catalogo");
function elemento(tipo, clase, texto) {
    const nodo = document.createElement(tipo);
    nodo.className = clase;
    if (texto) nodo.textContent = texto;
    return nodo;
}
window.eventAuraDibujar = eventos => {
    catalogo.replaceChildren(...(eventos || []).map(evento => {
        const tarjeta = elemento("article", "evento");
        tarjeta.dataset.evento = evento.id;
        const portada = elemento("span", "portada");
        portada.style.backgroundImage = "linear-gradient(120deg, " + evento.color + ", " + evento.colorFinal + ")";
        if (evento.imagen) {
            const imagen = elemento("img", "");
            imagen.alt = "";
            imagen.src = "data:image/png;base64," + evento.imagen;
            portada.append(imagen);
        } else {
            portada.append(elemento("span", "inicial", Array.from(evento.nombre)[0]));
        }
        portada.append(elemento("span", "estado " + evento.estado, evento.disponibilidad));
        const detalle = elemento("span", "detalle");
        const accion = elemento("button", "accion", "Ver entradas");
        accion.type = "button";
        accion.addEventListener("click", () => window.chrome.webview.postMessage({ tipo: "evento", id: evento.id }));
        detalle.append(elemento("span", "nombre", evento.nombre), elemento("span", "secundario", evento.fecha),
            elemento("span", "secundario", evento.lugar), elemento("span", "precio", evento.precio), accion);
        tarjeta.append(portada, detalle);
        return tarjeta;
    }));
};
