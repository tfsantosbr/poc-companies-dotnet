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

### How to Switch Environments

The REST Client extension provides multiple ways to switch between environments:

#### Method 1: Status Bar (Recommended)

1. Open any `.http` file in VS Code
2. Look at the bottom-right corner of the VS Code status bar
3. Click on **"No Environment"** or the currently selected environment name
4. A dropdown menu will appear with available environments:
   - `docker`
   - `local`
5. Select the desired environment
6. The status bar will update to show the selected environment
7. All requests will now use variables from the selected environment

#### Method 2: Command Palette

1. Press `Ctrl+Shift+P` (Windows/Linux) or `Cmd+Shift+P` (Mac)
2. Type "Rest Client: Switch Environment"
3. Select the command from the list
4. Choose between `docker` or `local`

#### Method 3: Right-Click Context Menu

1. Open any `.http` file
2. Right-click anywhere in the file
3. Select **"Switch Environment"** from the context menu
4. Choose the desired environment

### Environment Variables

Once an environment is selected, the following variables are automatically available:

| Variable | Docker Value | Local Value |
|----------|--------------|-------------|
| `{{baseUrl}}` | `http://localhost:8000` | `https://localhost:7274` |
| `{{apiVersion}}` | `v1` | `v1` |

**Example:** When you select the `docker` environment, `{{baseUrl}}` in your requests will be replaced with `http://localhost:8000`.

### Visual Guide

```
┌─────────────────────────────────────────────────────────────┐
│ VS Code Status Bar (bottom-right)                          │
├─────────────────────────────────────────────────────────────┤
│ ... │ UTF-8 │ LF │ HTTP │ [No Environment ▼] │ ⚙️ │      │
│                              ↑                               │
│                         Click here to switch                │
└─────────────────────────────────────────────────────────────┘
```

After selecting an environment, the status bar will show:
- `[docker ▼]` - Using Docker environment (port 8000)
- `[local ▼]` - Using Local environment (port 7274)

**💡 Tip:** Always verify the correct environment is selected before running requests to avoid connection errors!

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
- **Check environment selection**: Verify the correct environment is active in the status bar
  - If API is running in Docker → Select `docker` environment
  - If API is running locally → Select `local` environment
- **Verify the API is running**:
  - Docker: Run `docker ps` to check if `labs-companies-api` container is running
  - Local: Check if the API process is running on port 7274
- **Port verification**:
  - Docker uses port `8000` (HTTP)
  - Local uses port `7274` (HTTPS)
- **HTTPS certificate**: For local HTTPS, you may need to accept the self-signed certificate in your browser first

### Invalid Data
- Check data constraints (CNPJ length, valid IDs, etc.)
- Ensure required fields are present
- Verify JSON syntax is valid

## Additional Resources

- [REST Client Extension Documentation](https://github.com/Huachao/vscode-restclient)
- [Companies API Documentation](../../README.md)
- Original Postman collection: `postman/collections/Companies API.postman_collection.json`
