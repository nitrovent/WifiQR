using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fclp;
using QRCoder;
using static QRCoder.PayloadGenerator;

namespace WifiQR
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new FluentCommandLineParser();
            string ssid = "";
            string password = "";
            string filename = "wifi.png";

            parser.IsCaseSensitive = false;
            parser                
                .Setup<string>('s', "SSID")
                .Callback(v => ssid = v)
                .Required();
            parser.Setup<string>('p', "password")
                .Callback(v => password = v);
            parser.Setup<string>('f', "filename")
                .Callback(v => filename = v);
            var result = parser.Parse(args);
            if (result.HasErrors)
            {
                Console.Error.WriteLine(result.ErrorText);
            }
            else
            {
                var obfuscatedPassword = string.Join(
                    string.Empty, 
                    Enumerable.Repeat('*', password.Length));
                Console.WriteLine($"{ssid}:{obfuscatedPassword}");
            }

            WiFi generator = new WiFi(ssid, password, WiFi.Authentication.WPA);
            string payload = generator.ToString();

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            var qrCodeAsBitmap = qrCode.GetGraphic(20);
            qrCodeAsBitmap.Save(filename, ImageFormat.Png);
        }
    }
}
