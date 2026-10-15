using Microsoft.AspNetCore.Components.Forms;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;

namespace SchoolNewspaperBlazorApp.Service
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly string _uploadPath;

        public FileService(IFileRepository fileRepository, IWebHostEnvironment environment)
        {
            _fileRepository = fileRepository;
            _uploadPath = @"C:\Users\Uczeń2026\source\repos\SchoolNewspaperBlazorApp\Images";
        }

        public async Task<string> GetPreviewAsync(IBrowserFile file)
        {
            using var stream = file.OpenReadStream(5 * 1024 * 1024);
            using var memoryStream = new MemoryStream();

            await stream.CopyToAsync(memoryStream);

            byte[] bytes = memoryStream.ToArray();

            return $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}";
        }

        public async Task<int> UploadImage(IBrowserFile file)
        {
            Directory.CreateDirectory(_uploadPath);

            string extension = Path.GetExtension(file.Name).ToLowerInvariant();
            string fileName = $"{Guid.NewGuid()}{extension}";
            string fullPath = Path.Combine(_uploadPath, fileName);

            using var stream = file.OpenReadStream(5 * 1024 * 1024);
            using var fileStream = new FileStream(fullPath, FileMode.Create);

            await stream.CopyToAsync(fileStream);

            var mediaFile = new MediaFile
            {
                FileName = fileName,
                FileType = extension
            };

            await _fileRepository.AddFileAsync(mediaFile);

            return mediaFile.Id;
        }

        public async Task<string> GetFileNameByIdAsync(int id)
        {
            var file = await _fileRepository.GetFileByIdAsync(id);
            return file.FileName;
        }
    }
}