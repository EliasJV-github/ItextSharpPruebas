using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace Proyecto.Services
{
    public class HtmlToPdfPage
    {
        public static void AddHtmlPage(Document doc, PdfWriter writer, string html)
        {
            doc.NewPage(); // siempre empieza en nueva hoja
            using (StringReader sr = new StringReader(html))
            {
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);
            }
        }
    }
}
