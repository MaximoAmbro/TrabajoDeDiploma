"""Verifica los QR de los tickets PDF de EventAura sin usar un celular.

Para cada PDF de la carpeta indicada:
  - decodifica el QR de la imagen,
  - comprueba que haya exactamente uno y que coincida con el código impreso en el texto,
  - comprueba que ningún código se repita entre tickets.

No reemplaza el escaneo con cámara (pruebas 7.10 y 10.5 de docs/PRUEBAS-PENDIENTES.md):
sirve para revisar el contenido antes de esa prueba.

Requisitos (una sola vez):  py -m pip install pymupdf numpy zxing-cpp
Uso:  py -I tools/VerificarQrTickets.py "<carpeta de tickets>" [--salida TestResults/qr-tickets.json]
"""
import argparse
import json
import re
import sys
from pathlib import Path

try:
    import numpy as np
    import pymupdf
    import zxingcpp
except ImportError:
    sys.exit("Faltan dependencias. Instalá con: py -m pip install pymupdf numpy zxing-cpp")


def verificar(carpeta: Path) -> list:
    filas = []
    pdfs = sorted(p for p in carpeta.iterdir() if p.suffix.lower() == ".pdf")
    if not pdfs:
        sys.exit(f"No hay PDF en {carpeta}")
    for archivo in pdfs:
        codigos = []
        with pymupdf.open(archivo) as doc:
            texto = "".join(pagina.get_text() for pagina in doc)
            for pagina in doc:
                pix = pagina.get_pixmap(matrix=pymupdf.Matrix(3, 3), alpha=False)
                imagen = np.frombuffer(pix.samples, dtype=np.uint8).reshape(pix.height, pix.width, pix.n)
                codigos.extend(c.text for c in zxingcpp.read_barcodes(imagen))
        impresos = re.findall(r"EA-[A-F0-9]{32}", texto)
        ok = len(codigos) == 1 and codigos == impresos
        filas.append({"archivo": archivo.name, "qr": codigos, "impreso": impresos, "coincide": ok})
    return filas


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("carpeta", type=Path, help="Carpeta donde la aplicación guardó los tickets PDF")
    parser.add_argument("--salida", type=Path, help="Archivo JSON con el detalle (usar TestResults/, que no va a git)")
    args = parser.parse_args()

    filas = verificar(args.carpeta)
    todos = [c for f in filas for c in f["qr"]]
    repetidos = len(todos) != len(set(todos))

    for f in filas:
        print(("OK   " if f["coincide"] else "FALLA") + f"  {f['archivo']}  {', '.join(f['qr']) or '(sin QR)'}")
    if repetidos:
        print("FALLA  hay códigos QR repetidos entre tickets")
    if args.salida:
        args.salida.parent.mkdir(parents=True, exist_ok=True)
        args.salida.write_text(json.dumps(filas, ensure_ascii=False, indent=2), encoding="utf-8")

    return 0 if all(f["coincide"] for f in filas) and not repetidos else 1


if __name__ == "__main__":
    sys.exit(main())
