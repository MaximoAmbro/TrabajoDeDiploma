using System;
using System.Linq;
using Datos;
using Entidades;
using Negocio;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Pruebas {
 [TestClass] public class PruebasGuardasMutacion {
  private Cliente a,b; private Evento e;
  [TestInitialize] public void Preparar(){GestorSesion.Instance.Cerrar();Fake.Limpiar();a=Fake.Cliente(NivelFidelidad.Bronce);b=Fake.Cliente(NivelFidelidad.Bronce);e=Fake.EventoPublicado(1000m,5);}
  [TestCleanup] public void Limpiar(){GestorSesion.Instance.Cerrar();Fake.Limpiar();}
  private void Rechaza(Func<object> llamada,string regla,Cliente actor=null){
   actor=actor??b;
   foreach(bool sinSesion in new[]{false,true}){
    if(sinSesion)GestorSesion.Instance.Cerrar();else Fake.ComoCliente(actor);
    object dato=null;var error=Assert.ThrowsException<ReglaNegocioException>(()=>dato=llamada());
    Assert.AreEqual(regla,error.Regla);Assert.IsNull(dato,"No debe devolverse el dato protegido");
    if(sinSesion)Assert.IsNull(GestorSesion.Instance.UsuarioActual);else Assert.AreEqual(actor.Id,GestorSesion.Instance.UsuarioActual.Id);
   }
  }
  private Compra Compra(bool seguro=true){return Fake.CompraConfirmada(a,e,1,seguro);}
  private Entrada EntradaDe(Compra c){return RepositorioEntradas.Instance.ObtenerPorCompra(c.Id).Single();}
  private Entrada ParaIngreso(){var entrada=EntradaDe(Compra());Fake.CambiarFechaDeEvento(e.Id,DateTime.Now.AddMinutes(10));Fake.ComoOrganizador();return entrada;}
  // CP-126 — RF-10, RN-34: nombres y correos de la espera no se exponen a clientes ni anónimos.
  [TestMethod("CP-126: DeEvento exige administrador antes de devolver datos personales")]
  public void CP126_DeEvento(){
   e=Fake.EventoPublicado(1000m,1);Fake.CompraConfirmada(a,e,1,false);Fake.ComoCliente(b);GestorListaEspera.Instance.Inscribir(e.Id,b.Id);
   Rechaza(()=>GestorListaEspera.Instance.DeEvento(e.Id),"RN-48",a);Fake.ComoAdministrador();var filas=GestorListaEspera.Instance.DeEvento(e.Id);Assert.AreEqual(1,filas.Count);Assert.AreEqual(b.Id,filas.Single().IdUsuario);
  }
  // CP-127 — RF-06, RN-41: pagos privados del titular.
  [TestMethod("CP-127: PagosDe rechaza ajenos y ausencia de sesión")]
  public void CP127_Pagos(){var c=Compra();Fake.ComoCliente(a);Assert.AreEqual(1,GestorCompras.Instance.PagosDe(c).Count);Rechaza(()=>GestorCompras.Instance.PagosDe(c),"RN-41");Assert.AreEqual(1,RepositorioPagos.Instance.ObtenerPorCompra(c.Id).Count);}
  // CP-128 — RF-07, RN-41: compra privada del titular.
  [TestMethod("CP-128: ObtenerPorId rechaza la compra de otro")]
  public void CP128_Compra(){var c=Compra();Rechaza(()=>GestorCompras.Instance.ObtenerPorId(c.Id),"RN-41");Fake.ComoCliente(a);Assert.AreEqual(c.Id,GestorCompras.Instance.ObtenerPorId(c.Id).Id);Assert.AreEqual(EstadoCompra.Confirmada,RepositorioCompras.Instance.ObtenerPorId(c.Id).Estado);}
  // CP-129 — RF-06, RN-41: pólizas privadas del titular.
  [TestMethod("CP-129: SegurosDe rechaza pólizas ajenas")]
  public void CP129_Seguros(){var c=Compra();Fake.ComoCliente(a);Assert.AreEqual(1,GestorCompras.Instance.SegurosDe(c).Count);Rechaza(()=>GestorCompras.Instance.SegurosDe(c),"RN-41");Assert.AreEqual(1,RepositorioCompras.Instance.ObtenerSeguros(c.Id).Count);}
  // CP-130 — RF-02, RN-04: el hash almacenado nunca es credencial.
  [TestMethod("CP-130: Un hash robado no autentica ni se migra como contraseña")]
  public void CP130_Hash(){GestorUsuarios.Instance.Autenticar(a.Mail,"Cliente123");string hash=RepositorioUsuarios.Instance.ObtenerPorMail(a.Mail).Contraseña;GestorSesion.Instance.Cerrar();var h=Assert.ThrowsException<ReglaNegocioException>(()=>GestorUsuarios.Instance.Autenticar(a.Mail,hash));var otra=Assert.ThrowsException<ReglaNegocioException>(()=>GestorUsuarios.Instance.Autenticar(a.Mail,"OtraClaveIncorrecta1"));Assert.AreEqual(otra.Regla,h.Regla);Assert.IsNull(GestorSesion.Instance.UsuarioActual);Assert.AreEqual(hash,RepositorioUsuarios.Instance.ObtenerPorMail(a.Mail).Contraseña);}
  // CP-131 — RF-07, RN-18: suspender invalida el ingreso aunque esté dentro de horario.
  [TestMethod("CP-131: Evento suspendido no registra ingreso")]
  public void CP131_Suspendido(){var entrada=ParaIngreso();Fake.CambiarEstadoDeEvento(e.Id,GestorEventos.EstadoSuspendido);Assert.AreEqual(MotivoIngreso.FueraDeHorario,GestorIngresos.Instance.Registrar(entrada.CodigoQR,e.Id).Motivo);Assert.IsNull(RepositorioEntradas.Instance.PorCodigo(entrada.CodigoQR).FechaIngreso);Assert.AreEqual(0,GestorIngresos.Instance.Contar(e.Id));}
  // CP-132 — RF-07, RN-18: defensa SQL por evento, sin el filtro previo del gestor.
  [TestMethod("CP-132: UPDATE de ingreso rechaza evento distinto")]
  public void CP132_EventoSql(){var entrada=ParaIngreso();var otro=Fake.EventoPublicado(1000m,5);Assert.IsFalse(RepositorioEntradas.Instance.RegistrarIngreso(entrada.CodigoQR,otro.Id,DateTime.Now));Assert.IsNull(RepositorioEntradas.Instance.PorCodigo(entrada.CodigoQR).FechaIngreso);}
  // CP-133 — RF-07, RN-18/29: SQL no registra una entrada devuelta entre lectura y escritura.
  [TestMethod("CP-133: UPDATE de ingreso exige estado confirmado")]
  public void CP133_ConfirmadaSql(){var c=Compra();var entrada=EntradaDe(c);GestorCompras.Instance.Reembolsar(GestorCompras.Instance.EntradasDe(c));Assert.IsFalse(RepositorioEntradas.Instance.RegistrarIngreso(entrada.CodigoQR,e.Id,DateTime.Now));var guardada=RepositorioEntradas.Instance.PorCodigo(entrada.CodigoQR);Assert.IsNull(guardada.FechaIngreso);Assert.AreEqual(EstadoEntrada.Reembolsada,guardada.Estado);}
  // CP-134 — RF-07, RN-18: distingue rechazo previo del fallback del UPDATE.
  [TestMethod("CP-134: Pendiente y vencida tienen rechazo específico sin ingresar")]
  public void CP134_Estado(){var entrada=ParaIngreso();foreach(string estado in new[]{"Pendiente","Vencida"}){Fake.CambiarEstadoDeEntrada(entrada.Id,estado);var r=GestorIngresos.Instance.Registrar(entrada.CodigoQR,e.Id);Assert.AreEqual(MotivoIngreso.NoConfirmado,r.Motivo);Assert.AreEqual("La entrada está pendiente o vencida.",r.Mensaje);Assert.IsNull(RepositorioEntradas.Instance.PorCodigo(entrada.CodigoQR).FechaIngreso);}}
  // CP-135 — RF-07, RN-18: reloj inyectado, datetime releído y ambos bordes inclusivos.
  [TestMethod("CP-135: Los dos bordes de ingreso están incluidos y los segundos externos no")]
  public void CP135_Bordes(){var c=Fake.CompraConfirmada(a,e,2,false);var entradas=RepositorioEntradas.Instance.ObtenerPorCompra(c.Id);var fecha=RepositorioEventos.Instance.ObtenerPorId(e.Id).Fecha;int antes=GestorParametros.Instance.ObtenerEntero("INGRESO_HORAS_ANTES"),despues=GestorParametros.Instance.ObtenerEntero("INGRESO_HORAS_DESPUES");Fake.ComoOrganizador();Assert.AreEqual(MotivoIngreso.FueraDeHorario,GestorIngresos.Instance.Registrar(entradas[0].CodigoQR,e.Id,fecha.AddHours(-antes).AddSeconds(-1)).Motivo);Assert.AreEqual(MotivoIngreso.FueraDeHorario,GestorIngresos.Instance.Registrar(entradas[1].CodigoQR,e.Id,fecha.AddHours(despues).AddSeconds(1)).Motivo);Assert.IsTrue(entradas.All(x=>!RepositorioEntradas.Instance.PorCodigo(x.CodigoQR).FechaIngreso.HasValue));Assert.IsTrue(GestorIngresos.Instance.Registrar(entradas[0].CodigoQR,e.Id,fecha.AddHours(-antes)).Valido);Assert.IsTrue(GestorIngresos.Instance.Registrar(entradas[1].CodigoQR,e.Id,fecha.AddHours(despues)).Valido);}
  // CP-136 — RF-07, RN-18: los espacios del lector no alteran el código.
  [TestMethod("CP-136: Código con espacios se normaliza y persiste ingreso")]
  public void CP136_Espacios(){var entrada=ParaIngreso();Assert.IsTrue(GestorIngresos.Instance.Registrar("  "+entrada.CodigoQR+"  ",e.Id).Valido);Assert.IsNotNull(RepositorioEntradas.Instance.PorCodigo(entrada.CodigoQR).FechaIngreso);}
  // CP-137 — RF-10, RN-41: inscripción privada.
  [TestMethod("CP-137: EstaInscripto exige titular")]
  public void CP137_Inscripto(){Rechaza(()=>GestorListaEspera.Instance.EstaInscripto(e.Id,a.Id),"RN-41");Fake.ComoCliente(a);Assert.IsFalse(GestorListaEspera.Instance.EstaInscripto(e.Id,a.Id));}
  // CP-138 — RF-08, RN-41: conteo privado de fidelización.
  [TestMethod("CP-138: ComprasEnVentana exige titular")]
  public void CP138_Compras(){Rechaza(()=>GestorFidelizacion.Instance.ComprasEnVentana(a.Id,DateTime.Now),"RN-41");Fake.ComoCliente(a);Assert.AreEqual(0,GestorFidelizacion.Instance.ComprasEnVentana(a.Id,DateTime.Now));}
  // CP-139 — RF-08, RN-41: resumen privado.
  [TestMethod("CP-139: CalcularDescuento exige titular")]
  public void CP139_Descuento(){Fake.ComoCliente(a);var p=Fake.Paquete(e,1,false);Rechaza(()=>GestorFidelizacion.Instance.CalcularDescuento(a,p),"RN-41");Fake.ComoCliente(a);Assert.IsNotNull(GestorFidelizacion.Instance.CalcularDescuento(a,p));}
  // CP-140 — RF-05, RN-41: no armar un paquete para otro.
  [TestMethod("CP-140: Nuevo paquete exige titular")]
  public void CP140_Paquete(){Rechaza(()=>GestorPaquetes.Instance.Nuevo(e.Id,a.Id),"RN-41");Fake.ComoCliente(a);Assert.AreEqual(e.Id,GestorPaquetes.Instance.Nuevo(e.Id,a.Id).IdEvento);}
  // CP-141 — RF-12, RN-34: consulta operativa exclusiva del administrador.
  [TestMethod("CP-141: PorPropietario exige administrador")]
  public void CP141_Locales(){var organizador=Fake.Organizador();int id=organizador.Id;Rechaza(()=>GestorLocales.Instance.PorPropietario(id),"RN-48");Fake.ComoOrganizador();Assert.IsTrue(GestorLocales.Instance.PorPropietario(id).Count>0);}
  // CP-142 — RF-07, RN-34: listado de ingreso administrativo.
  [TestMethod("CP-142: EventosConIngresoAbierto exige administrador")]
  public void CP142_EventosIngreso(){var organizador=Fake.Organizador();Rechaza(()=>GestorIngresos.Instance.EventosConIngresoAbierto(),"RN-48");Fake.ComoOrganizador(organizador);Assert.IsNotNull(GestorIngresos.Instance.EventosConIngresoAbierto());}
  // CP-143 — RF-11, RN-34: ejecutar reporte ya creado requiere autenticar de nuevo.
  [TestMethod("CP-143: Ejecutar reporte existente exige administrador")]
  public void CP143_Reporte(){Fake.ComoAdministrador();var r=GestorReportes.Instance.Crear(GestorReportes.Codigos[0],DateTime.Today.AddDays(-30),DateTime.Today);Rechaza(()=>GestorReportes.Instance.Ejecutar(r),"RN-34");Fake.ComoAdministrador();Assert.IsNotNull(GestorReportes.Instance.Ejecutar(r));}
  // CP-144 — RF-11, RN-34: catálogo de eventos del reporte administrativo.
  [TestMethod("CP-144: EventosEntre exige administrador")]
  public void CP144_EventosReporte(){Rechaza(()=>GestorReportes.Instance.EventosEntre(DateTime.Today.AddDays(-30),DateTime.Today.AddDays(30)),"RN-34");Fake.ComoAdministrador();Assert.IsNotNull(GestorReportes.Instance.EventosEntre(DateTime.Today.AddDays(-30),DateTime.Today.AddDays(30)));}
  // CP-145 — RF-07, RN-18: índice único real y rollback sin corromper códigos.
  [TestMethod("CP-145: El índice QR rechaza duplicados y rollback conserva la entrada")]
  public void CP145_QrUnico(){var c=Fake.CompraConfirmada(a,e,2,false);var entradas=RepositorioEntradas.Instance.ObtenerPorCompra(c.Id);using(var cn=new SqlConnection(BaseDePrueba.CadenaDePrueba())){cn.Open();using(var tx=cn.BeginTransaction()){using(var cmd=cn.CreateCommand()){cmd.Transaction=tx;cmd.CommandText="UPDATE Entradas SET CodigoQR=@Codigo WHERE ID=@ID";cmd.Parameters.AddWithValue("@Codigo",entradas[0].CodigoQR);cmd.Parameters.AddWithValue("@ID",entradas[1].Id);var error=Assert.ThrowsException<SqlException>(()=>cmd.ExecuteNonQuery());Assert.AreEqual(2601,error.Number);}tx.Rollback();}}Assert.AreEqual(entradas[1].CodigoQR,RepositorioEntradas.Instance.ObtenerPorCompra(c.Id).Single(x=>x.Id==entradas[1].Id).CodigoQR);}
 }
}
