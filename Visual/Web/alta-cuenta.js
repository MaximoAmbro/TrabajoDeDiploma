"use strict";
// Alta de cuenta con la decisión al empezar: comprar entradas u organizar eventos.
// Las validaciones en vivo son de FORMATO (validaciones.js). La creación de la cuenta, los códigos y la
// solicitud los resuelve la aplicación; la habilitación como Organizador la decide el Administrador (RN-53).
(function () {
    const puente = window.EventAuraPuente;
    const V = window.EventAuraValidaciones;
    const $ = s => document.querySelector(s);

    const estado = {
        perfil: "cliente",
        paso: "elegir",
        parametros: { longitudMinima: 8, fortalezaMinima: 4, maxOriginalBytes: 10 * 1024 * 1024, maxGuardadoBytes: 512 * 1024 },
        documento: null,              // { nombreArchivo, base64, valida }
        telefono: null,               // normalizado
        telefonoVerificado: false
    };

    // ---------- Pasos ----------
    const orden = ["elegir", "datos", "correo", "telefono", "listo"];
    function mostrarPaso(paso) {
        estado.paso = paso;
        document.querySelectorAll("[data-vista]").forEach(v => v.classList.toggle("oculto", v.dataset.vista !== paso));
        const actual = orden.indexOf(paso);
        document.querySelectorAll("#pasos li").forEach(li => {
            const i = orden.indexOf(li.dataset.paso);
            li.classList.toggle("hecho", i < actual || paso === "listo");
            li.classList.toggle("actual", i === actual && paso !== "listo");
            if (i === actual) li.setAttribute("aria-current", "step"); else li.removeAttribute("aria-current");
        });
        const vista = document.querySelector('[data-vista="' + paso + '"]');
        const foco = vista.querySelector("input:not([type=radio]):not([type=checkbox]), h1");
        if (foco) foco.focus({ preventScroll: true });
    }

    function elegirPerfil(perfil) {
        estado.perfil = perfil;
        const org = perfil === "organizador";
        document.body.classList.toggle("organizador", org);
        $("#perfil-elegido").textContent = org ? "Cuenta para organizar eventos" : "Cuenta para comprar entradas";
        $("#crear").textContent = org ? "Crear cuenta y enviar solicitud" : "Crear cuenta";
        $("#crear").dataset.rotulo = $("#crear").textContent;
        document.querySelector("[data-numero-final]").textContent = org ? "5" : "4";
    }
    document.querySelectorAll('input[name="perfil"]').forEach(r => r.addEventListener("change", () => elegirPerfil(r.value)));
    $("#continuar").addEventListener("click", () => mostrarPaso("datos"));
    $("#volver-elegir").addEventListener("click", () => mostrarPaso("elegir"));

    // ---------- Marcas de estado ----------
    function marcar(id, tipo, texto) {
        const campo = document.getElementById(id);
        const linea = document.getElementById("estado-" + id);
        if (campo && campo.tagName === "INPUT") campo.setAttribute("aria-invalid", tipo === "mal" ? "true" : "false");
        if (linea) { linea.className = "estado" + (tipo ? " " + tipo : ""); linea.textContent = texto || ""; }
        return tipo !== "mal" && tipo !== "falta";
    }
    const texto = id => document.getElementById(id).value;
    const vacio = id => !texto(id).trim();

    // Cada validador devuelve true si el campo está en condiciones de enviarse.
    // «final» = se está enviando: los campos vacíos pasan a error.
    const validadores = {
        nombre: final => vacio("nombre") ? (final ? marcar("nombre", "mal", "Escribí tu nombre.") : marcar("nombre", "", "") && false) : marcar("nombre", "", ""),
        apellido: final => vacio("apellido") ? (final ? marcar("apellido", "mal", "Escribí tu apellido.") : marcar("apellido", "", "") && false) : marcar("apellido", "", ""),
        mail: final => {
            if (vacio("mail")) return final ? marcar("mail", "mal", "Escribí tu correo electrónico.") : marcar("mail", "", "") && false;
            if (!V.mailConFormato(texto("mail"))) return marcar("mail", "info", "Revisá el formato: nombre@dominio.");
            return marcar("mail", "", "");
        },
        contrasena: final => {
            const pwd = texto("contrasena"), p = estado.parametros;
            const puntaje = V.puntajeFortaleza(pwd, p.longitudMinima);
            const medidor = $("#fortaleza");
            medidor.dataset.nivel = puntaje < p.fortalezaMinima ? "bajo" : "ok";
            medidor.querySelectorAll(".barras span").forEach((b, i) => b.classList.toggle("lleno", i < puntaje));
            let linea = "Fortaleza: " + V.nivelFortaleza(puntaje, p.fortalezaMinima) + " (" + puntaje + "/5; mínimo " + p.fortalezaMinima + ")";
            const minimo = V.contrasenaCumpleMinimo(pwd, p.longitudMinima);
            if (pwd && !minimo) linea += ". Necesita al menos " + p.longitudMinima + " caracteres, con una letra y un número.";
            $("#fortaleza-texto").textContent = linea;
            const ok = minimo && puntaje >= p.fortalezaMinima;
            document.getElementById("contrasena").setAttribute("aria-invalid", final && !ok ? "true" : "false");
            if (final && !pwd) $("#fortaleza-texto").textContent = "Elegí una contraseña.";
            return ok;
        },
        confirmar: final => {
            if (vacio("confirmar")) return final ? marcar("confirmar", "mal", "Repetí tu contraseña.") : marcar("confirmar", "", "") && false;
            if (texto("confirmar") !== texto("contrasena")) return marcar("confirmar", "mal", "Las contraseñas no coinciden. Revisá lo que escribiste.");
            return marcar("confirmar", "ok", "Coinciden.");
        },
        responsable: final => vacio("responsable") ? (final ? marcar("responsable", "mal", "Escribí el nombre completo del responsable.") : marcar("responsable", "", "") && false) : marcar("responsable", "", ""),
        cuit: final => {
            const d = V.digitos(texto("cuit"));
            if (!d.length) return final ? marcar("cuit", "mal", "Escribí el CUIT del responsable.") : marcar("cuit", "", "") && false;
            if (d.length < 11 && !final) return marcar("cuit", "info", "Faltan " + (11 - d.length) + " dígitos.") && false;
            if (!V.cuitValido(d)) return marcar("cuit", "mal", "El CUIT no es válido. Revisá sus 11 dígitos y el dígito verificador.");
            return marcar("cuit", "ok", "Formato válido: el dígito verificador es correcto (" + d.slice(0, 2) + "-" + d.slice(2, 10) + "-" + d[10] + "). No confirma a quién pertenece.");
        },
        telefono: final => {
            if (vacio("telefono")) return final ? marcar("telefono", "mal", "Escribí un teléfono para recibir el código.") : marcar("telefono", "", "") && false;
            const normal = V.normalizarTelefono(texto("telefono"));
            estado.telefono = normal;
            if (!normal) return final || V.digitos(texto("telefono")).length >= 10
                ? marcar("telefono", "mal", "Ingresá un teléfono argentino válido, con característica y número.")
                : marcar("telefono", "info", "Con característica, por ejemplo 341 555-1234.") && false;
            return marcar("telefono", "ok", "Formato válido: se guarda como " + normal + ". Lo confirmás con un código después de crear la cuenta.");
        },
        local: final => vacio("local") ? (final ? marcar("local", "mal", "Escribí el nombre del local.") : marcar("local", "", "") && false) : marcar("local", "", ""),
        direccion: final => vacio("direccion") ? (final ? marcar("direccion", "mal", "Escribí la dirección del local.") : marcar("direccion", "", "") && false) : marcar("direccion", "", ""),
        capacidad: () => {
            const n = Number(texto("capacidad"));
            return Number.isInteger(n) && n > 0 ? marcar("capacidad", "", "") : marcar("capacidad", "mal", "La capacidad del local debe ser mayor que cero.");
        },
        cbu: final => {
            const valor = texto("cbu");
            if (!valor.trim()) return final ? marcar("cbu", "mal", "Escribí un CBU o un alias para cobrar tus ventas.") : marcar("cbu", "", "") && false;
            const tipo = V.tipoCbuAlias(valor);
            if (tipo === "cbu") return marcar("cbu", "ok", "Formato válido: CBU con sus dos dígitos verificadores correctos. No confirma que la cuenta exista ni que sea tuya.");
            if (tipo === "alias") return marcar("cbu", "ok", "Formato válido de alias (" + V.normalizarCbuAlias(valor) + "). No confirma que el alias exista.");
            const d = V.digitos(valor);
            if (!final && d.length === valor.trim().length && d.length < 22) return marcar("cbu", "info", "CBU: faltan " + (22 - d.length) + " dígitos.") && false;
            return marcar("cbu", "mal", "Ingresá un CBU válido de 22 dígitos o un alias bancario de 6 a 20 caracteres.");
        },
        documento: final => {
            if (!estado.documento) return final ? marcar("documento", "mal", "Adjuntá una imagen PNG o JPEG de la habilitación o constancia.") : false;
            return estado.documento.valida;
        }
    };
    const camposCuenta = ["nombre", "apellido", "mail", "contrasena", "confirmar"];
    const camposSolicitud = ["responsable", "cuit", "telefono", "local", "capacidad", "direccion", "cbu", "documento"];

    camposCuenta.concat(camposSolicitud).filter(id => id !== "documento").forEach(id => {
        const campo = document.getElementById(id);
        campo.addEventListener("input", () => { validadores[id](false); if (id === "contrasena" && !vacio("confirmar")) validadores.confirmar(false); });
        campo.addEventListener("blur", () => { if (!vacio(id)) validadores[id](false); });
    });

    // ---------- Documentación ----------
    const zona = $("#zona-documento"), entrada = $("#documento"), vista = $("#vista-documento");
    function leerArchivo(archivo) {
        if (!archivo) return;
        const lector = new FileReader();
        lector.onload = () => {
            const bytes = new Uint8Array(lector.result);
            const r = V.revisarImagen(bytes, estado.parametros.maxOriginalBytes, estado.parametros.maxGuardadoBytes);
            let binario = ""; bytes.forEach(b => { binario += String.fromCharCode(b); });
            const base64 = btoa(binario);
            estado.documento = { nombreArchivo: archivo.name, base64, valida: r.valida };
            $("#documento-nombre").textContent = archivo.name;
            $("#documento-detalle").textContent = Math.max(1, Math.round(bytes.length / 1024)) + " KB";
            if (r.valida) {
                vista.src = "data:image/" + (r.tipo === "PNG" ? "png" : "jpeg") + ";base64," + base64;
                vista.classList.remove("oculto");
                zona.classList.add("con-imagen");
                marcar("documento", "ok", "Formato válido: imagen " + r.tipo + " que se puede leer." +
                    (r.comprimir ? " Pesa más de 512 KB: la aplicación la reduce al enviarla." : "") +
                    " No confirma que el documento sea auténtico.");
            } else {
                vista.classList.add("oculto");
                zona.classList.remove("con-imagen");
                marcar("documento", "mal", r.motivo + " Adjuntá una imagen PNG o JPEG.");
            }
        };
        lector.readAsArrayBuffer(archivo);
    }
    entrada.addEventListener("change", () => leerArchivo(entrada.files[0]));
    ["dragenter", "dragover"].forEach(t => zona.addEventListener(t, e => { e.preventDefault(); zona.classList.add("arrastrando"); }));
    ["dragleave", "drop"].forEach(t => zona.addEventListener(t, e => { e.preventDefault(); zona.classList.remove("arrastrando"); }));
    zona.addEventListener("drop", e => leerArchivo(e.dataTransfer.files[0]));

    // ---------- Envío del formulario ----------
    const errorDatos = $("#error-datos");
    $("#formulario").addEventListener("submit", e => {
        e.preventDefault();
        const ids = estado.perfil === "organizador" ? camposCuenta.concat(camposSolicitud) : camposCuenta;
        const malos = ids.filter(id => !validadores[id](true));
        if (malos.length) {
            errorDatos.textContent = malos.length === 1 ? "Revisá el campo marcado." : "Revisá los " + malos.length + " campos marcados.";
            errorDatos.classList.remove("oculto");
            const primero = document.getElementById(malos[0] === "documento" ? "documento" : malos[0]);
            (malos[0] === "documento" ? zona : primero).scrollIntoView({ block: "center" });
            primero.focus();
            return;
        }
        errorDatos.classList.add("oculto");
        ocupado($("#crear"), true, "Creando cuenta…");
        puente.enviar({
            tipo: "registrar",
            perfil: estado.perfil,
            cuenta: { nombre: texto("nombre").trim(), apellido: texto("apellido").trim(), mail: texto("mail").trim(), contrasena: texto("contrasena") }
        });
    });

    function ocupado(boton, si, rotulo) {
        if (!boton.dataset.rotulo) boton.dataset.rotulo = boton.textContent;
        boton.disabled = si;
        boton.textContent = si ? rotulo : boton.dataset.rotulo;
    }

    // ---------- Verificaciones ----------
    let temporizadorReenvio = null;
    const grupoIntroCorreo = document.querySelector('[data-vista="correo"] .pila-2');
    const ayudaSesionCorreo = $("#ayuda-correo-sesion");
    const ayudaCorreoOriginal = grupoIntroCorreo && grupoIntroCorreo.querySelector("p");
    if (grupoIntroCorreo && ayudaSesionCorreo) grupoIntroCorreo.appendChild(ayudaSesionCorreo);
    function iniciarEsperaReenvio(segundos) {
        const fila = $("#reenviar-correo-fila"), boton = $("#reenviar-correo"), ayuda = $("#reenviar-correo-ayuda");
        if (!fila || !boton) return;
        fila.classList.remove("oculto");
        if (temporizadorReenvio) clearInterval(temporizadorReenvio);
        let restante = Math.max(0, Number(segundos) || 0);
        const pintar = () => {
            boton.disabled = restante > 0;
            ayuda.textContent = restante > 0 ? "Podés pedir otro código en " + restante + " s." : "¿No llegó?";
            restante--;
            if (restante < 0 && temporizadorReenvio) { clearInterval(temporizadorReenvio); temporizadorReenvio = null; }
        };
        pintar();
        if (restante >= 0) temporizadorReenvio = setInterval(pintar, 1000);
    }
    $("#reenviar-correo").addEventListener("click", () => {
        ocupado($("#reenviar-correo"), true, "Enviando…");
        puente.enviar({ tipo: "reenviarCorreo" });
    });
    $("#salir-verificacion").addEventListener("click", e => {
        if (e.currentTarget.dataset.volverCuenta === "true") {
            e.preventDefault();
            puente.enviar({ tipo: "cancelarVerificacion" });
        }
    });
    $("#confirmar-correo").addEventListener("click", () => {
        if (vacio("codigo-correo")) { marcar("codigo-correo", "mal", "Escribí el código que recibiste."); return; }
        ocupado($("#confirmar-correo"), true, "Confirmando…");
        puente.enviar({ tipo: "confirmarCorreo", codigo: texto("codigo-correo").trim() });
    });
    $("#enviar-codigo-telefono").addEventListener("click", () => {
        ocupado($("#enviar-codigo-telefono"), true, "Enviando…");
        puente.enviar({ tipo: "enviarCodigoTelefono", telefono: estado.telefono });
    });
    $("#confirmar-telefono").addEventListener("click", () => {
        if (vacio("codigo-telefono")) { marcar("codigo-telefono", "mal", "Escribí el código que recibiste."); return; }
        ocupado($("#confirmar-telefono"), true, "Confirmando…");
        puente.enviar({ tipo: "confirmarTelefono", telefono: estado.telefono, codigo: texto("codigo-telefono").trim() });
    });
    $("#presentar").addEventListener("click", () => {
        ocupado($("#presentar"), true, "Enviando solicitud…");
        puente.enviar({
            tipo: "presentarSolicitud",
            solicitud: {
                responsable: texto("responsable").trim(), cuit: V.digitos(texto("cuit")), telefono: estado.telefono,
                nombreLocal: texto("local").trim(), direccion: texto("direccion").trim(), capacidad: Number(texto("capacidad")),
                cbuAlias: V.normalizarCbuAlias(texto("cbu")),
                documentacion: { nombreArchivo: estado.documento.nombreArchivo, base64: estado.documento.base64 }
            }
        });
    });
    document.querySelectorAll("[data-destino]").forEach(b =>
        b.addEventListener("click", () => puente.enviar({ tipo: "navegar", destino: b.dataset.destino })));

    // ---------- Lo que manda la aplicación ----------
    // { paso, perfil?, parametros?, error?: { campo, mensaje }, envioTelefono?: string,
    //   telefonoVerificado?: bool, resultado?: { perfil } }   (ver README.md)
    puente.alDibujar(datos => {
        document.body.classList.toggle("verificacion-sesion", !!datos.verificacionSesion);
        ["crear", "confirmar-correo", "reenviar-correo", "enviar-codigo-telefono", "confirmar-telefono", "presentar"].forEach(id => ocupado(document.getElementById(id), false));
        if (datos.parametros) {
            Object.assign(estado.parametros, datos.parametros);
            validadores.contrasena(false);
        }
        if (datos.perfil) {
            elegirPerfil(datos.perfil);
            document.querySelector('input[name="perfil"][value="' + datos.perfil + '"]').checked = true;
        }
        if (datos.mail) $("#mail").value = datos.mail;
        if (datos.paso && datos.paso !== estado.paso) mostrarPaso(datos.paso);
        if (datos.paso === "correo") {
            $("#contrasena").value = "";
            $("#confirmar").value = "";
            validadores.contrasena(false);
            const ayudaNormal = ayudaCorreoOriginal;
            const ayudaSesion = $("#ayuda-correo-sesion");
            if (datos.verificacionSesion) {
                ayudaSesion.textContent = datos.ayudaBandeja || "Revisá tu correo.";
                ayudaSesion.classList.remove("oculto");
                if (ayudaNormal) ayudaNormal.classList.add("oculto");
                $("#salir-verificacion").textContent = "Volver a eventos";
                $("#salir-verificacion").removeAttribute("data-destino");
                $("#salir-verificacion").dataset.volverCuenta = "true";
            } else {
                ayudaSesion.classList.add("oculto");
                if (ayudaNormal) ayudaNormal.classList.remove("oculto");
                $("#salir-verificacion").textContent = "Volver al acceso";
                $("#salir-verificacion").dataset.destino = "acceso";
                delete $("#salir-verificacion").dataset.volverCuenta;
            }
            if (datos.reenviarRestante != null) iniciarEsperaReenvio(datos.reenviarRestante);
            else $("#reenviar-correo-fila").classList.add("oculto");
        }
        if (datos.paso === "telefono") {
            $("#telefono-normalizado").textContent = estado.telefono || "";
            if (datos.envioTelefono) marcar("envio-telefono", "info", datos.envioTelefono);
            if (datos.telefonoVerificado) {
                estado.telefonoVerificado = true;
                marcar("codigo-telefono", "ok", "Teléfono verificado.");
                $("#codigo-telefono").disabled = $("#confirmar-telefono").disabled = $("#enviar-codigo-telefono").disabled = true;
            }
            $("#presentar").disabled = !estado.telefonoVerificado;
        }
        if (datos.paso === "listo") dibujarResumen(datos.resultado || { perfil: estado.perfil });
        if (datos.error) {
            const destino = { datos: "error-datos", telefono: "error-telefono" }[datos.paso];
            if (datos.error.campo && document.getElementById("estado-" + datos.error.campo)) {
                marcar(datos.error.campo, "mal", datos.error.mensaje);
                document.getElementById(datos.error.campo).focus();
            } else if (datos.paso === "correo") {
                marcar("codigo-correo", "mal", datos.error.mensaje);
            } else if (destino) {
                $("#" + destino).textContent = datos.error.mensaje;
                $("#" + destino).classList.remove("oculto");
            }
        }
    });

    function dibujarResumen(resultado) {
        const org = resultado.perfil === "organizador";
        $("#titulo-listo").textContent = org ? "Tu cuenta está creada y tu solicitud, enviada" : "Tu cuenta está lista";
        $("#bajada-listo").textContent = org
            ? "Ya podés iniciar sesión y comprar entradas. Para organizar eventos falta la revisión de tu solicitud."
            : "Ya podés iniciar sesión y comprar entradas.";
        const filas = resultado.verificacionSesion
            ? [["hecho", "Correo verificado", texto("mail").trim()]]
            : [["hecho", "Cuenta creada", "Perfil: Cliente."], ["hecho", "Correo verificado", texto("mail").trim()]];
        if (org) {
            filas.push(["hecho", "Teléfono verificado", estado.telefono || ""]);
            filas.push(["pendiente", "Solicitud para organizar eventos", "Local: " + texto("local").trim() + ". La revisa una persona administradora.", "Pendiente de revisión"]);
        }
        $("#resumen").replaceChildren(...filas.map(([clase, titulo, detalle, etiqueta]) => {
            const li = document.createElement("li");
            li.className = clase;
            const cuerpo = document.createElement("div");
            const t = document.createElement("strong"); t.textContent = titulo;
            const d = document.createElement("span"); d.textContent = detalle;
            cuerpo.append(t, d);
            li.append(cuerpo);
            if (etiqueta) { const e = document.createElement("span"); e.className = "etiqueta"; e.textContent = etiqueta; li.append(e); }
            return li;
        }));
    }

    // ---------- Solo en la maqueta: la aplicación simulada ----------
    let codigoCorreo = null, codigoTelefono = null;
    const nuevoCodigo = () => String(Math.floor(100000 + Math.random() * 900000));
    function completarDemostracion() {
        const valores = { nombre: "Lucía", apellido: "Demo", mail: "lucia.demo@demo.eventaura.example", contrasena: "Rosario#Demo2026",
            confirmar: "Rosario#Demo2026", responsable: "Persona Ficticia de Demostración", cuit: "30-00000000-7", telefono: "341 000-0000",
            local: "Local Ficticio de Demostración", capacidad: "500", direccion: "Dirección inventada 000, Rosario", cbu: "0000000000000000000000" };
        Object.entries(valores).forEach(([id, v]) => { const c = document.getElementById(id); c.value = v; c.dispatchEvent(new Event("input")); });
        // Una imagen PNG mínima, para no depender de un archivo en la máquina.
        const png = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
        const bytes = Uint8Array.from(atob(png), c => c.charCodeAt(0));
        leerArchivo(new File([bytes], "constancia-de-ejemplo.png", { type: "image/png" }));
    }
    puente.simular(m => {
        switch (m.tipo) {
            case "registrar":
                codigoCorreo = nuevoCodigo();
                puente.anotarFuera("Correo simulado para " + m.cuenta.mail + " · Código: " + codigoCorreo + " (vence en 10 minutos)");
                puente.responder({ paso: "correo" });
                break;
            case "confirmarCorreo":
                if (m.codigo !== codigoCorreo) { puente.responder({ paso: "correo", error: { campo: "codigo-correo", mensaje: "El código no es válido o venció. Solicitá uno nuevo." } }); break; }
                if (estado.perfil === "organizador") {
                    puente.anotarFuera("La aplicación inicia la sesión del Cliente recién creado para poder pedir el código del teléfono (ver README, «Secuencia del organizador»).");
                    puente.responder({ paso: "telefono" });
                } else puente.responder({ paso: "listo", resultado: { perfil: "cliente" } });
                break;
            case "enviarCodigoTelefono":
                codigoTelefono = nuevoCodigo();
                puente.anotarFuera("SMS simulado para " + m.telefono + " · Código: " + codigoTelefono);
                puente.responder({ paso: "telefono", envioTelefono: "Si el teléfono es válido, guardamos un SMS simulado fuera de EventAura. Abrí el archivo del mensaje para continuar." });
                break;
            case "confirmarTelefono":
                puente.responder(m.codigo === codigoTelefono
                    ? { paso: "telefono", telefonoVerificado: true }
                    : { paso: "telefono", error: { campo: "codigo-telefono", mensaje: "El código no es válido o venció. Solicitá uno nuevo." } });
                break;
            case "presentarSolicitud":
                puente.anotarFuera("La solicitud queda Pendiente. La cuenta sigue siendo Cliente hasta que un Administrador la apruebe.");
                puente.responder({ paso: "listo", resultado: { perfil: "organizador" } });
                break;
            case "navegar":
                if (m.destino === "acceso") setTimeout(() => { window.location.href = "acceso.html"; }, 700);
                break;
        }
    }, {
        "Completar con datos de demostración": () => { elegirPerfil("organizador"); completarDemostracion(); return { paso: "datos", perfil: "organizador" }; },
        "Error del negocio en el CUIT": { paso: "datos", error: { campo: "cuit", mensaje: "El CUIT no es válido. Revisá sus 11 dígitos y el dígito verificador." } },
        "Error general al crear": { paso: "datos", error: { mensaje: "No pudimos completar el alta o enviar el código. Revisá la conexión y volvé a intentarlo." } },
        "Parámetros: mínimo 5": { parametros: { fortalezaMinima: 5 } }
    }, { paso: "elegir", parametros: { longitudMinima: 8, fortalezaMinima: 4 } });

    elegirPerfil("cliente");
    mostrarPaso("elegir");
    validadores.contrasena(false);

    // Solo en la maqueta: anclas para revisar un estado sin hacer clics.
    //   #datos-organizador  formulario del organizador vacío
    //   #demo-organizador   formulario del organizador completo y válido
    //   #listo-organizador  pantalla final del organizador
    if (!puente.enAplicacion && location.hash) {
        const ancla = location.hash.slice(1);
        if (ancla.endsWith("-organizador")) {
            elegirPerfil("organizador");
            document.querySelector('input[name="perfil"][value="organizador"]').checked = true;
        }
        if (ancla === "datos-organizador") mostrarPaso("datos");
        if (ancla === "demo-organizador") { completarDemostracion(); mostrarPaso("datos"); }
        if (ancla === "listo-organizador") { completarDemostracion(); estado.telefono = "+5493410000000"; mostrarPaso("listo"); dibujarResumen({ perfil: "organizador" }); }
    }
})();
