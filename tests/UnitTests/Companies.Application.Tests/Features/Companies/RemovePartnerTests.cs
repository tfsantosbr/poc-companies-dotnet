using Companies.Application.Features.Companies.Constants;
using Companies.Application.Tests.Features.Companies.Helpers;

namespace Companies.Application.Tests.Features.Companies;

public class RemovePartnerTests
{
    [Fact]
    public void ShouldThowExceptionWhenRemovingPartnerThatDoesntExistInTheCompany()
    {
        // arrange

        var company = CompanyHelper.GenerateValidCompany();

        // act

        var result = company.RemovePartner(new Guid("030068f4-d7bf-484f-8fd0-7b001ed8831a"));

        // assert

        Assert.Contains(CompanyErrors.CompanysPartnerNotExists(), result.Notifications);
    }

    [Fact]
    public void ShouldRemoveWithSuccessWhenPassingAPartnerThatExistsInTheCompany()
    {
        // arrange

        var company = CompanyHelper.GenerateValidCompany();
        company.AddPartner(new Guid("6c65317c-24bf-49b0-9d80-6ccf1c06658a"), 54, new DateOnly(2022, 1, 1));

        // act

        var result = company.RemovePartner(new Guid("6c65317c-24bf-49b0-9d80-6ccf1c06658a"));

        // assert

        Assert.True(result.IsSuccess);
    }
}
