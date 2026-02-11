using Companies.Application.Abstractions.ValueObjects;
using Companies.Application.Features.Companies;
using Companies.Application.Features.Companies.Enums;

namespace Companies.Application.Tests.Features.Companies;

public class CompanyIsOperationalTests
{
    [Fact]
    public void ShouldMarkCompanyAsInoperationalWhenIsCreatedWithtouPartner()
    {
        // arrange

        var company = Company.Create(
            id: new Guid("b9ffc898-c3e4-4dfb-b1c6-86778f383f73"),
            cnpj: new Cnpj("01244660000180"),
            name: "TF Santos Informática",
            legalNature: CompanyLegalNatureType.EIRELI,
            mainActivityId: 4781400,
            address: new Address(
                postalCode: "",
                street: "",
                number: "",
                complement: null,
                neighborhood: "Residencial Oliveira",
                city: "Campo Grande",
                state: "MS",
                country: "Brasil"
            )
        ).Data!;

        company.AddPhone(new Phone("11", "55555555"));

        // act

        var isCompanyInoperational = company.IsOperational!;

        // assert

        Assert.False(isCompanyInoperational);
    }

    [Fact]
    public void ShouldMarkCompanyAsInoperationalWhenIsCreatedWithtouPhone()
    {
        // arrange

        var company = Company.Create(
            id: new Guid("b9ffc898-c3e4-4dfb-b1c6-86778f383f73"),
            cnpj: new Cnpj("01244660000180"),
            name: "TF Santos Informática",
            legalNature: CompanyLegalNatureType.EIRELI,
            mainActivityId: 4781400,
            address: new Address(
                postalCode: "",
                street: "",
                number: "",
                complement: null,
                neighborhood: "Residencial Oliveira",
                city: "Campo Grande",
                state: "MS",
                country: "Brasil"
            )
        ).Data!;

        company.AddPartner(Guid.NewGuid(), 1, DateOnly.MinValue);

        // act

        var isCompanyInoperational = !company.IsOperational;

        // assert

        Assert.True(isCompanyInoperational);
    }

    [Fact]
    public void ShouldMarkCompanyAsOperationalWhenIsCreatedWithAtleastOnePhoneAndOnePartner()
    {
        // arrange

        var company = Company.Create(
            id: new Guid("b9ffc898-c3e4-4dfb-b1c6-86778f383f73"),
            cnpj: new Cnpj("01244660000180"),
            name: "TF Santos Informática",
            legalNature: CompanyLegalNatureType.EIRELI,
            mainActivityId: 4781400,
            address: new Address(
                postalCode: "",
                street: "",
                number: "",
                complement: null,
                neighborhood: "Residencial Oliveira",
                city: "Campo Grande",
                state: "MS",
                country: "Brasil"
            )
        ).Data!;

        company.AddPartner(Guid.NewGuid(), 1, DateOnly.MinValue);
        company.AddPhone(new Phone("11", "55555555"));

        // act

        var isCompanyOperational = company.IsOperational;

        // assert

        Assert.True(isCompanyOperational);
    }
}
