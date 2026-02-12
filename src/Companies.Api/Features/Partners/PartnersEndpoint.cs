using Companies.Api.Extensions;
using Companies.Api.Extensions.Endpoints;
using Companies.Api.Features.Partners.Requests;
using Companies.Application.Abstractions.Handlers;
using Companies.Application.Abstractions.Pagination;
using Companies.Application.Features.Partners.Command.CreatePartner;
using Companies.Application.Features.Partners.Command.RemovePartner;
using Companies.Application.Features.Partners.Models;
using Companies.Application.Features.Partners.Queries;

namespace Companies.Api.Features.Partners;

public class PartnerEndpoints : IEndpointBuilder
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("partners").WithTags("Partners");

        group.MapGet("/", FindPartners)
            .RequireAuthorization()
            .Produces<List<PartnerItem>>(StatusCodes.Status200OK);

        group.MapPost("/", CreatePartner)
            .RequireAuthorization()
            .Produces<PartnerItem>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{partnerId}", DeletePartner)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static async Task<IResult> FindPartners(
        [AsParameters] FindPartnersRequest request,
        IQueryHandler<FindPartnersQuery, IPagedList<PartnerItem>> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new FindPartnersQuery(
            request.FirstName, request.LastName, request.Page, request.PageSize, request.OrderBy);

        var result = await queryHandler.HandleAsync(query, cancellationToken);

        return result.Ok();
    }

    public static async Task<IResult> CreatePartner(
        CreatePartnerRequest request,
        ICommandHandler<CreatePartnerCommand, PartnerDetails> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new CreatePartnerCommand(request.FirstName, request.LastName, request.Email);

        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return result.Created();
    }

    public static async Task<IResult> DeletePartner(
        Guid partnerId,
        ICommandHandler<RemovePartnerCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var command = new RemovePartnerCommand(partnerId);

        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return result.NoContent();
    }
}
