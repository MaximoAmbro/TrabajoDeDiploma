using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Visual
{
    /// <summary>Validates and stores event covers as compact JPEG data.</summary>
    public static class ImagenEventoCodec
    {
        public const int MaximoArchivoBytes = 10 * 1024 * 1024;
        public const int MaximoGuardadoBytes = 512 * 1024;
        public const int AnchoMaximo = 1280;
        public const int AltoMaximo = 720;

        public static byte[] Preparar(Stream entrada)
        {
            if (entrada == null || !entrada.CanRead) throw new ArgumentException("Elegí un archivo de imagen válido.");
            if (entrada.CanSeek && entrada.Length > MaximoArchivoBytes)
                throw new ArgumentException("La imagen supera el límite de 10 MB. Elegí una imagen más liviana.");

            try
            {
                using (var memoria = new MemoryStream())
                {
                    entrada.CopyTo(memoria);
                    if (memoria.Length > MaximoArchivoBytes)
                        throw new ArgumentException("La imagen supera el límite de 10 MB. Elegí una imagen más liviana.");
                    memoria.Position = 0;
                    Image originalImagen;
                    try { originalImagen = Image.FromStream(memoria, true, true); }
                    catch (Exception ex) when (ex is ArgumentException || ex is OutOfMemoryException || ex is ExternalException)
                    {
                        throw new ArgumentException("El archivo no contiene una imagen válida o no se pudo leer. Elegí una imagen PNG o JPEG.", ex);
                    }
                    using (var original = originalImagen)
                    {
                        if (original.Width < 1 || original.Height < 1 || (long)original.Width * original.Height > 40000000L)
                            throw new ArgumentException("La imagen tiene dimensiones demasiado grandes para usarla como portada.");
                        double escala = Math.Min(1d, Math.Min((double)AnchoMaximo / original.Width, (double)AltoMaximo / original.Height));
                        int ancho = Math.Max(1, (int)Math.Round(original.Width * escala));
                        int alto = Math.Max(1, (int)Math.Round(original.Height * escala));
                        using (var reducida = new Bitmap(ancho, alto, PixelFormat.Format24bppRgb))
                        using (var grafico = Graphics.FromImage(reducida))
                        {
                            grafico.Clear(Color.White);
                            grafico.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            grafico.SmoothingMode = SmoothingMode.HighQuality;
                            grafico.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            grafico.DrawImage(original, new Rectangle(0, 0, ancho, alto));
                            ImageCodecInfo jpeg = ImageCodecInfo.GetImageEncoders().First(x => x.MimeType == "image/jpeg");
                            foreach (long calidad in new long[] { 85, 75, 65, 55, 45, 35 })
                            {
                                using (var salida = new MemoryStream())
                                using (var parametros = new EncoderParameters(1))
                                {
                                    parametros.Param[0] = new EncoderParameter(Encoder.Quality, calidad);
                                    reducida.Save(salida, jpeg, parametros);
                                    if (salida.Length <= MaximoGuardadoBytes) return salida.ToArray();
                                }
                            }
                        }
                    }
                }
                throw new ArgumentException("No se pudo reducir la imagen a un tamaño adecuado. Elegí otra imagen.");
            }
            catch (ArgumentException) { throw; }
            catch (Exception ex) when (ex is OutOfMemoryException || ex is ExternalException || ex is InvalidDataException || ex is IOException)
            {
                throw new ArgumentException("El archivo no contiene una imagen válida o no se pudo leer. Elegí una imagen PNG o JPEG.", ex);
            }
        }
    }
}
