using Companies.Api.Extensions;
using Companies.Api.Extensions.Endpoints;
using Companies.Api.Features.Companies.Requests;
using Companies.Application.Abstractions.Handlers;
using Companies.Application.Abstractions.Pagination;
using Companies.Application.Abstractions.Results;
using Companies.Application.Features.Companies.Commands.CreateCompany;
using Companies.Application.Features.Companies.Commands.ImportCompanies;
using Companies.Application.Features.Companies.Commands.RemoveCompany;
using Companies.Application.Features.Companies.Commands.UpdateCompany;
using Companies.Application.Features.Companies.Models;
using Companies.Application.Features.Companies.Queries.FindCompaniesQuery;
using Companies.Application.Features.Companies.Queries.GetCompanyDetailsQuery;
using Microsoft.AspNetCore.Mvc;

namespace Companies.Api.Features.Companies;

public class CompanyEndpoints : IEndpointBuilder
{
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("companies").WithTags("Companies");

        group.MapGet("/", FindCompanies)
            .RequireAuthorization()
            .Produces<IPagedList<CompanyItem>>();

        group.MapPost("/", CreateCompany)
            .RequireAuthorization()
            .Produces<CompanyDetails>(StatusCodes.Status201Created)
            .Produces<List<Error>>(StatusCodes.Status400BadRequest);

        group.MapPost("/import", ImportCompanies)
            .RequireAuthorization()
            .Produces<CompanyDetails>(StatusCodes.Status201Created)
            .Produces<List<Error>>(StatusCodes.Status400BadRequest);

        group.MapGet("/{companyId}", GetCompanyDetails)
            .RequireAuthorization()
            .Produces<CompanyDetails>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{companyId}", UpdateCompany)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<List<Error>>(StatusCodes.Status400BadRequest);

        group.MapDelete("/{companyId}", RemoveCompany)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static async Task<IResult> FindCompanies(
        [AsParameters] FindCompaniesRequest request,
        IQueryHandler<FindCompaniesQuery, IPagedList<CompanyItem>> handler,
        CancellationToken cancellationToken = default)
    {
        var query = new FindCompaniesQuery(
            request.Name,
            request.Cnpj,
            request.Page,
            request.PageSize,
            request.OrderBy);

        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Ok();
    }

    public static async Task<IResult> CreateCompany(
        CreateCompanyRequest request,
        ICommandHandler<CreateCompanyCommand, CompanyDetails> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateCompanyCommand(
            request.Cnpj,
            request.Name,
            request.LegalNature,
            request.MainActivityId,
            request.Address,
            request.Partners,
            request.Phones
            );

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Created($"companies/{result.Data!.Id}");
    }

    public static async Task<IResult> ImportCompanies(
        [FromBody] ImportCompaniesRequest request,
        ICommandHandler<ImportCompaniesCommand> commandHandler, CancellationToken cancellationToken = default)
    {
        var command = new ImportCompaniesCommand(request.Companies);

        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return result.Accepted();
    }

    public static async Task<IResult> GetCompanyDetails(
        Guid companyId,
        IQueryHandler<GetCompanyDetailsQuery, CompanyDetails> handler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCompanyDetailsQuery(companyId);

        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Ok();
    }

    public static async Task<IResult> UpdateCompany(
        Guid companyId,
        UpdateCompanyRequest request,
        ICommandHandler<UpdateCompanyCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateCompanyCommand(
            companyId,
            request.Name,
            request.LegalNature,
            request.MainActivityId,
            request.Address,
            request.Phones
            );

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.NoContent();
    }

    public static async Task<IResult> RemoveCompany(
        Guid companyId,
        ICommandHandler<RemoveCompanyCommand> handler,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveCompanyCommand(companyId);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.NoContent();
    }
}
