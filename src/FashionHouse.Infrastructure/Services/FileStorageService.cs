using FashionHouse.Application.Contracts.Services;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FashionHouse.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        private const long MaxFileSize = 5 * 1024 * 1024;

        public FileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveImageAsync(
            Stream stream,
            string originalFileName,
            string folder,
            CancellationToken cancellationToken = default)
        {
            var fileName = $"{Guid.NewGuid():N}{ValidateAndGetExtension(stream, originalFileName)}";
            await WriteFileAsync(stream, folder, fileName, cancellationToken);
            return fileName;
        }

        public async Task<string> SaveImageWithNameAsync(
            Stream stream,
            string desiredFileNameWithoutExtension,
            string originalFileName,
            string folder,
            CancellationToken cancellationToken = default)
        {
            var fileName = $"{desiredFileNameWithoutExtension}{ValidateAndGetExtension(stream, originalFileName)}";
            await WriteFileAsync(stream, folder, fileName, cancellationToken);
            return fileName;
        }

        public Task DeleteImageAsync(string imageName, string folder, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(imageName))
                return Task.CompletedTask;

            imageName = Path.GetFileName(imageName);

            var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", folder.ToLowerInvariant());
            var filePath = Path.Combine(uploadFolder, imageName);

            if (File.Exists(filePath))
                File.Delete(filePath);

            return Task.CompletedTask;
        }

        private static string ValidateAndGetExtension(Stream stream, string originalFileName)
        {
            if (stream == null || stream.Length == 0)
                throw new Exception("Empty file.");

            if (stream.Length > MaxFileSize)
                throw new Exception("File too large.");

            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
                throw new Exception("Invalid file type.");

            return extension;
        }

        private async Task WriteFileAsync(Stream stream, string folder, string fileName, CancellationToken cancellationToken)
        {
            var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", folder.ToLowerInvariant());
            Directory.CreateDirectory(uploadFolder);

            var filePath = Path.Combine(uploadFolder, fileName);

            await using var fileStream = new FileStream(filePath, FileMode.Create);
            stream.Position = 0;
            await stream.CopyToAsync(fileStream, cancellationToken);
        }
    }
}