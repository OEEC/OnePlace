using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QRCoder; 
using System.IO;

namespace OnePlace.Server.Helpers
{
    public class GenerarQR
    {
        public static string GenerarCode(string valor)
        {
            //instanciamos la clase qrcodegenertor
            QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();

            //se le pasa el valor que se convertira en qr 
            //Nivel de corrección de errores Q - Permite la recuperación de hasta el 25 % de la pérdida de datos
            QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(valor, QRCodeGenerator.ECCLevel.Q);

            //genera codigo para crear un png qrcode
            //este metodo si lo soporto blazor wasm del lado del cliente el que usa bitmap no por el system.drawing
            PngByteQRCode qRCode = new PngByteQRCode(qRCodeData);

            //convertimos los bytes en un grafico renderizado
            byte[] qrCodeImage = qRCode.GetGraphic(20);

            //convertimos los bytes en base 64 para poder verlo del lado del cliente
            var fileFormat = "image/png";           
            string model = $"data: {fileFormat}; base64,{Convert.ToBase64String(qrCodeImage)}";

            return model;
        }
    }
}
