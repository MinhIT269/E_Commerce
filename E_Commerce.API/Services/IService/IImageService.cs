namespace E_Commerce.API.Services.IService
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile image, Guid productId);
        Task<string> UploadImageTempAsync(IFormFile image, HttpContext httpContext);
        Task<string> ProcessDescriptionAndUploadImages(string description, Guid productId);
        Task<List<string>> GetAllImageUrlsForProductAsync(Guid productId);
        Task<bool> DeleteImageAsync(string imageUrl);
    }
}
