using Companies.Application.Abstractions.Persistence;
using Companies.Application.Features.Companies;
using Companies.Application.Features.Companies.Commands.RemovePartnerFromCompany;
using Companies.Application.Features.Companies.Constants;
using Companies.Application.Features.Companies.Repositories;
using Companies.Application.Tests.Features.Companies.Helpers;
using NSubstitute;

namespace Companies.Application.Tests.Features.Companies.Handlers;

public class RemovePartnerFromCompanyCommandHandlerTests
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemovePartnerFromCompanyCommandHandlerTests()
    {
        _companyRepository = Substitute.For<ICompanyRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenRemovePartnerInCompanyThatDoesntExist()
    {
        // arrange

        var command = new RemovePartnerFromCompanyCommand(Guid.NewGuid(), Guid.NewGuid());

        var handler = new RemovePartnerFromCompanyCommandHandler(
            companyRepository: _companyRepository,
            unitOfWork: _unitOfWork
        );

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.Contains(CompanyErrors.CompanyNotFound(command.CompanyId), result.Notifications);
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenRemovePartnerThatDoesntExist()
    {
        // arrange

        var company = CompanyHelper.GenerateValidCompany();

        _companyRepository.GetByIdAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult<Company?>(company));

        var command = new RemovePartnerFromCompanyCommand(Guid.NewGuid(), Guid.NewGuid());

        var handler = new RemovePartnerFromCompanyCommandHandler(
            companyRepository: _companyRepository,
            unitOfWork: _unitOfWork
        );

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.Contains(CompanyErrors.CompanysPartnerNotExists(), result.Notifications);
    }

    [Fact]
    public async Task ShouldRemovePartnerWithSuccessWhenAExistingPartnerIsProvided()
    {
        // arrange

        var company = CompanyHelper.GenerateValidCompany();

        _companyRepository.GetByIdAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult<Company?>(company));

        var command = new RemovePartnerFromCompanyCommand(
           CompanyId: company.Id,
           PartnerId: company.Partners.First().PartnerId
        );

        var handler = new RemovePartnerFromCompanyCommandHandler(
            companyRepository: _companyRepository,
            unitOfWork: _unitOfWork
        );

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.True(result.IsSuccess);
    }
}
