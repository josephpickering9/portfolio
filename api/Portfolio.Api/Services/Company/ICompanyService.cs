using Portfolio.Api.Models;
using Portfolio.Api.Requests;
using Portfolio.Api.Types;

namespace Portfolio.Api.Services;

public interface ICompanyService
{
    Task<Result<IReadOnlyList<Company>>> GetCompaniesAsync(string? search, CancellationToken cancellationToken = default);
    Task<Result<Company>> GetCompanyAsync(CompanyId id, CancellationToken cancellationToken = default);
    Task<Result<Company>> GetCompanyBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<Company>> CreateCompanyAsync(CreateCompany createCompany, CancellationToken cancellationToken = default);
    Task<Result<Company>> UpdateCompanyAsync(CompanyId id, CreateCompany updateCompany, CancellationToken cancellationToken = default);
    Task<Result> DeleteCompanyAsync(CompanyId id, CancellationToken cancellationToken = default);
}
