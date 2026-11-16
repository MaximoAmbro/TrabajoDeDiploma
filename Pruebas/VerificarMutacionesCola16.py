"""Verificación P-1: una mutación por vez, bytes restaurados en finally, rojo y verde por caso.
Ejecutar desde la raíz: python Pruebas/VerificarMutacionesCola16.py
No tocar fuentes en paralelo durante esta ejecución. Solo utiliza la base _Pruebas del fixture.
"""
from pathlib import Path
import re, subprocess, json, datetime, os
ROOT=Path(__file__).resolve().parent.parent
OUT=ROOT/'TestResults/Cola16/mutacion';OUT.mkdir(parents=True,exist_ok=True)
vswhere=Path('C:/Program Files (x86)/Microsoft Visual Studio/Installer/vswhere.exe')
vs=Path(subprocess.check_output([str(vswhere),'-latest','-products','*','-requires','Microsoft.Component.MSBuild','-property','installationPath'],text=True).strip())
msbuild=vs/'MSBuild/Current/Bin/MSBuild.exe'; vstest=vs/'Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe'
def method(s,name,old,new):
 matches=list(re.finditer(r'(?:public|internal|private)\s+(?:static\s+)?[\w<>]+\s+'+name+r'\s*\([^)]*\)\s*\{',s))
 for m in matches:
  i=m.end();depth=1
  while depth:
   if s[i]=='{':depth+=1
   if s[i]=='}':depth-=1
   i+=1
  body=s[m.start():i]
  if old in body:
   assert body.count(old)==1,(name,old)
   return s[:m.start()]+body.replace(old,new)+s[i:]
 raise AssertionError((name,old))
def replace(s,old,new):
 assert s.count(old)==1,(old,s.count(old));return s.replace(old,new)
guardA='GestorSesion.Instance.RequerirAdministrador();'
guardU=lambda arg:'GestorSesion.Instance.RequerirUsuario('+arg+');'
cases=[]
def add(task,cp,path,name,old,new='/* defensa retirada para verificar P-1 */',mutante=None):
 cases.append((str(task),str(cp),path,lambda s:method(s,name,old,new),mutante or str(task)))
add(1,126,'Repo/GestorListaEspera.cs','DeEvento',guardA)
add(2,127,'Repo/GestorCompras.cs','PagosDe','RequerirCompra(c == null ? 0 : c.Id);')
add(3,128,'Repo/GestorCompras.cs','ObtenerPorId','return RequerirCompra(idCompra);','return RepositorioCompras.Instance.ObtenerPorId(idCompra);')
add(4,129,'Repo/GestorCompras.cs','SegurosDe','RequerirCompra(c == null ? 0 : c.Id);')
add(5,130,'Repo/GestorUsuarios.cs','ContrasenaValida','!Seguridad.EsHash(usuario.Contraseña) && ','')
add(6,131,'Repo/GestorIngresos.cs','Registrar','evento.Estado != GestorEventos.EstadoPublicado ||','false ||')
add(7,132,'Datos/RepositorioEntradas.cs','RegistrarIngreso','WHERE CodigoQR = @Codigo AND ID_Evento = @IdEvento','WHERE CodigoQR = @Codigo')
add(8,133,'Datos/RepositorioEntradas.cs','RegistrarIngreso',"AND Estado = 'Confirmada' AND FechaIngreso IS NULL","AND FechaIngreso IS NULL")
add(9,134,'Repo/GestorIngresos.cs','RechazoDe','if (entrada.Estado != EstadoEntrada.Confirmada) return Resultado(MotivoIngreso.NoConfirmado, "La entrada está pendiente o vencida.");')
add(10,135,'Repo/GestorIngresos.cs','Registrar','ahora < evento.Fecha.AddHours(-antes) || ahora > evento.Fecha.AddHours(despues)','ahora <= evento.Fecha.AddHours(-antes) || ahora >= evento.Fecha.AddHours(despues)')
add(11,136,'Repo/GestorIngresos.cs','Registrar','codigo = (codigo ?? "").Trim();','codigo = (codigo ?? "");')
add(12,94,'Repo/GestorEventos.cs','EliminarSector',guardA,mutante='12a')
add(12,94,'Repo/GestorListaEspera.cs','Inscribir',guardU('idUsuario'),mutante='12b')
add(13,137,'Repo/GestorListaEspera.cs','EstaInscripto',guardU('idUsuario'))
add(14,138,'Repo/GestorFidelizacion.cs','ComprasEnVentana',guardU('idUsuario'))
add(15,139,'Repo/GestorFidelizacion.cs','CalcularDescuento',guardU('c == null ? 0 : c.Id'))
add(16,140,'Repo/GestorPaquetes.cs','Nuevo',guardU('idUsuario'))
add(17,141,'Repo/GestorLocales.cs','PorPropietario',guardA)
add(18,142,'Repo/GestorIngresos.cs','EventosConIngresoAbierto',guardA)
add(19,143,'Repo/GestorReportes.cs','Ejecutar','VerificarAdministrador();')
add(20,144,'Repo/GestorReportes.cs','EventosEntre','VerificarAdministrador();')
cases.append(('21','145','BaseDatos/01-esquema.sql',lambda s:re.sub(r'CREATE UNIQUE INDEX UX_Entradas_CodigoQR[^\n]*\n\s*ON Entradas \(CodigoQR\) WHERE CodigoQR IS NOT NULL;', '-- Índice retirado solo para P-1',s),'21'))
# También demostrar que las pruebas de prioridad detectan suprimir la defensa SQL.
add('prioridad',125,'Datos/RepositorioSectores.cs','Reservar','if (Convert.ToInt32(comando.ExecuteScalar()) != 0) throw new CupoPrioritarioException();','if (Convert.ToInt32(comando.ExecuteScalar()) != 0) { /* sin defensa */ }')
def run(args,log):
 with log.open('w',encoding='utf-8') as f:return subprocess.run([str(x) for x in args],cwd=ROOT,stdout=f,stderr=subprocess.STDOUT).returncode
def build(tag):
 code=run([msbuild,'SistemaEntradas.sln','-restore','-p:Configuration=Debug','-v:minimal','-nologo'],OUT/(tag+'-build.log'))
 if code:raise RuntimeError('No compiló '+tag)
def test(cp,tag):
 filtro='FullyQualifiedName~SinSesion' if cp=='94' else 'FullyQualifiedName~CP'+cp+'_'
 code=run([vstest,'Pruebas/bin/Debug/net48/Pruebas.dll','/TestCaseFilter:'+filtro,'/Logger:console;verbosity=normal'],OUT/(tag+'.log'))
 output=(OUT/(tag+'.log')).read_text(encoding='utf-8',errors='replace')
 # Un error del runner o de AssemblyInitialize nunca cuenta como mutante detectado.
 if 'No test matches' in output or 'No hay ninguna prueba' in output or 'Assembly Initialization' in output or 'AssemblyInitialize' in output:raise RuntimeError('Ejecución inválida '+tag)
 if code and ('Assert.' not in output and 'AssertFailedException' not in output):raise RuntimeError('Fallo no asertivo: revisar '+tag)
 return code
seleccion=os.environ.get('COLA16_MUTANTES')
if seleccion:
 cases=[c for c in cases if c[4] in seleccion.split(',')]
 OUT=OUT/'recontrol';OUT.mkdir(exist_ok=True)
result=[]
try:
 for task,cp,path,mutation,key in cases:
  target=ROOT/path;original=target.read_bytes();text=original.decode('utf-8-sig').replace('\r\n','\n');tag='tarea-'+key
  changed=mutation(text);assert changed!=text
  try:
   target.write_bytes((b'\xef\xbb\xbf' if original.startswith(b'\xef\xbb\xbf') else b'')+changed.replace('\n','\r\n').encode('utf-8'))
   build(tag+'-rojo');red=test(cp,tag+'-rojo')
  finally:target.write_bytes(original)
  build(tag+'-verde');green=test(cp,tag+'-verde')
  row=dict(tarea=task,cp=cp,mutante=key,detectado=red!=0,restauradoVerde=green==0,archivo=path)
  result.append(row);(OUT/'resultados.json').write_text(json.dumps(result,ensure_ascii=False,indent=2),encoding='utf-8')
  print(json.dumps(row,ensure_ascii=False),flush=True)
  if green!=0:raise RuntimeError('La fuente restaurada no pasa: '+tag)
finally:
 (OUT/'resultados.json').write_text(json.dumps(result,ensure_ascii=False,indent=2),encoding='utf-8')
print('FIN: '+str(len(result))+' mutantes, detectados '+str(sum(x['detectado'] for x in result)),flush=True)
