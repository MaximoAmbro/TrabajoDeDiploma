# Revisión del código de 6A (commit `36a0d80`)

Claude Code, 10/11/2026, a pedido de JC. Se leyeron:
- `Visual/Tema.cs`, `Avisos.cs`, `FormularioEventAura.cs` y `app.manifest`;
- los tres botones de `Visual/Componentes/`;
- `FrmCompraCliente.cs`, entero;
- los usos del tema en `FrmCompraCliente.Designer.cs`.

**No se modificó código.** Lo que figura como «a confirmar» no tiene evidencia todavía.

**Resumen:** 6A cumple el encargo y funciona: 77/77, verificado a escala real por Codex. Hay
**cuatro cosas que cumplen la letra del encargo y nos van a costar después**. Tres nacen de
cómo escribí yo el encargo. Conviene corregirlas en T-41, antes de repetirlas 18 veces en 6B.

---

## 1. Lo que cumple la letra y nos causa un problema después

### R-1. Guardar desde el diseñador rompe los botones (verificado)

Cada botón pone su estilo en el constructor (fondo, color de texto, fuente, borde). El
diseñador de Visual Studio, al guardar, escribe toda propiedad cuyo valor difiere del valor
por omisión.

**Medido el 10/11** con el host de diseño de .NET (`ShouldSerializeValue` sobre una copia de
`Visual.exe`):

| Componente | Lo que el diseñador escribiría al guardar |
|---|---|
| `BotonPrimario` | `Font`, `BackColor`, `ForeColor`, `FlatStyle`, `MinimumSize`, `Padding`, `Cursor`, `UseVisualStyleBackColor` |
| `BotonSecundario` | `Font`, `BackColor`, `ForeColor` (y el resto, igual que el primario) |
| `FormularioEventAura` como raíz | **Nada**: lo que pone el constructor de la base no se escribe. **La base está bien** |

**Consecuencia:**
- **qué pasa al guardar:** basta con que alguien guarde una vez el piloto desde Visual
  Studio. Cada botón queda con `new Font(…)` y `Color.FromArgb(…)` escritos en el Designer;
- **qué se pierde:** si después cambia un color del tema, esos botones no se enteran;
- **qué lo detecta:** CP-63 falla, que es lo correcto.

**Corrección, en T-41:**
- en cada componente, sobrescribir esas propiedades con `[DefaultValue]` igual al valor del
  tema (o `ShouldSerialize<Propiedad>() => false`), que es la forma estándar de WinForms;
- una prueba nueva, con el mismo método que esta medición: ninguna propiedad de estilo de un
  componente se serializa.

### R-2. Medidas del piloto dentro del tema (`Tema.Compra`)

**Qué hay hoy:**
- `Tema.cs` tiene una clase `Compra` con 27 posiciones y tamaños propios de una sola pantalla,
  y el Designer las referencia (`label1.Location = Tema.Compra.Titulo`);
- **el origen es mi encargo:** la regla 3 decía «ninguna medida nueva fuera de `Tema.cs`», y
  Codex la cumplió al pie de la letra.

**Tres problemas:**
1. **El tema deja de ser un juego de tokens.** En 6B crecería con `Tema.Pago`,
   `Tema.Historial` y otros 16 más, con medidas que solo usa una pantalla.
2. **Hay dos fuentes de verdad.** `AjustarEncabezado` recalcula en ejecución casi todas esas
   posiciones. Lo de `Tema.Compra` solo vale para el diseñador y para el primer instante.
3. **Visual Studio tiene que leer esas referencias, y no sabemos si lo hace** (ver R-3).

**Corrección:**
- las posiciones y tamaños de una pantalla van **en su Designer, como números a 96 DPI**, que
  es lo que Visual Studio escribe y lee siempre;
- `Tema.cs` conserva solo los tokens (colores, fuentes, espaciado, alturas comunes);
- **la regla 3 del encargo se reescribe:** «ningún color ni fuente fuera de `Tema.cs`; las
  medidas de una pantalla en su Designer, múltiplos de la escala de 8».

### R-3. Referencias al tema dentro del Designer

**La pregunta de JC: ¿es la forma correcta?** No, por dos razones:

1. **Al leer (inconcluso).** Visual Studio no ejecuta `InitializeComponent`: lo lee con su
   analizador de código. No está comprobado que resuelva `Tema.Compra.Titulo`, que es un tipo
   anidado. Lo dice la prueba de Máximo.
2. **Al guardar (seguro, por R-1).** Aunque lo lea, al guardar escribe
   `this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 18F)` y
   `this.label1.Location = new System.Drawing.Point(24, 4)`. **La referencia al tema se
   pierde en el primer guardado.**

**La forma que no depende del diseñador:**
- **el Designer lleva** controles, nombres, textos, `TabIndex` y geometría literal. **Ninguna
  fuente ni color;**
- **los botones** traen su estilo del componente, que con R-1 no se serializa;
- **las etiquetas con un rol** (título de ventana, título de tarjeta, ayuda) reciben fuente y
  color por código, después de `InitializeComponent`. Dos formas posibles:
  - un método de rol: `Tema.ComoTitulo(label1)`;
  - componentes chicos (`EtiquetaTitulo : Label`), con la misma técnica de R-1;
- así el diseñador muestra la pantalla con su forma y, si alguien guarda, no se pierde nada.

**Esta corrección es independiente del resultado de Máximo.** Si a Máximo le abre bien, el
problema de guardar sigue igual.

### R-4. Una recursión sin tope en `AjustarEncabezado`

**El código.** `FrmCompraCliente.cs` termina el método así:

```
if (anchoUsado != contenidoCompra.ClientSize.Width) AjustarEncabezado();
```

**El riesgo:**
- si la barra vertical aparece y desaparece en llamadas sucesivas, el método se llama sin
  fin, y eso termina en `StackOverflowException`;
- en .NET esa excepción **no se puede atrapar**: el manejador global de T-27 no la ve y la
  aplicación se cierra sin aviso;
- puede pasar cuando el contenido queda justo en el límite de la vista y el resumen cambia de
  alto según el ancho.

**Plausible, no reproducido.**

**Corrección** (una línea, en T-41, al extraer `Disposicion.cs`): repetir **una sola vez**,
con un contador.

---

## 2. ¿El patrón que resolvió CP-25 se puede reutilizar?

**La idea sí; el código no: está pegado al piloto.**

| Pieza | Estado | Reutilizable |
|---|---|---|
| Pie fijo (`Dock = Bottom`) y contenido con desplazamiento (`Dock = Fill`) | En el Designer del piloto | Sí, se copia como estructura |
| `AlinearAcciones` | Privada, usa `btnVolver` y `btnComprar` | No: hay que parametrizarla (pie, secundario, principal) |
| `AjustarAreaVisible` (ajuste al área de trabajo) | Privada | Sí, casi sin cambios |
| `PintarTarjeta`, `PintarSeparadorInferior` y `PintarSeparadorSuperior` | Privadas y estáticas | Sí, se mueven tal cual |
| Cálculo de la escala | Dentro de `AjustarEncabezado` | Sí, como método |
| **Documento y vista** (la tarjeta del resumen con su propio desplazamiento) | Dentro de `AjustarEncabezado`, mezclado con el título, el nivel, las dos columnas y las casillas | **No:** hay que extraerlo como `AjustarDocumento(tarjeta, etiqueta, altoDisponible)`. Lo necesitan `frmConfirmacion` (desglose) y `frmReportes` (lectura) |
| Guarda de reentrada y repetición por la barra | Con el defecto de R-4 | Sí, una vez corregida |

Es lo que pide T-41 en `ENCARGO-6B-BORRADOR.md`. Esta revisión agrega a T-41: R-1, R-2, R-3
(ver la sección 4) y R-4.

---

## 3. Otras observaciones

| # | Dónde | Qué | Severidad |
|---|---|---|---|
| O-1 | `BotonPrimario.OnGotFocus` | **El foco no se ve en el botón primario.** El borde de foco es de 2 px `Primario` sobre un fondo `Primario`. Lo pidió así mi `SISTEMA-DISENO.md` (5.1) sin distinguir el primario. Quien usa solo teclado (perfil de RNF-01) no sabe que está en «Comprar». **A confirmar** si WinForms dibuja además el rectángulo punteado del sistema en un botón plano | **Media**: es accesibilidad, que el estudio de usabilidad mide. Corrección: borde de foco `Texto` (#1F1F24) en el primario, o un borde blanco interior |
| O-2 | `Tema.Normalizar` | Crea un `new Font(…)` por cada control con fuente heredada, cada vez que se abre una pantalla, y nunca la libera. La navegación existente esconde las pantallas en lugar de cerrarlas (`Hide`), así que se acumulan | **Baja** para una demostración: hace falta abrir pantallas unas mil veces para acercarse al límite de objetos gráficos de Windows. Corrección: guardar las fuentes ya creadas por familia, tamaño y estilo |
| O-3 | `Tema.Normalizar` | `new Font(familia, tamaño)` pierde la cursiva y el subrayado de la fuente original | Baja. Hay que revisar si alguna etiqueta heredada era subrayada a propósito, como un enlace hecho con `Label` (T-1 a T-5): si lo era, deja de parecer un enlace |
| O-4 | `Tema.EstiloBoton` y los tres componentes | El mismo estilo de botón está escrito dos veces: uno para los botones heredados y otro en los componentes. Si se cambia en un lado, el otro queda distinto | Baja. Unificar en T-41 |
| O-5 | `FrmCompraCliente.GenerarPDF` | Se conservó la firma `(ruta, mensaje, QR)`, como pedía el encargo. Por eso el ticket arma un texto con «SECTOR:», «FECHA:»… y **después lo vuelve a separar** para ubicar cada dato. Funciona; si alguien cambia una etiqueta del texto, el dato desaparece del ticket sin error | Baja. Si se toca el ticket otra vez, pasar los datos como parámetros |
| O-6 | `GenerarPDF`, título | El nombre del evento va en un recuadro de unos 54 pt de alto (3 renglones de 16 pt). Si un nombre ocupa más, iText **descarta el sobrante sin avisar**. **A confirmar** con un nombre de 100 caracteres en el ticket: CP-65 usa un nombre corto | Baja |
| O-7 | `FrmCompraCliente.cs`, línea 706 | Quedó un acento perdido en un comentario: «varias l?neas». Codex corrigió cuatro y se le pasó este. No es visible | Mínima |
| O-8 | `FrmCompraCliente.cs`, `using` | `Microsoft.Identity.Client` e `iText.Pdfa.Checker` no se usan. **Anterior a 6A** | Mínima |
| O-9 | `app.manifest` | Correcto: DPI de sistema, Windows 10 y 11, `asInvoker`. Con DPI de sistema, en un segundo monitor con otra escala Windows estira la ventana. Es lo que se eligió en V-2 | Ninguna |
| O-10 | `Avisos.Regla` | Correcto. Si una `ReglaNegocioException` llegara sin código, el texto terminaría en « ()». Hoy todas lo traen | Mínima |
| O-11 | `FormularioEventAura` | Correcto. La guarda de diseño en `OnLoad` y el constructor sin lógica son lo que hacía falta (la lección de T-24) | — |

---

## 4. Qué propongo agregar a T-41 (el borrador de 6B no se tocó)

1. **R-1:**
   - los componentes no serializan su estilo;
   - una prueba lo comprueba.
2. **R-2 y R-3:**
   - sacar `Tema.Compra`;
   - el Designer del piloto pasa a geometría literal a 96 DPI, sin fuentes ni colores;
   - los roles de etiqueta se aplican por código;
   - reescribir la regla 3 del encargo y `SISTEMA-DISENO.md` 8.1.
3. **R-4:** repetir una sola vez.
4. **O-1:** el foco visible en el primario, con el cambio en `SISTEMA-DISENO.md` 5.1.
5. **O-2 y O-4:** caché de fuentes y un único estilo de botón.

**Estimación: 3 a 5 horas de Codex (estimadas).** Se ejecuta **después** de la prueba de
Máximo, que dice si el piloto se lee como está, y antes de rediseñar ninguna otra pantalla.

**Queda abierto para cuando JC pase el relevamiento de Codex** (colores y medidas escritos a
mano que pisan el tema): con esa lista, T-41 o cada tarea de 6B suma «quitar las asignaciones
que pisan el tema». Hoy se sabe de una sola, `frmMisLocales.cs:40` (A-02).
