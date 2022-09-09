using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using PitStopAutoShop.Web.Data.Repositories;
using PitStopAutoShop.Web.Helpers;
using SelectPdf;
using System;
using System.IO;
using System.Threading.Tasks;
using Vereyon.Web;

namespace PitStopAutoShop.Web.Controllers
{
    public class PdfController : Controller
    {
        private readonly ICompositeViewEngine _compositeViewEngine;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMailHelper _mailHelper;
   

        public PdfController(ICompositeViewEngine compositeViewEngine,
            IInvoiceRepository invoiceRepository,
            IMailHelper mailHelper
        )
        {
            _compositeViewEngine = compositeViewEngine;
            _invoiceRepository = invoiceRepository;
            _mailHelper = mailHelper;
           
        }

        public async Task<IActionResult> Invoice(int? id, bool sendEmail)
        {
            if(id == null)
            {
                return NotFound();
            }

            var invoice = await _invoiceRepository.GetInvoiceDetailsByIdAsync(id.Value);

            using(var stringWriter = new StringWriter())
            {
                var viewResult = _compositeViewEngine.FindView(ControllerContext, "_Invoice", false);

                if(viewResult == null)
                {
                    throw new ArgumentNullException("View cannot be found");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
                viewDictionary.Model = invoice;

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    viewDictionary,
                    TempData,
                    stringWriter,
                    new HtmlHelperOptions()
                    );
               
                await viewResult.View.RenderAsync(viewContext);

                var htmlToPdf = new HtmlToPdf(1000, 1414);
                htmlToPdf.Options.DrawBackground = true;

                var pdf = htmlToPdf.ConvertHtmlString(stringWriter.ToString());                
                var pdfBytes = pdf.Save();

                if(sendEmail == true)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\tempData\invoice"+invoice.Id+".pdf");                 
                    FileStream fs = new FileStream(path, FileMode.Create);

                    using (var streamWriter = new StreamWriter(fs))
                    {
                        await streamWriter.BaseStream.WriteAsync(pdfBytes, 0, pdfBytes.Length);
                    }

                    Response response = _mailHelper.SendEmail(invoice.Customer.Email,"Invoice Pitstop Autoshop Lisbon",$"<h1>Service Invoice</h1>" +
                        $" Mr/Mrs {invoice.Customer.FullName},<br>you can find attached to this email the invoice corresponding to the service done on " +
                        $"your {invoice.Vehicle.Brand.Name} {invoice.Vehicle.Model.Name} with the following plate number: {invoice.Vehicle.PlateNumber} .<br>" +
                        $"<br> Thanks for trusting in our services! <br> Best regards, <br> Pitstop Autoshop Lisbon ", path);

                    if(response.IsSuccess == true)
                    {                        
                        System.IO.File.Delete(path);
                    }
                }  
                
                return File(pdfBytes, "application/pdf");

            }            
        }
    }
}

