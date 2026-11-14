"""Comprueba las reglas de conexión de un diagrama de robustez escrito en PlantUML.

Uso:  py -3 tools/Diagramas/VerificarRobustez.py docs/diagramas/fuentes/robustez-comprar-paquete.puml

Permitidas: actor–frontera, frontera–control, control–control y control–entidad.
Prohibidas todas las demás (actor–control, actor–entidad, frontera–frontera, frontera–entidad,
entidad–entidad, actor–actor). Lee las declaraciones `actor|boundary|control|entity` y las
líneas `A -- B` (con o sin flecha). Sale con código 1 si encuentra alguna prohibida.
"""
import re
import sys

PERMITIDAS = {frozenset(p) for p in [("actor", "boundary"), ("boundary", "control"),
                                     ("control",), ("control", "entity")]}

texto = open(sys.argv[1], encoding="utf-8").read()
tipos = {}
for m in re.finditer(r'^(actor|boundary|control|entity)\s+(?:"[^"]*"\s+as\s+(\w+)|(\w+)(?:\s+as\s+(\w+))?)',
                     texto, flags=re.M):
    tipo, alias_comillas, nombre, alias = m.groups()
    tipos[alias_comillas or alias or nombre] = tipo

malas, total = [], 0
for m in re.finditer(r"^(\w+)\s+[-.<>]+\s+(\w+)", texto, flags=re.M):
    a, b = m.groups()
    if a not in tipos or b not in tipos:
        continue
    total += 1
    par = frozenset((tipos[a], tipos[b]))
    if par not in PERMITIDAS:
        malas.append(f"{a} ({tipos[a]}) — {b} ({tipos[b]})")

print(f"{len(tipos)} elementos, {total} conexiones, {len(malas)} prohibidas")
for x in malas:
    print("  PROHIBIDA:", x)
sys.exit(1 if malas else 0)
