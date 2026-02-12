using Companies.Api.Extensions.Endpoints;
using Companies.Application.Features.CompanyPartnerQualifications;
using Companies.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Companies.Api.Features.CompanyPartnerQualifications;

public class CompanyPartnerQualificationEndpoints : IEndpointBuilder
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("company-partner-qualifications")
            .WithTags("Company Partner Qualifications");

        group.MapGet("/", ListCompanyPartnerQualifications)
            .RequireAuthorization()
            .Produces<List<CompanyPartnerQualification>>(StatusCodes.Status200OK);
    }

    public static async Task<IResult> ListCompanyPartnerQualifications(
        CompaniesContext context,
        CancellationToken cancellationToken = default)
    {
        var companyPartnerQualifications = await context.CompanyPartnerQualifications.ToListAsync(cancellationToken);

        return Results.Ok(companyPartnerQualifications);
    }
}
