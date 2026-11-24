# Inventario del cierre del documento. Recorre docs/LISTA-PARA-MAXIMO.md (el texto que va al
# Word) y lista, por página, lo que depende del código final:
#
#   T  marcador de cierre [[...]] que todavía no se completó
#   B  cifra de la batería escrita a mano (casos, ejecuciones), en número o en letras
#   R  rango o numeración de casos escrita a mano (CP-xx a CP-yy, números sin usar)
#   K  cobertura de reglas por los casos (cambia solo si una prueba nueva cubre RN-07 o RN-33)
#   C  el commit del código que se entrega, escrito a mano
#
# Las notas para JC (*Razón:*, *Cómo se armó:*) y las instrucciones para Máximo no van al Word:
# se listan aparte, con --notas. Las filas de las tablas de casos (| CP-xx | ...) no cuentan: van
# dentro de un [[BLOQUE-...]]. Las páginas de pantallas en pausa no se listan línea por línea:
# van enteras (docs/CIERRE-DOCUMENTO.md, sección 3).
#
# Uso: py -3 -X utf8 tools/Cierre/Inventario.py [--resumen] [--notas] [--commit 2db272e]
import re, sys, os
from collections import Counter

raiz = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
ruta = os.path.join(raiz, 'docs', 'LISTA-PARA-MAXIMO.md')
texto = open(ruta, encoding='utf-8').read()
lineas = texto.split('\n')
commit = sys.argv[sys.argv.index('--commit') + 1] if '--commit' in sys.argv else '2db272e'

PATRONES = [
    ('T', re.compile(r'\[\[[A-Z0-9.\-]+\]\]')),
    ('B', re.compile(r'(?<!CP-)\b(214|205|360|348)\b(?=\s*(\(|casos|ejecuciones|,\s*todas))'
                     r'|doscientos\s+(catorce|cinco)\b|trescientas?\s+(sesenta|cuarenta\s+y\s+ocho)', re.I)),
    ('R', re.compile(r'CP-\d+ a CP-(206|211|357|353)\b|CP-212\b')),
    ('K', re.compile(r'\b50 de 52\b|(?<!vendieron )96,2 %|cincuenta de (las )?cincuenta y dos|cincuenta reglas distintas')),
    ('C', re.compile(re.escape(commit) + r'[0-9a-f]*')),
]

# Página y tipo de cada renglón.
pagina_de, nota_de = [], []
pagina = '(avisos para Máximo, antes de la p. 1)'
texto_plano = False   # la sección «Metodología de verificación» se pega entera, sin «>»
for l in lineas:
    s = l.strip()
    if l.startswith('## '):
        pagina = l[3:].strip()
        texto_plano = False
    if s == '**Metodología de verificación**':
        pagina, texto_plano = 'Sección nueva «Metodología de verificación» (después de la página 109)', True
    pagina_de.append(pagina)
    va_al_word = (re.search(r'[Pp]ágina', pagina) is not None
                  and (texto_plano or s.startswith('>') or (s.startswith('|') and not s.startswith('|---'))))
    nota_de.append(not va_al_word)
inicio = [0]
for l in lineas:
    inicio.append(inicio[-1] + len(l) + 1)

def renglon(pos):
    lo, hi = 0, len(lineas)
    while lo < hi:
        mid = (lo + hi) // 2
        if inicio[mid + 1] <= pos: lo = mid + 1
        else: hi = mid
    return lo

hallazgos = []
for tipo, patron in PATRONES:
    for m in patron.finditer(texto):
        i = renglon(m.start())
        if lineas[i].lstrip().startswith('| CP-') and tipo != 'T':
            continue
        hallazgos.append((i + 1, tipo, pagina_de[i], ' '.join(m.group(0).split()), nota_de[i], lineas[i].strip()[:100]))
hallazgos.sort()

notas = '--notas' in sys.argv
elegidos = [h for h in hallazgos if h[4] == notas]
if '--resumen' in sys.argv:
    por = Counter((h[2], h[1]) for h in elegidos)
    paginas = list(dict.fromkeys(h[2] for h in elegidos))
    print('| Sección | T | B | R | K | C |')
    print('|---|---|---|---|---|---|')
    for p in paginas:
        print(f'| {p} | ' + ' | '.join(str(por.get((p, t), '')) for t in 'TBRKC') + ' |')
    tot = Counter(h[1] for h in elegidos)
    print(f"\nTotal {'en notas e instrucciones' if notas else 'en texto que va al Word'}: {len(elegidos)} "
          f"(T {tot['T']}, B {tot['B']}, R {tot['R']}, K {tot['K']}, C {tot['C']}) en {len(paginas)} secciones.")
else:
    for n, tipo, pagina, valor, _, contexto in elegidos:
        print(f'{tipo}\tL{n}\t{pagina[:45]}\t{valor}\t{contexto}')
