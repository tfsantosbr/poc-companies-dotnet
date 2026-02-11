using Companies.Application.Abstractions.Models;
using Companies.Application.Abstractions.Persistence;
using Companies.Application.Abstractions.Validations;
using Companies.Application.Features.Companies;
using Companies.Application.Features.Companies.Commands.UpdateCompany;
using Companies.Application.Features.Companies.Constants;
using Companies.Application.Features.Companies.Enums;
using Companies.Application.Features.Companies.Repositories;
using Companies.Application.Tests.Features.Companies.Helpers;
using NSubstitute;

namespace Companies.Application.Tests.Features.Companies.Handlers;

public class UpdateCompanyCommandHandlerTests
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommandValidator<UpdateCompanyCommand> _validator;

    public UpdateCompanyCommandHandlerTests()
    {
        _companyRepository = Substitute.For<ICompanyRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _validator = new CommandValidator<UpdateCompanyCommand>(new UpdateCompanyCommandValidator());
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenUpdateCompanyWithInvalidData()
    {
        // arrange

        var command = new UpdateCompanyCommand(
            Guid.Empty,
            string.Empty,
            CompanyLegalNatureType.EIRELI,
            1,
            new AddressModel(
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty
                ),
            []
            );

        var handler = new UpdateCompanyCommandHandler(
            validator: _validator,
            companyRepository: _companyRepository,
            unitOfWork: _unitOfWork
        );

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenUpdateCompanyThatDoesntExist()
    {
        // arrange

        var command = CompanyHelper.GenerateValidUpdateCompanyCommand();

        var handler = new UpdateCompanyCommandHandler(
            validator: _validator,
            companyRepository: _companyRepository,
            unitOfWork: _unitOfWork
        );

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.Contains(CompanyErrors.CompanyNotFound(command.CompanyId), result.Notifications);
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenUpdateCompanyWithDuplicatedName()
    {
        // arrange

        _companyRepository.AnyByNameAsync(Arg.Any<string>()).ReturnsForAnyArgs(Task.FromResult(true));

        var command = CompanyHelper.GenerateValidUpdateCompanyCommand();

        var handler = new UpdateCompanyCommandHandler(
            validator: _validator,
            companyRepository: _companyRepository,
            unitOfWork: _unitOfWork
        );

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.Contains(CompanyErrors.CompanyNameAlreadyExists(command.Name), result.Notifications);
    }


    [Fact]
    public async Task ShouldUpdateCompanyWithSuccessWhenValidDataIsProvided()
    {
        // arrange

        var company = CompanyHelper.GenerateValidCompany();

        _companyRepository.GetByIdAsync(Arg.Any<Guid>()).ReturnsForAnyArgs(Task.FromResult<Company?>(company));

        var command = CompanyHelper.GenerateValidUpdateCompanyCommand();

        var handler = new UpdateCompanyCommandHandler(
            validator: _validator,
            companyRepository: _companyRepository,
            unitOfWork: _unitOfWork
        );

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.True(result.IsSuccess);
    }
}
