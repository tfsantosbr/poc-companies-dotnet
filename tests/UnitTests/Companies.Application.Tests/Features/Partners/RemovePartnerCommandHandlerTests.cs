using Companies.Application.Abstractions.Persistence;
using Companies.Application.Features.Partners.Command.RemovePartner;
using Companies.Application.Features.Partners.Contants;
using Companies.Application.Features.Partners.Repositories;
using Companies.Application.Tests.Features.Partners.Helpers;
using NSubstitute;

namespace Companies.Application.Tests.Features.Partners;

public class RemovePartnerCommandHandlerTests
{
    private readonly IPartnerRepository _partnerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemovePartnerCommandHandlerTests()
    {
        _partnerRepository = Substitute.For<IPartnerRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
    }

    [Fact]
    public async Task ShouldReturnNotFoundWhenPartnerDoesNotExist()
    {
        // arrange

        var command = new RemovePartnerCommand(Guid.NewGuid());
        var handler = new RemovePartnerCommandHandler(_partnerRepository, _unitOfWork);

        // act
        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert
        Assert.True(result.IsFailure);

        Assert.Contains(PartnerErrors.PartnerNotFound(command.PartnerId), result.Notifications);
    }

    [Fact]
    public async Task ShouldRemovePartnerWithSuccessWhenPartnerExists()
    {
        // arrange
        var partner = PartnerHelper.GenerateValidPartner();

        _partnerRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(partner);

        var command = new RemovePartnerCommand(Guid.NewGuid());
        var handler = new RemovePartnerCommandHandler(_partnerRepository, _unitOfWork);

        // act
        var result = await handler.HandleAsync(command, new CancellationToken());

        // assert
        Assert.True(result.IsSuccess);
    }
}
