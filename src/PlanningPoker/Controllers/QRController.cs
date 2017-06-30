using MessagingToolkit.QRCode.Codec;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;

namespace ScrumPlanningPoker.Controllers
{
    public class QrController : Controller
    {
        //
        // GET: /QR/GetQr/123

        public ActionResult GetQr(int id)
        {
            var qe = new QRCodeEncoder {QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE, QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L, QRCodeVersion = 4};
            
            var bm = qe.Encode("http://www.planning-poker.nl/Home/Developer?sleutel=" + id);

            using (var ms = new MemoryStream())
            {
                bm.Save(ms, ImageFormat.Png);
                
                return File(ms.ToArray(), "image/png");
            }
        }

    }
}