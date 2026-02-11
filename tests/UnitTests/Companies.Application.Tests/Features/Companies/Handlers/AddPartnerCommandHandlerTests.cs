using Companies.Application.Abstractions.Persistence;
using Companies.Application.Abstractions.Validations;
using Companies.Application.Features.Companies;
using Companies.Application.Features.Companies.Commands.AddPartnerInCompany;
using Companies.Application.Features.Companies.Constants;
using Companies.Application.Features.Companies.Repositories;
using Companies.Application.Features.Partners.Repositories;
using Companies.Application.Tests.Features.Companies.Helpers;
using NSubstitute;

namespace Companies.Application.Tests.Features.Companies.Handlers;

public class AddPartnerCommandHandlerTests
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommandValidator<AddPartnerInCompanyCommand> _validator;

    public AddPartnerCommandHandlerTests()
    {
        _companyRepository = Substitute.For<ICompanyRepository>();
        _partnerRepository = Substitute.For<IPartnerRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _validator = new CommandValidator<AddPartnerInCompanyCommand>(new AddPartnerInCompanyCommandValidator());
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenAddPartnerWithInvalidData()
    {
        // arrange

        var command = new AddPartnerInCompanyCommand(Guid.Empty, Guid.Empty, default, default);

        var handler = new AddPartnerInCompanyCommandHandler(
            validator: _validator,
            companyRepository: _companyRepository,
            partnerRepository: _partnerRepository,
            unitOfWork: _unitOfWork
        );

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenAddPartnerInCompanyThatDoesntExist()
    {
        // arrange

        var command = new AddPartnerInCompanyCommand(
            PartnerId: Guid.NewGuid(),
            CompanyId: Guid.NewGuid(),
            JoinedAt: new DateTime(2022, 1, 1),
            QualificationId: 1
        );

        var handler = new AddPartnerInCompanyCommandHandler(
            validator: _validator,
            companyRepository: _companyRepository,
            partnerRepository: _partnerRepository,
            unitOfWork: _unitOfWork
        );

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.Contains(CompanyErrors.CompanyNotFound(command.CompanyId), result.Notifications);
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenAddPartnerThatDoesntExist()
    {
        // arrange

        var company = CompanyHelper.GenerateValidCompany();

        _companyRepository.GetByIdAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult<Company?>(company));

        _partnerRepository.AnyPartnerByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult(false));

        var command = new AddPartnerInCompanyCommand(
            PartnerId: Guid.NewGuid(),
            CompanyId: Guid.NewGuid(),
            JoinedAt: new DateTime(2022, 1, 1),
            QualificationId: 1
        );

        var handler = new AddPartnerInCompanyCommandHandler(
            validator: _validator,
            companyRepository: _companyRepository,
            partnerRepository: _partnerRepository,
            unitOfWork: _unitOfWork
        );

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.Contains(CompanyErrors.PartnerNotFound(command.PartnerId), result.Notifications);
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenAddPartnerThatIsAlreadyLinkedInTheCompany()
    {
        // arrange

        var company = CompanyHelper.GenerateValidCompany();

        _companyRepository.GetByIdAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult<Company?>(company));

        _partnerRepository.AnyPartnerByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult(true));

        var command = new AddPartnerInCompanyCommand(
            PartnerId: new Guid("6c65317c-24bf-49b0-9d80-6ccf1c06658d"),
            CompanyId: new Guid("b9ffc898-c3e4-4dfb-b1c6-86778f383f73"),
            JoinedAt: new DateTime(2022, 1, 1),
            QualificationId: 1
        );

        var handler = new AddPartnerInCompanyCommandHandler(
            validator: _validator,
            companyRepository: _companyRepository,
            partnerRepository: _partnerRepository,
            unitOfWork: _unitOfWork
        );

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.Contains(CompanyErrors.PartnerAlreadyLinkedWithCompany(), result.Notifications);
    }

    [Fact]
    public async Task ShouldAddPartnerWithSuccessWhenValidPartnerIsProvided()
    {
        // arrange

        var company = CompanyHelper.GenerateValidCompany();

        _companyRepository.GetByIdAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult<Company?>(company));

        _partnerRepository.AnyPartnerByIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult(true));

        var command = new AddPartnerInCompanyCommand(
            PartnerId: Guid.NewGuid(),
            CompanyId: company.Id,
            JoinedAt: new DateTime(2022, 1, 1),
            QualificationId: 1
        );

        var handler = new AddPartnerInCompanyCommandHandler(
            validator: _validator,
            companyRepository: _companyRepository,
            partnerRepository: _partnerRepository,
            unitOfWork: _unitOfWork
        );

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.True(result.IsSuccess);
    }
}
