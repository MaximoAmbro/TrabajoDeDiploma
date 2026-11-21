using System;
using System.Linq;
using System.Windows.Forms;
using Entidades;
using Negocio;
using Datos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Pruebas.InterfazGrupoA;
namespace Pruebas {
 [TestClass] public class PruebasK7K8 {
        // CP-123 — RF-07, RN-18/RN-48/RN-49: ventana temporal y pertenencia de los eventos de ingreso.
  [DataTestMethod]
  [DataRow("ajeno",0,false,DisplayName="CP-123: Administrador no lista, registra ni cuenta eventos ajenos")]
  [DataRow("sin-sesion",0,false,DisplayName="CP-123: Sin sesión no se listan eventos para ingreso")]
  [DataRow("borde",-1,false,DisplayName="CP-123: Evento de 00:30 no aparece a las 18:29:59 anteriores")]
  [DataRow("borde",0,true,DisplayName="CP-123: Evento de 00:30 aparece a las 18:30 anteriores")]
  [DataRow("borde",64800,true,DisplayName="CP-123: Evento aparece hasta el cierre exacto de su ventana")]
  [DataRow("borde",64801,false,DisplayName="CP-123: Evento desaparece después del cierre de su ventana")]
  public void CP123_VentanaPrivada(string caso,int segundos,bool aparece) {
   EnSta(()=> {
    var evento=Fake.EventoPublicado(1000m,5);Fake.CambiarFechaDeEvento(evento.Id,DateTime.Now.AddMinutes(10));Fake.ComoOrganizador();
    if(caso=="ajeno") {
     Assert.IsTrue(GestorIngresos.Instance.EventosConIngresoAbierto().Any(e=>e.Id==evento.Id));
     var otro=Fake.PropietarioSinLocales();GestorSesion.Instance.Iniciar(otro);
     Assert.IsFalse(GestorIngresos.Instance.EventosConIngresoAbierto().Any(e=>e.Id==evento.Id));
     Assert.AreEqual("RN-49",Assert.ThrowsException<ReglaNegocioException>(()=>GestorIngresos.Instance.Registrar("inexistente",evento.Id)).Regla);
     Assert.ThrowsException<ReglaNegocioException>(()=>GestorIngresos.Instance.Contar(evento.Id));return;
    }
    if(caso=="sin-sesion") {GestorSesion.Instance.Cerrar();Assert.ThrowsException<ReglaNegocioException>(()=>GestorIngresos.Instance.EventosConIngresoAbierto());return;}
    DateTime fecha=DateTime.Today.AddDays(1).AddMinutes(30);Fake.CambiarFechaDeEvento(evento.Id,fecha);
    DateTime ahora=fecha.AddHours(-6).AddSeconds(segundos);
    Assert.AreEqual(aparece,GestorIngresos.Instance.EventosConIngresoAbierto(ahora).Any(e=>e.Id==evento.Id));
   });
  }
        // CP-124 — RF-02, RN-04/RN-40: unificar el rechazo de cambio de contraseña sin enumerar cuentas.
  [DataTestMethod]
  [DataRow(0,DisplayName="CP-124: Cambio sin sesión rechaza igual cuenta existente e inexistente")]
  [DataRow(4,DisplayName="CP-124: Quinto fallo al cambiar no revela bloqueo")]
  [DataRow(5,DisplayName="CP-124: Cuenta bloqueada y correo inexistente muestran lo mismo")]
  public void CP124_CambioUniforme(int fallos) {
   EnSta(()=> {
    var cliente=Fake.Cliente(NivelFidelidad.Bronce);RepositorioUsuarios.Instance.ActualizarIntentos(cliente.Id,fallos,fallos==5?DateTime.Now.AddMinutes(15):(DateTime?)null);
    GestorSesion.Instance.Cerrar();Assert.IsFalse(GestorSesion.Instance.HaySesion);
    using(var form=new Visual.frmCambiarContraseña()) {
     Mostrar(form);var respuestas=new System.Collections.Generic.List<DialogoEsperado>();
     foreach(string mail in new[]{cliente.Mail,Fake.MailNuevo()}) {
      ControlDe<TextBox>(form,"txtMail").Text=mail;ControlDe<TextBox>(form,"txtActual").Text="Incorrecta1";
      ControlDe<TextBox>(form,"TxtPassword").Text="NuevaClave123";ControlDe<TextBox>(form,"TxtConfirmpassword").Text="NuevaClave123";
      var aviso=new DialogoEsperado{Titulo="Cambiar contraseña",Texto="Mail o contraseña actual incorrectos."};
      DialogosPrueba.Comprobar(()=>ControlDe<Button>(form,"btnconfirmar").PerformClick(),aviso);respuestas.Add(aviso);
      Assert.IsTrue(form.Visible);Assert.IsFalse(GestorSesion.Instance.HaySesion);
     }
     Assert.AreEqual(respuestas[0].TextoObservado,respuestas[1].TextoObservado);Assert.AreEqual(respuestas[0].TituloObservado,respuestas[1].TituloObservado);
    }
   });
  }
 }
}
