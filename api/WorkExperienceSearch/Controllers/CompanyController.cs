using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Work_Experience_Search.Extensions;
using Work_Experience_Search.Models;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Services;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class CompanyController(ICompanyService companyService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Company>>> GetCompanies(string? search, CancellationToken cancellationToken) =>
        (await companyService.GetCompaniesAsync(search, cancellationToken)).ToResponse();

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Company>> GetCompany(CompanyId id, CancellationToken cancellationToken) =>
        (await companyService.GetCompanyAsync(id, cancellationToken)).ToResponse();

    [HttpGet("{slug}")]
    public async Task<ActionResult<Company>> GetCompany(string slug, CancellationToken cancellationToken) =>
        (await companyService.GetCompanyBySlugAsync(slug, cancellationToken)).ToResponse();

    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Company>> PostCompany([FromForm] CreateCompany createCompany, CancellationToken cancellationToken) =>
        (await companyService.CreateCompanyAsync(createCompany, cancellationToken)).ToResponse();

    [HttpPut("{id:guid}")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Company>> PutCompany(CompanyId id, [FromForm] CreateCompany createCompany, CancellationToken cancellationToken) =>
        (await companyService.UpdateCompanyAsync(id, createCompany, cancellationToken)).ToResponse();

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteCompany(CompanyId id, CancellationToken cancellationToken) =>
        (await companyService.DeleteCompanyAsync(id, cancellationToken)).ToResponse();
}
