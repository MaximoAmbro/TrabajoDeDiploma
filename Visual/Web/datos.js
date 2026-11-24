"use strict";
const destino = document.querySelector("#datos");
const entero = new Intl.NumberFormat("es-AR");
const svgNS = "http://www.w3.org/2000/svg";
const color = i => "var(--grafico-" + i % 6 + ")";
let vista = 0;
function nodo(tipo, texto, clase) {
    const el = document.createElement(tipo);
    if (texto != null) el.textContent = texto;
    if (clase) el.className = clase;
    return el;
}
function figura(tipo, atributos, texto) {
    const el = document.createElementNS(svgNS, tipo);
    Object.entries(atributos).forEach(([k, v]) => el.setAttribute(k, v));
    if (texto != null) el.textContent = texto;
    return el;
}
function baseGrafico(alto, titulo) {
    return figura("svg", { viewBox: "0 0 720 " + alto, role: "img", "aria-label": titulo });
}
function barras(serie) {
    const alto = 26;
    const svg = baseGrafico(alto * serie.length + 4, "Entradas vendidas por evento");
    const max = Math.max(1, ...serie.map(p => p.valor));
    serie.forEach((p, i) => {
        const y = 3 + alto * i, ancho = p.valor / max * 365;
        const nombre = p.etiqueta.length > 31 ? p.etiqueta.slice(0, 30) + "…" : p.etiqueta;
        const rotulo = figura("text", { x: 1, y: y + 15 }, nombre);
        rotulo.prepend(figura("title", {}, p.etiqueta));
        svg.append(rotulo, figura("rect", { x: 255, y, width: ancho, height: 19, fill: "var(--primario)" }),
            figura("text", { x: 265 + ancho, y: y + 15 }, entero.format(p.valor)));
    });
    return svg;
}
function categorias(serie) {
    const total = serie.reduce((s, p) => s + p.valor, 0);
    const svg = baseGrafico(30, "Distribución de entradas por categoría");
    const leyenda = nodo("div", null, "leyenda");
    let x = 0;
    serie.forEach((p, i) => {
        const ancho = total ? p.valor / total * 720 : 0;
        svg.append(figura("rect", { x, y: 0, width: ancho, height: 24, fill: color(i) }));
        x += ancho;
        const rotulo = nodo("span", p.etiqueta + " · " + entero.format(p.valor), "secundario");
        rotulo.style.setProperty("--muestra", color(i));
        leyenda.append(rotulo);
    });
    const grupo = nodo("div");
    grupo.append(svg, leyenda);
    return grupo;
}
function temporal(serie) {
    const svg = baseGrafico(150, "Entradas agrupadas por fecha del evento");
    const max = Math.max(1, ...serie.map(p => p.valor));
    svg.append(figura("path", { d: "M56 12 V114 H664", stroke: "var(--borde)", fill: "none" }),
        figura("text", { x: 0, y: 21 }, entero.format(max)), figura("text", { x: 0, y: 114 }, "0"));
    const puntos = serie.map((p, i) => {
        const [dd, mm, aa] = p.etiqueta.split("/").map(Number);
        return { ...p, fecha: Date.UTC(aa, mm - 1, dd) };
    });
    const delta = puntos[puntos.length - 1].fecha - puntos[0].fecha;
    puntos.forEach(p => {
        p.x = delta ? 56 + 608 * (p.fecha - puntos[0].fecha) / delta : 360;
        p.y = 114 - p.valor / max * 94;
    });
    svg.append(figura("polyline", { points: puntos.map(p => p.x + "," + p.y).join(" "),
        stroke: "var(--primario)", "stroke-width": 3, fill: "none" }));
    puntos.forEach((p, i) => {
        const punto = figura("circle", { cx: p.x, cy: p.y, r: 4, fill: "var(--primario)" });
        punto.append(figura("title", {}, p.etiqueta + " · " + entero.format(p.valor)));
        svg.append(punto);
        if (i === 0 || i === puntos.length - 1)
            svg.append(figura("text", { x: p.x, y: 136, "text-anchor": "middle" }, p.etiqueta));
    });
    return svg;
}
function graficos(modelo) {
    const opciones = [
        ["Entradas por evento", modelo.barras, barras],
        ["Por categoría", modelo.categorias, categorias],
        ["Por fecha de evento", modelo.temporal, temporal]
    ];
    const bloque = nodo("section", null, "grafico");
    const nav = nodo("div", null, "selector");
    const cuerpo = nodo("div");
    function activar() {
        Array.from(nav.children).forEach((b, i) => b.setAttribute("aria-pressed", i === vista));
        const [, serie, dibujar] = opciones[vista];
        cuerpo.replaceChildren(serie.length ? dibujar(serie) : nodo("p", "Sin datos para esta serie.", "secundario"));
    }
    opciones.forEach(([nombre], i) => {
        const b = nodo("button", nombre);
        b.type = "button";
        b.onclick = () => { vista = i; activar(); };
        nav.append(b);
    });
    bloque.append(nav, cuerpo);
    activar();
    return bloque;
}
function tabla(modelo, desplegable) {
    const tabla = nodo("table");
    const cabecera = tabla.createTHead().insertRow();
    modelo.columnas.forEach((titulo, i) => cabecera.append(nodo("th", titulo, modelo.numericas[i] ? "numerica" : "")));
    const cuerpo = tabla.createTBody();
    modelo.filas.forEach((fila, i) => {
        const tr = cuerpo.insertRow();
        tr.classList.toggle("total", modelo.hayTotal && i === modelo.filas.length - 1);
        fila.forEach((valor, j) => tr.append(nodo("td", valor, modelo.numericas[j] ? "numerica" : "")));
    });
    if (!desplegable) return tabla;
    const detalle = nodo("details");
    detalle.append(nodo("summary", "Ver tabla de datos"), tabla);
    return detalle;
}
function metricas(tarjetas) {
    const grilla = nodo("div", null, "metricas");
    tarjetas.forEach(tarjeta => {
        const bloque = nodo("div", null, "metrica");
        bloque.append(nodo("p", tarjeta.titulo, "secundario"), nodo("p", tarjeta.valor, "valor"));
        grilla.append(bloque);
    });
    return grilla;
}
window.eventAuraDibujar = datos => {
    destino.replaceChildren();
    if (!datos) return;
    const reporte = datos.reporte;
    if (datos.metricas) {
        destino.append(nodo("p", datos.periodo, "secundario"), metricas(datos.metricas),
            nodo("h2", "Pendientes de revisión"), metricas(datos.pendientes),
            nodo("h2", "Ventas por evento"));
        if (reporte) destino.append(graficos(reporte));
        return;
    }
    if (!reporte) return;
    if (reporte.barras.length) destino.append(graficos(reporte));
    destino.append(tabla(reporte, reporte.barras.length > 0));
    if (datos.lectura) destino.append(nodo("p", datos.lectura, "secundario"));
};