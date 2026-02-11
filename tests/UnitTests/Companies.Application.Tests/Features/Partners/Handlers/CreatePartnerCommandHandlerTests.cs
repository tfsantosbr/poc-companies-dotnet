using Companies.Application.Abstractions.Persistence;
using Companies.Application.Features.Partners.Command.CreatePartner;
using Companies.Application.Features.Partners.Contants;
using Companies.Application.Features.Partners.Repositories;
using Companies.Application.Tests.Features.Partners.Helpers;
using NSubstitute;

namespace Companies.Application.Tests.Features.Partners.Handlers;

public class CreatePartnerCommandHandlerTests
{
    private readonly IPartnerRepository _partnerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePartnerCommandHandlerTests()
    {
        _partnerRepository = Substitute.For<IPartnerRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenCreatePartnerWithDuplicatedEmail()
    {
        // arrange

        _partnerRepository.IsDuplicatedEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

        var command = PartnerHelper.GenerateValidCreatePartnerCommand();

        var handler = new CreatePartnerCommandHandler(_partnerRepository, _unitOfWork);

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.True(result.IsFailure);
        Assert.Contains(PartnerErrors.EmailAlreadyExists(command.Email), result.Notifications);
    }

    [Fact]
    public async Task ShouldCreatePartnerWithSuccessWhenValidDataIsProvided()
    {
        // arrange

        var command = PartnerHelper.GenerateValidCreatePartnerCommand();

        var handler = new CreatePartnerCommandHandler(_partnerRepository, _unitOfWork);

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(command.Email, result.Data.Email);
    }

    [Fact]
    public async Task ShouldAddPartnerToRepositoryWhenValidDataIsProvided()
    {
        // arrange

        var command = PartnerHelper.GenerateValidCreatePartnerCommand();

        var handler = new CreatePartnerCommandHandler(_partnerRepository, _unitOfWork);

        // act

        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert

        Assert.True(result.IsSuccess);
    }
}
