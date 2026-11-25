# Arma la columna «Casos de prueba» de la matriz de trazabilidad (p. 107) desde los comentarios
# de trazabilidad de las pruebas: «// CP-xx — RF-yy, RN-zz: …» (definición de terminado, punto 7).
# Lee Pruebas/*.cs tal como están en el commit indicado (por defecto, HEAD), no la copia de
# trabajo, para no tomar cambios de Codex a medio hacer.
#
# Uso: py -3 -X utf8 tools/Matriz/Extraer.py [commit]
import re, subprocess, sys
from collections import defaultdict

commit = sys.argv[1] if len(sys.argv) > 1 else 'HEAD'
raiz = subprocess.run(['git', 'rev-parse', '--show-toplevel'], capture_output=True, text=True).stdout.strip()

def git(*a):
    return subprocess.run(['git', '-C', raiz, *a], capture_output=True, text=True, encoding='utf-8').stdout

archivos = [f for f in git('ls-tree', '-r', '--name-only', commit, 'Pruebas/').splitlines() if f.endswith('.cs')]
COMENTARIO = re.compile(r'//\s*(CP-\d+(?:\s*(?:a|,|y)\s*CP-\d+)*)\s*[—–-]\s*([^:\n]*)')

por_cp = defaultdict(set)      # CP -> {RF-xx, RNF-xx}
reglas_cp = defaultdict(set)   # CP -> {RN-xx}
todos = set()
for f in archivos:
    texto = git('show', f'{commit}:{f}')
    todos |= {int(n) for n in re.findall(r'\bCP-(\d+)\b', texto)}
    for m in COMENTARIO.finditer(texto):
        ids = m.group(1)
        nums = []
        for a, b in re.findall(r'CP-(\d+)\s*a\s*CP-(\d+)', ids):
            nums += range(int(a), int(b) + 1)
        nums += [int(n) for n in re.findall(r'CP-(\d+)', ids)]
        refs = m.group(2)
        req = {r for r in re.findall(r'\b(RF-\d+|RNF-\d+)\b', refs)}
        rn = set(re.findall(r'\b(RN-\d+)\b', refs))
        for n in set(nums):
            por_cp[n] |= req
            reglas_cp[n] |= rn

def rangos(nums):
    nums = sorted(set(nums)); out = []; i = 0
    while i < len(nums):
        j = i
        while j + 1 < len(nums) and nums[j + 1] == nums[j] + 1: j += 1
        if j - i >= 2: out.append(f'CP-{nums[i]:02d} a CP-{nums[j]:02d}')
        else: out += [f'CP-{n:02d}' for n in nums[i:j + 1]]
        i = j + 1
    return ', '.join(out)

por_req = defaultdict(set)
for n, reqs in por_cp.items():
    for r in reqs: por_req[r].add(n)

sin_req = sorted(n for n in todos if not por_cp.get(n))
print(f'Commit: {git("rev-parse", "--short", commit).strip()}')
print(f'Casos citados en las pruebas: {len(todos)} (CP-{min(todos):02d} a CP-{max(todos):02d})')
faltan = sorted(set(range(1, max(todos) + 1)) - todos)
print('Números sin uso:', ', '.join(f'CP-{n:02d}' for n in faltan) or 'ninguno')
print('Casos sin requerimiento en su comentario:', ', '.join(f'CP-{n:02d}' for n in sin_req) or 'ninguno')
print()
clave = lambda r: (r.startswith('RNF'), int(r.split('-')[1]))
for r in sorted(por_req, key=clave):
    print(f'| {r} | {len(por_req[r])} | {rangos(por_req[r])} |')
print()
print('Reglas citadas por requerimiento (de los mismos comentarios):')
for r in sorted(por_req, key=clave):
    rn = set()
    for n in por_req[r]: rn |= reglas_cp[n]
    print(f'{r}: {", ".join(sorted(rn, key=lambda x: int(x[3:]))) or "—"}')

# Columnas que no salen de las pruebas: objetivo, prioridad (p. 36), reglas y casos de uso según
# la tabla de reglas (pp. 25 a 27, columna «RF / CU») y hallazgos de usabilidad.
FILAS = [
 ('RF-01', 'OE-2', 'Must', 'RN-01, RN-02, RN-03', 'CU-04', '—'),
 ('RF-02', 'OE-2', 'Must', 'RN-04, RN-40, RN-41', 'CU-05, CU-09', '—'),
 ('RF-03', 'OE-2', 'Must', 'RN-09', 'CU-01', '—'),
 ('RF-04', 'OE-2', 'Must', 'RN-08, RN-12, RN-33', 'CU-01', 'H-03'),
 ('RF-05', 'OE-2', 'Must', 'RN-13, RN-14, RN-28, RN-29', 'CU-01, CU-13', 'H-04'),
 ('RF-06', 'OE-5', 'Must', 'RN-15, RN-16, RN-30, RN-31, RN-32', 'CU-01', 'H-02'),
 ('RF-07', 'OE-2', 'Must', 'RN-17, RN-18', 'CU-01, CU-12', '—'),
 ('RF-08', 'OE-3', 'Should', 'RN-19 a RN-24', 'CU-01, CU-03', 'H-01, H-05'),
 ('RF-09', '—', "Won't", '—', '—', '—'),
 ('RF-10', 'OE-6 ind.', 'Could', 'RN-10, RN-35, RN-36', 'CU-07', '—'),
 ('RF-11', 'OE-6', 'Should', 'RN-34', 'CU-08', '—'),
 ('RF-12', 'OE-2', 'Must', 'RN-06, RN-07, RN-11, RN-38', 'CU-02, CU-10, CU-11', '—'),
 ('RF-13', 'OE-3', 'Should', 'RN-25, RN-26, RN-37, RN-39', 'CU-06', '—'),
 ('RF-14', 'OE-2', 'Must', 'RN-43, RN-44', 'CU-04, CU-14', '—'),
 ('RF-15', 'OE-2', 'Must', 'RN-44', 'CU-15', '—'),
 ('RF-16', 'OE-2', 'Must', 'RN-45', 'CU-20', '—'),
 ('RF-17', 'OE-2', 'Must', 'RN-46, RN-47', 'CU-04, CU-05, CU-09, CU-15', '—'),
 ('RF-18', 'OE-2', 'Should', '—', 'CU-01', '—'),
 ('RF-19', 'OE-2', 'Should', '—', 'CU-02', '—'),
 ('RF-20', 'OE-2', 'Must', 'RN-50 a RN-53', 'CU-16, CU-18', '—'),
 ('RF-21', 'OE-2', 'Must', 'RN-06, RN-54', 'CU-17, CU-19', '—'),
 ('RF-22', 'OE-2', 'Must', 'RN-05, RN-48, RN-49', 'CU-02, CU-07, CU-10 a CU-12, CU-21', '—'),
]
print()
print('| RF | OE | MoSCoW | Reglas de negocio | CU | Casos de prueba | Hall. |')
print('|---|---|---|---|---|---|---|')
for rf, oe, mo, rn, cu, h in FILAS:
    print(f'| {rf} | {oe} | {mo} | {rn} | {cu} | {rangos(por_req.get(rf, [])) or "—"} | {h} |')
