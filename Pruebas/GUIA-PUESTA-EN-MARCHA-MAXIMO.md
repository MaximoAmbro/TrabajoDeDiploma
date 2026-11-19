# EventAura en una PC nueva

Guía para dejar una instalación limpia lista para la demostración. Usá una base nueva llamada `BddEventAura`; no ejecutes el esquema ni la semilla sobre una base que ya tenga información.

## 1. Instalar y obtener el proyecto

1. Instalá **SQL Server Express** con una instancia local (por ejemplo, `SQLEXPRESS`) y **SQL Server Management Studio (SSMS)** para ejecutar los scripts.
2. Instalá **Visual Studio Community 2022**, la carga de trabajo **Desarrollo de escritorio de .NET** y el **paquete de desarrollador de .NET Framework 4.8**. En Windows donde .NET Framework 4.8 no esté habilitado, instalá también su runtime.
3. Cloná el repositorio y cambiá a la rama `JC` (o a `main` si Juan Cruz te indica que uses esa). Abrí `SistemaEntradas.sln`.

## 2. Crear y preparar la base

En SSMS conectate a la instancia. En **Databases > New Database…** creá una base vacía llamada `BddEventAura`; elegila en el selector y ejecutá **un archivo por vez**, en este orden:

1. `BaseDatos/01-esquema.sql`
2. `BaseDatos/02-semilla.sql`
3. `BaseDatos/04-seguridad-ingreso.sql`
4. `BaseDatos/05-check-estado-entradas.sql`
5. `BaseDatos/06-prioridad-temporal.sql`
6. `BaseDatos/07-indice-unico-seguros.sql`
7. `BaseDatos/08-seguridad-cuenta-auditoria.sql`
8. `BaseDatos/09-imagenes-eventos.sql`
9. `BaseDatos/10-restringir-cantidad-vendida.sql`
10. `BaseDatos/11-perfiles-administrador-organizador.sql`
11. `BaseDatos/12-solicitudes-organizador.sql`
12. `BaseDatos/13-revision-primer-evento.sql`
13. `BaseDatos/14-primer-evento-aprobado.sql`
14. `BaseDatos/15-motivo-rechazo-evento.sql`
15. `BaseDatos/16-fk-local-propietario.sql`

El `03-datos-reporte.sql` es opcional: agrega un volumen grande de datos simulados para explorar los reportes; no hace falta para el recorrido de demostración. No ejecutes `01` ni `02` una segunda vez sobre una base con datos. Si SSMS muestra un error en cualquier script, detenete y guardá el mensaje; no sigas con el siguiente.

## 3. Configurar, compilar y abrir

En `Visual/App.config`, ajustá **solo en tu copia local** `Data Source` a la instancia instalada y `Initial Catalog` a `BddEventAura`. No publiques ese cambio ni reemplaces `Pruebas/App.config`: cada máquina conserva su propia configuración.

En Visual Studio, elegí `Visual` como proyecto de inicio y compilá la solución en `Debug`. Después iniciá la aplicación con **F5**. También podés abrir `Visual/bin/Debug/Visual.exe`.

Al iniciar, la aplicación debe abrir la pantalla de acceso sin aviso de esquema desactualizado. Si falta una migración, el aviso nombra el primer script requerido: ejecutá ese archivo en `BddEventAura`, cerrá y volvé a abrir EventAura.

## 4. Comprobar que quedó lista

- En el acceso, ingresá como cliente con `bronce@eventaura.com` / `Cliente123`; debe abrir su inicio y mostrar sus datos.
- Cerrá sesión y verificá los perfiles de gestión: `organizador@eventaura.com` / `Cliente123` y `admin@eventaura.com` / `Admin1234`.
- En el panel del Administrador, abrí Reportes y Auditoría. Deben cargar sin error; la semilla tiene pocos datos de reporte, por lo que el informe puede mostrar un estado vacío o pocas filas.
- No hace falta SMTP para la demostración. Los códigos de verificación y recuperación se escriben en la bandeja local simulada, indicada por la aplicación; abrí el archivo fuera de EventAura.

Si el acceso no llega a la pantalla o aparece el aviso de esquema, comprobá primero que la conexión apunta a `BddEventAura` y que ejecutaste los scripts en orden y sobre esa misma base.
