using Work_Experience_Search.Models;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Services;

public interface ICompanyService
{
    Task<Result<IReadOnlyList<Company>>> GetCompaniesAsync(string? search, CancellationToken cancellationToken = default);
    Task<Result<Company>> GetCompanyAsync(CompanyId id, CancellationToken cancellationToken = default);
    Task<Result<Company>> GetCompanyBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<Company>> CreateCompanyAsync(CreateCompany createCompany, CancellationToken cancellationToken = default);
    Task<Result<Company>> UpdateCompanyAsync(CompanyId id, CreateCompany updateCompany, CancellationToken cancellationToken = default);
    Task<Result<Company>> DeleteCompanyAsync(CompanyId id, CancellationToken cancellationToken = default);
}
