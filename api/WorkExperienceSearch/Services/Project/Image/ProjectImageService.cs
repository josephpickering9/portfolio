using Work_Experience_Search.Extensions;
using Work_Experience_Search.Models;
using Work_Experience_Search.Repositories;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Services.Image;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Services;

public sealed class ProjectImageService(
    IProjectRepository projectRepository,
    IProjectImageRepository projectImageRepository,
    IFileService fileService,
    IImageService imageService,
    ILogger<ProjectImageService> logger) : IProjectImageService
{
    private static readonly IReadOnlySet<ImageType> SingleInstanceImageTypes =
        new HashSet<ImageType> { ImageType.Logo, ImageType.Banner, ImageType.Card };

    public async Task<Result<IReadOnlyList<ProjectImage>>> GetProjectImagesAsync(ProjectId projectId, CancellationToken cancellationToken = default)
    {
        var project = await projectRepository.GetAsync(projectId, cancellationToken);
        if (project is null)
        {
            logger.LogWarning("Project {ProjectId} not found when listing images.", projectId);
            return ErrorCodes.ProjectNotFound.ToFail();
        }

        return Result<IReadOnlyList<ProjectImage>>.Ok(project.Images);
    }

    public async Task<Result<ProjectImage>> GetProjectImageAsync(ProjectId projectId, ProjectImageId id, CancellationToken cancellationToken = default)
    {
        var project = await projectRepository.GetAsync(projectId, cancellationToken);
        if (project is null)
        {
            logger.LogWarning("Project {ProjectId} not found when fetching image {ProjectImageId}.", projectId, id);
            return ErrorCodes.ProjectNotFound.ToFail();
        }

        var image = project.Images.SingleOrDefault(i => i.Id == id);
        if (image is null)
        {
            logger.LogWarning("Image {ProjectImageId} not found on project {ProjectId}.", id, projectId);
            return ErrorCodes.ProjectImageNotFound.ToFail();
        }

        return image.ToOk();
    }

    public async Task<Result<IReadOnlyList<ProjectImage>>> SyncProjectImagesAsync(ProjectId projectId, IReadOnlyList<CreateProjectImage> images, CancellationToken cancellationToken = default)
    {
        var project = await projectRepository.GetTrackedAsync(projectId, cancellationToken);
        if (project is null)
        {
            logger.LogWarning("Project {ProjectId} not found when syncing images.", projectId);
            return ErrorCodes.ProjectNotFound.ToFail();
        }

        return await SyncProjectImagesAsync(project, images, cancellationToken);
    }

    public async Task<Result<IReadOnlyList<ProjectImage>>> SyncProjectImagesAsync(Project project, IReadOnlyList<CreateProjectImage> images, CancellationToken cancellationToken = default)
    {
        var imageIds = images.Select(i => i.Id).ToList();
        var imagesToDelete = project.Images.Where(i => !imageIds.Contains(i.Id)).ToList();
        var imagesToCreate = images.Where(i => i.Id is null).ToList();
        var imagesToSave = project.Images.Where(i => images.Any(x => x.Id == i.Id)).ToList();

        foreach (var image in imagesToDelete)
        {
            await projectImageRepository.RemoveAsync(image, cancellationToken);
            fileService.DeleteFile(image.Image);
        }

        foreach (var image in imagesToCreate)
        {
            if (SingleInstanceImageTypes.Contains(image.Type))
            {
                var existingImage = project.Images.SingleOrDefault(i => i.Type == image.Type);
                if (existingImage is not null)
                {
                    fileService.DeleteFile(existingImage.Image);
                    await projectImageRepository.RemoveAsync(existingImage, cancellationToken);
                }
            }

            var savedImage = await SaveUploadedImageAsync(image.Image, cancellationToken);
            if (!savedImage.IsSuccess)
            {
                logger.LogWarning("Failed to save image for project {ProjectId}: {ErrorCode}", project.Id, savedImage.Error);
                return savedImage.Error!.ToFail();
            }

            var projectImage = CreateProjectImage(project, image, savedImage.Data!);
            await projectImageRepository.AddAsync(projectImage, cancellationToken);
            imagesToSave.Add(projectImage);
        }

        foreach (var image in imagesToSave)
        {
            var request = images.SingleOrDefault(i => i.Id == image.Id);
            if (request?.Order is not null) image.Order = request.Order;
        }

        if (imagesToCreate.Count > 0 || imagesToDelete.Count > 0 || imagesToSave.Count > 0)
            await projectImageRepository.SaveChangesAsync(cancellationToken);

        return Result<IReadOnlyList<ProjectImage>>.Ok(imagesToSave);
    }

    public async Task<Result<bool>> OptimiseImagesAsync(CancellationToken cancellationToken = default)
    {
        var images = await projectImageRepository.GetUnoptimisedImagesTrackedAsync(cancellationToken);
        foreach (var image in images)
        {
            var file = await imageService.GetImageAsync(image.Image, cancellationToken);
            if (!file.IsSuccess)
            {
                logger.LogWarning("Skipping optimisation of image {ProjectImageId}: {ErrorCode}", image.Id, file.Error);
                continue;
            }

            var optimisedImage = await SaveImageBytesAsync(file.Data!.File, image.Image, cancellationToken);
            if (!optimisedImage.IsSuccess)
            {
                logger.LogWarning("Skipping optimisation of image {ProjectImageId}: {ErrorCode}", image.Id, optimisedImage.Error);
                continue;
            }

            image.Image = optimisedImage.Data!.FileName;
            image.IsOptimised = true;

            await projectImageRepository.UpdateAsync(image, cancellationToken);

            fileService.DeleteFile(file.Data.FileName);
        }

        return true.ToOk();
    }

    private static ProjectImage CreateProjectImage(Project project, CreateProjectImage request, ImageSaveData savedImage) =>
        new()
        {
            Image = savedImage.FileName,
            Type = request.Type,
            Project = project,
            IsOptimised = savedImage.IsOptimised
        };

    private async Task<Result<ImageSaveData>> SaveImageBytesAsync(byte[] file, string fileName, CancellationToken cancellationToken)
    {
        var optimisedImage = await imageService.OptimiseImageAsync(file, cancellationToken);
        var formFile = optimisedImage is { IsSuccess: true, Data: not null }
            ? optimisedImage.Data.ToFormFile(fileName, FileExtensions.GetContentType(fileName))
            : null;

        var savedFileName = await SaveFileAsync(formFile, cancellationToken);
        if (!savedFileName.IsSuccess) return savedFileName.Error!.ToFail();

        return new ImageSaveData(savedFileName.Data!, optimisedImage.IsSuccess).ToOk();
    }

    private async Task<Result<ImageSaveData>> SaveUploadedImageAsync(IFormFile? file, CancellationToken cancellationToken)
    {
        var optimisedImage = await imageService.OptimiseImageAsync(await file.ReadAllBytesAsync(cancellationToken), cancellationToken);
        var formFile = optimisedImage is { IsSuccess: true, Data: not null }
            ? optimisedImage.Data.ToFormFile(file?.FileName ?? FileExtensions.DefaultFileName, file?.ContentType ?? FileExtensions.DefaultContentType)
            : file;

        var savedFileName = await SaveFileAsync(formFile, cancellationToken);
        if (!savedFileName.IsSuccess) return savedFileName.Error!.ToFail();

        return new ImageSaveData(savedFileName.Data!, optimisedImage.IsSuccess).ToOk();
    }

    private async Task<Result<string>> SaveFileAsync(IFormFile? formFile, CancellationToken cancellationToken)
    {
        var savedFile = await fileService.SaveFileAsync(formFile, cancellationToken);
        if (!savedFile.IsSuccess) return savedFile.Error!.ToFail();

        var fileName = Path.GetFileName(savedFile.Data);
        return string.IsNullOrEmpty(fileName) ? ErrorCodes.FileUploadFailed.ToFail() : fileName.ToOk();
    }
}

public sealed record ImageSaveData(string FileName, bool IsOptimised);
