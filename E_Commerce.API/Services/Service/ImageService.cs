using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using E_Commerce.API.Services.IService;
using System.Text.RegularExpressions;

namespace E_Commerce.API.Services.Service
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly IHttpClientFactory _httpClientFactory;

        public ImageService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            var cloudinary = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(cloudinary);
            _httpClientFactory = httpClientFactory;
        }
        public async Task<string> UploadImageAsync(IFormFile image, Guid productId)
        {
            var fileName = Guid.NewGuid().ToString("N")[..8] + Path.GetExtension(image.FileName);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, image.OpenReadStream()),
                PublicId = $"products/{productId}/{fileName}"
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }
        // Tải lên hình ảnh tạm thời (temporary) cho CKEditor
        public async Task<string> UploadImageTempAsync(IFormFile image, HttpContext httpContext)
        {
            var fileName = Guid.NewGuid().ToString("N")[..8] + Path.GetExtension(image.FileName);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, image.OpenReadStream()),
                PublicId = $"temp/{fileName}" // Lưu vào thư mục tạm thời
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return uploadResult.SecureUrl.ToString();
        }
        public async Task<string> UploadImageFromUrlAsync(string imageUrl, Guid productId)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var imageData = await httpClient.GetByteArrayAsync(imageUrl);

            var fileName = Guid.NewGuid().ToString("N")[..8] + Path.GetExtension(imageUrl);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, new MemoryStream(imageData)),
                PublicId = $"products/{productId}/{fileName}" // Đặt PublicId cho ảnh
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return uploadResult.SecureUrl.ToString(); // Trả về URL ảnh sau khi tải lên
        }
        // Xử lý mô tả và thay thế các URL ảnh trong mô tả với URL đã tải lên Cloudinary
        public async Task<string> ProcessDescriptionAndUploadImages(string description, Guid productId)
        {
            var matches = Regex.Matches(description, "<img.*?src=\"(.*?)\"", RegexOptions.IgnoreCase);

            var uploadedUrls = new Dictionary<string, string>();

            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    var oldUrl = match.Groups[1].Value;

                    if (!uploadedUrls.ContainsKey(oldUrl))
                    {
                        var newUrl = await UploadImageFromUrlAsync(oldUrl, productId);
                        uploadedUrls[oldUrl] = newUrl;
                    }

                    description = description.Replace(oldUrl, uploadedUrls[oldUrl]);
                }
            }

            return description;
        }
        public async Task<List<string>> GetAllImageUrlsForProductAsync(Guid productId)
        {
            var imageUrls = new List<string>();

            // Sử dụng ListResourcesParams để lọc ảnh theo một số thông số khác
            var listParams = new ListResourcesParams()
            {
                MaxResults = 100, // Bạn có thể điều chỉnh số lượng kết quả tối đa
                ResourceType = ResourceType.Image // Chỉ tìm ảnh
            };

            // Lấy danh sách tài nguyên từ Cloudinary
            var resources = await _cloudinary.ListResourcesAsync(listParams);

            foreach (var resource in resources.Resources)
            {
                // Kiểm tra xem ảnh có chứa phần "products/{productId}" trong PublicId
                if (resource.PublicId.Contains($"products/{productId}/"))
                {
                    imageUrls.Add(resource.SecureUrl.ToString());
                }
            }

            return imageUrls;
        }
        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            var publicId = GetPublicIdFromUrl(imageUrl);
            var deleteParams = new DeletionParams(publicId);

            var deletionResult = await _cloudinary.DestroyAsync(deleteParams);

            return deletionResult?.Result == "ok";
        }
        private string GetPublicIdFromUrl(string imageUrl)
        {
            var uri = new Uri(imageUrl);
            var path = uri.AbsolutePath.Trim('/');
            return Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileNameWithoutExtension(path)).Replace('\\', '/');
        }
    }
}
