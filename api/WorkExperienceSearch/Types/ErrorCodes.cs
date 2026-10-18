using System.Reflection;

namespace Work_Experience_Search.Types;

public static class ErrorCodes
{
    public const string NotFound = "not_found";
    public const string ProjectNotFound = "project_not_found";
    public const string CompanyNotFound = "company_not_found";
    public const string TagNotFound = "tag_not_found";
    public const string ProjectImageNotFound = "project_image_not_found";
    public const string ProjectRepositoryNotFound = "project_repository_not_found";
    public const string ProjectStatNotFound = "project_stat_not_found";
    public const string FileNotFound = "file_not_found";

    public const string Conflict = "conflict";
    public const string ProjectAlreadyExists = "project_already_exists";
    public const string CompanyAlreadyExists = "company_already_exists";
    public const string TagAlreadyExists = "tag_already_exists";

    public const string BadRequest = "bad_request";
    public const string Unauthorized = "unauthorized";
    public const string Forbidden = "forbidden";
    public const string RateLimited = "rate_limited";
    public const string InternalError = "internal_error";

    public const string FileRequired = "file_required";
    public const string FileUploadFailed = "file_upload_failed";

    public const string UpstreamUnavailable = "upstream_unavailable";
    public const string UpstreamEmptyResponse = "upstream_empty_response";
    public const string ImageOptimisationUnavailable = "image_optimisation_unavailable";

    public const string TitleRequired = "title_required";
    public const string TitleTooLong = "title_too_long";
    public const string NameRequired = "name_required";
    public const string NameTooLong = "name_too_long";
    public const string ShortDescriptionRequired = "short_description_required";
    public const string ShortDescriptionTooLong = "short_description_too_long";
    public const string DescriptionRequired = "description_required";
    public const string DescriptionTooLong = "description_too_long";
    public const string ProblemStatementTooLong = "problem_statement_too_long";
    public const string KeyDecisionTooLong = "key_decision_too_long";
    public const string OutcomeTooLong = "outcome_too_long";
    public const string WebsiteTooLong = "website_too_long";
    public const string EndDateBeforeStartDate = "end_date_before_start_date";
    public const string RepositoryTitleRequired = "repository_title_required";
    public const string RepositoryTitleTooLong = "repository_title_too_long";
    public const string RepositoryUrlRequired = "repository_url_required";
    public const string RepositoryUrlTooLong = "repository_url_too_long";
    public const string StatLabelRequired = "stat_label_required";
    public const string StatLabelTooLong = "stat_label_too_long";
    public const string StatValueRequired = "stat_value_required";
    public const string StatValueTooLong = "stat_value_too_long";
    public const string IconTooLong = "icon_too_long";
    public const string CustomColourInvalid = "custom_colour_invalid";
    public const string QueryRequired = "query_required";
    public const string QueryTooLong = "query_too_long";

    public static readonly IReadOnlySet<string> All = typeof(ErrorCodes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(field => field.IsLiteral)
        .Select(field => (string)field.GetRawConstantValue()!)
        .ToHashSet();
}
