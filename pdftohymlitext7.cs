using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.Kernel.Pdf;
using iText.Layout.Font;
using System;
using System.IO;

public class HtmlToPdfConverter
{
    private PdfDocument pdfDocument;

    public HtmlToPdfConverter(PdfDocument pdfDocument)
    {
        this.pdfDocument = pdfDocument;
    }

    /// <summary>
    /// Convierte HTML a una nueva página del documento PDF
    /// </summary>
    /// <param name="htmlContent">Contenido HTML como string</param>
    public void ConvertHtmlToNewPage(string htmlContent)
    {
        try
        {
            // Crear configuración para html2pdf
            ConverterProperties converterProperties = new ConverterProperties();
            
            // Configurar el font provider para soportar diferentes fuentes
            DefaultFontProvider fontProvider = new DefaultFontProvider(true, true, true);
            converterProperties.SetFontProvider(fontProvider);
            
            // Configurar para procesar imágenes base64
            converterProperties.SetBaseUri("");
            
            // Crear un MemoryStream temporal para el HTML convertido
            using (MemoryStream htmlStream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(htmlStream))
                {
                    writer.Write(htmlContent);
                    writer.Flush();
                    htmlStream.Position = 0;

                    // Convertir HTML a PDF y agregarlo al documento existente
                    HtmlConverter.ConvertToPdf(htmlStream, pdfDocument, converterProperties);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al convertir HTML a PDF: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Método alternativo que crea un PDF temporal y luego lo fusiona
    /// </summary>
    /// <param name="htmlContent">Contenido HTML como string</param>
    public void ConvertHtmlToNewPageAlternative(string htmlContent)
    {
        try
        {
            // Configurar propiedades del conversor
            ConverterProperties converterProperties = new ConverterProperties();
            DefaultFontProvider fontProvider = new DefaultFontProvider(true, true, true);
            converterProperties.SetFontProvider(fontProvider);

            // Crear un PDF temporal en memoria
            using (MemoryStream tempPdfStream = new MemoryStream())
            {
                // Convertir HTML a PDF temporal
                HtmlConverter.ConvertToPdf(htmlContent, tempPdfStream, converterProperties);
                
                // Leer el PDF temporal
                tempPdfStream.Position = 0;
                using (PdfDocument tempPdf = new PdfDocument(new PdfReader(tempPdfStream)))
                {
                    // Copiar todas las páginas del PDF temporal al documento principal
                    int numberOfPages = tempPdf.GetNumberOfPages();
                    for (int i = 1; i <= numberOfPages; i++)
                    {
                        tempPdf.CopyPagesTo(i, i, pdfDocument);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al convertir HTML a PDF (método alternativo): {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Preprocesa el HTML para asegurar compatibilidad con iText7
    /// </summary>
    /// <param name="htmlContent">HTML original</param>
    /// <returns>HTML procesado</returns>
    public static string PreprocessHtml(string htmlContent)
    {
        // Asegurar que el HTML tenga estructura completa
        if (!htmlContent.Trim().StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase) &&
            !htmlContent.Trim().StartsWith("<html", StringComparison.OrdinalIgnoreCase))
        {
            htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 20px;
        }}
        img {{
            max-width: 100%;
            height: auto;
        }}
    </style>
</head>
<body>
    {htmlContent}
</body>
</html>";
        }

        return htmlContent;
    }
}

// Ejemplo de uso
public class EjemploUso
{
    public void CrearPdfConHtml()
    {
        // Crear el documento PDF
        using (FileStream fileStream = new FileStream("documento.pdf", FileMode.Create))
        {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(fileStream)))
            {
                // Crear el conversor
                HtmlToPdfConverter converter = new HtmlToPdfConverter(pdfDoc);

                // HTML de ejemplo con imagen base64 y estilos
                string htmlContent = @"
<div style='text-align: center; margin: 20px;'>
    <h1 style='color: #2c3e50; font-size: 24px;'>Documento con Imagen</h1>
    <p style='color: #34495e; font-size: 14px; line-height: 1.6;'>
        Este es un ejemplo de HTML convertido a PDF con iText7.
    </p>
    <img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==' 
         alt='Imagen de ejemplo' style='border: 2px solid #3498db; padding: 10px;' />
    <div style='background-color: #ecf0f1; padding: 15px; margin-top: 20px; border-radius: 5px;'>
        <h2 style='color: #e74c3c;'>Información Importante</h2>
        <p>Este contenido se ha agregado como una nueva página al documento PDF.</p>
    </div>
</div>";

                // Preprocesar el HTML
                htmlContent = HtmlToPdfConverter.PreprocessHtml(htmlContent);

                // Convertir a nueva página
                converter.ConvertHtmlToNewPage(htmlContent);

                // Agregar más contenido HTML si es necesario
                string segundoHtml = @"
<div style='padding: 30px;'>
    <h2 style='color: #8e44ad;'>Segunda Página</h2>
    <p style='text-align: justify;'>
        Esta es otra página agregada al mismo documento PDF.
        Puedes llamar el método tantas veces como necesites.
    </p>
</div>";

                segundoHtml = HtmlToPdfConverter.PreprocessHtml(segundoHtml);
                converter.ConvertHtmlToNewPage(segundoHtml);
            }
        }

        Console.WriteLine("PDF creado exitosamente con contenido HTML");
    }
}

// Clase extendida con más funcionalidades
public class HtmlToPdfConverterExtended : HtmlToPdfConverter
{
    public HtmlToPdfConverterExtended(PdfDocument pdfDocument) : base(pdfDocument) { }

    /// <summary>
    /// Convierte HTML con configuración personalizada de página
    /// </summary>
    /// <param name="htmlContent">Contenido HTML</param>
    /// <param name="pageSize">Tamaño de página (ej: PageSize.A4)</param>
    /// <param name="margins">Márgenes personalizados</param>
    public void ConvertHtmlWithCustomPage(string htmlContent, 
        iText.Kernel.Geom.PageSize pageSize = null, 
        float[] margins = null)
    {
        try
        {
            ConverterProperties converterProperties = new ConverterProperties();
            DefaultFontProvider fontProvider = new DefaultFontProvider(true, true, true);
            converterProperties.SetFontProvider(fontProvider);

            // Configurar tamaño de página si se especifica
            if (pageSize != null)
            {
                converterProperties.SetPageSize(pageSize);
            }

            // Configurar márgenes si se especifican
            if (margins != null && margins.Length == 4)
            {
                // margins: [top, right, bottom, left]
                converterProperties.SetMargins(margins[0], margins[1], margins[2], margins[3]);
            }

            using (MemoryStream tempPdfStream = new MemoryStream())
            {
                HtmlConverter.ConvertToPdf(htmlContent, tempPdfStream, converterProperties);
                
                tempPdfStream.Position = 0;
                using (PdfDocument tempPdf = new PdfDocument(new PdfReader(tempPdfStream)))
                {
                    int numberOfPages = tempPdf.GetNumberOfPages();
                    for (int i = 1; i <= numberOfPages; i++)
                    {
                        tempPdf.CopyPagesTo(i, i, pdfDocument);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al convertir HTML con configuración personalizada: {ex.Message}", ex);
        }
    }
}
