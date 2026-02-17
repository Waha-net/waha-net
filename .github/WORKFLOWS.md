# GitHub Workflows Documentation

This repository uses two main workflows for Continuous Integration (CI) and Continuous Deployment (CD).

## CI Workflow - Build and Test

**File:** `.github/workflows/ci.yml`

**Trigger:** 
- Pull requests to any branch
- Push to any branch (except tags)

**Purpose:**
- Validate code quality on every PR and push
- Build the solution
- Run tests (if available)
- Perform SonarQube analysis (optional)

### SonarQube Configuration (Optional)

To enable SonarQube analysis, configure the following repository secrets and variables:

**Required Secrets:**
- `SONAR_TOKEN` - SonarQube authentication token

**Required Variables:**
- `SONAR_HOST_URL` - SonarQube server URL (e.g., `https://sonarcloud.io`)
- `SONAR_PROJECT_KEY` - Your SonarQube project key
- `SONAR_ORGANIZATION` - Your SonarQube organization (for SonarCloud)

**Note:** If these are not configured, the workflow will skip SonarQube analysis and continue with the build.

## CD Workflow - Release and Publish

**File:** `.github/workflows/cd.yml`

**Trigger:**
- Tag creation matching pattern `v*` (e.g., `v1.0.0`, `v1.2.3`)

**Purpose:**
- Build and package the NuGet package with version from tag
- Publish to GitHub Packages
- Publish to NuGet.org (if configured)
- Create a GitHub Release with the tag

### Required Secrets

- `GITHUB_TOKEN` - Automatically provided by GitHub Actions (used for GitHub Packages and Release creation)
- `NUGET_API_KEY` - Your NuGet.org API key (optional, only needed for publishing to NuGet.org)

### How to Release

1. Ensure your code is ready for release on the main branch
2. Create and push a version tag:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```
3. The CD workflow will automatically:
   - Build the project
   - Create a NuGet package with version `1.0.0`
   - Publish to GitHub Packages
   - Publish to NuGet.org (if `NUGET_API_KEY` is configured)
   - Create a GitHub Release with release notes

### Version Format

The tag must start with `v` followed by a semantic version number:
- `v1.0.0` → Package version: `1.0.0`
- `v2.1.3-beta` → Package version: `2.1.3-beta`
- `v1.0.0-alpha.1` → Package version: `1.0.0-alpha.1`

## Setting Up Secrets and Variables

### GitHub Secrets
1. Go to repository **Settings** → **Secrets and variables** → **Actions**
2. Click **New repository secret**
3. Add the required secrets mentioned above

### GitHub Variables
1. Go to repository **Settings** → **Secrets and variables** → **Actions** → **Variables** tab
2. Click **New repository variable**
3. Add the required variables mentioned above

## Workflow Status

You can monitor workflow runs in the **Actions** tab of the repository.
