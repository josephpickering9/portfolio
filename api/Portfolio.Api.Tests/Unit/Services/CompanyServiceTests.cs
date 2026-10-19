using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Portfolio.Api.Requests;
using Portfolio.Api.Models;
using Portfolio.Api.Repositories;
using Portfolio.Api.Services;
using Portfolio.Api.Services.Image;
using Portfolio.Api.Types;
using Portfolio.Api.Extensions;
using Portfolio.Api.Tests;
using Xunit;

namespace Portfolio.Api.Tests.Unit.Services;

public class CompanyServiceTests : BaseServiceTests
{
    private readonly CompanyService _companyService;

    public CompanyServiceTests()
    {
        var mockFileService = new Mock<IFileService>();
        var mockImageService = new Mock<IImageService>();
        _companyService = new CompanyService(new CompanyRepository(Context, new MemoryCache(new MemoryCacheOptions()), new CacheInvalidator()), mockFileService.Object, mockImageService.Object, NullLogger<CompanyService>.Instance);
    }
    
    [Fact]
    public async Task GetCompaniesAsync_NoSearch_ReturnsAllCompanies()
    {
        var result = (await _companyService.GetCompaniesAsync(null)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetCompanyAsync_ValidId_ReturnsCompany()
    {
        var companyId = Company1Id;

        var result = (await _companyService.GetCompanyAsync(companyId)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(companyId, result.Id);
    }

    [Fact]
    public async Task GetCompanyAsync_InvalidId_ThrowsNotFoundFailure()
    {
        var companyId = CompanyId.New();

        var result = (await _companyService.GetCompanyAsync(companyId)).ExpectFailure();
        Assert.Equal(ErrorCodes.CompanyNotFound, result);
    }

    [Fact]
    public async Task CreateCompanyAsync_NewCompany_ReturnsCompany()
    {
        var createCompany = new CreateCompany
        {
            Name = "New Company",
            Description = "New Description",
            Logo = null,
            StartDate = new DateOnly(2020, 1, 1),
            EndDate = new DateOnly(2021, 1, 1),
            Website = "https://example.com"
        };

        var result = (await _companyService.CreateCompanyAsync(createCompany)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(createCompany.Name, result.Name);
        Assert.Equal(createCompany.StartDate, result.StartDate);
        Assert.Equal(createCompany.EndDate, result.EndDate);
    }

    [Fact]
    public async Task CreateCompanyAsync_ExistingCompany_ThrowsConflictFailure()
    {
        var createCompany = new CreateCompany
        {
            Name = "Conflict Company",
            Description = "Conflict Description",
            Logo = null,
            StartDate = new DateOnly(2020, 1, 1),
            EndDate = new DateOnly(2021, 1, 1),
            Website = "https://example.com"
        };
        (await _companyService.CreateCompanyAsync(createCompany)).ExpectSuccess();

        var result = (await _companyService.CreateCompanyAsync(createCompany)).ExpectFailure();
        Assert.Equal(ErrorCodes.CompanyAlreadyExists, result);
    }

    [Fact]
    public async Task UpdateCompanyAsync_ValidId_ReturnsUpdatedCompany()
    {
        var companyId = Company1Id;
        var updateCompany = new CreateCompany
        {
            Name = "Updated Company",
            Description = "Updated Description",
            Logo = null,
            StartDate = new DateOnly(2021, 1, 1),
            EndDate = new DateOnly(2022, 1, 1),
            Website = "https://updated-example.com"
        };

        var result = (await _companyService.UpdateCompanyAsync(companyId, updateCompany)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(companyId, result.Id);
        Assert.Equal(updateCompany.Name, result.Name);
        Assert.Equal(updateCompany.Description, result.Description);
        Assert.Equal(updateCompany.Website, result.Website);
        Assert.Equal(updateCompany.StartDate, result.StartDate);
        Assert.Equal(updateCompany.EndDate, result.EndDate);
    }

    [Fact]
    public async Task UpdateCompanyAsync_InvalidId_ThrowsNotFoundFailure()
    {
        var companyId = CompanyId.New();
        var updateCompany = new CreateCompany
        {
            Name = "Updated Company",
            Description = "Updated Description",
            Logo = null,
            StartDate = new DateOnly(2021, 1, 1),
            EndDate = new DateOnly(2022, 1, 1),
            Website = "https://updated-example.com"
        };

        var result = (await _companyService.UpdateCompanyAsync(companyId, updateCompany)).ExpectFailure();
        Assert.Equal(ErrorCodes.CompanyNotFound, result);
    }

    [Fact]
    public async Task DeleteCompanyAsync_ValidId_DeletesCompany()
    {
        var companyId = Company1Id;

        (await _companyService.DeleteCompanyAsync(Company1Id)).ExpectSuccess();

        var companyInDb = await Context.Company.FindAsync(companyId);
        Assert.Null(companyInDb);
    }

    [Fact]
    public async Task DeleteCompanyAsync_InvalidId_ThrowsNotFoundFailure()
    {
        var companyId = CompanyId.New();

        var result = (await _companyService.DeleteCompanyAsync(companyId)).ExpectFailure();
        Assert.Equal(ErrorCodes.CompanyNotFound, result);
    }

    [Fact]
    public async Task GetCompanyBySlugAsync_InvalidSlug_ReturnsCompanyNotFound()
    {
        var error = (await _companyService.GetCompanyBySlugAsync("non-existent-company")).ExpectFailure();

        Assert.Equal(ErrorCodes.CompanyNotFound, error);
    }

    [Fact]
    public async Task GetCompanyBySlugAsync_ValidSlug_ReturnsCompany()
    {
        var created = (await _companyService.CreateCompanyAsync(new CreateCompany { Name = "Slugged Company", Description = "Description" })).ExpectSuccess();

        var result = (await _companyService.GetCompanyBySlugAsync("slugged-company")).ExpectSuccess();

        Assert.Equal(created.Id, result.Id);
    }
}
