using Companies.Application.Abstractions.Persistence;
using Companies.Application.Features.Companies;
using Companies.Application.Features.Companies.Commands.RemoveCompany;
using Companies.Application.Features.Companies.Constants;
using Companies.Application.Features.Companies.Repositories;
using Companies.Application.Tests.Features.Companies.Helpers;
using NSubstitute;

namespace Companies.Application.Tests.Features.Companies.Handlers;

public class RemoveCompanyCommandHandlerTests
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveCompanyCommandHandlerTests()
    {
        _companyRepository = Substitute.For<ICompanyRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenRemoveCompanyThatDoesntExist()
    {
        // arrange

        var command = new RemoveCompanyCommand(Guid.NewGuid());

        var handler = new RemoveCompanyCommandHandler(
            companyRepository: _companyRepository,
            unitOfWork: _unitOfWork
        );

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.Contains(CompanyErrors.CompanyNotFound(command.CompanyId), result.Notifications);
    }

    [Fact]
    public async Task ShouldRemoveCompanyWithSuccessWhenRemoveCompanyThatExist()
    {
        // arrange

        var company = CompanyHelper.GenerateValidCompany();

        _companyRepository.GetByIdAsync(company.Id).Returns(Task.FromResult<Company?>(company));

        var command = new RemoveCompanyCommand(company.Id);

        var handler = new RemoveCompanyCommandHandler(
            companyRepository: _companyRepository,
            unitOfWork: _unitOfWork
        );

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.True(result.IsSuccess);
    }
}
