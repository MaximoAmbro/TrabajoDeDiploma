# Fase de pulido visual: propuesta (26/11/2026, noche)

Esto es **solo una propuesta**: no se instaló ni se implementó nada. JC decide qué entra. Lo que se apruebe va como
encargo para Codex, porque las vistas son de `Visual/Web/`.

## a. Skills y herramientas de diseño

**Criterio.** Una skill de terceros son instrucciones que Claude ejecuta con sus propios permisos. Por eso miré tres
cosas:
- **la licencia;**
- **qué ejecuta:** archivos `.sh`, `.py` o `.js` del repositorio, que la skill puede mandar a correr;
- **el mantenimiento:** el último commit y quién la mantiene.

Cada fila fija el commit revisado, como se hizo con `/taste` (`senlindesign/taste-skill@6dce223`, en
`~/.claude/skills/taste/ORIGEN.txt`). Si se instala una, se instala ese commit y no la rama principal.

| Nombre (commit revisado) | Qué aporta a EventAura | Licencia · qué ejecuta | Riesgo | Recomendación |
|---|---|---|---|---|
| **`/taste`**, ya instalada (`senlindesign/taste-skill@6dce223`) | Saca la «taste DNA» de un sitio de referencia, con medidas reales tomadas con Playwright | Revisada el 25/11 · un extractor JS que corre en la página analizada | Bajo, ya revisada | **Mantener** |
| **Playwright MCP**, ya en uso (`microsoft/playwright-mcp@2337aff`) | Mide y captura las maquetas de `Visual/Web`. Es la base de §7, §9 y de la verificación del Enter de la suscripción | Apache-2.0 · es un servidor y ejecuta código | Medio: `browser_run_code_unsafe` ejecuta JS arbitrario. Usarlo solo sobre archivos locales | **Mantener** |
| **`web-design-guidelines`** (`vercel-labs/agent-skills@5a37a63`) | Audita el HTML y el CSS contra las Web Interface Guidelines de Vercel: foco, estados, formularios, ARIA, números tabulares. Responde en formato `archivo:línea`, que sirve para encargos a Codex | Sin licencia declarada en el repositorio; las reglas son MIT (`web-interface-guidelines@434b7f9`) · sin scripts: `SKILL.md` y `references/command.md`, fijado por SHA-256 y sin red | Bajo | **Instalar** (la primera de esta lista). Es la que más se parece a la rúbrica de c. |
| **`frontend-design`** (`anthropics/skills@e4bb5d9`) | Criterio de dirección visual y una crítica de lo genérico: tipografías gastadas, degradés violetas | Apache-2.0 · sin scripts: `SKILL.md` y `LICENSE.txt` | Bajo | **Probar en una sola pantalla.** Está pensada para crear desde cero, y EventAura ya tiene sistema (`PRINCIPIOS-DISENO.md`): puede empujar a rediseñar por gusto, que `CLAUDE.md` prohíbe |
| **`emil-design-eng`**, **`animate`** y **`review-animations`** (`emilkowalski/skill@64eb3de`) | Movimiento con el método que ya se adoptó en §10 («Agents with Taste»). `review-animations` revisaría el catálogo de b. | MIT · sin scripts | Bajo | **Instalar solo `review-animations`**, si se aprueba b. Las otras repiten §10 |
| **Impeccable** (`pbakaus/impeccable@2fc5e14`) | Muchas skills de pulido (`animate`, `polish`, `audit`…) | Apache-2.0 · **450 archivos ejecutables** | Alto: demasiado código para revisar antes de la entrega | **No instalar** |
| **UI/UX Pro Max** (`nextlevelbuilder/ui-ux-pro-max-skill@13e1b98`) | Genera sistemas de diseño: estilos, paletas, tipografías | MIT · **202 scripts**, con un buscador en Python | Alto, y además genera sistemas nuevos: choca con el sistema que ya existe | **No instalar** |

**Fuentes:**
- los repositorios de GitHub (licencia, último commit y archivos, por `gh api`, el 26/11);
- [web-design-guidelines en skills.sh](https://skills.sh/vercel-labs/agent-skills/web-design-guidelines);
- [frontend-design en mcpservers.org](https://mcpservers.org/agent-skills/anthropic/frontend-design);
- [emilkowalski/skills](https://github.com/emilkowalski/skills);
- [la guía de skills de diseño de pasqualepillitteri.it](https://pasqualepillitteri.it/en/news/576/claude-code-skills-design-uiux-guide).

## b. Animaciones: un catálogo coherente con §10

**Hoy.** El proyecto tiene dos movimientos, los dos en el armador (`PRINCIPIOS-DISENO.md` §8): el total que cuenta
y el renglón nuevo que entra. Además están las transiciones de color de 120 ms y la apertura del desglose en
`compra.css`, de 140 ms.

**Las reglas de §10 que valen para todo el catálogo:**
- entrar o salir de la vista, `ease-out`; moverse dentro de la vista, `ease-in-out`;
- nada pasa de 300 ms, y las salidas son un 20 % más rápidas que sus entradas;
- lo que entra arranca en `scale(0.95)`, nunca en 0;
- con `prefers-reduced-motion: reduce`, `sistema.css` ya anula toda transición y toda animación (línea 250). Lo
  nuevo hereda esa regla sin trabajo extra, **siempre que se haga en CSS**. Lo que se anime con JavaScript tiene que
  consultar `matchMedia`, como ya hace el total.

| # | Animación | Dónde va | Cómo | Costo | Riesgo | Con `prefers-reduced-motion` |
|---|---|---|---|---|---|---|
| 1 | **Apertura del modal** | Contratar la suscripción, cuando Codex reemplace el MessageBox | Fondo de 0 a 0,5 de opacidad; la caja de `scale(0.95)` y opacidad 0 a su estado final, 200 ms `ease-out`. Cierre en 160 ms | Bajo: CSS sobre `<dialog>` | Bajo. Hay que confirmar que Enter y Esc respondan durante la animación | Aparece sin escala |
| 2 | **Transición entre pantallas** del shell de «Mi cuenta» | Al cambiar de sección desde el menú lateral | Solo el contenido web: opacidad de 0 a 1 y 4 px hacia arriba, 150 ms `ease-out`. El menú y la barra no se mueven | Medio: cada página tiene que arrancar oculta y mostrarse al primer `eventAuraActualizar` | **Medio-alto:** si falla, la página queda invisible. El respaldo WinForms no se anima. **Recomiendo no hacerla antes de la entrega** | Corte directo |
| 3 | **Estado de éxito** | «¡Compra confirmada!», «Correo verificado», «Suscripción contratada» | El ícono de confirmación entra de `scale(0.95)` a 1 con opacidad, 200 ms `ease-out`. El texto queda quieto | Bajo | Bajo | Aparece sin escala |
| 4 | **Carga** | Mientras se espera a la base en el catálogo y el armador, y al pagar | Primero, **nada** durante 300 ms. Si la espera sigue, un esqueleto con un brillo `linear` de 1,2 s | Medio | Medio: un brillo continuo distrae. Si la carga es casi siempre menor de 300 ms, no hace falta | Esqueleto quieto |
| 5 | **Botones** | Todos | Ya cumple: `scale(0.97)` en `:active` y color en 120 ms. **No agregar nada** | — | — | Ya resuelto |
| 6 | **Listas** | Catálogo e historial | **Sin escalonado al cargar:** se ve lindo una vez y demora cada vuelta. Solo un renglón que se agrega o se quita, como el desglose: 200 ms de entrada y 160 ms de salida | Bajo, si se reusa `entrar` y `salir` de `armador-paquete.css` | Bajo | Sin movimiento |
| 7 | **Aviso que aparece** | El aviso de RN-43 y los errores de formulario | Altura animada con `interpolate-size`, como el desglose, en 150 ms `ease-out`. El resto de la pantalla baja sin saltar | Bajo | Bajo; ya se usa en `compra.css` | Aparece de golpe |

**Recomendación:**
- **hacer 1, 3 y 7:** son baratas y van en CSS;
- **hacer 6** solo donde ya se agregan renglones;
- **4,** solo si se mide una espera mayor de 300 ms;
- **2, no** antes de la entrega.

El total del proyecto pasaría de dos movimientos a cinco o seis. §6 dice hoy «ninguna otra cosa se desplaza»: hay
que actualizarlo con lo que se apruebe.

## c. Auditoría con rúbrica

**Rúbrica, de 0 a 10 por criterio:**
- jerarquía;
- espaciado;
- tipografía;
- disciplina de color;
- estados: hover, foco, deshabilitado, vacío y error;
- movimiento;
- acabado: bordes, alineación, cortes y desbordes.

**Puntuación:** pendiente. Sale de capturas de la aplicación real dentro de «Mi cuenta», después de que Codex
entregue T-45, para no puntuar una pantalla que va a cambiar.
