using Companies.Api.Extensions.Endpoints;
using Companies.Application.Abstractions.Database;
using Companies.Application.Features.CompanyMainActivities;
using Microsoft.EntityFrameworkCore;

namespace Companies.Api.Features.CompanyMainActivities;

public class CompanyMainActivityEndpoints : IEndpointBuilder
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("company-main-activities")
            .WithTags("Company Main Activities");

        group.MapGet("/", ListCompanyMainActivities)
            .RequireAuthorization()
            .Produces<List<CompanyMainActivity>>(StatusCodes.Status200OK);
    }

    public static async Task<IResult> ListCompanyMainActivities(
        ICompaniesContext context,
        CancellationToken cancellationToken = default)
    {
        var companyMainActivities = await context.CompanyMainActivities.ToListAsync(cancellationToken);

        return Results.Ok(companyMainActivities);
    }
}
