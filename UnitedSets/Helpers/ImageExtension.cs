using Microsoft.UI.Xaml.Media.Imaging;
using System.Drawing;
using System.IO;

namespace UnitedSets.Helpers;

static class ImageExtension
{
    public async static ValueTask<BitmapImage> ToXAMLBitmapImageAsync(this Bitmap bmp)
    {
        using var ms = new MemoryStream();
        bmp.MakeTransparent(System.Drawing.Color.Black);
        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return await ms.ToXAMLBitmapImageAsync();
    }
    public async static ValueTask<BitmapImage> ToXAMLBitmapImageAsync(this Stream Stream)
    {
        var image = new BitmapImage();

        Stream.Seek(0, SeekOrigin.Begin);
        await image.SetSourceAsync(Stream.AsRandomAccessStream());

        return image;
    }
}
