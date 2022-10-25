using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace UnitedSets.Helpers;

static class IconHelper
{
    public async static ValueTask<BitmapImage> ImageFromIcon(Bitmap Icon)
    {
        using var ms = new MemoryStream();
        Icon.MakeTransparent(Color.Black);
        Icon.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return await ImageFromStream(ms);
    }
    public async static ValueTask<BitmapImage> ImageFromStream(Stream Stream)
    {
        var image = new BitmapImage();

        Stream.Seek(0, SeekOrigin.Begin);
        await image.SetSourceAsync(Stream.AsRandomAccessStream());


        return image;
    }
}
