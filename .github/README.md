# 🚀 GitHub Actions CI/CD for RadioControlApp

This directory contains GitHub Actions workflows for continuous integration and deployment of the RadioControlApp MAUI Android application.

## 📋 Workflows Overview

### 1. **Main CI/CD Pipeline** (`dotnet.yml`)
The primary workflow that handles building, testing, and releasing the application.

**Triggers:**
- Push to `main` branch
- Push to any `cursor/*` branch  
- Git tags starting with `v*` (e.g., `v1.1`, `v2.0`)
- Manual workflow dispatch

**Jobs:**
- 🏗️ **Build and Test**: Compiles the app and runs tests
- 📦 **Build APK**: Creates Android APK for releases
- 🚀 **Create Release**: Automatically creates GitHub releases for tags
- 📢 **Notify Status**: Provides build status summary

### 2. **Pull Request Validation** (`pr-validation.yml`)
Specialized workflow for validating pull requests before merge.

**Triggers:**
- Pull requests to `main` branch
- Manual workflow dispatch

**Jobs:**
- 🔍 **Code Quality**: Checks code formatting and analysis
- 🔒 **Security Scan**: Scans for potential security issues
- ✅ **Build Validation**: Validates both Debug and Release builds
- 💬 **PR Comment**: Posts validation results as PR comments

## 🔧 Workflow Features

### ✨ **Automated Building**
- **Multi-Configuration**: Builds both Debug and Release configurations
- **Artifact Management**: Uploads build artifacts with retention policies
- **Dependency Caching**: Efficient package restoration and caching

### 📱 **Android-Specific Setup**
- **Android SDK**: Automatically installs Android SDK (API 34)
- **MAUI Workloads**: Installs required .NET MAUI Android workloads
- **APK Generation**: Creates signed APK files for distribution

### 🚀 **Release Automation**
- **Automatic Releases**: Creates GitHub releases for version tags
- **Release Notes**: Generates comprehensive release notes
- **Asset Upload**: Automatically attaches APK files to releases

### 🔍 **Quality Assurance**
- **Code Formatting**: Validates code formatting standards
- **Security Scanning**: Basic security vulnerability checks
- **Build Validation**: Multi-configuration build testing

## 📦 Environment Configuration

### **Environment Variables**
```yaml
DOTNET_VERSION: '8.0.x'          # .NET version to use
PROJECT_PATH: './RadioControlApp.csproj'  # Main project file
ANDROID_API_LEVEL: 34            # Android API level
BUILD_CONFIGURATION: Release     # Default build configuration
```

### **Required Secrets**
- `GITHUB_TOKEN`: Automatically provided by GitHub (for releases)

### **Optional Secrets** (for advanced features)
- `ANDROID_KEYSTORE`: For APK signing (base64 encoded)
- `KEYSTORE_PASSWORD`: Keystore password
- `KEY_ALIAS`: Signing key alias
- `KEY_PASSWORD`: Signing key password

## 🎯 Usage Examples

### **Manual Triggering**
You can manually trigger workflows from the GitHub Actions tab:
1. Go to **Actions** tab in your repository
2. Select the workflow you want to run
3. Click **Run workflow**
4. Choose branch and run

### **Creating a Release**
To create a new release with automatic APK generation:

```bash
# Tag the commit
git tag v1.2.0

# Push the tag
git push origin v1.2.0
```

This will:
1. ✅ Build the application
2. 📦 Generate APK file  
3. 🚀 Create GitHub release
4. 📎 Attach APK to release

### **Pull Request Process**
When creating a pull request:
1. 🔍 Code quality checks run automatically
2. 🔒 Security scans are performed
3. ✅ Build validation ensures compatibility
4. 💬 Results are posted as PR comments

## 📊 Workflow Status Badges

Add these badges to your main README.md:

```markdown
[![.NET MAUI CI/CD](https://github.com/Murzyneczka/RadioControlApp/actions/workflows/dotnet.yml/badge.svg?branch=main&event=release)](https://github.com/Murzyneczka/RadioControlApp/actions/workflows/dotnet.yml)
![PR Validation](https://github.com/Murzyneczka/RadioControlApp/workflows/Pull%20Request%20Validation/badge.svg)
```

## 🔧 Customization

### **Adding New Build Configurations**
To add new build configurations, modify the matrix strategy:

```yaml
strategy:
  matrix:
    configuration: [Debug, Release, Staging]
```

### **Adding Code Coverage**
Add code coverage reporting:

```yaml
- name: 📊 Generate coverage report
  run: |
    dotnet test --collect:"XPlat Code Coverage"
    
- name: 📋 Upload coverage
  uses: codecov/codecov-action@v3
```

### **Adding Slack Notifications**
Add Slack notifications for build results:

```yaml
- name: 📢 Slack Notification
  uses: 8398a7/action-slack@v3
  with:
    status: ${{ job.status }}
    channel: '#development'
  env:
    SLACK_WEBHOOK_URL: ${{ secrets.SLACK_WEBHOOK }}
```

## 🚨 Troubleshooting

### **Common Issues**

**Issue: "Android SDK not found"**
```yaml
# Add Android SDK setup step
- name: 📱 Setup Android SDK
  uses: android-actions/setup-android@v3
  with:
    api-level: 34
```

**Issue: "MAUI workload not installed"**
```yaml
# Ensure workload installation
- name: 🔧 Install MAUI Workloads
  run: dotnet workload install maui-android --accept-eula
```

**Issue: "Build artifacts not found"**
```yaml
# Check artifact paths
- name: 🔍 List build outputs
  run: find . -name "*.apk" -o -name "*.aab"
```

### **Debugging Workflows**
- Enable debug logging: Add `ACTIONS_STEP_DEBUG: true` to secrets
- Use `echo` commands to debug values
- Check the Actions tab for detailed logs

## 📈 Performance Optimization

### **Build Time Improvements**
- **Parallel Jobs**: Use job dependencies efficiently
- **Caching**: Implement NuGet package caching
- **Incremental Builds**: Use `--no-restore` when appropriate

### **Resource Management**
- **Artifact Retention**: Set appropriate retention periods
- **Concurrent Jobs**: Limit concurrent workflow runs if needed

## 🔄 Maintenance

### **Regular Updates**
- 📅 **Monthly**: Update action versions (`@v4` → `@v5`)
- 📅 **Quarterly**: Review and update .NET versions
- 📅 **Yearly**: Review workflow efficiency and add new features

### **Monitoring**
- 📊 Monitor workflow success rates
- 📈 Track build times and optimize as needed
- 🔍 Review security scan results regularly

---

## 🎉 Benefits

With this CI/CD setup, you get:

- ✅ **Automated Testing**: Every commit is tested
- 🚀 **Fast Releases**: One-click releases with git tags
- 🔍 **Quality Assurance**: Code quality checks on every PR
- 📦 **Artifact Management**: Organized build outputs
- 🔒 **Security**: Automated security scans
- 📊 **Visibility**: Clear status reporting and notifications

**Your development workflow is now fully automated! 🎊**
