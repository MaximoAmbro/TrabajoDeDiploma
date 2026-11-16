# Ajustes sobre los bloques copiados: quitan notas internas y aplican D-6 (c) y los números de hoy.
R = [
 # A-11: la condición ya se cumple (RN-35 se reescribe en esta misma lista).
 (" (solo si se aplica el ítem 6 de CORRECCIONES, RN-35)", ""),
 # B-09 y B-10 con D-6 (c).
 ("Si un evento está agotado, el cliente puede inscribirse en su lista de espera.",
  "Si un evento está agotado, el cliente puede inscribirse en su lista de espera y consultar su\n> posición; si vuelve a haber entradas, el sistema se lo avisa al iniciar sesión."),
 ("> - la lista de espera para los eventos agotados;",
  "> - la lista de espera para los eventos agotados, con la consulta de la posición y el aviso al\n>   iniciar sesión cuando vuelve a haber entradas;"),
 ("> - los avisos por correo electrónico, incluido el aviso de la lista de espera (W-03);",
  "> - los avisos por correo electrónico o notificación push (W-03); el aviso de la lista de espera\n>   se muestra dentro de la aplicación;"),
 # Notas internas dentro de E-09 a E-16.
 (r"re:\n> FA4 describe lo que hace el negocio\..*?«Limitaciones»\.\n", "\n"),
 (r"re:\n> Desde `2af267a` \(14/11\) el inicio de sesión.*?el aviso de la lista de espera\.\n", "\n"),
 ("El paso 4 lo cubre el ítem 11 de CORRECCIONES.", "El paso 4 está al final de esta sección."),
 # Anexo 2: queda solo la versión de PorcentajePara que está en el código.
 (r"re:\*\*Hay dos versiones:\*\*.*?\*\*Si el tope ya está commiteado, poner:\*\*", "**Poner:**"),
 (r"re:\*\*Si todavía no está, poner:\*\*\n\n```csharp.*?```\n\n", ""),
 # Anexo 3, CU-13 con D-6 (c).
 ("- Las entradas devueltas quedan reembolsadas y su cupo se libera para otra compra. Nadie de la\n  lista de espera recibe aviso (W-03).",
  "- Las entradas devueltas quedan reembolsadas y su cupo se libera para otra compra. Los clientes\n  de la lista de espera del evento ven el aviso la próxima vez que inician sesión."),
 # 11b: la nota interna de CORRECCIONES.
 (r"re:> \*\*Definitivo\.\*\* El informe tiene las dos mediciones.*?material para la defensa\.\n\n", ""),
 (r"re:\n## (1[3-6])\. ", "\n### \\1. "),
 # E-14 (CU-06): con D-6 sin decidir la ventana, el texto de la postcondición no cambia.
]
