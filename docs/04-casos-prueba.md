# Casos de prueba — lógica de fidelización

Proyecto `Pruebas/` con MSTest. CP-01 a CP-04 se resuelven con `[DataTestMethod]`.

> **Los diez casos son pruebas de integración contra SQL Server, no pruebas unitarias.**
> No hay ningún punto de inyección: `GestorParametros` lee los valores de las reglas de la
> tabla `Parametros` por `RepositorioParametros`, y siete archivos de `Repo/` dependen de él.
> Hasta CP-01 (Bronce, 0 %) necesita la base para resolver `PorcentajePara`. Se decidió no
> abrir un seam para las pruebas: cada caso usa la base real, por los mismos gestores que usa
> la aplicación.
>
> **Cómo se prepara la base.** `Pruebas/BaseDePrueba.cs`, en `[AssemblyInitialize]`, una vez
> por corrida: borra `BddEventAura_Pruebas` si existe (antes cierra el pool de conexiones y la
> pone en `SINGLE_USER`), la crea de nuevo desde `master` y le aplica `01-esquema.sql` y
> `02-semilla.sql`, partidos por las líneas que son exactamente `GO`. Después recarga
> `GestorParametros`. Se niega a correr si la cadena del `App.config` de `Pruebas/` no apunta a
> un catálogo terminado en `_Pruebas`: nunca toca la base de desarrollo.
>
> **Aislamiento.** Sin paralelismo (`[assembly: DoNotParallelize]`): comparten la base y los
> singletons. Cada prueba arma sus datos con `Pruebas/Fake.cs`, que marca todo lo que crea
> (mails `test…@pruebas.eventaura.com`, locales y servicios «test …») y lo borra por esa marca
> antes de cada prueba y al final. Nunca modifica la semilla. La sesión (`GestorSesion`) se
> cierra antes y después de cada prueba.
>
> **Cómo correrlas.** Tarea «Probar» de VS Code: compila con el MSBuild de Visual Studio y
> corre `vstest.console.exe`, ubicado con `vswhere`. `dotnet test` no sirve: no puede compilar
> los `.resx` de `Visual.csproj`, que es un proyecto de .NET Framework.
>
> Además de los diez casos y la verificación de referencia, `Pruebas/PruebasFormularios.cs`
> verifica RNF-03: ningún formulario declara más de 790 px de alto (ver CLAUDE.md).

| CP | Condición de entrada | Resultado esperado | Reglas |
|---|---|---|---|
| CP-01 | Cliente con 2 compras confirmadas en 12 meses | Bronce, descuento 0 % | RN-19, RN-20 |
| CP-02 | Cliente con 3 compras confirmadas | Plata, descuento 5 % | RN-19, RN-20 |
| CP-03 | Cliente con 6 compras confirmadas | Plata; no asciende | RN-19 |
| CP-04 | Cliente con 7 compras confirmadas | Oro, descuento 7,5 % | RN-19, RN-20 |
| CP-05 | Paquete Oro con entradas $40.000 y hospedaje $30.000 | Descuento $3.000; el hospedaje no se descuenta | RN-21 |
| CP-06 | Cliente con 7 compras, una hace 13 meses | Plata: la antigua queda fuera de la ventana | RN-19 |
| CP-07 | Cambia la tarifa con una compra ya iniciada | Se factura el precio congelado | RN-16 |
| CP-08 | Cliente con 6 compras realiza la séptima | La séptima se cobra al 5 %; Oro rige desde la octava | RN-22 |
| CP-09 | Cliente Oro con una compra reembolsada | Contador baja a 6; el nivel se mantiene hasta la reevaluación | RN-23, RN-24 |
| CP-10 | Cliente Oro con suscripción activa | Descuento 7,5 %; la suscripción no suma porcentaje | RN-37 |

## Verificación numérica de referencia

Cliente Plata, 2 entradas Campo a $25.000, hospedaje $45.000:

| Paso | Concepto | Importe |
|---|---|---|
| 1 | Base de cálculo, solo entradas | $50.000 |
| 2 | Porcentaje nivel Plata | 5 % |
| 3 | Monto del descuento | $2.500 |
| 4 | Subtotal de entradas | $47.500 |
| 5 | Hospedaje, sin descuento | $45.000 |
| 6 | Cargo por servicio, 15 % sobre el subtotal | $7.125 |
| 7 | **Total a pagar** | **$99.625** |

Si el cálculo no da exactamente $99.625, hay un error en la implementación.

## Hallazgos de usabilidad a contemplar en la UI

| # | Hallazgo | Acción | Regla |
|---|---|---|---|
| H-01 | No se entiende por qué el servicio no recibe descuento | Leyenda junto al total: «El descuento se aplica sobre el valor de las entradas» | RN-21 |
| H-02 | La vigencia de la reserva no está a la vista | Temporizador fijo en el encabezado desde la pantalla de pago | RN-15 |
| H-03 | El selector no informa el cupo restante | Mostrar «Quedan N entradas» junto a cada sector | RN-08 |
| H-04 | La advertencia del seguro pasa inadvertida | Confirmación explícita al avanzar sin contratarlo | RN-28 |
| H-05 | El indicador de nivel no se comprende | Enlace «¿Cómo funcionan los niveles?» | RN-19 |
| H-06 | «Volver» se interpreta como cancelar todo | Renombrar a «Volver al listado» y pedir confirmación | — |
