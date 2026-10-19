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
    public Task<ActionResult<IReadOnlyList<Company>>> GetCompanies([FromQuery] SearchQuery query, CancellationToken cancellationToken) =>
        companyService.GetCompaniesAsync(query.Search, cancellationToken).ToResponse();

    [HttpGet("{id:guid}")]
    public Task<ActionResult<Company>> GetCompanyById(CompanyId id, CancellationToken cancellationToken) =>
        companyService.GetCompanyAsync(id, cancellationToken).ToResponse();

    [HttpGet("{slug}")]
    public Task<ActionResult<Company>> GetCompanyBySlug(string slug, CancellationToken cancellationToken) =>
        companyService.GetCompanyBySlugAsync(slug, cancellationToken).ToResponse();

    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public Task<ActionResult<Company>> PostCompany([FromForm] CreateCompany createCompany, CancellationToken cancellationToken) =>
        companyService.CreateCompanyAsync(createCompany, cancellationToken)
            .ToCreatedResponse(nameof(GetCompanyById), company => new { id = company.Id });

    [HttpPut("{id:guid}")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public Task<ActionResult<Company>> PutCompany(CompanyId id, [FromForm] CreateCompany createCompany, CancellationToken cancellationToken) =>
        companyService.UpdateCompanyAsync(id, createCompany, cancellationToken).ToResponse();

    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public Task<IActionResult> DeleteCompany(CompanyId id, CancellationToken cancellationToken) =>
        companyService.DeleteCompanyAsync(id, cancellationToken).ToNoContentOrError();
}
