using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace ColorExtractorApi.Services.Helpers
{
    public class ImageProcessor
    {
        // Main func to process img input img stream and return og img in bytes, thumbnail in bytes and hex color as string
        public async Task<(byte[] ImgBytes, byte[] ThumbBytes, string HexColor)> ProcessImageAsync(Stream imgStream)
        {
            try
            {
                using var image = await Image.LoadAsync<Rgba32>(imgStream);

                var centreRect = GetCentreRectangle(image);
                string hexColor = ExtractColor(image, centreRect);

                // Create 1 shared memory stream for efficiency
                using var ms = new MemoryStream();
                byte[] thumbBytes = CreateThumbnail(image, ms);

                // Clear and reuse stream
                ms.SetLength(0);
                ms.Position = 0;

                // Get og img
                await image.SaveAsPngAsync(ms);

                byte[] imgBytes = ms.ToArray();
                return (imgBytes, thumbBytes, hexColor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during image processing: {ex.Message}");
                return (Array.Empty<byte>(), Array.Empty<byte>(), "#000000");
            }
        }

        // Func to get rectangle around img centre while maintaining exact centre pos for memory efficiency -> returns 1x1 or 2x2 rectangle: 
        private Rectangle GetCentreRectangle(Image<Rgba32> img)
        {
            try
            {
                int width = img.Width;
                int height = img.Height;
                int centreX = width / 2;
                int centreY = height / 2;

                Rectangle centreRect;
                if (width % 2 == 0 && height % 2 == 0) // 2x2 img
                {
                    int left = Math.Max(centreX - 1, 0);
                    int top = Math.Max(centreY - 1, 0);
                    centreRect = new Rectangle(left, top, 2, 2); // Rectangle uses (X, Y, width, height)
                }
                else
                { centreRect = new Rectangle(centreX, centreY, 1, 1); } // 1x1 img

                return centreRect;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting centre of image: {ex.Message}");
                return Rectangle.Empty; // Return default rectangle
            }
        }
        // Func to extract centre most or avg of 4 centre most pixels in hex format:
        private string ExtractColor(Image<Rgba32> img, Rectangle rect)
        {
            try
            {
                int sumR = 0, sumG = 0, sumB = 0;
                int pxlCount = 0;

                // Loop through each pxl in Rectangle
                for (int y = rect.Top; y < rect.Bottom; y++)
                {
                    for (int x = rect.Left; x < rect.Right; x++)
                    {
                        var pxl = img[x, y]; // Get pxls at (x, y)
                        sumR += pxl.R;
                        sumG += pxl.G;
                        sumB += pxl.B;
                        pxlCount++;
                    }
                }
                if (pxlCount == 0) return "#000000"; // Fallback incase of error
                int avgR = sumR / pxlCount;
                int avgG = sumG / pxlCount;
                int avgB = sumB / pxlCount;

                return $"#{avgR:X2}{avgG:X2}{avgB:X2}"; // Format RGB to Hex
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exctracting pixel color: {ex.Message}");
                return "#000000"; // Safe Fallback
            }
        }
        // Func to create thumbnail of og img for display in list:
        private byte[] CreateThumbnail(Image<Rgba32> img, MemoryStream ms)
        {
            // Clear memory stream to reuse
            ms.SetLength(0);
            ms.Position = 0;

            // Clone img to not modify og
            using var thumbnail = img.Clone(x => x.Resize(new ResizeOptions
            {
                Size = new Size(100, 100),
                Mode = ResizeMode.Max // Keep aspect ratio in 100x100
            }));

            thumbnail.SaveAsJpeg(ms); // Save as jpeg for reduced storage
            return ms.ToArray();
        }
    }
}
