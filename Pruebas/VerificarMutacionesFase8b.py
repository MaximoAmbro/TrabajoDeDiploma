"""Comprobación negativa de las defensas de Fase 8b; restaura cada fuente en finally."""
from pathlib import Path
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[1]
MSBUILD = Path(r"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe")
VSTEST = Path(r"C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe")
DLL = ROOT / "Pruebas/bin/Debug/net48/Pruebas.dll"
LOG = ROOT / "TestResults/MutacionesFase8b.log"


def ejecutar(mutante, ruta, caso):
    archivo = ROOT / ruta
    original = archivo.read_bytes()
    mutado = mutante(original)
    if mutado == original:
        raise RuntimeError(f"No se aplicó la mutación de {caso}: cambió el texto fuente.")
    try:
        archivo.write_bytes(mutado)
        build = subprocess.run([str(MSBUILD), str(ROOT / "SistemaEntradas.sln"), "/t:Build", "/p:Configuration=Debug", "/m", "/v:minimal"], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
        if build.returncode:
            raise RuntimeError(f"No compiló el mutante {caso}:\n{build.stdout}")
        prueba = subprocess.run([str(VSTEST), str(DLL), f"/TestCaseFilter:FullyQualifiedName~PruebasSolicitudesOrganizador.{caso}", "/Logger:console;verbosity=minimal"], cwd=ROOT, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
        LOG.parent.mkdir(exist_ok=True)
        with LOG.open("a", encoding="utf-8") as salida:
            salida.write(f"\n===== {caso}; exit={prueba.returncode} =====\n{prueba.stdout}\n")
        if prueba.returncode == 0:
            raise RuntimeError(f"La prueba {caso} siguió verde con la defensa rota.")
        if "Correcto:     0" not in prueba.stdout and "Superado:     0" not in prueba.stdout and "Con error" not in prueba.stdout:
            raise RuntimeError(f"La corrida de {caso} falló sin confirmar el rojo esperado:\n{prueba.stdout}")
        print(f"ROJO CONFIRMADO: {caso}")
    finally:
        archivo.write_bytes(original)


def reemplazar(ruta, buscar, poner, caso, todos=False):
    origen = (ROOT / ruta).read_bytes()
    buscado = buscar.encode("utf-8")
    reemplazo = poner.encode("utf-8")
    if buscado not in origen:
        raise RuntimeError(f"No se encontró la defensa de {caso}.")
    return ejecutar(lambda b: b.replace(buscado, reemplazo) if todos else b.replace(buscado, reemplazo, 1), ruta, caso)


def main():
    (ROOT / "TestResults").mkdir(exist_ok=True)
    LOG.write_text("Mutaciones negativas fase 8b\n", encoding="utf-8")
    reemplazar("Datos/RepositorioSolicitudesOrganizador.cs",
               'const string sql = @"INSERT dbo.SolicitudesOrganizador',
               'const string sql = @"UPDATE dbo.Usuario SET TipoUsuario=\'Organizador\' WHERE ID=@UsuarioID; INSERT dbo.SolicitudesOrganizador',
               "CP184_SolicitudPendienteNoConcedePermisos")
    reemplazar("Repo/GestorSolicitudesOrganizador.cs", "GestorSesion.Instance.RequerirAdministrador();", "// defensa quitada para mutación", "CP187_ClienteNoPuedeAprobarSolicitud", todos=True)
    reemplazar("Repo/GestorSolicitudesOrganizador.cs", "GestorSesion.Instance.RequerirAdministrador();", "// defensa quitada para mutación", "CP190_OrganizadorNoPuedeRechazarSolicitudDePlataforma", todos=True)
    reemplazar("Visual/Propietario/frmRevisarSolicitudesOrganizador.cs", "GestorSesion.Instance.RequerirAdministrador();", "// defensa de interfaz quitada para mutación", "CP193_AccesoDeSolicitudVisibleYRevisionProtegida")
    reemplazar("Repo/GestorLocales.cs", "GestorSesion.Instance.RequerirOrganizadorDeLocal(idPropietario);", "// defensa de perfil quitada para mutación", "CP184_SolicitudPendienteNoConcedePermisos")
    reemplazar("Repo/GestorSolicitudesOrganizador.cs", "if (!CuitValido(solicitud.Cuit))", "if (false && !CuitValido(solicitud.Cuit))", "CP185_CuitConVerificadorIncorrectoSeRechaza")
    reemplazar("Repo/GestorSolicitudesOrganizador.cs", "if (!CbuAliasValido(solicitud.CbuAlias))", "if (false && !CbuAliasValido(solicitud.CbuAlias))", "CP186_CbuConVerificadorIncorrectoSeRechaza")
    reemplazar("Repo/GestorSolicitudesOrganizador.cs", "if (codigo == null || !codigo.Usado || codigo.VenceEl <= DateTime.UtcNow || !string.Equals(codigo.Destinatario, solicitud.Telefono, StringComparison.Ordinal))", "if (false)", "CP191_TelefonoSinConfirmarImpideLaSolicitud")
    reemplazar("Repo/GestorSolicitudesOrganizador.cs", "if (solicitud.Documentacion == null || solicitud.Documentacion.Length == 0 || solicitud.Documentacion.Length > 512 * 1024 || !CabeceraImagenValida(solicitud.Documentacion))", "if (false)", "CP192_DocumentacionQueNoEsImagenSeRechaza")
    archivo = ROOT / "Repo/GestorSolicitudesOrganizador.cs"
    original = archivo.read_bytes()
    anterior = 'if (string.IsNullOrWhiteSpace(motivo)) throw new ReglaNegocioException("RN-53", "Escribí el motivo del rechazo.");'.encode("utf-8")
    nuevo = b"if (false) throw new ReglaNegocioException(\"RN-53\", \"Escribi motivo mutante\");"
    if anterior not in original:
        raise RuntimeError("No se encontró la guarda de motivo.")
    # Mutar solo Rechazar, no la validación de motivo de Aprobar.
    ejecutar(lambda b: b.replace(anterior, nuevo, 1), "Repo/GestorSolicitudesOrganizador.cs", "CP188_RechazoSinMotivoSeRechaza")
    print("Todas las defensas negativas de 8b dieron rojo al retirar su guarda.")


if __name__ == "__main__":
    try:
        main()
    except Exception as error:
        print(error, file=sys.stderr)
        sys.exit(1)
