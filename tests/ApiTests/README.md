# API Tests - REST Client Files

This directory contains REST Client files (.http) for testing the Companies API. These files can be executed using the [REST Client extension](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) for Visual Studio Code.

## Files Overview

| File | Endpoints | Description |
|------|-----------|-------------|
| `authentication.http` | 1 | Authentication endpoint to obtain access tokens |
| `companies.http` | 6 | Company management (create, update, delete, list, import) |
| `partners.http` | 3 | Partner management (create, delete, list) |
| `company-partners.http` | 3 | Company-Partner relationship management |
| `main-activities.http` | 1 | List available main activities |
| `qualifications.http` | 1 | List available partner qualifications |
| `health.http` | 4 | Health check endpoints |

**Total: 19 endpoints**

## Environment Configuration

The `http-client.env.json` file defines two environments:

- **docker**: API running in Docker at `http://localhost:8000`
- **local**: API running locally at `https://localhost:7274`

To switch environments in VS Code REST Client, use the environment selector in the status bar.

## Getting Started

### 1. Install REST Client Extension

Install the [REST Client extension](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) for VS Code.

### 2. Start the API

Choose one of the following:

**Docker:**
```bash
docker-compose up
```

**Local Development:**
```bash
dotnet run --project src/Companies.Api
```

### 3. Run Requests

#### Basic Workflow

1. **Get Authentication Token**
   - Open `authentication.http`
   - Click "Send Request" above the `POST` line
   - The token is automatically captured in the `@access_token` variable

2. **Test Company Operations**
   - Open `companies.http`
   - Replace `@access_token = your-token-here` with the actual token from step 1
   - Or, run the authentication request first and copy the token
   - Click "Send Request" for any endpoint
   - The `Create Company` request will capture the company ID automatically

3. **Test Partner Operations**
   - Open `partners.http`
   - Follow the same authentication pattern
   - Create a partner and the ID will be captured automatically

4. **Test Company-Partner Relationships**
   - Open `company-partners.http`
   - Ensure you have both `@lastCreatedCompanyId` and `@lastCreatedPartnerId` set
   - Run the relationship endpoints

#### Full Integration Test Workflow

Execute requests in this order for a complete end-to-end test:

1. `authentication.http` → Request Token
2. `companies.http` → Create Company (captures ID)
3. `partners.http` → Create Partner (captures ID)
4. `company-partners.http` → Add Company Partner
5. `company-partners.http` → Find Company Partners
6. `companies.http` → Get Valid Company
7. `companies.http` → Update Valid Company
8. `company-partners.http` → Remove Company Partner
9. `companies.http` → Remove Valid Company
10. `partners.http` → Remove Partner

## Variable Management

### Environment Variables (from http-client.env.json)

- `{{baseUrl}}` - API base URL (depends on selected environment)
- `{{apiVersion}}` - API version (v1)

### Captured Variables (from responses)

- `@access_token` - Captured from authentication response
- `@lastCreatedCompanyId` - Captured from Create Company response
- `@lastCreatedPartnerId` - Captured from Create Partner response

### Manual Variables

Some files have placeholder variables that need to be set manually:

```http
@access_token = your-token-here
@lastCreatedCompanyId = your-company-id-here
@lastCreatedPartnerId = your-partner-id-here
```

Replace these with actual values or run the corresponding requests to capture them automatically.

## Request Syntax

REST Client files use the following syntax:

```http
### Request Name/Comment
GET {{baseUrl}}/{{apiVersion}}/endpoint
Authorization: Bearer {{access_token}}
```

- `###` separates different requests
- Variables are referenced with `{{variableName}}`
- Variables can be defined with `@variableName = value`
- Responses can be named with `# @name requestName` for capturing values

## Data Constraints

### CNPJ (Company Tax ID)
- Must be exactly 14 digits
- Example: `"12345678901234"`

### Legal Nature Options
- `EIRELI` - Individual Limited Liability Company
- `MEI` - Individual Microentrepreneur
- `LTDA` - Limited Company

### Main Activity IDs
Valid options: `4399103`, `4723700`, `4774100`, `5611203`, `9492800`

### Partner Qualification IDs
Valid options: `5`, `10`, `16`, `22`, `54`

### Sample Partner IDs
Use these existing partner IDs for testing:
- `012af8a2-8de0-45b7-b9c7-dcfdb3b491b3`
- `1a0592e2-71f0-48cc-8267-0f8d75fe0a5e`
- `9132b269-ff90-402d-88d0-2f9e7fdb312f`
- `c7baa255-6bfb-4d67-91da-fc1d858fdf00`
- `fd532165-d7fd-44bb-b19b-f8e2a1c2073f`

## Tips for AI Assistants

These REST Client files are designed to be easily read and executed by AI assistants:

1. **Self-documenting**: Each file includes comments explaining the endpoints and parameters
2. **Clear structure**: One file per functional area (companies, partners, etc.)
3. **Variable capture**: Important IDs are automatically captured from responses
4. **Environment support**: Easy switching between Docker and local environments
5. **Plain text**: No binary formats or complex tools required

To run a test:
1. Read the appropriate .http file
2. Replace placeholder variables if needed
3. Execute the HTTP request
4. Parse the response

## Troubleshooting

### Authentication Issues
- Ensure the API is running
- Check that the username/password in `authentication.http` are correct (default: test/test)
- Verify the token is being captured correctly

### Connection Refused
- Verify the API is running on the correct port
- Check that you've selected the correct environment (docker vs local)
- For local HTTPS, you may need to accept the self-signed certificate

### Invalid Data
- Check data constraints (CNPJ length, valid IDs, etc.)
- Ensure required fields are present
- Verify JSON syntax is valid

## Additional Resources

- [REST Client Extension Documentation](https://github.com/Huachao/vscode-restclient)
- [Companies API Documentation](../../README.md)
- Original Postman collection: `postman/collections/Companies API.postman_collection.json`
