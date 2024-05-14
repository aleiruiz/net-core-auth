using QRCoder;

namespace XLocker.Helpers
{
    public static class GenerateQRCodeByServiceToken
    {
        public static byte[] Exec(string serviceToken)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(serviceToken, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }
    }
}
