"use strict";
const app = document.querySelector("#app");
const nodo = (tag, clase, texto) => { const e = document.createElement(tag); if (clase) e.className = clase; if (texto != null) e.textContent = texto; return e; };
const seccion = titulo => { const s = nodo("section", "seccion"); s.append(nodo("h2", "encabezado", titulo)); return s; };
function dibujarHistorial(m) {
  app.replaceChildren();
  const compras = seccion("Compras");
  const lista = nodo("div", "grilla");
  (m.compras || []).forEach(c => {
    const t = nodo("article", "tarjeta" + (c.id === m.seleccionada ? " activa" : ""));
    const b = nodo("button", "principal", c.evento || "Evento"); b.type = "button";
    b.addEventListener("click", () => window.chrome.webview.postMessage({ tipo: "compra", id: c.id }));
    t.append(b, nodo("span", "secundario", "Pedido " + c.pedido + " · " + c.fecha + " · " + c.estado));
    t.append(nodo("span", "dato", c.entradas + " entradas · " + c.total + " · devuelto " + c.devuelto));
    lista.append(t);
  });
  if (!m.compras || !m.compras.length) { const vacio = nodo("div", "vacio"); vacio.append(nodo("strong", "", "Todavía no compraste entradas."), nodo("span", "secundario", "Cuando hagas una compra, el pedido y sus entradas van a aparecer acá.")); lista.append(vacio); }
  compras.append(lista);
  const detalle = seccion(m.detalle || "Entradas");
  const filas = nodo("div", "entradas");
  (m.entradas || []).forEach(e => {
    const f = nodo("label", "entrada"); const check = document.createElement("input"); check.type = "checkbox"; check.checked = e.devolver; check.disabled = !e.puedeDevolver;
    check.addEventListener("change", () => window.chrome.webview.postMessage({ tipo: "entrada", id: e.id, devolver: check.checked }));
    const descripcion = nodo("span", "", e.sector + " · " + e.precio + " · " + e.estado + " · seguro " + e.seguro);
    f.append(check, descripcion, nodo("span", "codigo", e.codigo)); filas.append(f);
  });
  if (!m.entradas || !m.entradas.length) filas.append(nodo("div", "vacio secundario", "Elegí un pedido para ver sus entradas."));
  detalle.append(filas); app.append(compras, detalle);
}
window.eventAuraDibujar = dibujarHistorial;
