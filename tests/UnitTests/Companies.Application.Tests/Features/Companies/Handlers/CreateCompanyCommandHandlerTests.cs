using Companies.Application.Abstractions.Models;
using Companies.Application.Abstractions.Persistence;
using Companies.Application.Abstractions.Validations;
using Companies.Application.Features.Companies.Commands.CreateCompany;
using Companies.Application.Features.Companies.Constants;
using Companies.Application.Features.Companies.Repositories;
using Companies.Application.Tests.Features.Companies.Helpers;
using NSubstitute;

namespace Companies.Application.Tests.Features.Companies.Handlers;

public class CreateCompanyCommandHandlerTests
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommandValidator<CreateCompanyCommand> _validator;

    public CreateCompanyCommandHandlerTests()
    {
        _companyRepository = Substitute.For<ICompanyRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _validator = new CommandValidator<CreateCompanyCommand>(new CreateCompanyCommandValidator());
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenCreateCompanyWithInvalidData()
    {
        // arrange

        var command = new CreateCompanyCommand(
            string.Empty,
            string.Empty,
            default,
            default,
            new AddressModel(
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty),
            [],
            []
            );

        var handler = new CreateCompanyCommandHandler(
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
    public async Task ShouldReturnErrorResponseWhenCreateCompanyWithDuplicatedCnpj()
    {
        // arrange

        _companyRepository.AnyByCnpjAsync(Arg.Any<string>()).ReturnsForAnyArgs(Task.FromResult(true));

        var command = CompanyHelper.GenerateValidCreateCompanyCommand();

        var handler = new CreateCompanyCommandHandler(
            validator: _validator,
            companyRepository: _companyRepository,
            unitOfWork: _unitOfWork
        );

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.True(result.IsFailure);

        Assert.Contains(CompanyErrors.CompanyCnpjAlreadyExists(command.Cnpj), result.Notifications);
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenCreateCompanyWithDuplicatedName()
    {
        // arrange

        _companyRepository.AnyByNameAsync(Arg.Any<string>()).ReturnsForAnyArgs(Task.FromResult(true));

        var command = CompanyHelper.GenerateValidCreateCompanyCommand();

        var handler = new CreateCompanyCommandHandler(
            validator: _validator,
            companyRepository: _companyRepository,
            unitOfWork: _unitOfWork
        );

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.True(result.IsFailure);
        Assert.Contains(CompanyErrors.CompanyNameAlreadyExists(command.Name), result.Notifications);
    }

    [Fact]
    public async Task ShouldCreateCompanyWithSuccessWhenValidDataIsProvided()
    {
        // arrange

        var command = CompanyHelper.GenerateValidCreateCompanyCommand();

        var handler = new CreateCompanyCommandHandler(
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
