# Diagramas para el documento — instrucciones para Máximo

Lo preparó Claude Code el 12/11/2026, a pedido de JC. Cada diagrama sale del código y de la
base actuales, no de los diagramas viejos. Por cada uno hay cinco cosas: el fuente, la imagen,
la tabla de justificación de relaciones, el texto para el documento y dónde va.

## Antes de empezar: tres reglas para insertar las imágenes

1. **Usá el PNG, no el SVG.** Están en `docs/diagramas/`, a 300 dpi.
2. **Insertalo con el ancho exacto que dice cada ficha y la proporción bloqueada.**
   - Ese ancho deja la letra más chica del diagrama en 8,5 pt o más, que es lo que hace falta para
     leerlo impreso al 100 %.
   - **Si lo achicás, deja de leerse.** Los diagramas viejos se veían a 3,4–5,5 pt.
   - En Word: clic derecho en la imagen › *Tamaño y posición* › *Bloquear relación de aspecto* ›
     ancho.
3. **Una figura por página**, salvo donde la ficha diga otra cosa. Las que miden 23 cm de alto
   ocupan la página entera: el título va arriba, en la misma página.

El texto de cada ficha («Texto para el documento») está escrito para pegarlo tal cual: castellano
formal, tercera persona.

Si hay que cambiar un diagrama, se cambia el `.puml` y se vuelve a generar la imagen. Cómo hacerlo
está al final, en «Para regenerar una imagen».

## Qué se reemplaza en el PDF (`Documentacion/EventAura_v2.pdf`, v2.4)

| Página hoy | Qué hay | Qué va | Ficha |
|---|---|---|---|
| 71 | «Diagramas de caso de uso»: el general viejo | Cuatro diagramas: cuenta, cliente, organizador y administrador | [1](#1-casos-de-uso) |
| 72 a 85 | Especificaciones CU-01 a CU-08, cada una con un diagrama chico | **Se quitan los ocho diagramas chicos** (D-7, 14/11): el general ya muestra todas sus relaciones | — |
| 86, arriba | «Modelo de dominio conceptual» | El dominio conceptual nuevo | [7](#7-modelo-de-dominio-conceptual) |
| 86, abajo | «Diagrama de clases» | Nueve vistas de clases más cuatro de patrones (13 figuras) | [2](#2-diagrama-de-clases) y [3](#3-patrones-de-diseño) |
| 87, arriba | «Diagrama de secuencia – Comprar paquete» | La secuencia en tres partes | [4](#4-secuencia-comprar-paquete) |
| 87, abajo | «Diagrama de estados – Ciclo de vida de la entrada» | El diagrama de estados nuevo | [5](#5-transición-de-estados-de-la-entrada) |
| 88, arriba | «Diagrama de robustez (ICONIX)» | La robustez de «Comprar paquete» | [9](#9-robustez-comprar-paquete) |
| 88, medio | «Diagrama de dominio actualizado» | Tres vistas del dominio actualizado | [8](#8-modelo-de-dominio-actualizado) |
| 88 abajo y 89 arriba | «Diagrama de Actividad (flujo del Cliente)» | La actividad del proceso de negocio en tres partes | [10](#10-actividad-del-proceso-de-negocio) |
| 89, abajo | «DER – Diagrama entidad-relación», **con la página en blanco** | Cuatro vistas del entidad-relación | [6](#6-entidad-relación) |

**Páginas nuevas.** Se agregan unas 20 páginas entre la 71 y la 89. Todo lo que sigue se corre, y
el índice (páginas 2 a 5) hay que regenerarlo.

---

## 1. Casos de uso

Cuatro diagramas, regenerados el 18/11 con los tres perfiles (RN-05).

| Diagrama | Fuente e imagen (`docs/diagramas/…`) | Insertar a | Letra impresa |
|---|---|---|---|
| Cuenta (común a los tres perfiles) | `casos-uso-cuenta` | **13,8 cm** de ancho (18,8 de alto) | 12,0 pt |
| Cliente | `casos-uso-cliente` | **16,0 cm** de ancho (11,5 de alto) | 9,0 pt |
| Organizador | `casos-uso-organizador` | **16,0 cm** de ancho (14,1 de alto) | 10,8 pt |
| Administrador | `casos-uso-administrador` | **16,0 cm** de ancho (11,6 de alto) | 10,4 pt |

El fuente de cada uno es `fuentes/<nombre>.puml` y la imagen, `<nombre>.png`.

**Dónde va:** página 71. Reemplaza la figura que está bajo el título «Diagramas de caso de uso».
Un diagrama por página, en este orden, cada uno con su subtítulo: «Cuenta», «Cliente»,
«Organizador» y «Administrador».

**Texto para el documento:**

> Los casos de uso se presentan en cuatro diagramas: uno con los casos comunes a todas las
> cuentas y uno por cada perfil, para que cada diagrama se lea a tamaño de página. El sistema
> distingue tres perfiles excluyentes (RN-05): el Cliente, que compra; el Organizador, que
> administra sus propios locales y eventos; y el Administrador de la plataforma, que no es dueño
> de locales y supervisa a los organizadores. Los tres son especializaciones de un Usuario
> registrado y heredan sus casos de uso: iniciar sesión, cambiar la contraseña y recuperarla. El
> Visitante, que todavía no tiene cuenta, solo puede registrarse; el registro incluye la
> verificación del correo, que se exige antes de la primera compra (RN-43).
>
> La mayoría de los casos de uso corresponde a una pantalla del sistema y a una operación de la
> capa de negocio. Otros ocurren dentro de la compra, sin pantalla propia: el descuento por nivel,
> que la compra aplica siempre, y el seguro, que el cliente elige mientras arma el paquete. Se
> dibujan para que esas reglas del negocio queden visibles y trazables en el diagrama. El pago se
> resuelve con una pasarela simulada que forma parte del sistema, por lo que no se modela como
> actor. Las relaciones «include» indican pasos que el caso de uso base realiza siempre; las
> «extend» indican comportamiento opcional, con su condición entre corchetes: contratar el
> seguro, inscribirse en la lista de espera cuando el evento está agotado, devolver una entrada
> asegurada con más de 48 horas de anticipación, agregar o quitar sectores de un evento y
> exportar un reporte.
>
> Un Cliente puede solicitar ser Organizador; la solicitud no le da permisos hasta que un
> Administrador la aprueba (RN-50 a RN-53). El Organizador publica sus eventos de forma
> explícita, y el primer evento de un Organizador nuevo pasa a revisión: un Administrador lo
> aprueba o lo rechaza con un motivo (RN-54). «Generar reportes» aparece en los diagramas de los
> dos perfiles de gestión porque lo usan ambos con distinto alcance: el Organizador ve sus
> eventos, y el Administrador, todos (RN-34).

**Justificación de relaciones:**

| Relación | Tipo | Condición | De dónde sale |
|---|---|---|---|
| Cliente, Organizador, Administrador → Usuario registrado | Generalización de actores | — | Un único perfil por cuenta (RN-05); `TipoUsuario` y las tablas `Cliente`, `Propietario` y `Administrador` |
| Visitante → Registrarse | Asociación | — | `frmCrearUsuario` → `GestorUsuarios.RegistrarCliente` |
| Registrarse → Verificar el correo | «include» | siempre | `GestorUsuarios.RegistrarCliente` llama a `GestorCodigosCorreo.EnviarVerificacion`; `frmCrearUsuario` → `ConfirmarVerificacion` (RN-43, RN-44) |
| Usuario registrado → Iniciar sesión | Asociación | — | `frmInicio` → `GestorUsuarios.Autenticar` |
| Usuario registrado → Cambiar contraseña | Asociación | — | `frmCambiarContraseña` → `GestorUsuarios.CambiarContrasena` (RN-40) |
| Usuario registrado → Recuperar contraseña | Asociación | — | `frmCambiarContraseña`, modo recuperación → `GestorCodigosCorreo.SolicitarRecuperacion` y `RecuperarContrasena` (RN-44) |
| Cliente → Buscar eventos | Asociación | — | `frmEventosCliente` → `GestorEventos.Buscar` |
| Cliente → Comprar paquete | Asociación | — | `FrmCompraCliente` → `GestorCompras.Iniciar`; `frmPago` → `GestorCompras.Confirmar` |
| Comprar paquete → Aplicar descuento por nivel | «include» | siempre | `GestorCompras.Iniciar` llama a `GestorFidelizacion.CalcularDescuento` |
| Comprar paquete → Pagar la compra | «include» | siempre | `frmPago` → `GestorCompras.Confirmar` → `PasarelaSimulada.Cobrar` |
| Contratar seguro → Comprar paquete | «extend» | el cliente lo elige | `Paquete.ConSeguro`; solo durante la compra (RN-28) |
| Inscribirse en lista de espera → Comprar paquete | «extend» | evento agotado | `FrmCompraCliente`: `GestorEventos.EstaAgotado` → `GestorListaEspera.Inscribir` |
| Cliente → Ver historial de compras | Asociación | — | `frmHistorialCliente` → `GestorCompras.HistorialDe` |
| Devolver entrada → Ver historial de compras | «extend» | con seguro y a más de 48 h | `frmHistorialCliente` → `GestorCompras.Reembolsar` (RN-29) |
| Cliente → Gestionar suscripción | Asociación | — | `frmSuscripcion` → `GestorSuscripciones.Contratar` / `Cancelar` |
| Cliente → Solicitar ser Organizador | Asociación | — | `frmSolicitudOrganizador` → `GestorSolicitudesOrganizador.EnviarCodigoTelefono`, `ConfirmarCodigoTelefono` y `Solicitar` (RN-50 a RN-52) |
| Organizador → Registrar local | Asociación | — | `frmRegistrarLocal` → `GestorLocales.Registrar` |
| Organizador → Consultar mis locales | Asociación | — | `frmMisLocales` → `GestorLocales.PorPropietario` (RN-49) |
| Organizador → Gestionar eventos | Asociación | — | `frmAgregarEvento`, `frmAdministrarEvento`, `frmEventosLocal` → `GestorEventos.Crear` / `Suspender` / `Eliminar` / `CambiarPreventa` / `CambiarImagenPortada` |
| Gestionar sectores → Gestionar eventos | «extend» | agrega o quita un sector | `frmAgregarSector` → `GestorEventos.AgregarSector`; `frmAdministrarEvento` → `EliminarSector` (RN-06, RN-38) |
| Organizador → Publicar evento | Asociación | — | `frmAdministrarEvento` → `GestorEventos.Publicar` (RN-06, RN-54) |
| Organizador → Consultar listas de espera | Asociación | — | `frmListasEspera` → `GestorListaEspera.DeEvento` (RN-49) |
| Organizador → Controlar ingreso | Asociación | — | `frmControlIngreso` → `GestorIngresos.Registrar` (RN-18, RN-49) |
| Organizador, Administrador → Generar reportes | Asociación | — | `frmReportes` → `GestorReportes.Crear` / `Ejecutar`, con el alcance de RN-34 |
| Exportar reporte a PDF → Generar reportes | «extend» | pide exportar | `frmReportes` → `GestorReportes.Exportar` |
| Administrador → Revisar solicitudes de Organizador | Asociación | — | `frmRevisarSolicitudesOrganizador` → `GestorSolicitudesOrganizador.Pendientes` / `Aprobar` / `Rechazar` (RN-53) |
| Administrador → Revisar el primer evento de un Organizador | Asociación | — | `frmRevisarEventos` → `GestorEventos.PendientesDeRevision` / `AprobarPrimerEvento` / `RechazarPrimerEvento` (RN-54) |
| Administrador → Consultar el registro de auditoría | Asociación | — | `frmAuditoria` → `GestorAuditoria.Consultar` |
| Administrador → Crear administrador | Asociación | — | `frmMenuAdministrador` → `frmCrearUsuario` en modo alta de administrador → `GestorUsuarios.CrearAdministrador` |

**Para la defensa (no se pega): «Aplicar descuento por nivel» y «Pagar la compra».** Criterio de
JC del 17/11: se quedan. La descomposición funcional no está entre los 16 errores frecuentes del
profesor (la regla 8 controla la dirección de «include» y «extend» y la condición de «extend», y
las dos cumplen).
- **Aplicar descuento por nivel** no es un objetivo del cliente: nadie entra al sistema a «aplicar
  un descuento». Es un caso de uso de nivel de subfunción, que UML admite como destino de un
  «include». Se dibuja por dos razones: el descuento por nivel es el diferencial del negocio
  (OE-3, RF-08, RN-19 a RN-24), y así la regla tiene un lugar visible en el diagrama, trazable
  hasta `GestorFidelizacion.CalcularDescuento`; y la compra lo ejecuta siempre, sin decisión del
  cliente, que es lo que expresa el «include». Su comportamiento está especificado dentro de CU-01,
  no en una especificación propia. Si el profesor pide quitarlo, se quita sin perder nada: el
  descuento ya vive en CU-01.
- **Pagar la compra** sí es defendible como objetivo del actor: el cliente paga con su tarjeta, en
  una pantalla propia (`frmPago`), y el pago puede fallar y reintentarse (RN-32). Va como
  «include» porque no hay compra sin pago.

**Para la defensa (no se pega): la generalización de actores.** Cliente, Organizador y
Administrador heredan de «Usuario registrado» los tres casos de la cuenta. Es la forma estándar de
UML de no repetir tres veces las mismas asociaciones, y responde a RN-05: los tres son la misma
cuenta con un perfil distinto. «Recuperar contraseña» la usa un usuario registrado que no inició
sesión: por eso cuelga de «Usuario registrado» y no del Visitante.

**Qué cambió respecto del diagrama viejo:**
- **Desaparecen** «Ver carrito», «Modificar local», «Eliminar local» y «Gestionar stock de
  entradas», que el sistema no tiene. La pasarela de pago deja de ser un actor.
- **Aparecen** Controlar ingreso, Devolver entrada, Cambiar contraseña e Inscribirse en lista de
  espera, que existen y no estaban.
- **Versión 2.6 (18/11):** el antiguo Administrador se divide en Organizador y Administrador de
  la plataforma. Aparecen Verificar el correo, Recuperar contraseña, Solicitar ser Organizador,
  Publicar evento, Revisar solicitudes de Organizador, Revisar el primer evento de un Organizador,
  Consultar el registro de auditoría y Crear administrador.

---

## 2. Diagrama de clases

El diagrama de clases completo tiene más de sesenta clases y no entra legible en una página. Se
presenta en nueve vistas, más cuatro de patrones (ficha 3). Regenerado el 18/11 con los tres
perfiles.

| Vista | Fuente e imagen (`docs/diagramas/…`) | Insertar a | Letra |
|---|---|---|---|
| 1. Dominio: cuentas y catálogo | `clases-1-dominio-catalogo` | **14,9 cm** (23,0 de alto, página entera) | 9,9 pt |
| 1b. Dominio: suscripción, lista de espera y solicitud de Organizador | `clases-1b-dominio-cuenta` | **11,4 cm** (17,3) | 12,0 pt |
| 2. Dominio: compra y pago | `clases-2-dominio-compra` | **16,0 cm** (15,9) | 9,8 pt |
| 3. Dominio: entrada y seguro | `clases-3-dominio-entrada` | **16,0 cm** (13,2) | 11,8 pt |
| 4. Negocio: gestores de la compra | `clases-4-negocio-compra` | **16,0 cm** (18,7) | 8,5 pt |
| 5. Negocio: administración | `clases-5-negocio-administracion` | **16,0 cm** (21,9) | 9,8 pt |
| 5b. Negocio: cuenta y seguridad | `clases-5b-negocio-cuenta-seguridad` | **16,0 cm** (19,9) | 10,7 pt |
| 6. Negocio: verificación de identidad, perfil y pertenencia | `clases-6-autorizacion` | **16,0 cm** (13,8) | 9,1 pt |
| 7. Datos: repositorios | `clases-7-datos` | **14,3 cm** (23,0, página entera) | 8,9 pt |

El fuente de cada una es `fuentes/<nombre>.puml` y la imagen, `<nombre>.png`.

**Dónde va:** página 86, debajo del título «Diagrama de clases», en lugar de la figura que hay
ahora. Una vista por página, cada una con el subtítulo de la tabla.

**Texto para el documento:**

> El diagrama de clases se presenta en vistas, porque el sistema completo supera las sesenta
> clases y en una sola figura resultaría ilegible. Las vistas siguen las capas de la arquitectura:
> cuatro para las entidades del dominio (proyecto Entidades), cuatro para la capa de negocio
> (proyecto Repositorio, espacio de nombres Negocio) y una para la capa de datos. Una clase que
> aparece en más de una vista se dibuja completa en una sola y, en las demás, solo con su nombre y
> la indicación de la vista donde está completa.
>
> Las entidades son clases sin lógica, con propiedades públicas. Cada asociación entre entidades
> se implementa con una propiedad que guarda el identificador de la otra (por ejemplo,
> `Entrada.IdEvento`); esas propiedades no se repiten como atributos, porque la asociación ya las
> representa. La navegabilidad de cada flecha sigue el sentido de esa propiedad en el código, y
> la tabla de justificación indica de qué propiedad sale cada una. Los estados de la entrada, de
> la compra y del pago son enumeraciones con los mismos valores que las restricciones `CHECK` de la
> base de datos.
>
> Tres conceptos del modelo de dominio llevan otro nombre en el código, y así aparecen en las
> vistas: el Organizador es la clase `Propietario`, que es el nombre de su tabla en la base de
> datos; la Inscripción en la lista de espera es la clase `ListaEspera`, y el Servicio contratado
> es la clase `CompraServicio`. El Administrador de la plataforma no tiene una clase propia: es un
> `Usuario` cuyo perfil (`TipoUsuario`) es «Administrador», porque no agrega datos ni
> asociaciones. El Paquete existe como clase pero no se guarda en la base: es el armado en curso,
> y al confirmarlo se convierte en una Compra.
>
> En la capa de negocio, las vistas 4, 5 y 5b muestran las operaciones públicas de cada gestor,
> más las internas que aparecen en el diagrama de secuencia. La vista 6 muestra la verificación
> que hace cada gestor antes de operar, a través de `GestorSesion`: que quien llama sea el
> titular de los datos (RN-41), que tenga el perfil Organizador (RN-48) y que el local o el evento
> sean suyos (RN-49), o que sea el Administrador de la plataforma (RN-34). La sesión guarda una
> copia de la identidad verificada (`SesionAutenticada`), y no el objeto de usuario que podría
> recibir de quien llama. La vista 7 muestra la capa de datos: dieciséis repositorios que heredan
> de `RepositorioBase`, únicos responsables del acceso a SQL Server; de cada uno se muestran los
> métodos que aparecen en los diagramas de secuencia.

**Justificación de relaciones, vistas 1 a 3 (entidades):**

| Vista | Relación | Tipo | Multiplicidad | Navegabilidad | De dónde sale en el código |
|---|---|---|---|---|---|
| 1 | Cliente → Usuario | Generalización | — | — | `class Cliente : Usuario` (`Entidade/Cliente.cs`); tabla `Cliente(ID)` FK a `Usuario` |
| 1 | Propietario → Usuario | Generalización | — | — | `class Propietario : Usuario`; el Organizador (RN-05) |
| 1 | Propietario — Local | Asociación | 1 / 0..* | Bidireccional | `Propietario.Locales` y `Local.IdPropietario` |
| 1 | Local — Evento | Asociación | 1 / 0..* | Bidireccional | `Local.Eventos` y `Evento.IdLocal` (`LocalID NOT NULL`) |
| 1 | Cliente → Evento | Asociación | 0..* / 0..* | Cliente → Evento | `Cliente.entradasUsuario : List<Evento>` |
| 1 | Evento — Sector | Asociación | 1 / 0..* | Bidireccional | `Evento.Sectores` y `Sector.IdEvento` |
| 1 | ServicioEvento → Evento | Asociación | 0..* / 1 | → Evento | `ServicioEvento.IdEvento` (tabla intermedia N:M) |
| 1 | ServicioEvento → Servicio | Asociación | 0..* / 1 | → Servicio | `ServicioEvento.IdServicio` |
| 1b | Suscripcion → Usuario | Asociación | 0..* / 1 | → Usuario | `Suscripcion.IdUsuario` |
| 1b | ListaEspera → Evento | Asociación | 0..* / 1 | → Evento | `ListaEspera.IdEvento` |
| 1b | ListaEspera → Usuario | Asociación | 0..* / 1 | → Usuario | `ListaEspera.Usuario` e `IdUsuario` |
| 1b | SolicitudOrganizador → Usuario (quien la presenta) | Asociación | 0..* / 1 | → Usuario | `SolicitudOrganizador.UsuarioId` |
| 1b | SolicitudOrganizador → Usuario (quien la revisa) | Asociación | 0..* / 0..1 | → Usuario | `SolicitudOrganizador.RevisadaPor : int?` |
| 2 | Paquete → Evento | Asociación | 0..* / 1 | → Evento | `Paquete.IdEvento` |
| 2 | Paquete → Entrada | Asociación | 0..1 / 0..* | → Entrada | `Paquete.Entradas : List<Entrada>` |
| 2 | Paquete → Servicio | Asociación | 0..* / 0..* | → Servicio | `Paquete.Servicios : List<Servicio>` |
| 2 | Compra → Usuario | Asociación | 0..* / 1 | → Usuario | `Compra.IdUsuario` |
| 2 | Pago → Compra | Asociación | 0..* / 1 | → Compra | `Pago.IdCompra` |
| 2 | CompraServicio → Compra | Asociación | 0..* / 1 | → Compra | `CompraServicio.IdCompra` |
| 2 | CompraServicio → Servicio | Asociación | 0..* / 1 | → Servicio | `CompraServicio.IdServicio` |
| 3 | Entrada → Evento | Asociación | 0..* / 1 | → Evento | `Entrada.IdEvento` |
| 3 | Entrada → Sector | Asociación | 0..* / 0..1 | → Sector | `Entrada.IdSector : int?` |
| 3 | Entrada → Compra | Asociación | 0..* / 0..1 | → Compra | `Entrada.IdCompra : int?` |
| 3 | Entrada → Usuario | Asociación | 0..* / 1 | → Usuario | `Entrada.IdUsuario` |
| 3 | Entrada → Cliente | Asociación | 0..* / 0..1 | → Cliente | `Entrada.Cliente` |
| 3 | Seguro → Entrada | Asociación | 0..1 / 1 | → Entrada | `Seguro.IdEntrada`; a lo sumo uno por entrada por el índice único `UX_Seguros_ID_Entrada` (script 07) |

Las enumeraciones `NivelFidelidad`, `EstadoCompra`, `EstadoPago` y `EstadoEntrada` son el tipo de
un atributo. Por eso no llevan línea: llevarla repetiría el atributo, que es la regla 1.

**Justificación, vista 5b.** `GestorSesion → SesionAutenticada` es una asociación 1 / 0..1
(`UsuarioActual`, que es nula sin sesión). Las demás clases de la vista no se relacionan entre
sí con líneas: sus dependencias están en la tabla de abajo.

**Justificación, vista 6 (dependencias de autorización).** Todas son dependencias de uso
(`..>`), sin multiplicidad. En las flechas, cada verificación se nombra sin el prefijo
«Requerir»:

| Gestor | Llama a `GestorSesion.…` | Para qué |
|---|---|---|
| GestorUsuarios | `Iniciar`, `RequerirUsuario`, `RequerirAdministrador` | Abre la sesión al autenticar; solo el Administrador crea administradores |
| GestorCodigosCorreo | `RequerirUsuario` | El código telefónico se pide para la cuenta propia |
| GestorCompras, GestorFidelizacion, GestorSuscripciones, GestorListaEspera | `RequerirUsuario` | Solo el titular de los datos (RN-41) |
| GestorPaquetes | `RequerirUsuario`, `HaySesion` | El titular arma su paquete (RN-41); si quien consulta un evento es un Organizador, el evento tiene que ser suyo (RN-49) |
| GestorEventos | `RequerirOrganizador`, `RequerirOrganizadorDeLocal`, `RequerirAdministrador` | Gestiona el Organizador dueño (RN-48, RN-49); revisa el primer evento el Administrador (RN-54) |
| GestorLocales | `RequerirOrganizador`, `RequerirOrganizadorDeLocal` | Solo el Organizador, sobre sus locales (RN-48, RN-49) |
| GestorIngresos | `RequerirOrganizador` | Solo el Organizador (RN-48); la pertenencia del evento se verifica después (RN-49) |
| GestorReportes | `UsuarioActual` | Alcance según el perfil: el Organizador ve sus eventos y el Administrador, todos (RN-34) |
| GestorAuditoria, GestorSolicitudesOrganizador | `RequerirAdministrador` | Solo el Administrador consulta la auditoría y revisa solicitudes (RN-53) |

**Dependencias que no se dibujan, para no saturar las vistas.** Todas son dependencias de uso, que
salen de llamadas `X.Instance.Operacion`:

| Desde | Hacia |
|---|---|
| GestorCompras | GestorPaquetes (`Validar`, `VerificarPreventa`), GestorFidelizacion (`CalcularDescuento`, `ObtenerCliente`, `ReevaluarNivel`, `AscenderSiCorresponde`), GestorUsuarios (`ExigirCorreoVerificado`), PasarelaSimulada (`Cobrar`), GeneradorQR (`NuevoCodigo`) |
| GestorFidelizacion | GestorPaquetes (subtotales), GestorSuscripciones (`TieneActiva`) |
| GestorPaquetes | GestorEventos (`ObtenerPorId`), GestorSuscripciones (`TieneActiva`), GestorUsuarios (`ExigirCorreoVerificado`) |
| GestorListaEspera | GestorEventos (`EstaAgotado`, `ObtenerPorId`), GestorSuscripciones (`TieneActiva`) |
| GestorUsuarios | GestorCodigosCorreo (`EnviarVerificacion`) |
| GestorCodigosCorreo | GestorUsuarios (`ValidarContrasena`), `SeleccionEnviadorCodigos.Actual` (ficha 3, vista 8d) |
| GestorSolicitudesOrganizador | GestorCodigosCorreo (`EnviarCodigoTelefono`, `ConfirmarCodigoTelefono`), `SeleccionVerificadorIdentidad.Actual` (vista 8d) |
| GestorCompras, GestorEventos, GestorIngresos, GestorUsuarios | GestorAuditoria (`Registrar`, interna) |
| GestorCompras, GestorFidelizacion, GestorPaquetes, GestorSuscripciones, GestorIngresos, GestorUsuarios, GestorCodigosCorreo, GestorSesion, ReporteUsuariosPorNivel | GestorParametros: leen los valores de las reglas |
| RepositorioCompras | RepositorioEntradas, RepositorioSectores (dentro de la misma transacción) |
| RepositorioEventos | RepositorioCompras, RepositorioSectores |
| RepositorioSectores | RepositorioCompras |

**Justificación, vista 7:** cada uno de los dieciséis repositorios → `RepositorioBase` es una
generalización (`: RepositorioBase`). `RepositorioBase` → `Conexion` es una dependencia de uso
(`AbrirConexion` llama a `Conexion.Instance.Crear()`).

**Qué cambió (versión 2.6, 18/11):**
- **Siete vistas pasan a nueve.** La vista 1 vieja no entraba legible con la solicitud de
  Organizador: la suscripción, la lista de espera y la solicitud pasan a la vista 1b. Los
  gestores de la cuenta y la seguridad pasan a la vista 5b.
- **Clases nuevas:**
  - `SolicitudOrganizador` (entidad);
  - `GestorAuditoria`, `GestorCodigosCorreo`, `GestorSolicitudesOrganizador` y `SesionAutenticada` (negocio);
  - `RepositorioAuditoria`, `RepositorioCodigosCorreo` y `RepositorioSolicitudesOrganizador` (datos).
- **Atributos y operaciones nuevos:**
  - en `Evento`, la portada y el motivo de rechazo;
  - en `GestorEventos`, publicar, aprobar y rechazar el primer evento, y cambiar la portada;
  - en `GestorUsuarios`, la fortaleza de la contraseña y la exigencia del correo verificado;
  - en `GestorSesion`, las verificaciones de Organizador y de pertenencia, y el control de inactividad.
- **`UsuarioActual` es ahora una `SesionAutenticada`,** no un `Usuario` (RN-41).

---

## 3. Patrones de diseño

| Vista | Fuente e imagen (`docs/diagramas/…`) | Insertar a | Letra |
|---|---|---|---|
| 8a. Singleton y Repository | `clases-8a-patron-singleton-repository` | **16,0 cm** (14,9) | 11,2 pt |
| 8b. Strategy | `clases-8b-patron-strategy` | **15,5 cm** (20,6) | 12,0 pt |
| 8c. Jerarquía de Reporte | `clases-8c-jerarquia-reportes` | **16,0 cm** (8,4) | 11,1 pt |
| 8d. Strategy: envío de códigos y verificación de identidad | `clases-8d-patron-strategy-cuenta` | **13,9 cm** (23,0, página entera) | 8,8 pt |

**Dónde va:** a continuación de las vistas de clases, con el título «Patrones de diseño». **Es
una sección nueva:** el documento hoy no la tiene. La 8c es baja y puede compartir página con el
texto.

**Texto para el documento:**

> El sistema aplica cuatro patrones de diseño. Cada uno se muestra con su estructura y con un caso
> real del código.
>
> **Singleton.** Los quince gestores que aplican las reglas de negocio, los dieciséis
> repositorios de datos, la conexión, la pasarela simulada y el generador de códigos QR tienen una
> única instancia: un campo estático privado `_instance`, un constructor privado y una propiedad
> estática `Instance` que crea la instancia la primera vez que se pide (treinta y cuatro clases en
> total). La implementación no usa bloqueo, porque la
> aplicación es de escritorio y todas las llamadas ocurren en el hilo de la interfaz. En la figura,
> el campo `_instance` aparece subrayado por ser estático.
>
> **Repository.** Cada repositorio es el único lugar del sistema que contiene SQL para sus tablas.
> La capa de negocio le pide y recibe entidades, nunca filas ni sentencias. La clase abstracta
> `RepositorioBase` reúne lo común: abrir la conexión, crear comandos con parámetros y leer valores
> que pueden ser nulos.
>
> **Strategy.** El formato de salida de los reportes es una estrategia. `GestorReportes` recibe un
> `IExportador` y no sabe qué formato produce; la pantalla de reportes elige la estrategia concreta,
> `ExportadorPDF`, que está en la capa de presentación porque usa la biblioteca iText. Agregar otro
> formato es agregar otra clase que implemente la interfaz, sin modificar ningún reporte. Hoy existe
> una sola estrategia concreta.
>
> El mismo patrón resuelve dos integraciones externas que la versión entregada simula. El envío
> de los códigos de un solo uso (verificación del correo, recuperación de la contraseña y
> confirmación del teléfono) depende de la interfaz `IEnviadorCodigos`: la estrategia elegida es
> `EnviadorSimulado`, que deja el mensaje en una bandeja simulada, y existe `EnviadorSmtp`, que lo
> enviaría por correo real. La verificación de identidad de quien solicita ser Organizador depende
> de `IVerificadorIdentidad`, con una estrategia simulada, que valida el formato del CUIT, y una
> externa, que consultaría un servicio de verificación. En los dos casos, la capa de negocio solo
> conoce la interfaz, y pasar a la estrategia real es una decisión de configuración, no un cambio
> de arquitectura.
>
> **Jerarquía de reportes.** Los cinco reportes del anexo A02 heredan de la clase abstracta
> `Reporte`, que define qué se informa (encabezados, datos y lectura); cada subclase redefine esas
> tres operaciones. `GestorReportes` elige la subclase según el código del reporte.

**Justificación de relaciones:**

| Vista | Relación | Tipo | De dónde sale en el código |
|---|---|---|---|
| 8a | GestorCompras → RepositorioCompras | Dependencia | `RepositorioCompras.Instance.…` en `GestorCompras` |
| 8a | RepositorioCompras → RepositorioBase | Generalización | `class RepositorioCompras : RepositorioBase` |
| 8a | RepositorioBase → Conexion | Dependencia | `Conexion.Instance.Crear()` en `AbrirConexion` |
| 8a | RepositorioCompras → Compra | Dependencia | `ObtenerPorId` devuelve `Compra` |
| 8b | frmReportes → GestorReportes | Dependencia | `GestorReportes.Instance.Exportar(reporte, new ExportadorPDF(), ruta)` (`frmReportes.cs:237`) |
| 8b | GestorReportes → IExportador | Dependencia | parámetro `IExportador exp` de `Exportar` |
| 8b | ExportadorPDF → IExportador | Realización | `class ExportadorPDF : IExportador` (`Visual/ExportadorPDF.cs`) |
| 8b | GestorReportes, IExportador → Reporte | Dependencia | parámetros `Reporte r` |
| 8c | Cada una de las cinco subclases → Reporte | Generalización | `sealed class ReporteVentasPorEvento : Reporte` y las otras cuatro |
| 8d | EnviadorSimulado, EnviadorSmtp → IEnviadorCodigos | Realización | `Repo/IEnviadorCodigos.cs`, `Visual/EnviadorSmtp.cs` |
| 8d | GestorCodigosCorreo → SeleccionEnviadorCodigos | Dependencia | `SeleccionEnviadorCodigos.Actual.Enviar(…)` en `GestorCodigosCorreo`; `Visual/Program.cs` fija `new EnviadorSimulado()` |
| 8d | SeleccionEnviadorCodigos → IEnviadorCodigos | Asociación | propiedad estática `Actual` |
| 8d | VerificadorIdentidadSimulado, VerificadorIdentidadExterno → IVerificadorIdentidad | Realización | `Repo/GestorSolicitudesOrganizador.cs` |
| 8d | GestorSolicitudesOrganizador → SeleccionVerificadorIdentidad | Dependencia | `SeleccionVerificadorIdentidad.Actual.Verificar(…)` en `Solicitar` |
| 8d | SeleccionVerificadorIdentidad → IVerificadorIdentidad | Asociación | propiedad estática `Actual`, que arranca con la simulada |

**Qué no se documenta, a propósito.** State, Facade, Observer y Composite **no están en el
código**, aunque Composite (permisos) y Observer (idioma) los pide la Carpeta modelo para anexos
futuros. **No se nombran.** `GestorClientes` y `GestorPropietario` tampoco aparecen: están vacíos y
no tienen constructor privado, así que no son Singleton.

---

## 4. Secuencia «Comprar paquete»

| Parte | Fuente e imagen (`docs/diagramas/…`) | Insertar a | Letra |
|---|---|---|---|
| 1. Reserva del cupo | `secuencia-compra-1-reserva` | **16,0 cm** (17,2) | 9,2 pt |
| 2. Pago | `secuencia-compra-2-pago` | **16,0 cm** (21,6) | 8,6 pt |
| 3. Emisión de las entradas | `secuencia-compra-3-emision` | **16,0 cm** (14,5) | 8,6 pt |

**Dónde va:** página 87, debajo de «Diagrama de secuencia – Comprar paquete». Reemplaza la
figura de ahí, una parte por página.

**Texto para el documento:**

> La secuencia de la compra se presenta en tres partes, que corresponden a las dos operaciones de
> la capa de negocio: `Iniciar`, que reserva el cupo, y `Confirmar`, que cobra y emite las entradas.
> Los participantes son las clases reales del sistema: los formularios de la capa de presentación,
> los gestores de negocio y los repositorios de datos. La pantalla nunca accede a la base de datos:
> lo hace siempre a través de un gestor y un repositorio.
>
> Antes de operar, `GestorCompras` verifica con `GestorSesion` que quien compra es el titular
> (RN-41). La reserva congela los precios y el descuento por nivel, y deja la compra y sus entradas
> en estado Pendiente durante el plazo de reserva (RN-15). Si el pago se rechaza, el cupo se
> devuelve y la compra admite un reintento (RN-32). Si se aprueba, cada entrada recibe un código QR
> único (RN-18), y la compra y sus entradas pasan a Confirmada en una sola transacción. El nivel
> del cliente se reevalúa después de la compra y rige desde la siguiente (RN-22, RN-23).

**Correspondencia con el código**: cada mensaje es una llamada que existe:

| Parte | Mensaje | Código |
|---|---|---|
| 1 | `Iniciar(p, c)` | `FrmCompraCliente.cs:267` → `GestorCompras.Iniciar` |
| 1 | `RequerirUsuario`, `Validar`, `VerificarPreventa`, `CalcularDescuento` | `GestorCompras.Iniciar`, en ese orden |
| 1 | `Iniciar(compra, servicios, entradas)` | `RepositorioCompras.Iniciar`; si no hay cupo lanza `CupoInsuficienteException` |
| 2 | `Confirmar(c, t)` | `frmPago.cs:76` → `GestorCompras.Confirmar` |
| 2 | `RequerirUsuario`, `ValidarTarjeta`, `ObtenerPorId` | `GestorCompras.RequerirCompra` y `Confirmar` |
| 2 | `MarcarVencidas`, `Rereservar`, `Cobrar`, `Insertar`, `Liberar` | `GestorCompras.Confirmar`, por las ramas del diagrama |
| 3 | `ObtenerPorCompra`, `NuevoCodigo`, `Confirmar(..)` | `GestorCompras.Confirmar`, segunda mitad |
| 3 | `ObtenerCliente`, `ReevaluarNivel`, `AscenderSiCorresponde` | `GestorCompras.Confirmar`, al final |
| 3 | genera el ticket y abre `frmConfirmacion` | `FrmCompraCliente.cs:294-295` |

**Qué cambió respecto del diagrama viejo:**
- **Desaparecen** ControladorCompra, ServicioCompra, ServicioNivel, RepositorioCompra y
  `actualizarPuntos`, que no existen.
- **Aparecen** la verificación de identidad y la pasarela.

---

## 5. Transición de estados de la entrada

| | |
|---|---|
| Fuente | `docs/diagramas/fuentes/estados-entrada.puml` |
| Imagen | `docs/diagramas/estados-entrada.png` |
| Insertar a | **16,0 cm** de ancho (18,2 de alto) |
| Letra impresa | 9,5 pt |

**Dónde va:** página 87, debajo de «Diagrama de estados – Ciclo de vida de la entrada».
Reemplaza la figura de ahí. Como ocupa la página entera, el título pasa a la página nueva.

**Texto para el documento:**

> Una entrada tiene cuatro estados: Pendiente, Confirmada, Reembolsada y Vencida. Son los mismos en
> el diagrama, en la enumeración `EstadoEntrada`, en la restricción `CK_Entradas_Estado` de la
> tabla `Entradas` y en las sentencias de la capa de datos. La entrada nace Pendiente al iniciarse
> la compra, con el cupo del sector reservado; pasa a Confirmada cuando el pago se aprueba y recibe
> su código QR; pasa a Vencida si vence el plazo de reserva o se da de baja el evento o el sector;
> y pasa a Reembolsada si el cliente la devuelve con seguro contratado, con más de 48 horas de
> anticipación y antes de usarla para ingresar.
>
> El ingreso al evento no es un estado de venta: la entrada sigue Confirmada y solo se registra la
> fecha y hora de la primera lectura del código QR. Por eso se modela como un subestado de
> Confirmada, y no como un quinto estado. Una segunda lectura del mismo código se rechaza (RN-18),
> y una entrada que ya ingresó no se puede devolver: la devolución sale del subestado sin ingreso.

**Justificación de transiciones:**

| Transición | Disparador | Regla | Código |
|---|---|---|---|
| inicio → Pendiente | el cliente inicia la compra | RN-08, RN-15 | `GestorCompras.Iniciar`: `Estado = EstadoEntrada.Pendiente` |
| Pendiente → Pendiente | pago rechazado: devuelve el cupo | RN-32 | `GestorCompras.Liberar` → `RepositorioCompras.Liberar` |
| Pendiente → Confirmada | pago aprobado: asigna el QR | RN-17, RN-18 | `RepositorioEntradas.cs:147`: `SET Estado = 'Confirmada' … WHERE Estado = 'Pendiente'` |
| Pendiente → Vencida | vence el plazo, o baja del evento o del sector | RN-15 | `RepositorioCompras.MarcarVencidas` y `VencerAbiertas` |
| Sin ingreso → Reembolsada | el cliente la devuelve | RN-28, RN-29 | `RepositorioCompras.cs:203-204`: `WHERE … Estado = 'Confirmada' AND FechaIngreso IS NULL`; el plazo y el rechazo de una entrada ya usada, en `GestorCompras.Reembolsar` (`GestorCompras.cs:301`) |
| Sin ingreso → Ingresada | primera lectura del QR dentro de la ventana | RN-18 | `RepositorioEntradas.cs:31-33`: `SET FechaIngreso … WHERE Estado = 'Confirmada' AND FechaIngreso IS NULL` |
| Vencida → fin | baja del evento: se borra la fila | — | `RepositorioEventos.cs:172` |

**Qué cambió respecto del diagrama viejo:**
- **Desaparecen** «Disponible», «Reservada», «Vendida» y «Cancelada», que no existen.
- **Aparece** «Vencida».
- **El reembolso** ya no lo dispara un evento cancelado, sino el cliente.
- **La devolución sale de «Sin ingreso»** (revisión externa de JC, 17/11): antes salía del borde de
  Confirmada, y eso decía que se podía devolver una entrada ya ingresada, cosa que el código
  rechaza.

---

## 6. Entidad-relación

Cuatro vistas, regeneradas el 18/11 con las veinte tablas de `BaseDatos/01-esquema.sql`.

| Vista | Fuente e imagen (`docs/diagramas/…`) | Insertar a | Letra impresa |
|---|---|---|---|
| 1. Usuarios y catálogo | `er-1-usuarios-catalogo` | **13,7 cm** (23,0 de alto, página entera) | 9,0 pt |
| 2. Compra, pago y entradas | `er-2-compras` | **14,6 cm** (23,0 de alto, página entera) | 9,3 pt |
| 3. Suscripciones, lista de espera y parámetros | `er-3-suscripciones-espera` | **12,7 cm** (13,9 de alto) | 12,0 pt |
| 4. Seguridad de la cuenta, auditoría y organizadores | `er-4-seguridad-organizadores` | **16,0 cm** (14,4 de alto) | 9,4 pt |

El fuente de cada una es `fuentes/<nombre>.puml` y la imagen, `<nombre>.png`.

**Dónde va:** página 89, debajo de «DER – Diagrama entidad-relación». **Esa página hoy tiene el
título y nada más**: el diagrama se perdió al armar la versión 2 del PDF. Una vista por página,
en orden.

**Texto para el documento:**

> El diagrama entidad-relación se obtiene del script de creación de la base
> (`BaseDatos/01-esquema.sql`) y muestra sus veinte tablas en cuatro vistas. La primera reúne
> los usuarios con sus tres perfiles, los locales, los eventos, los sectores y los servicios; la
> segunda, la compra con sus pagos, entradas, servicios contratados y pólizas; la tercera, las
> suscripciones, la lista de espera y los parámetros; y la cuarta, los códigos de un solo uso, el
> registro de auditoría y las solicitudes de Organizador. Las tablas que aparecen en más de una
> vista se muestran completas en una sola y, en las otras, solo con su clave.
>
> Cada columna indica su tipo; el asterisco marca las obligatorias (`NOT NULL`), y las marcas PK,
> FK, UQ y CK señalan la clave primaria, las claves foráneas, las restricciones de unicidad y las de
> verificación. Las relaciones usan la notación de pata de gallo, con la cardinalidad escrita en
> cada extremo. Cliente, Propietario y Administrador comparten la clave de Usuario: una tabla por
> perfil (RN-05). La tabla Propietario conserva el nombre del esquema original y hoy es la de los
> organizadores; guarda además si el Organizador ya tuvo un evento aprobado, que es lo que decide
> si su próxima publicación pasa a revisión (RN-54). La relación entre Eventos y Servicios es de
> muchos a muchos y se resuelve con la tabla intermedia ServiciosEvento. La contraseña se guarda
> como `PBKDF2$iteraciones$sal$hash`, nunca en texto plano, y los códigos de verificación y de
> recuperación, también como hash con su sal.
>
> La tabla Entradas proviene del esquema original, anterior a las compras. Por eso guarda el
> usuario además de la compra, y por eso la compra y el sector son columnas agregadas después que
> admiten valores nulos. El sistema escribe en cada entrada el mismo usuario de su compra, y nunca
> crea una entrada sin compra ni sin sector. Cada fila es una sola entrada, y cada una tiene su
> propio código QR, que se admite una sola vez en el ingreso (RN-18). La cantidad elegida para
> cada sector existe solo mientras se arma el paquete, que no se guarda en la base.
>
> El criterio para restringir en la base es uno solo: se restringe lo que, al violarse, hace daño,
> y se documenta lo que no. Dos seguros sobre una misma entrada serían dos cargos al cliente: por
> eso un índice único lo impide. La cantidad vendida entra en el cálculo de los lugares
> disponibles de cada sector: un valor distinto de 1 altera el cupo, y uno vacío o en cero haría
> que la entrada no se descontara de la capacidad, una sobreventa. Por eso la columna es
> obligatoria y solo admite el valor 1.
>
> Dos vínculos con Usuario se dibujan con línea punteada porque no tienen clave foránea
> declarada. El dueño de un local (`Locales.PropietarioID`) viene del esquema original. El autor
> de un registro de auditoría (`Auditoria.UsuarioID`) puede no existir: el registro conserva el
> correo de un intento de inicio de sesión aunque la cuenta no exista.

**Restricciones que no entran en la figura** (van en el texto o en una tabla debajo):

| Tabla | Restricción | Definición |
|---|---|---|
| Usuario | `UQ_Usuario_Mail` | `UNIQUE (Mail)` |
| Usuario | `CK_Usuario_TipoUsuario` | `TipoUsuario IN ('Cliente','Organizador','Administrador')` (RN-05, script 11) |
| Locales | `CK_Locales_Capacidad` | `Capacidad > 0` |
| Eventos | `CK_Eventos_Estado` | `Estado IN ('Borrador','EnRevision','Publicado','Suspendido')` (RN-54, script 13) |
| Entradas | `CK_Entradas_Estado` | `Estado IN ('Pendiente','Confirmada','Reembolsada','Vencida')` |
| Entradas | `CK_Entradas_CantidadVendida` | `CantidadVendida = 1`, columna `NOT NULL` (script 10) |
| Entradas | `UX_Entradas_CodigoQR` | único cuando `CodigoQR IS NOT NULL` |
| Compras | `CK_Compras_Estado` | `Estado IN ('Pendiente','Confirmada','Reembolsada','Vencida')` |
| Pagos | `CK_Pagos_Estado`, `CK_Pagos_Ultimos4` | `Estado IN ('Aprobado','Rechazado')`; cuatro dígitos |
| Suscripciones | `UX_Suscripcion_Activa` | una sola activa por usuario (índice único filtrado, RN-25) |
| ListaEspera | `UX_ListaEspera` (UQ1 en la figura) | `UNIQUE (ID_Evento, ID_Usuario)` (RN-36) |
| CodigosCorreo | `CK_CodigosCorreo_Proposito` | `Proposito IN ('Verificacion','Recuperacion','TelefonoOrg')` (RN-44) |
| SolicitudesOrganizador | `CK_SolicitudesOrganizador_Estado`, `_Capacidad`, `_TelefonoVerificado` | `Estado IN ('Pendiente','Aprobada','Rechazada')`; `Capacidad > 0`; `TelefonoVerificado = 1` (RN-51) |
| SolicitudesOrganizador | `UX_SolicitudesOrganizador_Pendiente` | una sola pendiente por usuario (índice único filtrado, RN-50) |

**Justificación de relaciones:**

| Vista | Relación | Cardinalidad | Clave foránea | Opcionalidad |
|---|---|---|---|---|
| 1 | Usuario — Cliente | 1 : 0..1 | `Cliente.ID` → `Usuario.ID` (también PK) | obligatoria |
| 1 | Usuario — Propietario | 1 : 0..1 | `Propietario.ID` → `Usuario.ID` (también PK) | obligatoria |
| 1 | Usuario — Administrador | 1 : 0..1 | `Administrador.ID` → `Usuario.ID` (también PK) | obligatoria |
| 1 | Propietario — Locales | 1 : 0..* | `Locales.PropietarioID` **sin FK declarada** (línea punteada) | `NOT NULL` |
| 1 | Locales — Eventos | 1 : 0..* | `Eventos.LocalID` | `NOT NULL` |
| 1 | Eventos — Sectores | 1 : 0..* | `Sectores.EventoID` | `NOT NULL` |
| 1 | Eventos — ServiciosEvento — Servicios | N : M | PK compuesta `(ID_Evento, ID_Servicio)` | — |
| 2 | Usuario — Compras | 1 : 0..* | `FK_Compras_Usuario` | `NOT NULL` |
| 2 | Compras — Pagos | 1 : 0..* | `FK_Pagos_Compras` | `NOT NULL` |
| 2 | Compras — Entradas | 0..1 : 0..* | `FK_Entradas_Compras` | admite NULL |
| 2 | Usuario — Entradas | 1 : 0..* | `FK_Entradas_Usuario` | `NOT NULL` |
| 2 | Eventos — Entradas | 1 : 0..* | `Entradas.ID_Evento` | `NOT NULL` |
| 2 | Sectores — Entradas | 0..1 : 0..* | `FK_Entradas_Sectores` | admite NULL |
| 2 | Compras — CompraServicios | 1 : 0..* | `CompraServicios.ID_Compra` | `NOT NULL` |
| 2 | Servicios — CompraServicios | 1 : 0..* | `CompraServicios.ID_Servicio` | `NOT NULL` |
| 2 | Entradas — Seguros | 1 : 0..1 | `Seguros.ID_Entrada` | `NOT NULL`, índice único `UX_Seguros_ID_Entrada` (script 07) |
| 3 | Usuario — Suscripciones | 1 : 0..* | `Suscripciones.ID_Usuario` | `NOT NULL` |
| 3 | Eventos — ListaEspera | 1 : 0..* | `ListaEspera.ID_Evento` | `NOT NULL` |
| 3 | Usuario — ListaEspera | 1 : 0..* | `ListaEspera.ID_Usuario` | `NOT NULL` |
| 4 | Usuario — CodigosCorreo | 1 : 0..* | `FK_CodigosCorreo_Usuario`, con borrado en cascada | `NOT NULL` |
| 4 | Usuario — SolicitudesOrganizador («presenta») | 1 : 0..* | `FK_SolicitudesOrganizador_Usuario` | `NOT NULL` |
| 4 | Usuario — SolicitudesOrganizador («revisa») | 0..1 : 0..* | `FK_SolicitudesOrganizador_Revisor` (`RevisadaPor`) | admite NULL mientras está pendiente |
| 4 | Usuario — Auditoria | 0..1 : 0..* | `Auditoria.UsuarioID` **sin FK declarada** (línea punteada) | admite NULL |

`Parametros` no tiene relaciones: guarda los valores de las reglas por clave.

**Qué cambió (versión 2.6, 18/11):**
- **Dos vistas pasan a cuatro:** con las cuatro tablas nuevas, la vista 1 vieja no entraba
  legible. Las suscripciones, la lista de espera y los parámetros pasan a la vista 3.
- **Tablas nuevas:** Administrador (script 11), CodigosCorreo y Auditoria (script 08) y
  SolicitudesOrganizador (script 12).
- **Columnas nuevas:**
  - `Usuario.CorreoVerificado` (script 08);
  - `Propietario.PrimerEventoAprobado` (script 14);
  - `Eventos.ImagenPortada` (script 09);
  - `Eventos.MotivoRechazo` (script 15).
- **`Sectores.CupoLiberadoEl`** (script 06) faltaba en la versión anterior del diagrama.
- **Restricciones nuevas:**
  - `Entradas.CantidadVendida` pasa a obligatoria y con `CK` (script 10);
  - `Eventos.Estado` suma «EnRevision» (script 13);
  - `TipoUsuario` tiene `CK` (script 11).

---

## 7. Modelo de dominio conceptual

| | |
|---|---|
| Fuente | `docs/diagramas/fuentes/dominio-conceptual.puml` |
| Imagen | `docs/diagramas/dominio-conceptual.png` |
| Insertar a | **16,0 cm** de ancho (15,3 de alto) |
| Letra impresa | 8,9 pt |

**Dónde va:** página 86, arriba, debajo de «Modelo de dominio conceptual». Reemplaza la figura de
ahí. Como el diagrama de clases que sigue ocupa varias páginas nuevas, este queda solo en su
página.

**Texto para el documento:**

> El modelo de dominio conceptual muestra los conceptos del negocio y cómo se relacionan, sin
> atributos ni operaciones y sin clases de software. Surge de los requerimientos y de las reglas
> de negocio. Los usuarios tienen uno de tres perfiles: Cliente, Organizador o Administrador
> (RN-05). El Organizador administra locales y los eventos que se realizan en ellos, divididos en
> sectores, y cada evento ofrece servicios complementarios. Un Cliente puede presentar una
> solicitud para ser Organizador, que revisa un Administrador; y cada operación relevante deja un
> registro de auditoría, atribuido al usuario que la hizo. El Cliente arma un paquete para un evento, con
> entradas y servicios; al confirmarse, el paquete se convierte en una compra, que emite las
> entradas, cada una correspondiente a un sector y opcionalmente cubierta por un seguro. Una compra
> admite varios intentos de pago. El Cliente tiene un nivel de fidelidad, puede contratar
> suscripciones y anotarse en la lista de espera de un evento agotado.

**Justificación de relaciones:**

| Relación | Multiplicidad | Requerimiento o regla | Dónde está en el código |
|---|---|---|---|
| Cliente, Organizador, Administrador → Usuario | Generalización | RN-05 (tres perfiles) | `Cliente : Usuario`, `Propietario : Usuario`; el Administrador es un `Usuario` con `TipoUsuario` «Administrador» |
| Organizador administra Local | 1 / 0..* | RF-12, RN-49 | `Local.IdPropietario` |
| Cliente presenta Solicitud de Organizador | 1 / 0..* | RN-50 | `SolicitudOrganizador.UsuarioId` |
| Administrador revisa Solicitud de Organizador | 0..1 / 0..* | RN-53 | `SolicitudOrganizador.RevisadaPor` |
| Usuario genera Registro de auditoría | 0..1 / 0..* | — | `Auditoria.UsuarioID`, que admite NULL: el registro conserva el correo de un intento fallido aunque la cuenta no exista |
| Evento se realiza en Local | 0..* / 1 | RF-12, RN-38 | `Evento.IdLocal` |
| Evento se divide en Sector | 1 / 0..* | RF-12, RN-06 | `Sector.IdEvento` |
| Evento ofrece Servicio complementario | 0..* / 0..* | RN-14 | tabla `ServiciosEvento` |
| Cliente tiene Nivel de fidelidad | 0..* / 1 | RF-08, RN-19 | `Cliente.Nivel` |
| Cliente contrata Suscripción | 1 / 0..* (una activa) | RF-13, RN-25 | `Suscripcion.IdUsuario` |
| Cliente arma Paquete | 1 / 0..* | RF-04, RF-05 | `GestorPaquetes.Nuevo(idEvento, idUsuario)` |
| Paquete es para Evento | 0..* / 1 | RF-05 | `Paquete.IdEvento` |
| Paquete agrega Servicio complementario | 0..* / 0..* | RF-05 | `Paquete.Servicios` |
| Paquete se confirma como Compra | 1 / 0..1 | RF-07 | `GestorCompras.Iniciar(paquete, cliente)` |
| Cliente realiza Compra | 1 / 0..* | RF-07 | `Compra.IdUsuario` |
| Compra emite Entrada | 1 / 1..* | RF-07, RN-18 | `Entrada.IdCompra`; `GestorPaquetes.Validar` exige al menos una entrada |
| Compra incluye Servicio complementario | 0..* / 0..* | RF-05, RN-16 | tabla `CompraServicios` |
| Entrada corresponde a Sector | 0..* / 1 | RN-08 | `Entrada.IdSector` |
| Compra se intenta pagar con Pago | 1 / 0..* | RF-06, RN-31 | `Pago.IdCompra` |
| Entrada está cubierta por Seguro | 1 / 0..1 | RN-28, RN-29 | `Seguro.IdEntrada` |
| Cliente se anota en Inscripción; Inscripción es para Evento | 1 / 0..*; 0..* / 1 | RF-10, RN-36 | tabla `ListaEspera` |

**Dos decisiones de modelado:**
- **Las entradas pedidas viajan en el paquete** (`Paquete.Entradas`), pero el modelo las muestra
  del lado de la compra, que es la que las emite. Dibujarlas en los dos lugares cruza siete líneas
  sobre Paquete y el diagrama deja de leerse.
- **«Compra emite 1..* entradas»** es la regla del negocio: no se confirma una compra sin
  entradas. En la base, `Entradas.ID_Compra` admite NULL; por eso el entidad-relación dice 0..*.

---

## 8. Modelo de dominio actualizado

| Vista | Fuente e imagen (`docs/diagramas/…`) | Insertar a | Letra |
|---|---|---|---|
| 1. Cuentas y catálogo | `dominio-actualizado-1-cuentas-catalogo` | **16,0 cm** (20,8 de alto) | 9,0 pt |
| 2. Compra | `dominio-actualizado-2-compra` | **16,0 cm** (20,2 de alto) | 9,4 pt |
| 3. Organización, auditoría y enumeraciones | `dominio-actualizado-3-organizacion` | **11,4 cm** (23,0, página entera) | 10,5 pt |

**Dónde va:** página 88, debajo de «Diagrama de dominio actualizado». Reemplaza la figura de ahí,
una vista por página.

**Texto para el documento:**

> El modelo de dominio actualizado retoma los conceptos del modelo conceptual y les agrega sus
> atributos con el tipo de cada uno. No incluye identificadores ni claves foráneas, porque las
> asociaciones ya representan esos vínculos. Se presenta en tres vistas para que se lea a tamaño de
> página: la primera reúne los usuarios con sus tres perfiles, el catálogo, las suscripciones y la
> lista de espera; la segunda, el paquete, la compra y lo que la compra genera; y la tercera, la
> solicitud para ser Organizador, el registro de auditoría y las enumeraciones del dominio.
>
> Respecto del conceptual, el nivel de fidelidad pasa a ser un atributo del cliente con un tipo
> enumerado (Bronce, Plata, Oro), y la cantidad de compras de los últimos doce meses aparece como
> atributo derivado, porque se calcula y no se guarda (RN-19). Los estados de la compra, de la
> entrada, del pago, del evento y de la solicitud son enumeraciones con los mismos valores que la
> base de datos. La relación de
> muchos a muchos entre compra y servicio complementario lleva una clase de asociación, Servicio
> contratado, con la cantidad y el precio unitario congelado al comprar (RN-16).
>
> El dominio expresa la regla del negocio: toda entrada pertenece a una compra y a un sector. En la
> base de datos, las columnas de la compra y del sector de la entrada admiten valores nulos, porque
> se agregaron a una tabla de entradas anterior a ellas; por eso el diagrama de clases y el
> entidad-relación muestran esas dos asociaciones con multiplicidad 0..1. Es una holgura del
> esquema cuya garantía está en el código: el sistema crea las entradas en un único lugar, dentro
> de la transacción de la compra, y siempre con su compra y su sector.

**Justificación.** Las asociaciones son las mismas de la ficha 7, con la misma multiplicidad.
Lo que se agrega:

| Elemento | Qué es | De dónde sale |
|---|---|---|
| Atributos de cada clase | Los del código, sin `Id` ni `IdX` | `Entidade/*.cs` |
| `nivel : NivelFidelidad` | Atributo con tipo enumerado. En el conceptual era una asociación con el concepto «Nivel de fidelidad» | `Cliente.Nivel`, `Entidade/NivelFidelidad.cs` |
| `/comprasUltimos12Meses` | Atributo derivado (se calcula) | `GestorFidelizacion.ComprasEnVentana`, RN-19 |
| `EstadoCompra`, `EstadoEntrada`, `EstadoPago` | Enumeraciones | `Entidade/Estado*.cs`; `CHECK` de la base |
| `EstadoEvento`, `EstadoSolicitud` | Enumeraciones del dominio | En el código son textos (`Evento.Estado`, `SolicitudOrganizador.Estado`); sus valores son los de `CK_Eventos_Estado` y `CK_SolicitudesOrganizador_Estado` |
| `correoVerificado`, `primerEventoAprobado`, `imagenPortada`, `motivoRechazo` | Atributos nuevos | `Usuario.CorreoVerificado`, `Propietario.PrimerEventoAprobado`, `Eventos.ImagenPortada`, `Evento.MotivoRechazo` |
| `mailDelAutor` | Atributo del registro de auditoría | `Auditoria.UsuarioMail`: el correo de quien hizo la operación o del intento de inicio de sesión |
| Servicio contratado | Clase de asociación de Compra–Servicio | tabla `CompraServicios` (`Cantidad`, `PrecioUnit`) |

**Qué no está, a propósito:**
- `TipoUsuario`: la generalización ya distingue Cliente, Organizador y Administrador; repetirlo
  como atributo es redundante (regla 7).
- Los códigos de un solo uso y su hash: son un mecanismo de seguridad, no un concepto del negocio.
- `Evento.Hora`: se deriva de la fecha.
- `Entrada.CantidadVendida`: siempre vale 1, porque cada entrada es una fila.

---

## 9. Robustez «Comprar paquete»

| | |
|---|---|
| Fuente | `docs/diagramas/fuentes/robustez-comprar-paquete.puml` |
| Imagen | `docs/diagramas/robustez-comprar-paquete.png` |
| Insertar a | **16,0 cm** de ancho (18,1 de alto) |
| Letra impresa | 9,5 pt |
| Verificación de conexiones | `py -3 tools/Diagramas/VerificarRobustez.py docs/diagramas/fuentes/robustez-comprar-paquete.puml` → 22 elementos, 26 conexiones, 0 prohibidas |

**Dónde va:** página 88, arriba, debajo de «Diagrama de robustez (ICONIX)». Reemplaza la figura
de ahí, que era un diagrama de secuencia con los íconos de robustez.

**Texto para el documento:**

> El diagrama de robustez del caso de uso «Comprar paquete» vincula su especificación con el
> diseño. Incluye el curso alternativo de la lista de espera cuando el evento está agotado. Las
> fronteras son las tres pantallas que usa el cliente; los controles, las clases de negocio que
> resuelven cada paso; las entidades, los conceptos del dominio que se leen o se modifican. Se
> respetan las reglas de conexión del método: el actor solo se comunica con fronteras, las
> fronteras solo con controles, y las entidades solo con controles. Los controles pueden
> comunicarse entre sí: el control de la compra verifica la identidad del cliente, cobra con la
> pasarela simulada, genera los códigos QR y usa los controles de paquetes y fidelización.
>
> El Cliente aparece dos veces, con dos papeles distintos: como actor es la persona que usa las
> pantallas, y como entidad es la información que el sistema guarda de ella, su identidad para la
> sesión y su nivel de fidelidad. El método distingue así a quien interactúa con el sistema de los
> datos que el sistema conserva sobre él.

**Justificación de conexiones:**

| Conexión | Tipo | Dónde está en el código |
|---|---|---|
| Cliente — tres pantallas | Actor–frontera | `FrmCompraCliente`, `frmPago`, `frmConfirmacion` |
| Pantalla de compra — GestorEventos, GestorPaquetes, GestorFidelizacion, GestorListaEspera, GestorCompras | Frontera–control | llamadas `X.Instance.…` en `FrmCompraCliente.cs` |
| Pantalla de pago — GestorCompras | Frontera–control | `Confirmar`, `Liberar` en `frmPago.cs` |
| Pantalla de confirmación — GestorCompras | Frontera–control | `PagosDe` en `frmConfirmacion.cs` |
| GestorCompras — GestorSesion, PasarelaSimulada, GeneradorQR, GestorPaquetes, GestorFidelizacion | Control–control | `GestorCompras.Iniciar` y `Confirmar` |
| GestorEventos — Evento, Sector | Control–entidad | `ObtenerPorId`, `Sectores`, `EstaAgotado` |
| GestorPaquetes — Paquete, Servicio | Control–entidad | `Nuevo`, `AgregarEntradas`, `AgregarServicio` |
| GestorFidelizacion — Cliente; GestorSesion — Cliente | Control–entidad | `ObtenerCliente`, `CalcularDescuento`; `UsuarioActual` |
| GestorListaEspera — Inscripción | Control–entidad | `Inscribir`, `EstaInscripto` |
| GestorCompras — Compra, Entrada, Pago, Seguro | Control–entidad | `Iniciar`, `Confirmar` |

**Lo que no aparece, porque está prohibido:**
- la navegación entre pantallas (de compra a pago), que sería frontera–frontera;
- el acceso de una pantalla a una entidad, que sería frontera–entidad.

---

## 10. Actividad del proceso de negocio

| Parte | Fuente e imagen (`docs/diagramas/…`) | Insertar a | Letra |
|---|---|---|---|
| 1. Del evento a la reserva | `actividad-1-armado` | **16,0 cm** (20,9) | 8,7 pt |
| 2. Pago y entrega | `actividad-2-pago` | **13,6 cm** (17,6) | 12,0 pt |
| 3. Devolución e ingreso | `actividad-3-ingreso` | **16,0 cm** (16,4) | 9,8 pt |

**Dónde va:** página 88, abajo, debajo de «Diagrama de Actividad (flujo del Cliente)», que conviene
renombrar «Diagrama de actividad (proceso de venta)». Reemplaza la figura de la página 89. Una
parte por página. Las partes se encadenan con los conectores A y B.

**Texto para el documento:**

> El diagrama de actividad describe el proceso de negocio de punta a punta, desde que el
> Administrador carga un evento hasta que el Cliente ingresa con su entrada. Tiene un carril por
> actor y ninguno para el sistema, que no es un actor del proceso. Se presenta en tres partes,
> unidas por los conectores A y B. El evento se crea en borrador y queda publicado cuando se le
> agrega el primer sector con capacidad (RN-06). El Cliente busca un evento; si está agotado, se
> anota en la lista de espera. Si hay cupo, arma el paquete, elige si agrega el seguro, revisa el
> resumen con el descuento de su nivel y confirma, con lo que reserva el cupo por un plazo
> limitado (RN-15). Puede reintentar el pago mientras dure ese plazo; si no lo concreta, pierde la
> reserva. Con el pago aprobado recibe sus entradas con código QR. Antes del evento puede devolver
> una entrada asegurada con más de 48 horas de anticipación (RN-29); el día del evento la presenta
> en el ingreso, y el Administrador la acepta solo si está confirmada, corresponde al evento, está
> dentro del horario y no ingresó antes (RN-18).

**Justificación de cada decisión del diagrama:**

| Elemento | Regla | Dónde está en el código |
|---|---|---|
| Agregar sectores; con el primero queda publicado | RN-06 | `GestorEventos.Crear` (Borrador) y `AgregarSector` |
| ¿Hay cupo? → lista de espera | RF-10, RN-35, RN-36 | `GestorEventos.EstaAgotado`, `GestorListaEspera.Inscribir` |
| ¿Quiere seguro? | RN-28 | `Paquete.ConSeguro` |
| Revisar el resumen con el descuento por nivel | RF-08, RN-19 | `GestorFidelizacion.CalcularDescuento` |
| Confirmar y reservar el cupo | RN-15 | `GestorCompras.Iniciar` |
| Reintentar el pago dentro del plazo | RN-15, RN-32 | `GestorCompras.Confirmar`, `Liberar`, `Rereservar` |
| Recibir las entradas con su QR | RF-07, RN-18 | `GestorCompras.Confirmar`, `GeneradorQR` |
| Devolver la entrada | RN-29 | `GestorCompras.Reembolsar` |
| Leer el QR y registrar o rechazar | RN-18 | `GestorIngresos.Registrar` y sus motivos de rechazo |

**Qué cambió respecto del diagrama viejo:**
- **Desaparecen:**
  - el descuento por suscripción;
  - «actualizar puntos»;
  - el «carrito»;
  - las recomendaciones de eventos (RF-09, que no está construido);
  - los pasos del sistema dentro del flujo del cliente.
- **Aparecen:**
  - el Administrador;
  - el plazo de reserva;
  - la devolución;
  - el control de ingreso.

---

## Regla de ancho de las figuras (capturas y diagramas)

Decidida el 17/11, a pedido de JC, una vez para todas las figuras: las que ya están y las capturas
nuevas de las cuatro fases (pantallas más anchas, como el mapa de sectores). Cuando llegue una
captura, se aplica esto. No se decide pantalla por pantalla.

**Las medidas de la página.** A4 vertical: el área de texto mide **16,0 cm de ancho** y unos
**23 cm de alto** sin encabezado, pie ni epígrafe (las mismas constantes que usa
`tools/Diagramas/Renderizar.py`). Ninguna figura pasa de 16 cm en una página vertical: más ancha,
rompe el margen. Si el documento tiene 2,5 cm de margen a cada lado (21 − 16 = 5), una página
apaisada da unos **24 cm de ancho** útil y unos **14 cm de alto**. Hay que comprobarlo en el Word
la primera vez (*Disposición › Márgenes*), porque el fuente no está versionado.

**La letra mínima.** Se mide la letra impresa con la fórmula de siempre: tamaño de la letra en el
PNG (px) × ancho de inserción (pt) / ancho del PNG (px).
- **Diagramas: 8,5 pt**, como hasta ahora (`LETRA_MINIMA` del script).
- **Capturas: 8 pt.**
- **Por debajo de 8 pt, solo por decisión escrita** de JC, con su razón al lado de la figura, como
  la figura 1 del instructivo (7,2 pt). **Por debajo de 7 pt, nunca.**

**El orden de decisión.** Se aplica el primer paso que da 8 pt o más:

1. **Vertical, al ancho que entra.** Hasta 16 cm, sin ampliar la imagen por encima de su tamaño
   real: una captura al 100 % mide `ancho en px / 96 × 2,54` cm. Ampliarla más sube los puntos,
   pero no la nitidez: la figura 2 del instructivo se imprimiría a 64 ppp a 16 cm, y por eso va a
   su tamaño real. Si hace falta más tamaño, se usa la versión al 150 % de la misma pantalla, que
   tiene más píxeles.
2. **Recortar a lo que la figura tiene que mostrar.** Sirve si lo que se saca no hace falta en
   ese lugar o ya está en otra figura. Ejemplo: la figura 2 del instructivo, cuyo resumen ya estaba
   en la figura 3. No sirve si el recorte saca lo que el texto explica. Ejemplo: la figura 1, que
   perdía los filtros, las columnas o el botón Comprar.
3. **Partir en dos, una debajo de la otra, con un solo epígrafe.** Sirve si la pantalla tiene dos
   zonas que se leen por separado. Ejemplo: la figura 4, confirmación arriba y comprobante abajo.
   Cada mitad se mide sola y cada una tiene que dar 8 pt.
4. **Página apaisada, hasta 24 cm.** Para una pantalla que no admite recorte ni partición (una
   grilla o un mapa que se lee entero). A 24 cm la letra sube un 50 % respecto de 16 cm: una
   captura que da 6 pt a 16 cm, da 9 pt a 24.
5. **Si nada de eso llega a 8 pt:** decisión escrita de JC, como la figura 1, o la figura no va.

**Qué cuesta una página apaisada en Word,** y por qué es el paso 4 y no el 1:
- **Dos saltos de sección** (*Disposición › Saltos › Página siguiente*), uno antes y otro después
  de la figura, y la orientación horizontal solo en la sección del medio.
- **Encabezado y pie:** en la sección nueva y en la siguiente hay que dejar marcado «Vincular al
  anterior», o el pie («EventAura — versión 2.6», materia, docente) desaparece o se duplica.
  Comprobarlo en la página apaisada y en la siguiente.
- **Numeración:** tiene que continuar. Comprobarlo en el número de la página apaisada y de la
  siguiente.
- **Márgenes:** al cambiar la orientación, revisar que izquierdo y derecho den los 24 cm útiles.
- **El epígrafe** va en la misma página apaisada, debajo de la figura.
- **En el PDF** la página sale girada: se lee rotando la pantalla, e impresa, girando la hoja.

Son unos minutos por página, pero es el paso que más puede romper el documento sin que se note.
Por eso conviene **agrupar en una sola sección apaisada todas las figuras anchas que van seguidas**
(por ejemplo, varias pantallas del administrador) y no abrir una sección por cada una.

**Qué se informa con cada figura nueva,** para que la lista le diga a Máximo todo de una vez:
archivo, paso aplicado (1 a 5), ancho en cm, orientación y letra más chica impresa. Si es el paso 4,
también con qué figuras comparte la sección apaisada.

## Las 16 reglas del profesor, diagrama por diagrama

| Regla | Cómo quedó |
|---|---|
| 1. Clave foránea repetida con la línea | **Cumple.** En clases, las propiedades `IdX` no se dibujan como atributo cuando hay línea; las enumeraciones no llevan línea. En el ER las FK van como columna, que es lo correcto en un ER |
| 2. Multiplicidad en los dos extremos | **Cumple** en todas las asociaciones. Las dependencias y generalizaciones no llevan |
| 3. Composición mal usada | **No aplica:** no hay composiciones; el código no tiene ciclo de vida dependiente entre entidades |
| 4. Navegabilidad invertida | **Cumple:** cada flecha sigue la propiedad del código (tabla de la ficha 2). Bidireccional solo donde hay propiedad en las dos clases |
| 5. 1:N dibujado como 1:1 | **Cumple.** Las únicas 1:0..1 son Usuario–Cliente y Usuario–Propietario, respaldadas por la PK compartida |
| 6. N:M sin tabla intermedia | **Cumple:** Eventos–Servicios por ServiciosEvento |
| 7. Herencia por conveniencia | **Cumple:** Cliente y Propietario son Usuario y no repiten atributos; los reportes son Reporte |
| 8. include/extend al revés o extend sin condición | **Cumple:** «include» va de la base al incluido; «extend», del que extiende a la base, siempre con condición |
| 9. Robustez | **Cumple, verificado por programa:** `VerificarRobustez.py` da 0 conexiones prohibidas en las 26. Comprobado también al revés: sobre una copia con tres conexiones prohibidas, las detecta las tres |
| 10. Secuencia que accede a la base o no coincide | **Cumple:** la vista habla con gestores, y cada mensaje es una llamada real (tabla de la ficha 4) |
| 11. Clases de software en el dominio | **Cumple:** los dos modelos de dominio tienen solo conceptos del negocio; ningún gestor, repositorio ni pantalla |
| 12. Sistema como actor o como carril | **Cumple:** la pasarela simulada dejó de ser actor, y la actividad tiene solo los carriles Cliente y Administrador |
| 13. Estados distintos en diagrama, clase y base | **Cumple:** los cuatro estados coinciden en diagrama, `EstadoEntrada`, `CK_Entradas_Estado` y el SQL |
| 14. Objetivos | **No aplica** a estos diagramas |
| 15. Patrón nombrado que no se ve | **Cumple:** los cuatro patrones nombrados están en las vistas 8a a 8c, con estereotipo; no se nombra ningún otro |
| 16. Legible al 100 % | **Cumple:** la letra más chica impresa va de 8,5 a 12 pt, medida sobre una página A4 armada con cada diagrama |

**Un detalle visual, sin efecto en las reglas.** El motor de diagramación pone los números de
multiplicidad cerca de la punta de la flecha. Se corrieron para que no se pisen, pero en algún
extremo quedan pegados a la línea.

## Lo que no se hizo en esta entrega

Se rehace en la próxima tanda, a partir de los requerimientos y contrastado con el código.

| Artefacto | Página | Problema |
|---|---|---|
| Diagramas chicos de cada especificación, CU-01 a CU-08 | 72 a 85 | Repiten errores del general: descuento por suscripción en CU-01, pasarela como actor en CU-06, listas de espera solo del administrador en CU-07 |
| Especificaciones de los cuatro casos de uso nuevos | — | No existen |
| Secuencias de los otros casos de uso | — | La Carpeta modelo pide una por caso de uso |
| Robustez de los otros casos de uso | — | Se entregó la del caso principal; si el docente pide una por caso de uso, faltan |

## Para regenerar una imagen

Solo si se cambia un `.puml`. Hace falta Java 21 y `plantuml.jar`; en la PC de JC están en
`D:\jc-tools\plantuml`.

```
py -3 tools/Diagramas/Renderizar.py estados-entrada
```

- **Sin nombre**, regenera todos.
- **Por cada diagrama**, informa el ancho al que hay que insertarlo y la letra impresa que queda.
  Si no entra legible en una página, lo marca como FALLA.
- `_estilo.iuml` fija la letra y los colores de todos los diagramas.
