"""Modelo de tiempos, errores y satisfacción por perfil — estudio de usabilidad SIMULADO.

Las ACCIONES de cada paso salen de la medición sobre la aplicación real
(docs/usabilidad/medicion.txt). Los TIEMPOS se derivan con el Keystroke-Level Model (Card,
Moran y Newell, 1980; valores de Kieras, 2001). Los ERRORES, la SATISFACCIÓN y el SUS se
derivan con reglas explícitas a partir de los puntos de fricción medidos. Nada de esto es
una sesión con personas: es un modelo, y cada número se puede rastrear hasta una regla.

Uso:  python tools/Usabilidad/modelo.py <archivo de salida> [antes|despues]
      «antes»: la interfaz medida antes de T-22 a T-26 (docs/usabilidad/medicion-antes-T22.txt).
      «despues» (por defecto): la interfaz corregida (docs/usabilidad/medicion.txt).
"""

# Operadores KLM (segundos)
P, B, H, M = 1.10, 0.20, 0.40, 1.35   # apuntar, clic, mano teclado↔mouse, preparación mental

# Pasos del documento (págs. 52 y 93) + ingreso. Conteos medidos para mouse y para teclado.
#   m: preparaciones mentales; k: teclas; p: apuntar; b: clics; h: cambios de mano;
#   palabras: texto que hay que leer para decidir (diálogos, resumen, rótulos).
PASOS = [
    # n, nombre, estimado doc (s), mouse {...}, teclado {...}
    (0, "Iniciar sesión", None, dict(m=2, k=30, p=1, b=1, h=1, palabras=0), dict(m=2, k=33, palabras=0)),
    (1, "Buscar el evento", 45, dict(m=2, k=0, p=1, b=1, h=0, palabras=12), dict(m=2, k=1, palabras=12)),
    (2, "Seleccionar el evento", 15, dict(m=1, k=0, p=2, b=2, h=0, palabras=0), dict(m=1, k=8, palabras=0)),
    (3, "Elegir sector y cantidad", 40, dict(m=1, k=2, p=1, b=1, h=1, palabras=20), dict(m=1, k=7, palabras=20)),
    (4, "Agregar servicios", 60, dict(m=1, k=0, p=1, b=1, h=1, palabras=12), dict(m=1, k=2, palabras=12)),
    (5, "Decidir el seguro", 20, dict(m=2, k=0, p=1, b=1, h=0, palabras=25), dict(m=2, k=2, palabras=25)),
    (6, "Revisar el resumen", 30, dict(m=1, k=0, p=0, b=0, h=0, palabras=45), dict(m=1, k=0, palabras=45)),
    (7, "Completar el pago", 60, dict(m=2, k=39, p=1, b=1, h=1, palabras=25), dict(m=2, k=45, palabras=25)),
    (8, "Confirmar y recibir el comprobante", 10, dict(m=1, k=0, p=1, b=1, h=0, palabras=30), dict(m=1, k=1, palabras=30)),
]

# Perfiles: tipeo (s por tecla), lectura (palabras por minuto), factor sobre M y sobre P.
# Edades y experiencia de P1 a P5 como en el documento (pág. 90); P6 agrega el caso de solo teclado.
PERFILES = {
    "P1": dict(desc="21, estudiante universitario, experiencia alta", k=0.20, ppm=250, fm=1.0, fp=1.0, teclado=False),
    "P2": dict(desc="34, empleada administrativa, experiencia media", k=0.28, ppm=220, fm=1.3, fp=1.0, teclado=False),
    "P3": dict(desc="52, comerciante, sin experiencia en compras en línea", k=1.20, ppm=150, fm=2.0, fp=1.5, teclado=False),
    "P4": dict(desc="19, estudiante secundario, experiencia alta, usa más el celular", k=0.35, ppm=250, fm=1.1, fp=1.0, teclado=False),
    "P5": dict(desc="45, docente, experiencia media", k=0.40, ppm=200, fm=1.5, fp=1.2, teclado=False),
    "P6": dict(desc="67, jubilado, baja visión, usa solo teclado y lupa", k=0.90, ppm=110, fm=2.2, fp=0.0, teclado=True),
}

# Errores: acción que obliga a rehacer un paso (definición del documento, pág. 91). Cada uno
# está atado a un hecho medido y tiene un costo en operadores.
ERRORES = {
    "E-A": dict(paso=4, perfiles=["P3"], costo=dict(m=2, p=1, b=1, palabras=20),
                hecho="«Comprar», servicios y seguro habilitados sin entradas; tildar un servicio no avisa nada; al comprar aparece un aviso titulado «RN-13» (medición S8 a S10)"),
    "E-B": dict(paso=3, perfiles=["P3", "P5"], costo=dict(m=2, p=1, b=1, k=2, palabras=20),
                hecho="la cantidad se escribe en una celda de la grilla; al abrir, la celda activa es «Sector» de VIP y no la cantidad de Campo; ningún rótulo indica dónde escribir (medición, recorrido)"),
    "E-C": dict(paso=7, perfiles=["P3", "P6"], costo=dict(m=1, p=1, b=1, k=8, palabras=8),
                hecho="el vencimiento exige «MM/AA» con barra; tras el error se borra el código de seguridad y hay que volver a escribirlo (medición S15)"),
    "E-D": dict(paso=5, perfiles=["P6"], costo=dict(m=2, k=8, palabras=25),
                hecho="en el aviso del seguro el botón por defecto es «No»: con Enter se vuelve al armado sin avanzar (medición, diálogo «Seguro de entrada»)"),
}
# Fricciones que no obligan a rehacer un paso, pero cuestan tiempo.
FRICCIONES = {
    "F-1": dict(paso=0, perfiles=["P4", "P6"], costo=dict(m=1, p=1, b=1),
                hecho="Enter en la contraseña no inicia sesión (no hay AcceptButton)"),
    "F-2": dict(paso=0, perfiles=["P6"], costo=dict(m=1, k=3),
                hecho="el campo Mail está fuera del orden de tabulación: con el teclado no se vuelve a él"),
}
# Asistencia: el recorrido cognitivo da «No» en «¿notará que la acción correcta está disponible?»
# en un paso sin alternativa visible, para perfiles sin experiencia o sin mouse.
ASISTENCIA = {"P3": "paso 3: nada indica que la cantidad se escribe en la celda",
              "P6": "paso 3: con el teclado hay que llegar con flechas a la celda Cantidad de Campo, sin indicación"}


# Si cerrar con la X deja la aplicación sin ventanas (U-01). Lo cambia la versión.
CIERRE_X_DEJA_PROCESO = True
VERSION = "antes"


def aplicar_version(v):
    """Ajusta el modelo a la interfaz medida. Solo cambia lo que la nueva medición cambió."""
    global ASISTENCIA, CIERRE_X_DEJA_PROCESO, VERSION
    VERSION = v
    if v == "antes":
        return
    # Después de T-22 a T-26 (medición del 04/11/2026):
    # - T-25: la columna dice «Cantidad (escribí aquí)», tiene ayuda y el foco arranca en la
    #   Cantidad de la primera fila con cupo: el paso 3 deja de necesitar asistencia, y con el
    #   teclado ya no hay que moverse tres columnas (Abajo ×2, «2», Enter = 4 teclas).
    PASOS[3] = (3, "Elegir sector y cantidad", 40,
                dict(m=1, k=2, p=1, b=1, h=1, palabras=20), dict(m=1, k=4, palabras=20))
    ASISTENCIA = {}
    # - T-26: «Comprar» deshabilitado sin entradas: ya no se llega al aviso «RN-13» (medición S8 y S10).
    ERRORES["E-A"]["perfiles"] = []
    # - E-B sigue: el foco arranca en la fila de VIP; quien no mira la columna Sector escribe la
    #   cantidad ahí en lugar de Campo, y tiene que corregirla.
    ERRORES["E-B"]["hecho"] = ("el foco arranca en la Cantidad de la primera fila (VIP); quien no lee la columna "
                               "Sector escribe ahí en lugar de en Campo (medición, recorrido)")
    # - T-24: cerrar con la X termina la aplicación y la compra pide confirmación (medición S14;
    #   UI Automation sobre el ejecutable: proceso vivo = False).
    CIERRE_X_DEJA_PROCESO = False

def tiempo(op, perfil):
    k = op.get("k", 0) * perfil["k"]
    leer = op.get("palabras", 0) / perfil["ppm"] * 60
    m = op.get("m", 0) * M * perfil["fm"]
    if perfil["teclado"]:
        return k + leer + m
    p = op.get("p", 0) * P * perfil["fp"] + op.get("b", 0) * B + op.get("h", 0) * H
    return k + leer + m + p


def mmss(s):
    s = int(round(s))
    return f"{s // 60}:{s % 60:02d}"


def main():
    print(f"# Resultados del modelo — interfaz «{VERSION}» (generado por tools/Usabilidad/modelo.py)\n")
    print("> Estudio de usabilidad **simulado**. Las acciones salen de la medición sobre la aplicación real;")
    print("> los tiempos, errores, satisfacción y SUS se derivan con las reglas del script.\n")

    # Tiempo experto KLM (operador de referencia: K = 0,28 s, 250 ppm, sin factores)
    ref = dict(k=0.28, ppm=250, fm=1.0, fp=1.0, teclado=False)
    print("## Tiempo por paso: estimado del documento y KLM experto\n")
    print("| Nº | Paso | Estimado doc. | KLM experto | Acciones medidas (mouse) |")
    print("|---|---|---|---|---|")
    tot_doc = tot_klm = 0
    for n, nombre, est, mouse, _ in PASOS:
        t = tiempo(mouse, ref)
        acc = f"{mouse['b']} clic(s), {mouse['k']} tecla(s), {mouse['palabras']} palabras a leer"
        if n > 0:
            tot_doc += est; tot_klm += t
        print(f"| {n} | {nombre} | {'—' if est is None else mmss(est)} | {t:.1f} s | {acc} |")
    print(f"| | **Pasos 1 a 8** | **{mmss(tot_doc)}** | **{mmss(tot_klm)}** | |\n")

    print("## Tiempos, errores y asistencia por perfil\n")
    print("| Perfil | Descripción | Tiempo total (con ingreso) | Pasos 1 a 8 | Errores | Fricciones | Asistencia |")
    print("|---|---|---|---|---|---|---|")
    por_paso = {p: [] for p in PERFILES}
    resumen = {}
    for pid, perf in PERFILES.items():
        total = 0.0; total18 = 0.0
        for n, nombre, est, mouse, tecl in PASOS:
            op = tecl if perf["teclado"] else mouse
            t = tiempo(op, perf)
            for eid, e in list(ERRORES.items()) + list(FRICCIONES.items()):
                if pid in e["perfiles"] and e["paso"] == n:
                    t += tiempo(e["costo"], perf)
            por_paso[pid].append(t)
            total += t
            if n > 0:
                total18 += t
        errs = [e for e, d in ERRORES.items() if pid in d["perfiles"]]
        fr = [f for f, d in FRICCIONES.items() if pid in d["perfiles"]]
        resumen[pid] = dict(total=total, t18=total18, errores=len(errs), asist=pid in ASISTENCIA)
        print(f"| {pid} | {perf['desc']} | {mmss(total)} | {mmss(total18)} | {len(errs)} ({', '.join(errs) or '—'}) | "
              f"{', '.join(fr) or '—'} | {'Sí — ' + ASISTENCIA[pid] if pid in ASISTENCIA else 'No'} |")
    prom = sum(r["total"] for r in resumen.values()) / len(resumen)
    prom18 = sum(r["t18"] for r in resumen.values()) / len(resumen)
    sin = [r for r in resumen.values() if not r["asist"]]
    print(f"| **Promedio** | | **{mmss(prom)}** | **{mmss(prom18)}** | "
          f"**{sum(r['errores'] for r in resumen.values()) / len(resumen):.1f}** | | "
          f"**{len(sin)} de {len(resumen)} sin asistencia** |\n")
    print(f"Promedio sin los perfiles asistidos: {mmss(sum(r['total'] for r in sin) / len(sin))}.\n")

    print("## Tiempo por paso y perfil (segundos, incluye errores y fricciones)\n")
    print("| Nº | Paso | " + " | ".join(PERFILES) + " | Promedio | Estimado doc. |")
    print("|---|---|" + "---|" * (len(PERFILES) + 2))
    for i, (n, nombre, est, _, _) in enumerate(PASOS):
        vals = [por_paso[p][i] for p in PERFILES]
        print(f"| {n} | {nombre} | " + " | ".join(f"{v:.0f}" for v in vals) +
              f" | {sum(vals) / len(vals):.0f} | {'—' if est is None else est} |")
    print()

    # Satisfacción por criterio (1 a 5): parte de 5 y resta 1 por cada fricción medida que el
    # perfil encuentra en ese criterio. Reglas:
    crit = ["Claridad de la navegación", "Facilidad para armar el paquete", "Comprensión del resumen y del descuento",
            "Claridad de los mensajes de error", "Confianza en el proceso de pago", "Satisfacción general"]
    reglas = {
        "Claridad de la navegación": lambda p: 5 - 1 - (p in FRICCIONES["F-1"]["perfiles"]) - (p == "P6") - (CIERRE_X_DEJA_PROCESO and p in ("P3", "P6")),
        "Facilidad para armar el paquete": lambda p: 5 - (p in ERRORES["E-A"]["perfiles"]) - (p in ERRORES["E-B"]["perfiles"]) - (p in ASISTENCIA),
        "Comprensión del resumen y del descuento": lambda p: 5 - (p in ("P3", "P6")) - (p in ("P2", "P5")),
        "Claridad de los mensajes de error": lambda p: 5 - (p in ERRORES["E-A"]["perfiles"]) - (p in ERRORES["E-C"]["perfiles"]) - (p in ERRORES["E-D"]["perfiles"]),
        "Confianza en el proceso de pago": lambda p: 5 - (p in ERRORES["E-C"]["perfiles"]),
    }
    print("## Satisfacción por criterio (1 a 5) — derivada\n")
    print("Regla: se parte de 5 y se resta 1 por cada fricción medida que el perfil encuentra en ese criterio "
          "(detalle en el informe). «Satisfacción general» es el promedio redondeado de los otros cinco, "
          "menos 1 si el perfil necesitó asistencia.\n")
    print("| Criterio | " + " | ".join(PERFILES) + " | Prom. |")
    print("|---|" + "---|" * (len(PERFILES) + 1))
    notas = {p: {} for p in PERFILES}
    for c in crit[:-1]:
        for p in PERFILES:
            notas[p][c] = max(1, reglas[c](p))
    for p in PERFILES:
        notas[p][crit[-1]] = max(1, round(sum(notas[p][c] for c in crit[:-1]) / 5) - (p in ASISTENCIA))
    glob = []
    for c in crit:
        vals = [notas[p][c] for p in PERFILES]
        glob.append(sum(vals) / len(vals))
        print(f"| {c} | " + " | ".join(str(v) for v in vals) + f" | {sum(vals) / len(vals):.1f} |")
    print(f"| **Promedio global** | | | | | | | **{sum(glob) / len(glob):.2f}** |\n")
    buenas = sum(1 for p in PERFILES if notas[p]["Satisfacción general"] >= 4)
    print(f"Perfiles con satisfacción general buena o muy buena (4 o 5): {buenas} de {len(PERFILES)} "
          f"({buenas / len(PERFILES) * 100:.0f} %).\n")

    # SUS: cada ítem sale de un criterio o de un hecho medido (ver comentarios en cada línea).
    print("## System Usability Scale — derivado\n")
    print("Reglas de las respuestas (1 a 5) en el informe, sección «System Usability Scale».\n")
    print("| Perfil | Ítems 1 a 10 | Puntaje SUS | Grado (Bangor et al., 2009) |")
    print("|---|---|---|---|")
    sus_vals = []
    for p in PERFILES:
        g = notas[p]["Satisfacción general"]
        exp = {"P1": "alta", "P4": "alta", "P2": "media", "P5": "media"}.get(p, "nula")
        errores_p = sum(1 for d in ERRORES.values() if p in d["perfiles"])
        r = [min(4, g),                                              # 1 lo usaría con frecuencia: compra ocasional, tope 4
             6 - notas[p]["Facilidad para armar el paquete"],         # 2 innecesariamente complejo
             g,                                                       # 3 fácil de usar
             4 if p in ASISTENCIA else (2 if errores_p else 1),       # 4 necesitaría ayuda técnica
             notas[p]["Facilidad para armar el paquete"],             # 5 funciones bien integradas
             3 if (p in FRICCIONES["F-1"]["perfiles"] or p in ERRORES["E-D"]["perfiles"] or p in ERRORES["E-A"]["perfiles"]) else 2,  # 6 inconsistencias
             {"alta": 5, "media": 4}.get(exp, 2),                     # 7 se aprende rápido
             6 - notas[p]["Claridad de la navegación"],               # 8 engorroso
             notas[p]["Confianza en el proceso de pago"],             # 9 me sentí seguro usándolo
             {"alta": 1, "media": 2}.get(exp, 4)]                     # 10 tuve que aprender mucho antes        r = [min(5, max(1, x)) for x in r]
        sus = (sum(r[i] - 1 for i in range(0, 10, 2)) + sum(5 - r[i] for i in range(1, 10, 2))) * 2.5
        sus_vals.append(sus)
        grado = "A" if sus >= 90 else "B" if sus >= 80 else "C" if sus >= 70 else "D" if sus >= 60 else "F"
        print(f"| {p} | {' '.join(map(str, r))} | {sus:.1f} | {grado} |")
    print(f"| **Promedio** | | **{sum(sus_vals) / len(sus_vals):.1f}** | |")


if __name__ == "__main__":
    import io, sys
    aplicar_version(sys.argv[2] if len(sys.argv) > 2 else "despues")
    if len(sys.argv) > 1:   # escribir directo en UTF-8, sin pasar por la consola
        sys.stdout = io.open(sys.argv[1], "w", encoding="utf-8", newline="\n")
    main()
