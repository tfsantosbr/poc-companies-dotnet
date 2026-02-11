using Companies.Application.Abstractions.ValueObjects;
using Companies.Application.Features.Partners;
using Companies.Application.Features.Partners.Command.CreatePartner;

namespace Companies.Application.Tests.Features.Partners.Helpers;

public class PartnerHelper
{
    public static CreatePartnerCommand GenerateValidCreatePartnerCommand()
    {
        return new CreatePartnerCommand("Test", "Test", "test@email.com");
    }

    public static Partner GenerateValidPartner()
    {
        return Partner.Create(new CompleteName("Test", "Test"), new Email("email@test.com"));
    }
}
