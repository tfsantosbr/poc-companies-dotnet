using Companies.Application.Features.Companies.Constants;
using Companies.Application.Tests.Features.Companies.Helpers;

namespace Companies.Application.Tests.Features.Companies;

public class AddPartnerTests
{
    [Fact]
    public void ShouldReturnErrorWhenAddDuplicatedPartner()
    {
        // arrange

        var company = CompanyHelper.GenerateValidCompany();
        company.AddPartner(
            new Guid("6c65317c-24bf-49b0-9d80-6ccf1c06658d"), 54, new DateOnly(2022, 1, 1)
        );

        // act

        var result = company.AddPartner(
            new Guid("6c65317c-24bf-49b0-9d80-6ccf1c06658d"), 54, new DateOnly(2022, 1, 1)
        );

        // assert

        Assert.Contains(CompanyErrors.PartnerAlreadyLinkedWithCompany(), result.Notifications);
    }

    [Fact]
    public void ShouldReturnSuccessWhenAddAValidPartner()
    {
        // arrange

        var company = CompanyHelper.GenerateValidCompany();

        // act

        var result = company.AddPartner(
            new Guid("fe092825-b7b6-4bc7-9a42-ec56559c119a"), 54, new DateOnly(2022, 1, 1));

        // assert

        Assert.True(result.IsSuccess);
    }
}
