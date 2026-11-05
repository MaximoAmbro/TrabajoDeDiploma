# -*- coding: utf-8 -*-
import os, sys
_AQUI = os.path.dirname(os.path.abspath(__file__))
_RAIZ = os.path.dirname(_AQUI)
_SAL  = os.path.join(_AQUI, "salida")
os.makedirs(_SAL, exist_ok=True)
sys.path.insert(0, _RAIZ)
sys.path.insert(0, _AQUI)
from ea_style import Doc
import doc1, doc2, doc3

SAL = os.path.join(_SAL, "EventAura_Parcial2.pdf")
d = Doc(SAL)
doc1.construir(d)
doc2.construir(d)
doc3.construir(d)
d.save()
print("generado:", SAL)
