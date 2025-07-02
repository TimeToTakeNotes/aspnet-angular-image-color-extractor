public interface IImageProcessor
{
    Task<(byte[] ImgBytes, byte[] ThumbBytes, string HexColor)> ProcessImageAsync(Stream imgStream);
}