# Encargo de la revisión de seguridad — BORRADOR

Lo escribe Claude Code a pedido de JC y sale de la revisión de SEG-1 a SEG-3 contra
[LISTA-REVISION-SEGURIDAD.md](LISTA-REVISION-SEGURIDAD.md).

- **No es un encargo vigente.** Pasa a `TAREAS-CODEX-PRUEBAS.md` cuando JC lo apruebe y Codex no
  esté trabajando.
- **La revisión no terminó.** Paró en el primer punto (V-3.5) y sigue después de la tanda de
  diagramas. Los ítems que salgan se agregan acá, ordenados por gravedad.
- Las horas son estimadas, de Codex.

**Reglas:**
- las de siempre: zona de Codex, la batería al terminar, sin commit ni push;
- ninguna prueba existente se debilita;
- el reporte trae las horas reales.

---

## Alta

### R-01. Cualquier código puede abrir una sesión de Administrador sin contraseña — 1 a 2 h

**Qué pasa.** `GestorSesion.Iniciar(Usuario usuario)` es **público** (`Repo/GestorSesion.cs:31`) y no
verifica nada: guarda el usuario que le pasen. Toda la autorización de SEG-3
(`RequerirAdministrador` y `RequerirUsuario`, `GestorSesion.cs:37-47`) le cree a ese objeto. Basta
con una línea:

```csharp
GestorSesion.Instance.Iniciar(new Usuario { Id = 1, TipoUsuario = "Propietario" });
```

Con ella se pasan los 29 controles de SEG-3 como Administrador.

**Segundo camino al mismo resultado.** `UsuarioActual` devuelve el mismo objeto guardado, y
`Usuario.TipoUsuario` tiene un setter público (`Entidade/Usuario.cs:16`). Un cliente con sesión
iniciada se convierte en Administrador con:

```csharp
GestorSesion.Instance.UsuarioActual.TipoUsuario = "Propietario";
```

**Por qué es Alta.** SEG-3 promete que el negocio verifica quién llama (RN-41, RN-42), y la
verificación se saltea con una llamada pública.

- **El matiz:** no se explota desde afuera de la aplicación. Hace falta ejecutar código adentro, y
  quien puede hacerlo ya tiene la base con la cuenta de Windows (límite anotado en el ítem 25 de
  `CORRECCIONES-DOCUMENTO.md`).
- **El riesgo concreto es el mismo que justificó SEG-3:** una pantalla nuestra que se equivoque.

**De dónde viene.**
- `Iniciar` ya era público antes de SEG-3 (commit `e700f1b`).
- Hoy lo llaman en un solo lugar del código de producción, `GestorUsuarios.Autenticar`
  (`Repo/GestorUsuarios.cs:72`), y desde las pruebas, `Fake.ComoCliente` y `Fake.ComoAdministrador`
  (`Pruebas/Fake.cs:24-31`).
- `Repo/AccesoPruebas.cs` (`InternalsVisibleTo("Pruebas")`) no es el problema: es justamente lo
  que permite cerrarlo sin romper las pruebas.

**Arreglo propuesto:**
1. `GestorSesion.Iniciar` pasa a `internal`. Lo siguen viendo `Autenticar`, que está en el mismo
   proyecto, y `Fake`, por `InternalsVisibleTo`.
2. La sesión deja de exponer un objeto modificable. Hay dos formas:
   - `UsuarioActual` devuelve una copia;
   - la sesión guarda solo el `Id` y el perfil, en campos de solo lectura, y `RequerirAdministrador`
     y `RequerirUsuario` comparan contra esos campos.

   **Recomiendo la segunda:** una copia sigue permitiendo modificar la que se devolvió, aunque
   ya no afecte a la sesión.
3. **Prueba nueva, CP con el número que siga:**
   - comprueba por reflexión que `GestorSesion.Iniciar` no es público;
   - comprueba que modificar el `Usuario` que devuelve `UsuarioActual` no cambia el resultado de
     `RequerirAdministrador`.

   Tiene que fallar contra el código de hoy.

**Hecho cuando:**
- las dos líneas de arriba ya no compilan desde `Visual`, o compilan sin efecto sobre la sesión;
- la batería da los mismos resultados que antes más el caso nuevo.

**Relación con SEG-4.** SEG-4 cerró que el perfil se asignara desde un método público de alta.
R-01 es el mismo problema en la sesión.

---

## Pendiente de la revisión

Se completa cuando se retome la revisión: el inventario de SEG-3, la atomicidad de SEG-2, la
medición del canal de tiempo de SEG-1, las banderas rojas de la batería y la mutación.

**Dato ya verificado: entrar al evento y después pedir el reembolso.** Hoy no se puede, pero por
la combinación de dos parámetros y no por una regla explícita:
- la devolución exige que falten más de `SEGURO_HORAS_COBERTURA` (48 h) para el evento (RN-29);
- el ingreso abre `INGRESO_HORAS_ANTES` (6 h) antes (RN-18);
- el SQL de la devolución no mira `FechaIngreso` (`Datos/RepositorioCompras.cs:203`).

Si alguien sube `INGRESO_HORAS_ANTES` a 48 o más, el caso se abre. La decisión de escribirlo como
regla es de JC; no se agrega hasta que la confirme y Codex la implemente con su prueba.
