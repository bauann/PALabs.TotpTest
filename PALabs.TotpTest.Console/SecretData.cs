using OtpNet;
using QRCoder;
using SixLabors.ImageSharp;

namespace PALabs.TotpTest.Console;

public class SecretData
{
    public string Issuer { get; set; }
    public string Label { get; set; }
    public string Secret { get; set; }

    public string GenQRCodeUrl() =>
        $"otpauth://totp/{Label}?issuer={Uri.EscapeDataString(Issuer)}&secret={Uri.EscapeDataString(Secret)}";
    
    public Image GenQRCode()
    {
        var qrcg = new QRCodeGenerator();
        var data = qrcg.CreateQrCode(GenQRCodeUrl(), QRCodeGenerator.ECCLevel.Q);
        var qrc = new BitmapByteQRCode(data);
        Image image = Image.Load(new MemoryStream(qrc.GetGraphic(20)));
        return image;
    }

    Totp totpInstance = null;

    public string ValidateTotp(string totp)
    {
        if (totpInstance == null)
        {
            totpInstance = new Totp(Base32Encoding.ToBytes(this.Secret));
        }

        long timedWindowUsed;
        if (totpInstance.VerifyTotp(totp, out timedWindowUsed))
        {
            return $"驗證通過 - {timedWindowUsed}";
        }
        else
        {
            return "驗證失敗";
        }
    }
}