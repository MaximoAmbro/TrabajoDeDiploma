# Hoja de la defensa

Detalle y evidencia de cada punto: `docs/AUDITORIA-RIESGOS-DEFENSA.md`.

## Qué llevar

- [ ] **La carpeta `Visual\bin\Debug` entera**, con `Web\`, `runtimes\` y todas las DLL, en un
      pendrive. Copiada desde un pendrive y no desde un `.zip` bajado, para que SmartScreen no la
      bloquee.
- [ ] **`BaseDatos\`:** `01-esquema.sql`, `02-semilla.sql` y `semilla-demostracion.sql`.
- [ ] **Un plan B:** tu notebook, con la base de demostración ya cargada y la aplicación probada
      esa mañana.
- [ ] **Las cuentas, anotadas en papel:**
  - `admin@eventaura.com` / `Admin1234`;
  - `plata@eventaura.com` y los demás clientes / `Cliente123`;
  - `organizador@eventaura.com` / `Cliente123`.

## En la PC de la defensa, antes de que llegue el jurado

1. [ ] **Windows 10 1903 o posterior, u 11.** .NET 4.8: en el registro,
       `HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full`, el valor `Release` tiene que ser
       528040 o más. Sin internet no se puede instalar.
2. [ ] **SQL Server 2017 o posterior, iniciado.** Con 2016 falla el panel del Administrador. Anotá el
       nombre de la instancia: `.\SQLEXPRESS`, `.` o `(localdb)\MSSQLLocalDB`.
3. [ ] **Base nueva** con un nombre que **termine en `_Demostracion`**, por ejemplo
       `BddEventAura_Demostracion`; si no, la semilla no carga. Correr en orden: `01-esquema.sql`,
       `02-semilla.sql` y `semilla-demostracion.sql`. No hace falta 04 a 16. No corras `03` si
       querés el panel prolijo.
4. [ ] **`Visual.exe.config`** (con el Bloc de notas): en `connectionString`, poner la instancia del
       paso 2 en `Data Source` y la base del paso 3 en `Initial Catalog`.
5. [ ] **Abrir `Visual.exe` y escribir algo en «Contraseña».** Si se ve en texto plano o la ventana
       tardó unos 15 segundos, **la aplicación no llega a la base**: revisá los pasos 2 a 4 antes de
       seguir (hasta que se corrija T-20).
6. [ ] **Entrar como Administrador.** El panel tiene que mostrar los indicadores y el gráfico, sin
       ningún aviso rojo. Si aparece el aviso rojo o el panel queda en blanco, falta un archivo
       (T-03): volvé a copiar la carpeta entera.
7. [ ] **Hacer una compra de prueba** y exportar un reporte. Si falla al generar el ticket,
       Documentos está bloqueado (Defender o OneDrive): usá el plan B.

## Durante la demostración

- **No te equivoques cinco veces con la contraseña:** la cuenta se bloquea 15 minutos. Si pasa,
  seguí con otra cuenta.
- **Quince minutos sin tocar la aplicación cierran la sesión.** Avisa 60 segundos antes; movés el
  mouse y sigue.
- **El pago reserva el cupo 10 minutos.**
- **Registrar una cuenta en vivo pide un código** que no se ve en la pantalla. Está en
  `%LOCALAPPDATA%\EventAura\BandejaSimulada`: tené esa carpeta abierta, o mostrá una cuenta ya
  creada.
- **Si algo falla:** el detalle queda en `%LOCALAPPDATA%\EventAura\errores.log`.
