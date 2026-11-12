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
| 71 | «Diagramas de caso de uso»: el general viejo | Dos diagramas: cliente y administrador | [1](#1-casos-de-uso) |
| 72 a 85 | Especificaciones CU-01 a CU-08, cada una con un diagrama chico | **Sin cambios en esta entrega** (ver «Lo que no se hizo») | — |
| 86, arriba | «Modelo de dominio conceptual» | **Sin cambios en esta entrega** | — |
| 86, abajo | «Diagrama de clases» | Ocho vistas de clases más tres de patrones (11 figuras) | [2](#2-diagrama-de-clases) y [3](#3-patrones-de-diseño) |
| 87, arriba | «Diagrama de secuencia – Comprar paquete» | La secuencia en tres partes | [4](#4-secuencia-comprar-paquete) |
| 87, abajo | «Diagrama de estados – Ciclo de vida de la entrada» | El diagrama de estados nuevo | [5](#5-transición-de-estados-de-la-entrada) |
| 88 | Robustez y dominio actualizado | **Sin cambios en esta entrega** | — |
| 89, arriba | Actividad (flujo del cliente) | **Sin cambios en esta entrega** | — |
| 89, abajo | «DER – Diagrama entidad-relación», **con la página en blanco** | Dos vistas del entidad-relación | [6](#6-entidad-relación) |

**Páginas nuevas.** Se agregan unas 14 páginas entre la 71 y la 89. Todo lo que sigue se corre, y
el índice (páginas 2 a 5) hay que regenerarlo.

---

## 1. Casos de uso

| | Cliente | Administrador |
|---|---|---|
| Fuente | `docs/diagramas/fuentes/casos-uso-cliente.puml` | `docs/diagramas/fuentes/casos-uso-administrador.puml` |
| Imagen | `docs/diagramas/casos-uso-cliente.png` | `docs/diagramas/casos-uso-administrador.png` |
| Insertar a | **16,0 cm** de ancho (13,4 de alto) | **16,0 cm** de ancho (15,1 de alto) |
| Letra impresa | 8,9 pt | 10,5 pt |

**Dónde va:** página 71. Reemplaza la figura que está bajo el título «Diagramas de caso de uso».
El diagrama del administrador va en la página siguiente, con el título «Diagramas de caso de uso
(Administrador)».

**Texto para el documento:**

> Los casos de uso se presentan en dos diagramas, uno por actor, para que cada uno se lea a
> tamaño de página. Cada caso de uso corresponde a una pantalla del sistema y a una operación de
> la capa de negocio. El pago se resuelve con una pasarela simulada que forma parte del sistema,
> por lo que no se modela como actor. Las relaciones «include» indican pasos que la compra
> realiza siempre (el descuento por nivel y el pago); las «extend» indican comportamiento
> opcional, con su condición entre corchetes: contratar el seguro, inscribirse en la lista de
> espera cuando el evento está agotado y devolver una entrada asegurada con más de 48 horas de
> anticipación.
>
> El Administrador corresponde al perfil «Propietario» de la base de datos (RN-05). Además de los
> casos de uso de gestión, incorpora el control de ingreso, que verifica que cada código QR se use
> una sola vez (RN-18).

**Justificación de relaciones:**

| Relación | Tipo | Condición | De dónde sale |
|---|---|---|---|
| Visitante → Registrarse | Asociación | — | `frmCrearUsuario` → `GestorUsuarios.RegistrarCliente` |
| Cliente, Administrador → Iniciar sesión | Asociación | — | `frmInicio` → `GestorUsuarios.Autenticar` |
| Cliente, Administrador → Cambiar contraseña | Asociación | — | `frmCambiarContraseña` → `GestorUsuarios.CambiarContrasena` (RN-40) |
| Cliente → Buscar eventos | Asociación | — | `frmEventosCliente` → `GestorEventos.Buscar` |
| Cliente → Comprar paquete | Asociación | — | `FrmCompraCliente` → `GestorCompras.Iniciar`; `frmPago` → `GestorCompras.Confirmar` |
| Comprar paquete → Aplicar descuento por nivel | «include» | siempre | `GestorCompras.Iniciar` llama a `GestorFidelizacion.CalcularDescuento` |
| Comprar paquete → Pagar la compra | «include» | siempre | `frmPago` → `GestorCompras.Confirmar` → `PasarelaSimulada.Cobrar` |
| Contratar seguro → Comprar paquete | «extend» | el cliente lo elige | `Paquete.ConSeguro`; solo durante la compra (RN-28) |
| Inscribirse en lista de espera → Comprar paquete | «extend» | evento agotado | `FrmCompraCliente`: `GestorEventos.EstaAgotado` → `GestorListaEspera.Inscribir` |
| Cliente → Ver historial de compras | Asociación | — | `frmHistorialCliente` → `GestorCompras.HistorialDe` |
| Devolver entrada → Ver historial de compras | «extend» | con seguro y a más de 48 h | `frmHistorialCliente` → `GestorCompras.Reembolsar` (RN-29) |
| Cliente → Gestionar suscripción | Asociación | — | `frmSuscripcion` → `GestorSuscripciones.Contratar` / `Cancelar` |
| Administrador → Registrar local | Asociación | — | `frmRegistrarLocal` → `GestorLocales.Registrar` |
| Administrador → Consultar mis locales | Asociación | — | `frmMisLocales` → `GestorLocales.PorPropietario` |
| Administrador → Gestionar eventos | Asociación | — | `frmAgregarEvento`, `frmAdministrarEvento`, `frmEventosLocal` → `GestorEventos.Crear` / `Suspender` / `Eliminar` / `CambiarPreventa` |
| Gestionar sectores → Gestionar eventos | «extend» | edita sectores | `frmAgregarSector` → `GestorEventos.AgregarSector`; `frmAdministrarEvento` → `EliminarSector` (RN-38) |
| Administrador → Consultar listas de espera | Asociación | — | `frmListasEspera` → `GestorListaEspera.DeEvento` |
| Administrador → Controlar ingreso | Asociación | — | `frmControlIngreso` → `GestorIngresos.Registrar` (RN-18) |
| Administrador → Generar reportes | Asociación | — | `frmReportes` → `GestorReportes.Crear` / `Ejecutar` |
| Exportar reporte a PDF → Generar reportes | «extend» | pide exportar | `frmReportes` → `GestorReportes.Exportar` |

**Qué cambió respecto del diagrama viejo:**
- **Desaparecen** «Ver carrito», «Modificar local», «Eliminar local» y «Gestionar stock de
  entradas», que el sistema no tiene. La pasarela de pago deja de ser un actor.
- **Aparecen** Controlar ingreso, Devolver entrada, Cambiar contraseña e Inscribirse en lista de
  espera, que existen y no estaban.

---

## 2. Diagrama de clases

El diagrama de clases completo tiene más de 40 clases y no entra legible en una página. Se
presenta en ocho vistas, más tres de patrones (ficha 3).

| Vista | Fuente e imagen (`docs/diagramas/…`) | Insertar a | Letra |
|---|---|---|---|
| 1. Dominio: cuentas y catálogo | `clases-1-dominio-catalogo` | **15,9 cm** (23,0 de alto, página entera) | 8,6 pt |
| 2. Dominio: compra y pago | `clases-2-dominio-compra` | **16,0 cm** (15,9) | 9,8 pt |
| 3. Dominio: entrada y seguro | `clases-3-dominio-entrada` | **16,0 cm** (13,2) | 11,8 pt |
| 4. Negocio: gestores de la compra | `clases-4-negocio-compra` | **16,0 cm** (17,6) | 8,5 pt |
| 5. Negocio: administración y cuenta | `clases-5-negocio-administracion` | **16,0 cm** (19,6) | 9,2 pt |
| 6. Negocio: verificación de identidad | `clases-6-autorizacion` | **16,0 cm** (11,9) | 9,9 pt |
| 7. Datos: repositorios | `clases-7-datos` | **15,1 cm** (23,0, página entera) | 11,1 pt |

El fuente de cada una es `fuentes/<nombre>.puml` y la imagen, `<nombre>.png`.

**Dónde va:** página 86, debajo del título «Diagrama de clases», en lugar de la figura que hay
ahora. Una vista por página, cada una con el subtítulo de la tabla. **El «Modelo de dominio
conceptual» de la misma página no se toca en esta entrega.**

**Texto para el documento:**

> El diagrama de clases se presenta en vistas, porque el sistema completo supera las cuarenta
> clases y en una sola figura resultaría ilegible. Las vistas siguen las capas de la arquitectura:
> tres para las entidades del dominio (proyecto Entidades), tres para la capa de negocio (proyecto
> Repositorio, espacio de nombres Negocio) y una para la capa de datos. Una clase que aparece en
> más de una vista se dibuja completa en una sola y, en las demás, solo con su nombre y la
> indicación de la vista donde está completa.
>
> Las entidades son clases sin lógica, con propiedades públicas. Cada asociación entre entidades
> se implementa con una propiedad que guarda el identificador de la otra (por ejemplo,
> `Entrada.IdEvento`); esas propiedades no se repiten como atributos, porque la asociación ya las
> representa. La navegabilidad de cada flecha sigue el sentido de esa propiedad en el código, y
> la tabla de justificación indica de qué propiedad sale cada una. Los estados de la entrada, de
> la compra y del pago son enumeraciones con los mismos valores que las restricciones `CHECK` de la
> base de datos.
>
> En la capa de negocio, las vistas 4 y 5 muestran las operaciones públicas de cada gestor, y la
> vista 6, la verificación de identidad que agregó la revisión de seguridad: antes de operar, cada
> gestor consulta a `GestorSesion` si quien llama es el titular de los datos (RN-41) o el
> Administrador (RN-34, RN-42). La vista 7 muestra la capa de datos: trece repositorios que
> heredan de `RepositorioBase`, únicos responsables del acceso a SQL Server.

**Justificación de relaciones, vistas 1 a 3 (entidades):**

| Vista | Relación | Tipo | Multiplicidad | Navegabilidad | De dónde sale en el código |
|---|---|---|---|---|---|
| 1 | Cliente → Usuario | Generalización | — | — | `class Cliente : Usuario` (`Entidade/Cliente.cs`); tabla `Cliente(ID)` FK a `Usuario` |
| 1 | Propietario → Usuario | Generalización | — | — | `class Propietario : Usuario` |
| 1 | Propietario — Local | Asociación | 1 / 0..* | Bidireccional | `Propietario.Locales` y `Local.IdPropietario` |
| 1 | Local — Evento | Asociación | 1 / 0..* | Bidireccional | `Local.Eventos` y `Evento.IdLocal` (`LocalID NOT NULL`) |
| 1 | Cliente → Evento | Asociación | 0..* / 0..* | Cliente → Evento | `Cliente.entradasUsuario : List<Evento>` |
| 1 | Evento — Sector | Asociación | 1 / 0..* | Bidireccional | `Evento.Sectores` y `Sector.IdEvento` |
| 1 | ServicioEvento → Evento | Asociación | 0..* / 1 | → Evento | `ServicioEvento.IdEvento` (tabla intermedia N:M) |
| 1 | ServicioEvento → Servicio | Asociación | 0..* / 1 | → Servicio | `ServicioEvento.IdServicio` |
| 1 | Suscripcion → Usuario | Asociación | 0..* / 1 | → Usuario | `Suscripcion.IdUsuario` |
| 1 | ListaEspera → Evento | Asociación | 0..* / 1 | → Evento | `ListaEspera.IdEvento` |
| 1 | ListaEspera → Usuario | Asociación | 0..* / 1 | → Usuario | `ListaEspera.Usuario` e `IdUsuario` |
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
| 3 | Seguro → Entrada | Asociación | 0..* / 1 | → Entrada | `Seguro.IdEntrada` |

Las enumeraciones `NivelFidelidad`, `EstadoCompra`, `EstadoPago` y `EstadoEntrada` son el tipo de
un atributo. Por eso no llevan línea: llevarla repetiría el atributo, que es la regla 1.

**Justificación, vista 6 (dependencias de autorización).** Todas son dependencias de uso
(`..>`), sin multiplicidad:

| Gestor | Llama a `GestorSesion.…` | Para qué |
|---|---|---|
| GestorEventos, GestorLocales, GestorIngresos | `RequerirAdministrador()` | Solo el Administrador gestiona y controla ingreso (RN-42) |
| GestorReportes | `UsuarioActual` | Verifica el perfil Administrador (RN-34) |
| GestorUsuarios | `Iniciar(Usuario)`, `RequerirAdministrador()` | Abre la sesión al autenticar; solo el Administrador crea administradores |
| GestorCompras, GestorPaquetes, GestorFidelizacion, GestorSuscripciones | `RequerirUsuario(id)` | Solo el titular o el Administrador (RN-41) |
| GestorListaEspera | `RequerirUsuario(id)`, `RequerirAdministrador()` | Inscribirse es del titular; consultar la lista, del Administrador |

**Dependencias que no se dibujan, para no saturar las vistas.** Todas son dependencias de uso, que
salen de llamadas `X.Instance.Operacion`:

| Desde | Hacia |
|---|---|
| GestorCompras | GestorPaquetes (`Validar`, `VerificarPreventa`), GestorFidelizacion (`CalcularDescuento`, `ObtenerCliente`, `ReevaluarNivel`, `AscenderSiCorresponde`), PasarelaSimulada (`Cobrar`), GeneradorQR (`NuevoCodigo`) |
| GestorFidelizacion | GestorPaquetes (subtotales), GestorSuscripciones (`TieneActiva`) |
| GestorPaquetes | GestorSuscripciones (`TieneActiva`) |
| GestorListaEspera | GestorEventos (`EstaAgotado`, `ObtenerPorId`), GestorSuscripciones (`TieneActiva`) |
| GestorCompras, GestorFidelizacion, GestorPaquetes, GestorSuscripciones, GestorIngresos, GestorUsuarios, ReporteUsuariosPorNivel | GestorParametros: leen los valores de las reglas |
| RepositorioCompras | RepositorioEntradas, RepositorioSectores (dentro de la misma transacción) |
| RepositorioEventos | RepositorioCompras, RepositorioSectores |
| RepositorioSectores | RepositorioCompras |

**Justificación, vista 7:** cada repositorio → `RepositorioBase` es una generalización
(`: RepositorioBase`). `RepositorioBase` → `Conexion` es una dependencia de uso
(`AbrirConexion` llama a `Conexion.Instance.Crear()`).

---

## 3. Patrones de diseño

| Vista | Fuente e imagen (`docs/diagramas/…`) | Insertar a | Letra |
|---|---|---|---|
| 8a. Singleton y Repository | `clases-8a-patron-singleton-repository` | **16,0 cm** (14,9) | 11,2 pt |
| 8b. Strategy | `clases-8b-patron-strategy` | **15,5 cm** (20,6) | 12,0 pt |
| 8c. Jerarquía de Reporte | `clases-8c-jerarquia-reportes` | **16,0 cm** (8,4) | 11,1 pt |

**Dónde va:** a continuación de las vistas de clases, con el título «Patrones de diseño». **Es
una sección nueva:** el documento hoy no la tiene. La 8c es baja y puede compartir página con el
texto.

**Texto para el documento:**

> El sistema aplica cuatro patrones de diseño. Cada uno se muestra con su estructura y con un caso
> real del código.
>
> **Singleton.** Todos los gestores de negocio, los repositorios de datos, la conexión, la pasarela
> simulada y el generador de códigos QR tienen una única instancia: un campo estático privado
> `_instance`, un constructor privado y una propiedad estática `Instance` que crea la instancia la
> primera vez que se pide (veintiocho clases en total). La implementación no usa bloqueo, porque la
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
| Insertar a | **16,0 cm** de ancho (22,1 de alto, página entera) |
| Letra impresa | 10,4 pt |

**Dónde va:** página 87, debajo de «Diagrama de estados – Ciclo de vida de la entrada».
Reemplaza la figura de ahí. Como ocupa la página entera, el título pasa a la página nueva.

**Texto para el documento:**

> Una entrada tiene cuatro estados: Pendiente, Confirmada, Reembolsada y Vencida. Son los mismos en
> el diagrama, en la enumeración `EstadoEntrada`, en la restricción `CK_Entradas_Estado` de la
> tabla `Entradas` y en las sentencias de la capa de datos. La entrada nace Pendiente al iniciarse
> la compra, con el cupo del sector reservado; pasa a Confirmada cuando el pago se aprueba y recibe
> su código QR; pasa a Vencida si vence el plazo de reserva o se da de baja el evento o el sector;
> y pasa a Reembolsada si el cliente la devuelve con seguro contratado y con más de 48 horas de
> anticipación.
>
> El ingreso al evento no es un estado de venta: la entrada sigue Confirmada y solo se registra la
> fecha y hora de la primera lectura del código QR. Por eso se modela como un subestado de
> Confirmada, y no como un quinto estado. Una segunda lectura del mismo código se rechaza (RN-18).

**Justificación de transiciones:**

| Transición | Disparador | Regla | Código |
|---|---|---|---|
| inicio → Pendiente | el cliente inicia la compra | RN-08, RN-15 | `GestorCompras.Iniciar`: `Estado = EstadoEntrada.Pendiente` |
| Pendiente → Pendiente | pago rechazado: devuelve el cupo | RN-32 | `GestorCompras.Liberar` → `RepositorioCompras.Liberar` |
| Pendiente → Confirmada | pago aprobado: asigna el QR | RN-17, RN-18 | `RepositorioEntradas.cs:147`: `SET Estado = 'Confirmada' … WHERE Estado = 'Pendiente'` |
| Pendiente → Vencida | vence el plazo, o baja del evento o del sector | RN-15 | `RepositorioCompras.MarcarVencidas` y `VencerAbiertas` |
| Confirmada → Reembolsada | el cliente la devuelve | RN-28, RN-29 | `RepositorioCompras.cs:203`; el plazo, en `GestorCompras.Reembolsar` |
| Sin ingreso → Ingresada | primera lectura del QR dentro de la ventana | RN-18 | `RepositorioEntradas.cs:31-33`: `SET FechaIngreso … WHERE Estado = 'Confirmada' AND FechaIngreso IS NULL` |
| Vencida → fin | baja del evento: se borra la fila | — | `RepositorioEventos.cs:172` |

**Qué cambió respecto del diagrama viejo:**
- **Desaparecen** «Disponible», «Reservada», «Vendida» y «Cancelada», que no existen.
- **Aparece** «Vencida».
- **El reembolso** ya no lo dispara un evento cancelado, sino el cliente.

---

## 6. Entidad-relación

| | Vista 1: usuarios y catálogo | Vista 2: compra, pago y entradas |
|---|---|---|
| Fuente | `docs/diagramas/fuentes/er-1-usuarios-catalogo.puml` | `docs/diagramas/fuentes/er-2-compras.puml` |
| Imagen | `docs/diagramas/er-1-usuarios-catalogo.png` | `docs/diagramas/er-2-compras.png` |
| Insertar a | **15,4 cm** (23,0 de alto, página entera) | **14,6 cm** (23,0 de alto, página entera) |
| Letra impresa | 9,0 pt | 9,3 pt |

**Dónde va:** página 89, debajo de «DER – Diagrama entidad-relación». **Esa página hoy tiene el
título y nada más**: el diagrama se perdió al armar la versión 2 del PDF. La vista 2 va en la
página siguiente.

**Texto para el documento:**

> El diagrama entidad-relación se obtiene del script de creación de la base
> (`BaseDatos/01-esquema.sql`) y muestra sus quince tablas en dos vistas. La primera reúne
> usuarios, locales, eventos, sectores, servicios, suscripciones, lista de espera y parámetros; la
> segunda, la compra con sus pagos, entradas, servicios contratados y pólizas. Las tablas que
> aparecen en las dos vistas se muestran completas en una sola y, en la otra, solo con su clave.
>
> Cada columna indica su tipo; el asterisco marca las obligatorias (`NOT NULL`), y las marcas PK,
> FK, UQ y CK señalan la clave primaria, las claves foráneas, las restricciones de unicidad y las de
> verificación. Las relaciones usan la notación de pata de gallo, con la cardinalidad escrita en
> cada extremo. Cliente y Propietario comparten la clave de Usuario (una tabla por tipo de
> usuario). La relación entre Eventos y Servicios es de muchos a muchos y se resuelve con la tabla
> intermedia ServiciosEvento. La contraseña se guarda como `PBKDF2$iteraciones$sal$hash`, nunca en
> texto plano.

**Restricciones que no entran en la figura** (van en el texto o en una tabla debajo):

| Tabla | Restricción | Definición |
|---|---|---|
| Usuario | `UQ_Usuario_Mail` | `UNIQUE (Mail)` |
| Locales | `CK_Locales_Capacidad` | `Capacidad > 0` |
| Eventos | `CK_Eventos_Estado` | `Estado IN ('Borrador','Publicado','Suspendido')` |
| Entradas | `CK_Entradas_Estado` | `Estado IN ('Pendiente','Confirmada','Reembolsada','Vencida')` |
| Entradas | `UX_Entradas_CodigoQR` | único cuando `CodigoQR IS NOT NULL` |
| Compras | `CK_Compras_Estado` | `Estado IN ('Pendiente','Confirmada','Reembolsada','Vencida')` |
| Pagos | `CK_Pagos_Estado`, `CK_Pagos_Ultimos4` | `Estado IN ('Aprobado','Rechazado')`; cuatro dígitos |
| Suscripciones | `UX_Suscripcion_Activa` | una sola activa por usuario (índice único filtrado, RN-25) |
| ListaEspera | `UX_ListaEspera` (UQ1 en la figura) | `UNIQUE (ID_Evento, ID_Usuario)` (RN-36) |

**Justificación de relaciones:**

| Relación | Cardinalidad | Clave foránea | Opcionalidad |
|---|---|---|---|
| Usuario — Cliente | 1 : 0..1 | `Cliente.ID` → `Usuario.ID` (también PK) | obligatoria |
| Usuario — Propietario | 1 : 0..1 | `Propietario.ID` → `Usuario.ID` (también PK) | obligatoria |
| Propietario — Locales | 1 : 0..* | `Locales.PropietarioID` **sin FK declarada** (línea punteada) | `NOT NULL` |
| Locales — Eventos | 1 : 0..* | `Eventos.LocalID` | `NOT NULL` |
| Eventos — Sectores | 1 : 0..* | `Sectores.EventoID` | `NOT NULL` |
| Eventos — ServiciosEvento — Servicios | N : M | PK compuesta `(ID_Evento, ID_Servicio)` | — |
| Usuario — Suscripciones | 1 : 0..* | `Suscripciones.ID_Usuario` | `NOT NULL` |
| Eventos — ListaEspera | 1 : 0..* | `ListaEspera.ID_Evento` | `NOT NULL` |
| Usuario — ListaEspera | 1 : 0..* | `ListaEspera.ID_Usuario` | `NOT NULL` |
| Usuario — Compras | 1 : 0..* | `FK_Compras_Usuario` | `NOT NULL` |
| Compras — Pagos | 1 : 0..* | `FK_Pagos_Compras` | `NOT NULL` |
| Compras — Entradas | 0..1 : 0..* | `FK_Entradas_Compras` | admite NULL |
| Usuario — Entradas | 1 : 0..* | `FK_Entradas_Usuario` | `NOT NULL` |
| Eventos — Entradas | 1 : 0..* | `Entradas.ID_Evento` | `NOT NULL` |
| Sectores — Entradas | 0..1 : 0..* | `FK_Entradas_Sectores` | admite NULL |
| Compras — CompraServicios | 1 : 0..* | `CompraServicios.ID_Compra` | `NOT NULL` |
| Servicios — CompraServicios | 1 : 0..* | `CompraServicios.ID_Servicio` | `NOT NULL` |
| Entradas — Seguros | 1 : 0..* | `Seguros.ID_Entrada` | `NOT NULL` |

`Parametros` no tiene relaciones: guarda los valores de las reglas por clave.

---

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
| 9. Robustez | **No se entregó todavía** |
| 10. Secuencia que accede a la base o no coincide | **Cumple:** la vista habla con gestores, y cada mensaje es una llamada real (tabla de la ficha 4) |
| 11. Clases de software en el dominio | **No aplica todavía:** el modelo de dominio no se rehízo |
| 12. Sistema como actor | **Cumple:** la pasarela simulada dejó de ser actor |
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
| Modelo de dominio conceptual y dominio actualizado | 86 y 88 | Tienen «puntos», carrito, NivelUsuario como clase y Entrada heredando de ProductoServicio, nada de lo cual existe |
| Robustez | 88 | Es un diagrama de secuencia con íconos de robustez |
| Actividad | 88-89 | Muestra el descuento por suscripción, «actualizar puntos» y recomendaciones, que no están construidas |
| Secuencias de los otros casos de uso | — | La Carpeta modelo pide una por caso de uso |

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
