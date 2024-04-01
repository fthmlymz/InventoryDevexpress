﻿using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Shared;

namespace Application.Features.FileManager.Commands
{
    public sealed record UploadFileCommand(IFormFile File, string FolderType) : IRequest<Result<bool>>;
    public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, Result<bool>>
    {
        private readonly IWebHostEnvironment? _environment;

        public UploadFileCommandHandler(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<Result<bool>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return Result<bool>.Failure("Dosya boş veya bulunamadı.");
            }

            var safeFolderName = GetSafeFolderName(request.FolderType);
            if (string.IsNullOrEmpty(safeFolderName))
            {
                return Result<bool>.Failure("Geçersiz klasör tipi.");
            }

            string uploadPath = Path.Combine(_environment.WebRootPath, safeFolderName);

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            try
            {
                string formattedFileName = $"{Path.GetFileNameWithoutExtension(request.File.FileName)} - {DateTime.Now:dd.MM.yyyy}{Path.GetExtension(request.File.FileName)}";
                string filePath = Path.Combine(uploadPath, formattedFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }

                return Result<bool>.Success(true, "Dosya başarıyla yüklendi.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Dosya yüklenemedi: {ex.Message}");
            }
        }

        private string GetSafeFolderName(string folderType)
        {
            if (!string.IsNullOrEmpty(folderType) &&
                folderType.All(char.IsLetterOrDigit))
            {
                return folderType;
            }
            return null;
        }
    }
}
