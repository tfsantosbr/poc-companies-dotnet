using Companies.Application.Abstractions.Models;
using Companies.Application.Abstractions.ValueObjects;
using Companies.Application.Features.Companies;
using Companies.Application.Features.Companies.Commands.CreateCompany;
using Companies.Application.Features.Companies.Commands.UpdateCompany;
using Companies.Application.Features.Companies.Enums;
using Companies.Application.Features.Companies.Models;

namespace Companies.Application.Tests.Features.Companies.Helpers;

public class CompanyHelper
{
    public static Company GenerateValidCompany()
    {
        var company = Company.Create(
            id: new Guid("b9ffc898-c3e4-4dfb-b1c6-86778f383f73"),
            cnpj: new Cnpj("01244660000180"),
            name: "TF Santos Informática",
            legalNature: CompanyLegalNatureType.EIRELI,
            mainActivityId: 4781400,
            address: new Address(
                postalCode: "00000001",
                street: "Street Test",
                number: "1",
                complement: null,
                neighborhood: "Residencial Oliveira",
                city: "Campo Grande",
                state: "MS",
                country: "Brasil"
            )
        ).Data!;

        company.AddPartner(new Guid("6c65317c-24bf-49b0-9d80-6ccf1c06658d"), 54, new DateOnly(2022, 1, 1));
        company.AddPartner(new Guid("0016668e-3e63-4565-8b78-577e47f8482d"), 54, new DateOnly(2022, 1, 1));
        company.AddPartner(new Guid("7924572b-830b-4f7e-8b3d-e5cea7dd5c25"), 54, new DateOnly(2022, 1, 1));

        company.AddPhone(new Phone("11", "999999999"));
        company.AddPhone(new Phone("11", "988888888"));
        company.AddPhone(new Phone("11", "977777777"));

        return company;
    }

    public static CreateCompanyCommand GenerateValidCreateCompanyCommand()
    {
        return new CreateCompanyCommand(
            Cnpj: "00000000000001",
            Name: "Company Test",
            LegalNature: CompanyLegalNatureType.EIRELI,
            MainActivityId: 1,
            Address: new AddressModel(
                PostalCode: "00000001",
                Street: "Test",
                Number: "1",
                Complement: "Test",
                Neighborhood: "Test",
                City: "Test",
                State: "TS",
                Country: "Test"
            ),
            Partners:
            [
                new CompanyPartnerModel(Guid.NewGuid(), 1, new DateTime(2022,1,1))
            ],
            Phones:
            [
                new PhoneModel("11","999999999")
            ]
        );
    }

    public static UpdateCompanyCommand GenerateValidUpdateCompanyCommand()
    {
        return new UpdateCompanyCommand(
            CompanyId: Guid.NewGuid(),
            Name: "Company Test",
            LegalNature: CompanyLegalNatureType.EIRELI,
            MainActivityId: 1,
            Address: new AddressModel(
                PostalCode: "00000001",
                Street: "Test",
                Number: "1",
                Complement: "Test",
                Neighborhood: "Test",
                City: "Test",
                State: "TS",
                Country: "Test"
            ),
            Phones:
            [
                new PhoneModel(
                    CountryCode: "11",
                    Number: "999999999"
                )
            ]
        );
    }
}
