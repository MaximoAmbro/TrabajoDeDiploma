"use strict";
// Armador de paquete. La página calcula una VISTA PREVIA con la misma fórmula que
// GestorFidelizacion.CalcularDescuento (RN-20, RN-21, RN-28, RN-37):
//   base = Σ precio × cantidad;  descuento = Redondear(base × porcentaje del nivel)
//   subtotalEntradas = base − descuento;  servicios = Σ precio de cada servicio elegido (sin descuento)
//   cargo = Redondear(subtotalEntradas × CARGO_SERVICIO);  seguro = entradas × SEGURO_PRECIO (0 con suscripción)
//   total = subtotalEntradas + servicios + cargo + seguro;  Redondear = 2 decimales, «lejos de cero»
// Cada cambio se manda a la aplicación («seleccion»); su respuesta («resumen») es la que vale y reemplaza la
// vista previa. Así el número que ve el cliente es siempre el del negocio.
(function () {
    const puente = window.EventAuraPuente;
    const $ = s => document.querySelector(s);
    const pesos = new Intl.NumberFormat("es-AR", { style: "currency", currency: "ARS", minimumFractionDigits: 2 });
    const plata = v => pesos.format(v).replace(/ /g, " ");
    const redondear = v => Math.sign(v) * Math.round((Math.abs(v) + Number.EPSILON) * 100) / 100;
    const coloresSector = [4, 3, 5, 2, 1, 0];   // índices de --portada-N por orden de cercanía al escenario

    let datos = null;
    const eleccion = { cantidades: {}, servicios: new Set(), seguro: false };
    let version = 0;

    // ---------- El único movimiento del proyecto (PRINCIPIOS-DISENO.md §8) ----------
    // El total cuenta hasta el valor nuevo en 200 ms, con desaceleración. Con prefers-reduced-motion, salta.
    const sinMovimiento = window.matchMedia("(prefers-reduced-motion: reduce)");
    let totalMostrado = 0, cuadro = 0, pedidoTotal = 0;
    function escribirTotal(valor) {
        const [entero, decimales] = plata(valor).split(",");
        const total = $("#total");
        total.textContent = entero;
        const dec = document.createElement("span");
        dec.className = "decimales"; dec.textContent = "," + decimales;
        total.append(dec);
    }
    function animarTotal(destino) {
        cancelAnimationFrame(cuadro);
        const desde = totalMostrado;
        if (desde === destino || sinMovimiento.matches) { totalMostrado = destino; escribirTotal(destino); return; }
        const inicio = performance.now(), duracion = 200;
        const paso = ahora => {
            const t = Math.min(1, (ahora - inicio) / duracion);
            const suave = 1 - Math.pow(1 - t, 3);
            totalMostrado = t < 1 ? Math.round((desde + (destino - desde) * suave) * 100) / 100 : destino;
            escribirTotal(totalMostrado);
            if (t < 1) cuadro = requestAnimationFrame(paso);
        };
        cuadro = requestAnimationFrame(paso);
        // Respaldo: si no hay cuadros (vista oculta, sin pintura), a los 260 ms queda el valor exacto.
        const pedido = ++pedidoTotal;
        setTimeout(() => {
            if (pedido !== pedidoTotal || totalMostrado === destino) return;
            cancelAnimationFrame(cuadro); totalMostrado = destino; escribirTotal(destino);
        }, 260);
    }

    // ---------- Dibujo del evento y de las opciones (una vez por carga de datos) ----------
    function dibujarEvento(e) {
        $("#evento-nombre").textContent = e.nombre;
        $("#evento-categoria").textContent = e.categoria + " · " + e.lugar;
        $("#evento-detalle").textContent = e.fecha + " · " + e.direccion;
        $("#evento-inicial").textContent = Array.from(e.nombre)[0];
        $("#evento-portada").style.background = "linear-gradient(135deg, var(--portada-" + e.colorIndice + "), var(--portada-" + ((e.colorIndice + 2) % 6) + "))";
    }

    const icono = { menos: '<svg viewBox="0 0 12 12" aria-hidden="true"><path d="M2 6h8" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/></svg>',
                    mas: '<svg viewBox="0 0 12 12" aria-hidden="true"><path d="M2 6h8M6 2v8" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/></svg>' };

    function dibujarSectores() {
        const ordenados = [...datos.sectores].sort((a, b) => b.precio - a.precio);
        $("#max-entradas").textContent = datos.parametros.maxEntradas;
        $("#sectores").replaceChildren(...ordenados.map((s, k) => {
            const tarjeta = document.createElement("article");
            tarjeta.className = "sector" + (s.disponibles <= 0 ? " agotado" : "");
            tarjeta.dataset.sector = s.id;
            tarjeta.style.setProperty("--color-sector", "var(--portada-" + coloresSector[k % coloresSector.length] + ")");
            const ocupado = s.capacidad > 0 ? Math.round((1 - s.disponibles / s.capacidad) * 100) : 100;
            tarjeta.innerHTML =
                '<div class="pila pila-1"><h3 class="n4 fuerte"></h3><p class="precio"></p></div>' +
                '<div class="disponibilidad"><div class="ocupacion" aria-hidden="true"><span></span></div><p class="n5 secundario cifra"></p></div>' +
                '<div class="cantidad" role="group"><button type="button" class="menos" aria-label="Quitar una entrada">' + icono.menos + '</button>' +
                '<output aria-live="polite">0</output><button type="button" class="mas" aria-label="Agregar una entrada">' + icono.mas + '</button></div>';
            tarjeta.querySelector("h3").textContent = s.nombre;
            tarjeta.querySelector(".precio").textContent = plata(s.precio);
            tarjeta.querySelector(".ocupacion span").style.width = ocupado + "%";
            tarjeta.querySelector(".disponibilidad p").textContent = s.disponibles <= 0 ? "Agotado"
                : s.disponibles.toLocaleString("es-AR") + (s.disponibles === 1 ? " disponible" : " disponibles");
            tarjeta.querySelector(".cantidad").setAttribute("aria-label", "Entradas de " + s.nombre);
            tarjeta.querySelector(".menos").addEventListener("click", () => cambiarCantidad(s, -1));
            tarjeta.querySelector(".mas").addEventListener("click", () => cambiarCantidad(s, +1));
            if (s.disponibles <= 0) {
                const espera = document.createElement("button");
                espera.type = "button"; espera.className = "enlace n5 al-inicio";
                espera.textContent = datos.listaEsperaInscripto ? "Ya estás en la lista de espera" : "Anotarme en la lista de espera";
                espera.disabled = !!datos.listaEsperaInscripto;
                espera.setAttribute("aria-live", "polite");
                if (!datos.listaEsperaInscripto)
                    espera.addEventListener("click", () => puente.enviar({ tipo: "listaEspera", sectorId: s.id }));
                tarjeta.append(espera);
            }
            return tarjeta;
        }));
    }

    function dibujarServicios() {
        $("#servicios").replaceChildren(...datos.servicios.map(sv => {
            const tarjeta = document.createElement("label");
            tarjeta.className = "interruptor";
            tarjeta.innerHTML = '<input type="checkbox"><span class="pila pila-1"><span class="n6 secundario"></span><span class="n4 fuerte"></span></span>' +
                '<span class="n4 precio"></span><span class="llave" aria-hidden="true"></span>';
            tarjeta.querySelector(".n6").textContent = sv.tipo;
            tarjeta.querySelector(".fuerte").textContent = sv.nombre;
            tarjeta.querySelector(".precio").textContent = plata(sv.precio);
            const casilla = tarjeta.querySelector("input");
            casilla.checked = eleccion.servicios.has(sv.id);
            casilla.addEventListener("change", () => {
                if (casilla.checked) eleccion.servicios.add(sv.id); else eleccion.servicios.delete(sv.id);
                cambio();
            });
            return tarjeta;
        }));
    }

    // ---------- Elecciones ----------
    const totalEntradas = () => Object.values(eleccion.cantidades).reduce((a, b) => a + b, 0);

    function cambiarCantidad(sector, delta) {
        const actual = eleccion.cantidades[sector.id] || 0;
        const nueva = actual + delta;
        if (nueva < 0 || nueva > sector.disponibles) return;
        if (delta > 0 && totalEntradas() >= datos.parametros.maxEntradas) return;
        if (nueva === 0) delete eleccion.cantidades[sector.id]; else eleccion.cantidades[sector.id] = nueva;
        cambio();
    }
    $("#seguro").addEventListener("change", e => { eleccion.seguro = e.target.checked; cambio(); });

    function actualizarControles() {
        const total = totalEntradas(), max = datos.parametros.maxEntradas;
        document.querySelectorAll(".sector").forEach(t => {
            const s = datos.sectores.find(x => String(x.id) === t.dataset.sector);
            const n = eleccion.cantidades[s.id] || 0;
            t.classList.toggle("elegido", n > 0);
            const salida = t.querySelector("output"); if (salida) salida.textContent = n;
            const menos = t.querySelector(".menos"), mas = t.querySelector(".mas");
            if (menos) menos.disabled = n === 0;
            if (mas) mas.disabled = n >= s.disponibles || total >= max;
        });
        $("#cupo").textContent = total >= max ? "Llegaste al máximo de " + max : total + " de " + max;
        const seguroUnitario = datos.cliente.suscripcionActiva ? 0 : datos.parametros.seguroPrecio;
        $("#seguro-detalle").textContent = datos.cliente.suscripcionActiva
            ? "Devolución hasta 48 h antes del evento. Sin cargo por tu suscripción."
            : "Devolución hasta 48 h antes del evento. " + plata(datos.parametros.seguroPrecio) + " por entrada.";
        $("#seguro-precio").textContent = total ? plata(seguroUnitario * total) : (seguroUnitario ? plata(seguroUnitario) + " c/u" : "Sin cargo");
        $("#continuar").disabled = total === 0;
        $("#confirmar-seguro").classList.add("oculto");
    }

    // ---------- Cálculo (vista previa) ----------
    function calcular() {
        const p = datos.parametros, c = datos.cliente;
        let base = 0, cantidad = 0;
        datos.sectores.forEach(s => { const n = eleccion.cantidades[s.id] || 0; base += s.precio * n; cantidad += n; });
        const montoDescuento = redondear(base * c.porcentaje);
        const subtotalEntradas = base - montoDescuento;
        const servicios = datos.servicios.filter(s => eleccion.servicios.has(s.id)).reduce((a, s) => a + s.precio, 0);
        const cargoServicio = redondear(subtotalEntradas * p.cargoServicio);
        let seguro = eleccion.seguro ? cantidad * p.seguroPrecio : 0;
        if (seguro > 0 && c.suscripcionActiva) seguro = redondear(seguro * (1 - p.bonificacionSeguro));
        return { baseEntradas: base, porcentaje: c.porcentaje, montoDescuento, subtotalEntradas, servicios, cargoServicio, seguro,
                 total: subtotalEntradas + servicios + cargoServicio + seguro };
    }

    // ---------- El desglose, línea por línea ----------
    function lineas(r) {
        const filas = [], cantidad = totalEntradas();
        if (!cantidad) return filas;
        filas.push({ clave: "g-entradas", grupo: "Entradas" });
        datos.sectores.forEach(s => {
            const n = eleccion.cantidades[s.id];
            if (n) filas.push({ clave: "s" + s.id, texto: n + " × " + s.nombre, importe: plata(s.precio * n) });
        });
        if (r.montoDescuento > 0) {
            const porcentaje = (r.porcentaje * 100).toLocaleString("es-AR", { maximumFractionDigits: 1 });
            filas.push({ clave: "descuento", clase: "descuento", texto: "Descuento " + datos.cliente.nivel + " " + porcentaje + " %", importe: "−" + plata(r.montoDescuento) });
        }
        const elegidos = datos.servicios.filter(s => eleccion.servicios.has(s.id));
        if (elegidos.length) {
            filas.push({ clave: "g-servicios", grupo: "Servicios" });
            elegidos.forEach(s => filas.push({ clave: "v" + s.id, texto: s.nombre, importe: plata(s.precio) }));
        }
        filas.push({ clave: "g-cargos", grupo: "Cargos" });
        const cargo = (datos.parametros.cargoServicio * 100).toLocaleString("es-AR", { maximumFractionDigits: 1 });
        filas.push({ clave: "cargo", texto: "Cargo por servicio " + cargo + " %", importe: plata(r.cargoServicio) });
        if (eleccion.seguro) {
            const bonificado = datos.cliente.suscripcionActiva && r.seguro === 0;
            filas.push({ clave: "seguro", clase: bonificado ? "bonificado" : "", texto: "Seguro (" + cantidad + (cantidad === 1 ? " entrada)" : " entradas)"),
                         importe: bonificado ? "Sin cargo" : plata(r.seguro) });
        } else {
            filas.push({ clave: "sin-seguro", clase: "secundaria", texto: "Sin seguro", importe: "" });
        }
        return filas;
    }

    function dibujarResumen(r) {
        const lista = $("#desglose");
        const existentes = new Map([...lista.children].filter(li => !li.classList.contains("saliendo")).map(li => [li.dataset.clave, li]));
        const nuevas = lineas(r).map(f => {
            let li = existentes.get(f.clave);
            const recien = !li;
            if (recien) {
                li = document.createElement("li");
                li.dataset.clave = f.clave;
                if (f.grupo) li.textContent = f.grupo;
                else li.innerHTML = '<span class="texto"></span><span class="importe"></span>';
            }
            li.className = (f.grupo ? "grupo" : "linea" + (f.clase ? " " + f.clase : "")) + (recien || li.classList.contains("nueva") ? " nueva" : "");
            if (!f.grupo) {
                li.querySelector(".texto").textContent = f.texto;
                li.querySelector(".importe").textContent = f.importe;
            }
            if (recien) {
                // Respaldo: si la animación no corre (vista oculta, movimiento reducido), igual se limpia.
                const listo = () => li.classList.remove("nueva");
                li.addEventListener("animationend", listo, { once: true });
                setTimeout(listo, 260);
            }
            return li;
        });
        // Reconciliación en el lugar: las líneas que ya estaban no se sacan ni se reinsertan (reinsertarlas
        // reiniciaría su animación). Solo entra lo nuevo y sale lo que ya no corresponde.
        const deseadas = new Set(nuevas);
        // Lo que sale se desvanece en 160 ms (un 20 % menos que la entrada) y recién después se quita.
        [...lista.children].forEach(li => {
            if (deseadas.has(li) || li.classList.contains("saliendo")) return;
            if (sinMovimiento.matches) { li.remove(); return; }
            li.classList.remove("nueva");
            li.classList.add("saliendo");
            // Se quita al terminar de desvanecerse, o a los 220 ms si la animación no llega a correr.
            li.addEventListener("animationend", () => li.remove(), { once: true });
            setTimeout(() => li.remove(), 220);
        });
        // Ordena lo deseado sin tocar lo que ya está en su lugar; los renglones que salen se saltean.
        let referencia = lista.firstChild;
        nuevas.forEach(li => {
            while (referencia && referencia.classList && referencia.classList.contains("saliendo")) referencia = referencia.nextSibling;
            if (referencia === li) referencia = referencia.nextSibling;
            else lista.insertBefore(li, referencia);
        });
        $("#vacio").classList.toggle("oculto", nuevas.length > 0);

        animarTotal(r.total);
        $("#total").setAttribute("aria-label", "Total " + plata(r.total));
        $("#total-anuncio").textContent = "Total " + plata(r.total);   // se anuncia una vez, no cada cuadro del conteo
        const ahorros = [];
        if (r.montoDescuento > 0) ahorros.push("Ahorrás " + plata(r.montoDescuento) + " por tu nivel " + datos.cliente.nivel + ".");
        if (eleccion.seguro && datos.cliente.suscripcionActiva) ahorros.push("El seguro va sin cargo por tu suscripción.");
        $("#ahorro").textContent = ahorros.join(" ");
    }

    function nivelChip() {
        const c = datos.cliente;
        $("#nivel-chip").textContent = c.porcentaje > 0
            ? c.nivel + " · " + (c.porcentaje * 100).toLocaleString("es-AR", { maximumFractionDigits: 1 }) + " %"
            : c.nivel + " · sin descuento";
    }

    function seleccion() {
        return {
            version,
            entradas: Object.entries(eleccion.cantidades).map(([sectorId, cantidad]) => ({ sectorId: Number(sectorId), cantidad })),
            servicios: [...eleccion.servicios],
            seguro: eleccion.seguro
        };
    }

    function cambio() {
        version++;
        actualizarControles();
        dibujarResumen(calcular());
        puente.enviar(Object.assign({ tipo: "seleccion" }, seleccion()));
    }

    // ---------- Continuar ----------
    $("#continuar").addEventListener("click", () => {
        if (!eleccion.seguro) { $("#confirmar-seguro").classList.remove("oculto"); $("#agregar-seguro").focus(); return; }
        comprar();
    });
    $("#agregar-seguro").addEventListener("click", () => { $("#seguro").checked = true; eleccion.seguro = true; cambio(); $("#continuar").focus(); });
    $("#seguir-sin-seguro").addEventListener("click", comprar);
    function comprar() {
        $("#confirmar-seguro").classList.add("oculto");
        $("#continuar").disabled = true; $("#continuar").textContent = "Reservando…";
        puente.enviar(Object.assign({ tipo: "comprar" }, seleccion()));
    }
    $("#volver").addEventListener("click", () => puente.enviar({ tipo: "navegar", destino: "eventos" }));
    $("#niveles").addEventListener("click", () => puente.enviar({ tipo: "niveles" }));

    // ---------- Lo que manda la aplicación ----------
    // { evento, cliente, parametros, sectores, servicios }   carga inicial (y al recargar disponibles)
    // { resumen: { version, baseEntradas, porcentaje, montoDescuento, subtotalEntradas, servicios, cargoServicio, seguro, total } }
    // { error: { mensaje } }   rechazo del negocio, por ejemplo cupo agotado al reservar
    puente.alDibujar(d => {
        if (d.sectores) {
            datos = Object.assign({}, datos || {}, d);
            if (d.seleccion) {
                eleccion.cantidades = {};
                (d.seleccion.entradas || []).forEach(e => { if (e.cantidad > 0) eleccion.cantidades[e.sectorId] = e.cantidad; });
                eleccion.servicios = new Set(d.seleccion.servicios || []);
                eleccion.seguro = !!d.seleccion.seguro;
                $("#seguro").checked = eleccion.seguro;
            }
            // Quita lo que ya no se puede elegir (por ejemplo, si cambió la disponibilidad).
            datos.sectores.forEach(s => { if ((eleccion.cantidades[s.id] || 0) > s.disponibles) eleccion.cantidades[s.id] = s.disponibles; });
            dibujarEvento(datos.evento); nivelChip(); dibujarSectores(); dibujarServicios();
            actualizarControles(); dibujarResumen(calcular());
        } else if (d.cliente && datos) {
            Object.assign(datos.cliente, d.cliente); nivelChip(); actualizarControles(); dibujarResumen(calcular());
        }
        if (d.resumen && d.resumen.version === version) dibujarResumen(d.resumen);
        $("#continuar").textContent = "Continuar al pago";
        if (datos) $("#continuar").disabled = totalEntradas() === 0;
        $("#error").classList.toggle("oculto", !d.error);
        if (d.error) $("#error").textContent = d.error.mensaje;
    });

    // ---------- Solo en la maqueta: la aplicación simulada, con los datos de la semilla ----------
    const semilla = {
        evento: { nombre: "Duki en Rosario", categoria: "Concierto", lugar: "Galpón 17", direccion: "Estévez Boero 600, Rosario",
                  fecha: "Sábado 23 de enero · 21:00", colorIndice: 1 },
        cliente: { nombre: "Paula Plata", nivel: "Plata", porcentaje: 0.05, suscripcionActiva: false },
        parametros: { cargoServicio: 0.15, seguroPrecio: 5000, maxEntradas: 6, bonificacionSeguro: 1 },
        sectores: [{ id: 1, nombre: "Campo", precio: 25000, disponibles: 2000, capacidad: 2000 },
                   { id: 2, nombre: "Platea", precio: 40000, disponibles: 800, capacidad: 800 },
                   { id: 3, nombre: "VIP", precio: 60000, disponibles: 100, capacidad: 100 }],
        servicios: [{ id: 1, nombre: "Hotel Ríos 1 noche", tipo: "Hospedaje", precio: 45000 },
                    { id: 2, nombre: "Traslado ida y vuelta", tipo: "Traslado", precio: 12000 },
                    { id: 3, nombre: "Remera oficial", tipo: "Merchandising", precio: 18000 },
                    { id: 4, nombre: "Meet & Greet", tipo: "VIP", precio: 80000 }]
    };
    const copia = o => JSON.parse(JSON.stringify(o));
    puente.simular(m => {
        if (m.tipo === "seleccion") puente.responder({ resumen: Object.assign({ version: m.version }, calcular()) });
        else if (m.tipo === "comprar") puente.anotarFuera("La aplicación reservaría el cupo (RN-15, 10 minutos) y abriría el pago.");
        else if (m.tipo === "navegar") puente.anotarFuera("La aplicación volvería a «Eventos disponibles».");
        else if (m.tipo === "niveles") puente.anotarFuera("La aplicación mostraría cómo funcionan los niveles.");
        else if (m.tipo === "listaEspera") puente.anotarFuera("La aplicación anotaría al cliente en la lista de espera (RN-35).");
    }, {
        "Bronce": { cliente: { nivel: "Bronce", porcentaje: 0, nombre: "Bruno Bronce" } },
        "Plata": { cliente: { nivel: "Plata", porcentaje: 0.05, nombre: "Paula Plata" } },
        "Oro": { cliente: { nivel: "Oro", porcentaje: 0.075, nombre: "Oscar Oro" } },
        "Con suscripción": () => ({ cliente: { suscripcionActiva: !datos.cliente.suscripcionActiva } }),
        "VIP agotado": () => { const d = copia(semilla); d.sectores[2].disponibles = 0; return d; },
        "Cupo agotado al reservar": { error: { mensaje: "En el sector Campo quedan 1 entradas." } }
    }, copia(semilla));

    // Solo en la maqueta: #guion arma el paquete del guion (2 Campo, hospedaje, traslado y seguro: $ 121.625,00).
    if (!puente.enAplicacion && location.hash === "#guion") {
        eleccion.cantidades[1] = 2; eleccion.servicios.add(1); eleccion.servicios.add(2); eleccion.seguro = true;
        $("#seguro").checked = true; dibujarServicios(); cambio();
    }
})();
