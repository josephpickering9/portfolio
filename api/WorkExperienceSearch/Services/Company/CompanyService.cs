using Work_Experience_Search.Extensions;
using Work_Experience_Search.Models;
using Work_Experience_Search.Repositories;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Services.Image;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Services;

public sealed class CompanyService(
    ICompanyRepository repository,
    IFileService fileService,
    IImageService imageService,
    ILogger<CompanyService> logger) : ICompanyService
{
    public async Task<Result<IReadOnlyList<Company>>> GetCompaniesAsync(string? search, CancellationToken cancellationToken = default)
    {
        var companies = await repository.SearchAsync(search, cancellationToken);
        return companies.ToOk();
    }

    public async Task<Result<Company>> GetCompanyAsync(CompanyId id, CancellationToken cancellationToken = default)
    {
        var company = await repository.GetAsync(id, cancellationToken);
        if (company is null)
        {
            logger.LogWarning("Company {CompanyId} not found.", id);
            return ErrorCodes.CompanyNotFound.ToFail();
        }

        return company.ToOk();
    }

    public async Task<Result<Company>> GetCompanyBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var company = await repository.GetAsync(slug, cancellationToken);
        if (company is null)
        {
            logger.LogWarning("Company with slug {Slug} not found.", slug);
            return ErrorCodes.CompanyNotFound.ToFail();
        }

        return company.ToOk();
    }

    public async Task<Result<Company>> CreateCompanyAsync(CreateCompany createCompany, CancellationToken cancellationToken = default)
    {
        if (await repository.ExistsAsync(createCompany.Name, cancellationToken))
        {
            logger.LogWarning("Company creation conflict: a company named {Name} already exists.", createCompany.Name);
            return ErrorCodes.CompanyAlreadyExists.ToFail();
        }

        var logo = await SaveLogoIfPresentAsync(createCompany.Logo, cancellationToken);
        if (!logo.IsSuccess)
        {
            logger.LogWarning("Logo could not be saved while creating company {Name}: {ErrorCode}", createCompany.Name, logo.Error);
            return logo.Error!.ToFail();
        }

        var company = new Company { Logo = logo.Data };
        ApplyDetails(company, createCompany);

        await repository.AddAsync(company, cancellationToken);

        return company.ToOk();
    }

    public async Task<Result<Company>> UpdateCompanyAsync(CompanyId id, CreateCompany updateCompany, CancellationToken cancellationToken = default)
    {
        var company = await repository.GetTrackedAsync(id, cancellationToken);
        if (company is null)
        {
            logger.LogWarning("Company {CompanyId} not found for update.", id);
            return ErrorCodes.CompanyNotFound.ToFail();
        }

        if (await repository.ExistsAsync(updateCompany.Name, id, cancellationToken))
        {
            logger.LogWarning("Company {CompanyId} update conflict: a company named {Name} already exists.", id, updateCompany.Name);
            return ErrorCodes.CompanyAlreadyExists.ToFail();
        }

        var logo = await SaveLogoIfPresentAsync(updateCompany.Logo, cancellationToken);
        if (!logo.IsSuccess)
        {
            logger.LogWarning("Logo could not be saved while updating company {CompanyId}: {ErrorCode}", id, logo.Error);
            return logo.Error!.ToFail();
        }

        if (logo.Data is not null) company.Logo = logo.Data;
        ApplyDetails(company, updateCompany);

        await repository.UpdateAsync(company, cancellationToken);

        return company.ToOk();
    }

    public async Task<Result<Company>> DeleteCompanyAsync(CompanyId id, CancellationToken cancellationToken = default)
    {
        var company = await repository.GetTrackedAsync(id, cancellationToken);
        if (company is null)
        {
            logger.LogWarning("Company {CompanyId} not found for deletion.", id);
            return ErrorCodes.CompanyNotFound.ToFail();
        }

        await repository.DeleteAsync(company, cancellationToken);

        return company.ToOk();
    }

    private static void ApplyDetails(Company company, CreateCompany request)
    {
        company.Name = request.Name;
        company.Description = request.Description;
        company.Website = request.Website;
        company.StartDate = request.StartDate;
        company.EndDate = request.EndDate;
        company.Slug = request.Name.ToSlug();
    }

    private async Task<Result<string?>> SaveLogoIfPresentAsync(IFormFile? logo, CancellationToken cancellationToken)
    {
        if (logo is not { Length: > 0 }) return Result<string?>.Ok(null);

        var optimisedImage = await imageService.OptimiseImageAsync(await logo.ReadAllBytesAsync(cancellationToken), cancellationToken);
        var formFile = optimisedImage is { IsSuccess: true, Data: not null }
            ? optimisedImage.Data.ToFormFile(logo.FileName, logo.ContentType)
            : logo;

        var savedFile = await fileService.SaveFileAsync(formFile, cancellationToken);
        if (!savedFile.IsSuccess) return savedFile.Error!.ToFail();

        var fileName = Path.GetFileName(savedFile.Data);
        return string.IsNullOrEmpty(fileName)
            ? ErrorCodes.FileUploadFailed.ToFail()
            : Result<string?>.Ok(fileName);
    }
}
