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
    const bloque = nodo("div");
    const filas = nodo("div", null, "filas-barras");
    filas.tabIndex = 0;
    filas.setAttribute("aria-label", "Desplazar barras de eventos");
    const alto = 24;
    const altoSvg = alto * serie.length;
    const svg = baseGrafico(altoSvg, "Entradas vendidas por evento");
    const eje = baseGrafico(22, "Escala en entradas");
    const marcoEje = nodo("div", null, "eje-barras");
    const mayor = Math.max(1, ...serie.map(p => p.valor));
    const orden = 10 ** Math.floor(Math.log10(mayor / 5));
    const paso = Math.max(1, [1, 2, 5, 10].find(multiplo => multiplo * orden >= mayor / 5) * orden);
    const max = Math.ceil(mayor / paso) * paso;
    const x0 = 250;
    const anchoEscala = 413;
    for (let marca = 0; marca <= max; marca += paso) {
        const x = x0 + marca * anchoEscala / max;
        eje.append(figura("text", { x, y: 16, "text-anchor": "middle" }, entero.format(marca)));
        svg.append(figura("line", { x1: x, x2: x, y1: 0, y2: altoSvg, stroke: "var(--borde)" }));
    }
    eje.append(figura("line", { x1: x0, x2: x0 + anchoEscala, y1: 21, y2: 21, stroke: "var(--borde)" }));
    serie.slice().sort((a, b) => b.valor - a.valor).forEach((p, i) => {
        const y = 3 + alto * i;
        const ancho = p.valor * anchoEscala / max;
        const nombre = p.etiqueta.length > 32 ? p.etiqueta.slice(0, 31) + "…" : p.etiqueta;
        const rotulo = figura("text", { x: 1, y: y + 14 }, nombre);
        rotulo.prepend(figura("title", {}, p.etiqueta));
        svg.append(rotulo, figura("rect", { x: x0, y, width: ancho, height: 18, fill: "var(--primario)" }),
            figura("text", { x: x0 + ancho + 9, y: y + 14 }, entero.format(p.valor)));
    });
    filas.append(svg);
    marcoEje.append(eje);
    bloque.append(marcoEje, filas);
    return bloque;
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
function tabla(modelo) {
    const tabla = nodo("table");
    const cabecera = tabla.createTHead().insertRow();
    modelo.columnas.forEach((titulo, i) => cabecera.append(nodo("th", titulo, modelo.numericas[i] ? "numerica" : "")));
    const cuerpo = tabla.createTBody();
    modelo.filas.forEach((fila, i) => {
        const tr = cuerpo.insertRow();
        tr.classList.toggle("total", modelo.hayTotal && i === modelo.filas.length - 1);
        fila.forEach((valor, j) => tr.append(nodo("td", valor, modelo.numericas[j] ? "numerica" : "")));
    });
    const marco = nodo("div", null, "tabla-resultados");
    marco.tabIndex = 0;
    marco.setAttribute("aria-label", "Tabla del reporte");
    marco.append(tabla);
    return marco;
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
    destino.append(tabla(reporte));
    if (datos.lectura) destino.append(nodo("p", datos.lectura, "secundario"));
};