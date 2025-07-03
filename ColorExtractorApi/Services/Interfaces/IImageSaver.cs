public interface IImageSaver
{
    Task<(string ImagePath, string ThumbnailPath)> SaveImageAndThumbnailAsync(byte[] imgBytes, byte[] thumbBytes, string baseFileName);
}