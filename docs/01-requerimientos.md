# Requerimientos

## Funcionales

| ID | Requerimiento | MoSCoW | Pts | Criterio de aceptación |
|---|---|---|---|---|
| RF-01 | Registro de usuarios | Must | 2 | Valida formato de correo, registra y habilita el inicio de sesión. |
| RF-02 | Inicio de sesión | Must | 3 | Verifica credenciales y redirige al menú del perfil. |
| RF-03 | Búsqueda de eventos | Must | 5 | Filtra por nombre, categoría, fecha o ubicación. |
| RF-04 | Selección de entradas | Must | 5 | Las entradas se agregan correctamente al paquete. |
| RF-05 | Armado de paquetes personalizados | Must | 13 | Genera un resumen claro del paquete armado. **Requerimiento core.** |
| RF-06 | Simulación de pago seguro | Must | 8 | Muestra tokenización simulada y confirmación visual. |
| RF-07 | Confirmación de compra | Must | 5 | Mensaje de compra exitosa con identificación del pedido. |
| RF-08 | Aplicación de descuentos por rankeo | Should | 5 | Muestra el descuento aplicado en el resumen. |
| RF-09 | Recomendaciones personalizadas | Could | 8 | Al menos dos recomendaciones tras la compra. |
| RF-10 | Lista de espera para eventos agotados | Could | 3 | Registra al usuario e informa que será notificado. |
| RF-11 | Generación de reportes de ventas | Should | 8 | Tablas o gráficos con datos exportables. |
| RF-12 | Gestión de eventos | Must | 8 | Impide publicar sin sector con capacidad > 0; impide eliminar con ventas. |
| RF-13 | Contratación de suscripciones | Should | 5 | Impide dos suscripciones activas; beneficios en especie, sin descuento porcentual. |

Total: 78 puntos (1 punto = 4 horas). Must 49 (62,8 %), Should 18 (23,1 %), Could 11 (14,1 %).

## No funcionales

| ID | Requerimiento | MoSCoW | Detalle verificable |
|---|---|---|---|
| RNF-01 | Usabilidad | Must | El flujo completo de compra se resuelve en menos de 5 minutos. Línea base medida: 4 min 40 s. |
| RNF-02 | Experiencia de usuario | Must | Navegación fluida; acciones principales siempre visibles. |
| RNF-03 | Compatibilidad multipantalla | Should | Adaptable a notebook y monitor de escritorio. |
| RNF-04 | Seguridad conceptual | Must | HTTPS y tokenización representados; no se almacenan datos sensibles. |
| RNF-05 | Mantenibilidad | Must | Documentación que permita retomar el proyecto. |

## Casos de uso

| CU | Nombre | Origen |
|---|---|---|
| CU-01 | Comprar paquete | RF-03 a RF-08 |
| CU-02 | Gestionar eventos | RF-12 |
| CU-03 | Ver historial de compras | RF-08 |
| CU-04 | Registrarse | RF-01 |
| CU-05 | Iniciar sesión | RF-02 |
| CU-06 | Suscribirse al sistema | RF-13 |
| CU-07 | Gestionar listas de espera | RF-10 |
| CU-08 | Ver reportes de ventas | RF-11 |
