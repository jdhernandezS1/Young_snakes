using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;

public interface IImageUploadService
{
    Task<ImageUploadResult> UploadVectorImageAsync(IFormFile file);
}