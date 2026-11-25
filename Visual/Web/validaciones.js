"use strict";
// Validaciones de FORMATO para el acceso y el alta. Replican, una por una, las de la capa de negocio:
//   Repo/GestorSolicitudesOrganizador.cs  CuitValido, CbuValido, CbuAliasValido, NormalizarTelefonoArgentino,
//                                         CabeceraImagenValida y el límite de 512 KB (RN-51, RN-52)
//   Repo/GestorUsuarios.cs                ValidarContrasena (RN-02) y PuntajeFortaleza (RN-47)
// La página las usa solo para avisar en vivo. La decisión final la toma siempre el negocio, que vuelve a
// validar todo, y ninguna de estas funciones comprueba identidad: un CUIT con dígito verificador correcto
// puede no existir o no pertenecer a quien lo escribe (RN-52). Eso lo revisa el Administrador (RN-53).
(function () {
    const digitos = valor => Array.from(String(valor ?? "")).filter(c => /\p{Nd}/u.test(c)).join("");

    function cuitValido(valor) {
        const d = digitos(valor);
        if (d.length !== 11) return false;
        const pesos = [5, 4, 3, 2, 7, 6, 5, 4, 3, 2];
        let suma = 0;
        for (let i = 0; i < 10; i++) suma += Number(d[i]) * pesos[i];
        let verificador = 11 - suma % 11;
        if (verificador === 11) verificador = 0;
        if (verificador === 10) {
            if (d.slice(0, 2) !== "23") return false;
            const pesos23 = [4, 3, 2, 7, 6, 5, 4, 3, 2, 0];
            suma = 0;
            for (let i = 0; i < 10; i++) suma += Number(d[i]) * pesos23[i];
            verificador = 11 - suma % 11;
            if (verificador === 11) verificador = 0;
        }
        return verificador === Number(d[10]);
    }

    function digitoCbu(tramo, pesos) {
        let suma = 0;
        for (let i = 0; i < pesos.length; i++) suma += Number(tramo[i]) * pesos[i];
        return (10 - suma % 10) % 10;
    }

    function cbuValido(valor) {
        const d = digitos(valor);
        if (d.length !== 22) return false;
        if (digitoCbu(d.slice(0, 7), [7, 1, 3, 9, 7, 1, 3]) !== Number(d[7])) return false;
        return digitoCbu(d.slice(8, 21), [3, 9, 7, 1, 3, 9, 7, 1, 3, 9, 7, 1, 3]) === Number(d[21]);
    }

    // El negocio normaliza antes de validar: recorta y pasa a minúsculas.
    function normalizarCbuAlias(valor) { return String(valor ?? "").trim().toLowerCase(); }

    // Devuelve "cbu", "alias" o null.
    function tipoCbuAlias(valor) {
        const normal = normalizarCbuAlias(valor);
        const d = digitos(normal);
        if (d.length === 22 && d.length === normal.length) return cbuValido(d) ? "cbu" : null;
        const caracteres = Array.from(normal);
        const aliasOk = caracteres.length >= 6 && caracteres.length <= 20 &&
            caracteres.every(c => /\p{Ll}/u.test(c) || /\p{Nd}/u.test(c) || c === "." || c === "-");
        return aliasOk ? "alias" : null;
    }

    // Devuelve el teléfono normalizado (+549 y 10 dígitos) o null si no es un teléfono argentino válido.
    function normalizarTelefono(valor) {
        let d = digitos(valor);
        if (d.startsWith("549")) d = d.slice(3);
        else if (d.startsWith("54")) d = d.slice(2);
        if (d.startsWith("0")) d = d.slice(1);
        if (d.length >= 11 && d[0] === "9") d = d.slice(1);
        if (d.length !== 10 || d[0] < "2" || d[0] > "4" || /^0+$/.test(d)) return null;
        return "+549" + d;
    }

    // RN-47: un punto por cada condición; el máximo es 5.
    function puntajeFortaleza(pwd, longitudMinima) {
        if (!pwd) return 0;
        let puntaje = Array.from(pwd).length >= longitudMinima ? 1 : 0;
        if (/\p{Ll}/u.test(pwd)) puntaje++;
        if (/\p{Lu}/u.test(pwd)) puntaje++;
        if (/\p{Nd}/u.test(pwd)) puntaje++;
        if (/[^\p{L}\p{Nd}]/u.test(pwd)) puntaje++;
        return puntaje;
    }

    // RN-02: longitud mínima, al menos una letra y un dígito.
    function contrasenaCumpleMinimo(pwd, longitudMinima) {
        return !!pwd && Array.from(pwd).length >= longitudMinima && /\p{L}/u.test(pwd) && /\p{Nd}/u.test(pwd);
    }

    function nivelFortaleza(puntaje, minimo) {
        return puntaje < minimo ? "Insuficiente" : puntaje < 5 ? "Aceptable" : "Fuerte";
    }

    // Solo una ayuda de escritura: el negocio no rechaza por formato de correo, sino al enviar el código.
    function mailConFormato(valor) { return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(String(valor ?? "").trim()); }

    // RN-52: PNG o JPEG por cabecera, no por extensión. El original puede pesar hasta 10 MB
    // (ImagenEventoCodec.MaximoArchivoBytes): la aplicación lo reduce a 1280 x 720 y lo recomprime hasta
    // 512 KB (MaximoGuardadoBytes) antes de que el negocio valide los 512 KB. Por eso acá un original de más
    // de 512 KB no es un error: se avisa que se va a comprimir.
    function revisarImagen(bytes, maxOriginal, maxGuardado) {
        const tamano = bytes.length;
        const png = tamano >= 8 && [137, 80, 78, 71, 13, 10, 26, 10].every((b, i) => bytes[i] === b);
        const jpeg = tamano >= 3 && bytes[0] === 255 && bytes[1] === 216 && bytes[2] === 255;
        if (!png && !jpeg) return { valida: false, motivo: "El archivo no es una imagen PNG o JPEG." };
        if (tamano > maxOriginal) return { valida: false, motivo: "La imagen supera el límite de " + Math.round(maxOriginal / 1048576) + " MB. Elegí una imagen más liviana." };
        return { valida: true, tipo: png ? "PNG" : "JPEG", comprimir: tamano > maxGuardado };
    }

    window.EventAuraValidaciones = {
        digitos, cuitValido, cbuValido, normalizarCbuAlias, tipoCbuAlias, normalizarTelefono,
        puntajeFortaleza, contrasenaCumpleMinimo, nivelFortaleza, mailConFormato, revisarImagen
    };
})();
