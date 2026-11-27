"use strict";
const $ = selector => document.querySelector(selector);
const enviar = mensaje => window.chrome.webview.postMessage(mensaje);
const formulario = $("#form-pago");
$("#vencimiento").addEventListener("input", e => {
  const el = e.currentTarget; const cursor = el.selectionStart; const anterior = el.value;
  const digitos = anterior.replace(/\D/g, "").slice(0, 4);
  const valor = digitos.length > 2 ? digitos.slice(0, 2) + "/" + digitos.slice(2) : digitos;
  if (valor !== anterior) { el.value = valor; el.setSelectionRange(Math.min(valor.length, cursor + (valor.length > anterior.length ? 1 : 0)), Math.min(valor.length, cursor + (valor.length > anterior.length ? 1 : 0))); }
});
formulario.addEventListener("submit", e => {
  e.preventDefault();
  enviar({ tipo: "pagar", titular: $("#titular").value, numero: $("#numero").value,
    vencimiento: $("#vencimiento").value, cvv: $("#cvv").value });
});
$("#cancelar").addEventListener("click", () => enviar({ tipo: "cancelar" }));
window.eventAuraDibujar = m => {
  if (!m) return;
  document.body.classList.toggle("en-shell", !!m.enShell);
  $("#total").textContent = m.total || "Total a pagar";
  $("#temporizador").textContent = m.temporizador || "Reserva: 10:00";
  $("#temporizador").style.color = m.colorTemporizador || "var(--texto-secundario)";
  $("#estado").textContent = m.estado || "";
  $("#ayuda-tarjeta").textContent = m.ayuda || "";
  $("#prototipo").textContent = m.prototipo || "Prototipo: usá estos datos de prueba";
  $("#pagar").disabled = m.pagarHabilitado === false;
  if (!document.activeElement || document.activeElement === document.body) $("#titular").focus({ preventScroll: true });
};
