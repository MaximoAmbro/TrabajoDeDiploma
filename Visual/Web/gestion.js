"use strict";
const app = document.querySelector("#app");
const nodo = (tag, clase, texto) => { const e = document.createElement(tag); if (clase) e.className = clase; if (texto != null) e.textContent = texto; return e; };
const enviar = mensaje => window.chrome.webview.postMessage(mensaje);
const seccion = titulo => { const s = nodo("section", "seccion"); s.append(nodo("h2", "encabezado", titulo)); return s; };
function estadoVacio(mensaje, accion, tipo) {
  const v = nodo("div", "vacio"); v.append(nodo("strong", "", mensaje));
  if (accion) { const b = nodo("button", "accion", accion); b.type = "button"; b.addEventListener("click", () => enviar({ tipo })); v.append(b); }
  return v;
}
function dibujarHistorial(m) {
  app.replaceChildren();
  const compras = seccion("Compras"); const lista = nodo("div", "grilla");
  (m.compras || []).forEach(c => {
    const t = nodo("article", "tarjeta" + (c.id === m.seleccionada ? " activa" : ""));
    const b = nodo("button", "principal", c.evento || "Evento"); b.type = "button";
    b.addEventListener("click", () => enviar({ tipo: "compra", id: c.id }));
    t.append(b, nodo("span", "secundario", "Pedido " + c.pedido + " · " + c.fecha + " · " + c.estado));
    t.append(nodo("span", "dato", c.entradas + " entradas · " + c.total + " · devuelto " + c.devuelto)); lista.append(t);
  });
  if (!m.compras || !m.compras.length) lista.append(estadoVacio("Todavía no compraste entradas.", "Ver eventos", "eventos"));
  compras.append(lista);
  const detalle = seccion(m.detalle || "Entradas"); const filas = nodo("div", "entradas");
  (m.entradas || []).forEach(e => {
    const f = nodo("label", "entrada-ticket"); const check = document.createElement("input"); check.type = "checkbox"; check.checked = e.devolver; check.disabled = !e.puedeDevolver;
    check.addEventListener("change", () => enviar({ tipo: "entrada", id: e.id, devolver: check.checked }));
    f.append(check, nodo("span", "", e.sector + " · " + e.precio + " · " + e.estado + " · seguro " + e.seguro), nodo("span", "codigo", e.codigo)); filas.append(f);
  });
  if (!m.entradas || !m.entradas.length) filas.append(nodo("div", "vacio secundario", "Elegí un pedido para ver sus entradas."));
  detalle.append(filas); app.append(compras, detalle);
}
function dibujarLocales(m) {
  app.replaceChildren(); const s = seccion("Mis locales"); const grilla = nodo("div", "grilla");
  (m.locales || []).forEach(l => {
    const t = nodo("button", "tarjeta seleccionable" + (l.id === m.seleccionado ? " activa" : "")); t.type = "button";
    t.addEventListener("click", () => enviar({ tipo: "local", id: l.id }));
    t.append(nodo("strong", "dato", l.nombre), nodo("span", "secundario", l.direccion), nodo("span", "dato", "Capacidad " + l.capacidad)); grilla.append(t);
  });
  if (!m.locales || !m.locales.length) grilla.append(estadoVacio("Todavía no registraste un local.", "Registrar local", "registro"));
  s.append(grilla); app.append(s);
}
function dibujarEventosLocal(m) {
  app.replaceChildren(); const s = seccion(m.local || "Eventos del local"); const grilla = nodo("div", "grilla");
  (m.eventos || []).forEach(e => {
    const t = nodo("button", "tarjeta seleccionable" + (e.id === m.seleccionado ? " activa" : "")); t.type = "button";
    t.addEventListener("click", () => enviar({ tipo: "eventoLocal", id: e.id }));
    t.append(nodo("strong", "dato", e.nombre), nodo("span", "secundario", e.fecha), nodo("span", "etiqueta", e.estado));
    if (e.motivo) t.append(nodo("span", "secundario", "Motivo: " + e.motivo)); grilla.append(t);
  });
  if (!m.eventos || !m.eventos.length) grilla.append(estadoVacio("Todavía no hay eventos en este local.", "Agregar evento", "agregar"));
  s.append(grilla); app.append(s);
}
function dibujarControlIngreso(m) {
  app.replaceChildren(); const s = seccion("Registrar ingreso");
  if (!m.eventos || !m.eventos.length) s.append(estadoVacio("No hay eventos con ingreso abierto en tus locales.", "Volver al panel", "volver"));
  else {
    const form = nodo("form", "formulario"); const labelEvento = nodo("label", "campo"); labelEvento.append(nodo("span", "encabezado", "Evento"));
    const select = document.createElement("select"); (m.eventos || []).forEach(e => { const o = document.createElement("option"); o.value = e.id; o.textContent = e.nombre; o.selected = e.id === m.seleccionado; select.append(o); }); labelEvento.append(select);
    const labelCodigo = nodo("label", "campo"); labelCodigo.append(nodo("span", "encabezado", "Código del ticket"));
    const codigo = document.createElement("input"); codigo.autocomplete = "off"; codigo.spellcheck = false; codigo.value = m.codigo || ""; codigo.placeholder = "Escaneá el QR o escribí el código"; labelCodigo.append(codigo);
    const boton = nodo("button", "accion", "Registrar ingreso"); boton.type = "submit";
    form.append(labelEvento, labelCodigo, boton);
    form.addEventListener("submit", e => { e.preventDefault(); enviar({ tipo: "ingreso", id: Number(select.value), codigo: codigo.value }); });
    const resultado = nodo("div", "resultado", m.resultado || "Escaneá una entrada.");
    s.append(form, resultado, nodo("p", "secundario dato", m.contador || ""));
  }
  app.append(s);
}
function dibujarAuditoria(m) {
  app.replaceChildren(); const s = seccion("Movimientos registrados");
  const tabla = nodo("table", "tabla"); const head = document.createElement("thead"); const hr = document.createElement("tr");
  ["Fecha y hora", "Correo", "Acción", "Qué se modificó", "Resultado"].forEach(x => hr.append(nodo("th", "", x))); head.append(hr); tabla.append(head);
  const body = document.createElement("tbody"); (m.filas || []).forEach(f => { const tr = document.createElement("tr");
    [f.fecha, f.correo, f.accion, f.entidad, f.resultado].forEach(x => tr.append(nodo("td", "", x || "—"))); body.append(tr); }); tabla.append(body);
  if (!m.filas || !m.filas.length) s.append(estadoVacio("No hay movimientos con estos filtros.", null, null)); else s.append(tabla);
  app.append(s);
}
function dibujarRevision(m) {
  app.replaceChildren(); const s = seccion("Eventos pendientes de revisión"); const layout = nodo("div", "revision"); const lista = nodo("div", "grilla");
  (m.eventos || []).forEach(e => { const b = nodo("button", "tarjeta seleccionable" + (e.id === m.seleccionado ? " activa" : "")); b.type = "button";
    b.addEventListener("click", () => enviar({ tipo: "revision", id: e.id })); b.append(nodo("strong", "dato", e.nombre), nodo("span", "secundario", e.fecha)); lista.append(b); });
  if (!m.eventos || !m.eventos.length) lista.append(estadoVacio("No hay eventos pendientes de revisión.", "Actualizar", "actualizar"));
  const detalle = nodo("article", "tarjeta detalleRevision"); detalle.append(nodo("h3", "dato", m.nombre || "Elegí un evento"), nodo("p", "secundario", m.detalle || ""));
  detalle.append(nodo("label", "campo", "Motivo del rechazo (obligatorio)")); const motivo = document.createElement("textarea"); motivo.value = m.motivo || "";
  motivo.addEventListener("input", () => enviar({ tipo: "motivoRevision", texto: motivo.value })); detalle.append(motivo); layout.append(lista, detalle); s.append(layout); app.append(s);
}
window.eventAuraDibujar = m => {
  switch (m && m.vista) {
    case "historial": dibujarHistorial(m); break;
    case "locales": dibujarLocales(m); break;
    case "eventosLocal": dibujarEventosLocal(m); break;
    case "controlIngreso": dibujarControlIngreso(m); break;
    case "auditoria": dibujarAuditoria(m); break;
    case "revisionEventos": dibujarRevision(m); break;
    default: app.replaceChildren(estadoVacio("Vista no disponible.", null, null)); return;
  }
};
