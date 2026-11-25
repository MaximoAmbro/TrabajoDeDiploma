"""Solo comprobaciones locales de las correcciones finales; no reestima la medición global 73/77."""
from pathlib import Path
import subprocess,json,re,sys
root=Path(__file__).resolve().parent.parent;out=root/'TestResults/Ultimo16/mutacion';out.mkdir(parents=True,exist_ok=True)
vs=Path(subprocess.check_output(['C:/Program Files (x86)/Microsoft Visual Studio/Installer/vswhere.exe','-latest','-products','*','-requires','Microsoft.Component.MSBuild','-property','installationPath'],text=True).strip());ms=vs/'MSBuild/Current/Bin/MSBuild.exe';vt=vs/'Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe'
def run(args,name):
 with (out/name).open('w',encoding='utf-8') as f:return subprocess.run([str(x) for x in args],cwd=root,stdout=f,stderr=subprocess.STDOUT).returncode

def build(nombre):
 if run([ms,'SistemaEntradas.sln','-restore','-p:Configuration=Debug','-v:minimal','-nologo'],nombre+'-build.log'):raise RuntimeError('No compiló '+nombre)
def test(cp,nombre):
 codigo=run([vt,'Pruebas/bin/Debug/net48/Pruebas.dll','/TestCaseFilter:FullyQualifiedName~CP'+str(cp)+'_','/Logger:console;verbosity=normal'],nombre+'.log')
 texto=(out/(nombre+'.log')).read_text(encoding='utf-8',errors='replace')
 if codigo and 'Assert.' not in texto:raise RuntimeError('Revisar fallo no asertivo de '+nombre)
 if 'No test matches' in texto or 'No hay ninguna prueba' in texto:raise RuntimeError('Filtro vacío')
 return codigo
seleccion=sys.argv[1].split(',') if len(sys.argv)>1 else ['seguros','u09']
resultado={}
if (out/'resultados.json').exists():resultado=json.loads((out/'resultados.json').read_text(encoding='utf-8'))
for caso in seleccion:
 cambios={};cp={'seguros':147,'u09':148,'m22':146}[caso]
 if caso=='seguros':
  p=root/'BaseDatos/01-esquema.sql';s=p.read_bytes().decode('utf-8-sig');s,n=re.subn(r'CREATE UNIQUE INDEX UX_Seguros_ID_Entrada ON Seguros \(ID_Entrada\);[^\r\n]*','-- índice retirado solo para comprobar CP-147',s);assert n==1;cambios[p]=s
  p=root/'BaseDatos/07-indice-unico-seguros.sql';s=p.read_bytes().decode('utf-8-sig');inicio=s.index('IF NOT EXISTS');fin=s.index('COMMIT;',inicio);s=s[:inicio]+'-- creación retirada solo para CP-147\r\n'+s[fin:];cambios[p]=s
 elif caso=='u09':
  p=root/'Visual/Cliente/FrmCompraCliente.cs';cambios[p]=subprocess.check_output(['git','show','65225e7:Visual/Cliente/FrmCompraCliente.cs'],cwd=root).decode('utf-8-sig')
 else:
  p=root/'Repo/GestorIngresos.cs';s=p.read_bytes().decode('utf-8-sig');pos=s.index('internal ResultadoIngreso Registrar(');a=s.index('GestorSesion.Instance.RequerirAdministrador();',pos);s=s[:a]+s[a:].replace('GestorSesion.Instance.RequerirAdministrador();','/* M-22: defensa retirada */',1);cambios[p]=s
 originales={p:p.read_bytes() for p in cambios}
 try:
  for p,s in cambios.items():p.write_bytes((b'\xef\xbb\xbf' if originales[p].startswith(b'\xef\xbb\xbf') else b'')+s.replace('\r\n','\n').replace('\n','\r\n').encode('utf-8'))
  build(caso+'-rojo');rojo=test(cp,caso+'-rojo')
 finally:
  for p,b in originales.items():p.write_bytes(b)
 build(caso+'-verde');verde=test(cp,caso+'-verde');resultado[caso]={'cp':cp,'rojo':rojo!=0,'verde':verde==0};(out/'resultados.json').write_text(json.dumps(resultado,ensure_ascii=False,indent=2),encoding='utf-8');print(caso,resultado[caso],flush=True)
 if rojo==0 or verde!=0:raise RuntimeError('No se verificó el par rojo/verde de '+caso)
