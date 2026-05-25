using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class CloudinaryUploadService : IImageUploadService
{
    private readonly Cloudinary _cloudinary;
    // Centralizamos la configuración en constantes o variables
    private const string DefaultFolder = "teams_logos";
    private const bool UseUniqueFilename = true;

    public CloudinaryUploadService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<ImageUploadResult> UploadVectorImageAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        
        // AQUÍ ESTANDARIZAS EL MÉTODO
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            UniqueFilename = UseUniqueFilename,
            Folder = DefaultFolder,            
        };

        return await _cloudinary.UploadAsync(uploadParams);
    }
}