// 此 Console 參考黑大的文大，出處：https://blog.darkthread.net/blog/mfa-with-ms-authenticator/

using System.Diagnostics;
using OtpNet;
using PALabs.TotpTest.Console;
using SixLabors.ImageSharp.Formats.Png;

var secret = KeyGeneration.GenerateRandomKey();
var sd = new SecretData
{
    Issuer = "sample.com",
    Label = "your.account",    //留意不要有空格，不要有中文
    Secret = Base32Encoding.ToString(secret)
};
//產生 QRCode
//補充說明：此處為求簡便寫成暫存檔以瀏覽器開啟，實際應用時宜全程於記憶體處理資料不落地
//並於網頁顯示完即銷毁，勿以 Email 或其他方傳遞，以降低外流風險
var qrCodeImgFile = System.IO.Path.GetTempFileName() + ".png";
FileStream fileStream = new FileStream(qrCodeImgFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
sd.GenQRCode().Save(fileStream, new PngEncoder());
//使用預設圖片檢視軟體開啟
var p = Process.Start(new ProcessStartInfo()
{
    FileName = $"file:///{qrCodeImgFile}",
    UseShellExecute = true
});
while (true)
{
    Console.WriteLine("輸入一次性密碼進行驗證，或直接按 Enter 結束");
    var pwd = Console.ReadLine();
    if (string.IsNullOrEmpty(pwd)) break;
    Console.WriteLine(" " + sd.ValidateTotp(pwd));
}