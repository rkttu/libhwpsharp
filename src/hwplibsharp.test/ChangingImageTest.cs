using HwpLib.Object;
using HwpLib.Object.BinData;
using HwpLib.Reader;
using HwpLib.Writer;
using SkiaSharp;

namespace HwpLibSharp.Test;

/// <summary>
/// 이미지 변경 테스트
/// </summary>
[TestClass]
public class ChangingImageTest
{
    [TestMethod]
    public void ChangeImage_ToGrayscale_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetSamplePath("changing-image.hwp");
        var hwpFile = HWPReader.FromFile(filePath);
        
        Assert.IsNotNull(hwpFile);
        
        // Act
        ProcessAllImages(hwpFile);
        
        var writePath = TestHelper.GetResultPath("result-changing-image.hwp");
        HWPWriter.ToFile(hwpFile, writePath);
        
        // Assert
        Assert.IsTrue(File.Exists(writePath), "이미지 변경 성공");
    }

    private static void ProcessAllImages(HWPFile hwpFile)
    {
        foreach (var ebd in hwpFile.BinData.EmbeddedBinaryDataList)
        {
            if (ebd.Data == null) continue;
            
            using var img = LoadImage(ebd.Data);
            if (img != null && ebd.Name != null)
            {
                using var grayImg = MakeGray(img);
                
                var changedFileBinary = MakeFileBinary(ebd.Name, grayImg);
                if (changedFileBinary != null)
                {
                    ebd.Data = changedFileBinary;
                }
            }
        }
    }

    private static SKBitmap? LoadImage(byte[] data)
    {
        try
        {
            return SKBitmap.Decode(data);
        }
        catch
        {
            return null;
        }
    }

    private static SKBitmap MakeGray(SKBitmap img)
    {
        var grayBitmap = new SKBitmap(img.Width, img.Height);
        
        using var canvas = new SKCanvas(grayBitmap);
        using var paint = new SKPaint();
        
        paint.ColorFilter = SKColorFilter.CreateColorMatrix(
        [
            0.2126f, 0.7152f, 0.0722f, 0, 0,
            0.2126f, 0.7152f, 0.0722f, 0, 0,
            0.2126f, 0.7152f, 0.0722f, 0, 0,
            0,       0,       0,       1, 0
        ]);
        
        canvas.DrawBitmap(img, 0, 0, paint);
        return grayBitmap;
    }

    private static byte[]? MakeFileBinary(string name, SKBitmap img)
    {
        var ext = GetExtension(name);
        if (ext == null) return null;
        
        var format = ext.ToLowerInvariant() switch
        {
            "jpg" or "jpeg" => SKEncodedImageFormat.Jpeg,
            "png" => SKEncodedImageFormat.Png,
            "bmp" => SKEncodedImageFormat.Bmp,
            "gif" => SKEncodedImageFormat.Gif,
            "webp" => SKEncodedImageFormat.Webp,
            _ => SKEncodedImageFormat.Png
        };
        
        using var image = SKImage.FromBitmap(img);
        using var data = image.Encode(format, 90);
        return data.ToArray();
    }

    private static string? GetExtension(string name)
    {
        int pos = name.LastIndexOf('.');
        if (pos != -1)
        {
            return name.Substring(pos + 1);
        }
        return null;
    }
}
