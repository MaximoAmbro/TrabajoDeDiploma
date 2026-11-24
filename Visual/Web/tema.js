"use strict";
window.eventAuraDibujar = () => {};
window.eventAuraActualizar = ({ tema, datos }) => {
    Object.entries(tema).forEach(([nombre, valor]) => document.documentElement.style.setProperty("--" + nombre, valor));
    window.eventAuraDibujar(datos);
    return true;
};
window.addEventListener("keydown", e => {
    if (e.key !== "Escape") return;
    e.preventDefault();
    window.chrome.webview.postMessage({ tipo: "escape" });
});