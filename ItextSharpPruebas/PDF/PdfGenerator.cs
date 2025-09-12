using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.codec.wmf;
using iTextSharp.text.pdf.draw;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace Proyecto.Services
{
    public class PdfGenerator
    {
        public byte[] GeneratePdf()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (MemoryStream ms = new MemoryStream())
            {
                using (Document doc = new Document(PageSize.LETTER, 40, 40, 40, 40))
                {

                    // Si se quiere usar una fuente TTF personalizada
                    //string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                    //BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    //Font customFont = new Font(bf, 12, Font.NORMAL, BaseColor.BLUE);

                    //doc.Add(new Paragraph("Texto con Arial desde archivo TTF", customFont));



                    PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                    doc.Open();

                    // Página 1: contenido normal
                    doc.Add(new Paragraph("Este es el contenido de la primera página"));
                    doc.Add(new Paragraph("Generado con iTextSharp en .NET"));


                    //string urlImagen = "https://img.freepik.com/vector-gratis/nina-feliz-mariposa_1450-103.jpg?semt=ais_hybrid&w=740&q=80"; //por URL
                    string urlImagen = "E:\\JoseV\\Descargas\\link.jpeg"; //por Archivo Local
                    // Crear instancia de imagen
                    Image imagen = Image.GetInstance(urlImagen);

                    // Configurar tamaño (opcional)
                    imagen.ScaleToFit(300f, 200f); // Ancho máximo 300, alto máximo 200

                    // Centrar imagen
                    imagen.Alignment = Element.ALIGN_CENTER;
                    imagen.SpacingBefore = 10;
                    imagen.SpacingAfter = 10;

                    // Agregar al documento
                    doc.Add(imagen);

                    LineSeparator line = new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, -2);
                    doc.Add(new Chunk(line));

                    // Agregar descripción
                    Paragraph descripcion = new Paragraph("Imagen cargada desde archivo local o URL segun sea el caso");
                    descripcion.Alignment = Element.ALIGN_CENTER;
                    doc.Add(descripcion);





                    // Página 2: contenido HTML
                    string html = @"
                        <html>
                            <head>
                                <style>
                                    body { font-family: Arial; font-size: 12pt; }
                                    h1 { color: darkblue; text-align: center; }
                                    p { color: black; }
                                    .resaltado { color: red; font-weight: bold; }
                                </style>
                            </head>
                            <body>
                                <h1>Página generada desde HTML</h1>
                                <p>Este es un párrafo con <span class='resaltado'>estilo</span>.</p>
                                <table border='1' style='width:100%; border-collapse: collapse;'>
                                    <tr><th>Columna 1</th><th>Columna 2</th></tr>
                                    <tr><td>Dato A</td><td>Dato B</td></tr>
                                    <tr><td>Dato C</td><td>Dato D</td></tr>
                                </table>
                            </body>
                        </html>";

                    HtmlToPdfPage.AddHtmlPage(doc, writer, html);




                    doc.Close();
                }

                return ms.ToArray(); // el PDF final en byte[]
            }
        }
        // Método para agregar imagen local
        public static void AgregarImagenLocal(Document document, string rutaImagen)
        {
            try
            {
                if (File.Exists(rutaImagen))
                {
                    // Crear instancia de imagen
                    Image imagen = Image.GetInstance(rutaImagen);

                    // Configurar tamaño (opcional)
                    imagen.ScaleToFit(300f, 200f); // Ancho máximo 300, alto máximo 200

                    // Centrar imagen
                    imagen.Alignment = Element.ALIGN_CENTER;
                    imagen.SpacingBefore = 10;
                    imagen.SpacingAfter = 10;

                    // Agregar al documento
                    document.Add(imagen);

                    // Agregar descripción
                    Paragraph descripcion = new Paragraph("Imagen cargada desde archivo local");
                    descripcion.Alignment = Element.ALIGN_CENTER;
                    document.Add(descripcion);
                }
                else
                {
                    // Si no existe la imagen, agregar texto alternativo
                    Paragraph error = new Paragraph("No se pudo cargar la imagen local: archivo no encontrado");
                    document.Add(error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar imagen local: {ex.Message}");
            }
        }

        // Método para agregar imagen desde URL
        public static void AgregarImagenDesdeURL(Document document, string urlImagen)
        {
            try
            {
                // Descargar imagen desde URL
                using (WebClient webClient = new WebClient())
                {
                    byte[] imageBytes = webClient.DownloadData(urlImagen);

                    // Crear imagen desde bytes
                    Image imagen = Image.GetInstance(imageBytes);

                    // Configurar imagen
                    imagen.ScaleToFit(250f, 150f);
                    imagen.Alignment = Element.ALIGN_CENTER;
                    imagen.SpacingBefore = 20;
                    imagen.SpacingAfter = 10;

                    // Agregar al documento
                    document.Add(imagen);

                    // Agregar descripción con URL
                    Paragraph descripcion = new Paragraph($"Imagen descargada desde: {urlImagen}");
                    descripcion.Alignment = Element.ALIGN_CENTER;
                    document.Add(descripcion);
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine($"Error al descargar imagen desde URL: {ex.Message}");
                Paragraph error = new Paragraph($"No se pudo cargar la imagen desde: {urlImagen}");
                document.Add(error);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar imagen desde URL: {ex.Message}");
            }
        }
    }
}
