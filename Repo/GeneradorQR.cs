using System;
using QRCoder;

namespace Negocio
{
    // RN-18: un código por entrada, único e irrepetible. Un Guid ya es único; el índice
    // UX_Entradas_CodigoQR lo garantiza además en la base.
    public sealed class GeneradorQR
    {
        private const string Prefijo = "EA-";

        private static GeneradorQR _instance;

        public static GeneradorQR Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GeneradorQR();
                }
                return _instance;
            }
        }

        private GeneradorQR() { }

        // 35 caracteres: entra en Entradas.CodigoQR VARCHAR(64).
        public string NuevoCodigo()
        {
            return Prefijo + Guid.NewGuid().ToString("N").ToUpperInvariant();
        }

        // PNG del QR que codifica el código tal cual: es lo que lee el control de ingreso.
        public byte[] Imagen(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                throw new ArgumentException("El código del QR no puede estar vacío.");
            }
            using (var generador = new QRCodeGenerator())
            using (QRCodeData datos = generador.CreateQrCode(codigo, QRCodeGenerator.ECCLevel.Q))
            using (var qr = new PngByteQRCode(datos))
            {
                return qr.GetGraphic(20);
            }
        }
    }
}
