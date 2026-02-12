using Companies.Api.Extensions;
using Companies.Api.Extensions.Endpoints;
using Companies.Api.Features.Companies.Requests;
using Companies.Application.Abstractions.Handlers;
using Companies.Application.Features.Companies.Commands.AddPartnerInCompany;
using Companies.Application.Features.Companies.Commands.RemovePartnerFromCompany;
using Companies.Application.Features.Companies.Models;
using Companies.Application.Features.Companies.Queries.FindCompanyPartnersQuery;
using Companies.Application.Features.Partners.Models;

namespace Companies.Api.Features.Companies;

public class CompanyPartnerEndpoints : IEndpointBuilder
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("companies/{companyId}/partners").WithTags("Company Partners");

        group.MapGet("/", FindPartners)
            .RequireAuthorization()
            .Produces<List<CompanyPartnerModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", AddPartnerInCompany)
            .RequireAuthorization()
            .Produces<CompanyPartnerModel>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{partnerId}", RemovePartnerFromCompany)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);
    }

    public static async Task<IResult> FindPartners(
        Guid companyId,
        IQueryHandler<FindCompanyPartnersQuery, IEnumerable<PartnerItem>> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new FindCompanyPartnersQuery(companyId);

        var result = await queryHandler.HandleAsync(query, cancellationToken);

        return result.Ok();
    }

    public static async Task<IResult> AddPartnerInCompany(
        Guid companyId,
        AddPartnerInCompanyRequest request,
        ICommandHandler<AddPartnerInCompanyCommand, CompanyPartnerModel> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new AddPartnerInCompanyCommand(
            companyId, request.PartnerId, request.QualificationId, request.JoinedAt);

        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return result.Created();
    }

    public static async Task<IResult> RemovePartnerFromCompany(
        Guid companyId,
        Guid partnerId,
        ICommandHandler<RemovePartnerFromCompanyCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new RemovePartnerFromCompanyCommand(companyId, partnerId);

        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return result.NoContent();
    }
}
