using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace EVCharging.WebApi.Utils
{
    public static class QrCodeUtil
    {
        // PNG as base64 (no System.Drawing)
        public static string GeneratePngBase64(string payload, int pixelsPerModule = 20)
        {
            using var gen = new QRCodeGenerator();
            using var data = gen.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var pngQr = new PngByteQRCode(data);
            var bytes = pngQr.GetGraphic(pixelsPerModule);
            return Convert.ToBase64String(bytes); // "iVBORw0KGgo..." (no prefix)
        }

        // SVG string (tiny and crisp at any zoom)
        public static string GenerateSvg(string payload, int pixelsPerModule = 5)
        {
            using var gen = new QRCodeGenerator();
            using var data = gen.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var svgQr = new SvgQRCode(data);
            return svgQr.GetGraphic(pixelsPerModule); // "<svg.../>"
        }

        // If you want base64 *with* a data-URI prefix for easy embedding in <img>
        public static string GeneratePngDataUri(string payload, int ppm = 20)
            => "data:image/png;base64," + GeneratePngBase64(payload, ppm);
    }
}
