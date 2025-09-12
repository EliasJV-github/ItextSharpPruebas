using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proyecto.Services;

namespace ItextSharpPruebas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportControllers : ControllerBase
    {


        // Constructor


        [HttpGet("pdf")]
        public IActionResult GetPdf()
        {
            PdfGenerator _pdfGenerator = new PdfGenerator();
            byte[] fileBytes = _pdfGenerator.GeneratePdf();
            string fileName = "ReporteFinal.pdf";

            return File(fileBytes, "application/pdf", fileName);
        }
    }
}
