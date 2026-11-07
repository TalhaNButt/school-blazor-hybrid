# .NET MAUI Blazor Hybrid Development Setup Guide

A comprehensive guide for setting up and running .NET MAUI Blazor Hybrid applications on macOS with Apple Silicon (M1/M2/M3).

## Table of Contents

- [Solution Architecture](#solution-architecture)
- [Prerequisites & Environment Setup](#prerequisites--environment-setup)
- [Clean Build Commands](#clean-build-commands)
- [Build and Run Commands](#build-and-run-commands)
- [Platform Comparison](#platform-comparison)
- [Common Issues & Fixes](#common-issues--fixes)
- [Quick Cheatsheet](#quick-cheatsheet)
- [Daily Workflow](#daily-workflow)

---

## Solution Architecture

The API follows **Clean Architecture**. The client is a thin MAUI host around a reusable Razor Class Library.

```
MyHybridApp2.slnx
global.json · Directory.Build.props · Directory.Packages.props · .editorconfig
src/
  Server/
    MyHybridApp2.Domain           entities and business rules (no dependencies)
    MyHybridApp2.Application      use cases (handlers), validators, IAppDbContext abstraction
    MyHybridApp2.Infrastructure   EF Core + PostgreSQL, entity configurations, migrations
    MyHybridApp2.Api              minimal API endpoints, ProblemDetails, OpenAPI, composition root
  Shared/
    MyHybridApp2.Shared           API contracts (request/response DTOs) used by the API and the clients
  Client/
    MyHybridApp2.UI               Razor Class Library: pages, layout, typed HttpClient services
    MyHybridApp2                  .NET MAUI host: platforms, resources, DI wiring
tests/
  MyHybridApp2.Application.Tests     unit tests (EF Core InMemory)
  MyHybridApp2.Api.IntegrationTests  end-to-end API tests against PostgreSQL in Docker (Testcontainers)
```

**Dependency rule:** `Api → Infrastructure → Application → Domain`. `Application` and `UI` both depend on `Shared`, and the client never references server projects.

**Conventions**
- Every NuGet version lives in `Directory.Packages.props`. Project files list packages without versions.
- Add a feature as `Application/<Feature>/<UseCase>.cs` (handler + validator), map it in `Api/Endpoints/<Feature>Endpoints.cs`, and register the handler in `Application/DependencyInjection.cs`.
- Validation errors return `400` ValidationProblem, `NotFoundException` returns `404`, and everything else returns `500` ProblemDetails (see `GlobalExceptionHandler`).
- Multi-school support: when it's needed, add `SchoolId` to `Domain/Common/Entity.cs` plus an EF global query filter in `AppDbContext`.

### Running the API

```bash
# Connection string: appsettings.Development.json has a local default. Override it with user secrets:
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=school_database;Username=...;Password=..." --project src/Server/MyHybridApp2.Api

# Apply migrations
dotnet ef database update --project src/Server/MyHybridApp2.Infrastructure --startup-project src/Server/MyHybridApp2.Api

# Add a migration
dotnet ef migrations add <Name> --project src/Server/MyHybridApp2.Infrastructure --startup-project src/Server/MyHybridApp2.Api --output-dir Persistence/Migrations

# Run (http://localhost:5206, API reference UI at /scalar, health at /health)
dotnet run --project src/Server/MyHybridApp2.Api --launch-profile http
```

In Debug builds the MAUI app calls `http://localhost:5206` (`http://10.0.2.2:5206` on the Android emulator), so start the API first.

### Tests

```bash
dotnet test                                                   # all tests (integration tests need Docker running)
dotnet test --project tests/MyHybridApp2.Application.Tests    # unit tests only
```

> The MAUI build and run commands below assume you are in `src/Client/MyHybridApp2`.

---

## Prerequisites & Environment Setup

### Verify Your Environment

Before running build commands, verify your environment is correctly configured:

```bash
# Check .NET SDK version
dotnet --version                                    # Should show 10.0.xxx

# Check installed workloads
dotnet workload list                                # Should include: maui, android, ios, maccatalyst

# Check Xcode version (critical for iOS builds)
xcodebuild -version                                 # Should be 27.0 or matching .NET workload

# Check Java JDK version
java -version                                       # Should be 17.x (not 27 or other versions)

# Check JAVA_HOME path
echo $JAVA_HOME                                     # Should point to JDK 17 installation

# Check Android emulator is running (for Android deployments)
adb devices                                         # Should list: emulator-5554 device

# Check available iOS simulators
xcrun simctl list devices available                 # List all iOS simulators
```

### Required Components

| Component | Version | Notes |
|-----------|---------|-------|
| .NET SDK | 10.0.401+ | Use ARM64 build for Apple Silicon |
| .NET MAUI Workload | 10.0+ | Includes android, ios, maccatalyst |
| Xcode | 27.0 | Must match .NET workload requirements |
| JDK | 17.x | Microsoft Build of OpenJDK or Temurin 17 |
| Android SDK | API 36+ | Install via Android Studio or sdkmanager |

### Setting JAVA_HOME Correctly

For Homebrew-installed OpenJDK 17 on Apple Silicon:

```bash
# Set JAVA_HOME in ~/.zshrc
export JAVA_HOME="/opt/homebrew/opt/openjdk@17/libexec/openjdk.jdk/Contents/Home"
export PATH="$JAVA_HOME/bin:$PATH"

# Reload shell
source ~/.zshrc

# Verify
java -version
```

For other JDK installations:

```bash
# List all available Java installations
/usr/libexec/java_home -V

# Set to specific version
export JAVA_HOME=$(/usr/libexec/java_home -v 17)
```

### Installing Missing Android SDK Platform

If you get `XA5207: Could not find android.jar for API level 36`:

```bash
# Install via sdkmanager
$HOME/Library/Android/sdk/cmdline-tools/latest/bin/sdkmanager "platforms;android-36"

# Or via Android Studio:
# Android Studio → Tools → SDK Manager → SDK Platforms → Check Android SDK Platform 36 → Apply
```

### Installing Android Emulator (AVD)

If you get `No devices are available for this project`:

```bash
# Create an Android Virtual Device
$SDKMGR="emulator" "platform-tools" "system-images;android-36;google_apis;arm64-v8a"

# Create the AVD
$AVDMGR create avd -n Pixel_API36 -k "system-images;android-36;google_apis;arm64-v8a" -d pixel_7

# Boot the emulator
$ANDROID_HOME/emulator/emulator -avd Pixel_API36 &

# Verify it's detected
adb devices
```

> **Note:** Always use `arm64-v8a` system images on Apple Silicon Macs for proper emulation performance.

---

## Clean Build Commands

### Full Clean Build (Recommended When Encountering Weird Errors)

```bash
# Remove all build artifacts
rm -rf obj bin
rm -rf **/obj **/bin

# Restore NuGet packages
dotnet restore

# Build all targets
dotnet build -f net10.0-maccatalyst
dotnet build -f net10.0-ios
dotnet build -f net10.0-android
```

### Clean NuGet Cache (If Dependency Issues Occur)

```bash
# Clear local NuGet cache
dotnet nuget locals all --clear

# Reinstall all NuGet packages
dotnet restore
```

### Repair Workload State (For Severe Corruption)

```bash
# Reset all workload manifests
dotnet workload repair

# Reinstall workloads
dotnet workload install maui
```

### Update Workload Versions

```bash
# Update to latest workload versions
dotnet workload update

# Update a specific workload
dotnet workload update --from-rollback-file <manifest-file>
```

---

## Build and Run Commands

### macOS (Mac Catalyst) ⚡ Fastest

Runs a native macOS app. No emulator needed.

```bash
# Build only
dotnet build -f net10.0-maccatalyst

# Build and run
dotnet run -f net10.0-maccatalyst

# Or in one command
dotnet build -t:Run -f net10.0-maccatalyst
```

### iOS Simulator

Requires Xcode 27.0 and iOS Simulator to be running.

```bash
# Build only
dotnet build -f net10.0-ios

# Build and run (auto-selects available simulator)
dotnet run -f net10.0-ios

# Specify a particular simulator
dotnet build -t:Run -f net10.0-ios \
  -p:RuntimeIdentifier=iossimulator-arm64 \
  -p:_DeviceName="iPhone 15 Pro" \
  -p:_TargetIdiom=Simulator

# List available simulators first
xcrun simctl list devices available
```

### Android Emulator

Requires an Android emulator to be running (check with `adb devices`).

```bash
# Build only
dotnet build -f net10.0-android

# Build and run (deploys to first available emulator)
dotnet run -f net10.0-android

# Target specific emulator
dotnet build -t:Run -f net10.0-android \
  -p:AdbTarget=-s\ emulator-5554
```

---

## Platform Comparison

| Platform | Build Time | Runtime | Emulator Required | Notes |
|----------|------------|---------|-------------------|-------|
| **macOS** | ~15-30s | Native | ❌ No | Fastest development loop |
| **iOS** | ~20-40s | Simulator | ✅ Yes | Need Xcode 27.0 + Simulator |
| **Android** | ~30-50s | Emulator | ✅ Yes | Need AVD (arm64-v8a images) |

### Build Performance Tips

```bash
# Use Release mode for faster builds (no debugger symbols)
dotnet build -c Release -f net10.0-maccatalyst

# Parallel build for multiple targets
dotnet build -f net10.0-maccatalyst -f net10.0-ios &
dotnet build -f net10.0-android
wait

# Skip specific target if not needed
dotnet build -f net10.0-maccatalyst -p:BuildAndroid=false
```

---

## Common Issues & Fixes

### Xcode Version Mismatch

**Error:** `This version of .NET for iOS (27.0.10722) requires Xcode 27.0. The current version of Xcode is 27.2.`

**Solution:**
```bash
# Install Xcode 27.0 alongside 27.2
# 1. Download from Apple Developer site
# 2. Rename before opening: mv Xcode.app Xcode_27.0.app
# 3. Move to Applications
# 4. Switch to it
sudo xcode-select --switch /Applications/Xcode_27.0.app

# Verify
xcodebuild -version
```

### Java SDK Validation Error

**Error:** `Failed to get the Java SDK version because the returned value does not appear to contain a valid version number.`

**Solution:**
```bash
# Ensure JDK 17 is installed and JAVA_HOME is set correctly
export JAVA_HOME="/opt/homebrew/opt/openjdk@17/libexec/openjdk.openjdk.jdk/Contents/Home"

# Or pin in project file
# In .csproj:
# <JavaSdkDirectory>/path/to/jdk17</JavaSdkDirectory>
```

### Missing Android Platform SDK

**Error:** `XA5207: Could not find android.jar for API level 36`

**Solution:**
```bash
# Install the missing SDK platform
sdkmanager "platforms;android-36"

# Or via dotnet target
dotnet build -t:InstallAndroidDependencies -f net10.0-android \
  -p:AndroidSdkDirectory=$HOME/Library/Android/sdk
```

### No Devices Found for Android

**Error:** `No devices are available for this project`

**Solution:**
```bash
# Start Android emulator first
open -a "Android Studio"

# Then verify with adb
adb devices

# Should show: emulator-5554 device
```

### Workspace Permission Errors

**Error:** `Operation not permitted` when renaming/moving files

**Solution:**
```bash
# Remove quarantine attribute
xattr -rd com.apple.quarantine /path/to/file

# Or move from Downloads folder before final location
```

---

## Quick Cheatsheet

| Goal | Command |
|------|---------|
| **Quick test** | `dotnet run -f net10.0-maccatalyst` |
| **iOS test** | `dotnet run -f net10.0-ios` |
| **Android test** | `dotnet run -f net10.0-android` |
| **Full rebuild** | `rm -rf obj bin; dotnet restore; dotnet build` |
| **Check emulators** | `adb devices` & `xcrun simctl list devices` |
| **Check workloads** | `dotnet workload list` |
| **Check Xcode** | `xcodebuild -version` |
| **Clear NuGet cache** | `dotnet nuget locals all --clear` |
| **Repair workloads** | `dotnet workload repair` |

---

## Daily Workflow

```bash
# 1. Start with macOS for fastest feedback loop
dotnet run -f net10.0-maccatalyst

# 2. Test on iOS when making platform-specific changes
dotnet run -f net10.0-ios

# 3. Final validation on Android
dotnet run -f net10.0-android

# 4. Clean only if you hit weird caching issues
dotnet clean && dotnet build
```

### Complete Bash Script

Save this as `run-all-platforms.sh`:

```bash
#!/bin/bash
# run-all-platforms.sh - Build and run on all platforms

echo "=== Cleaning All Builds ==="
rm -rf obj bin **/obj **/bin
dotnet restore

echo "=== Building All Targets ==="
dotnet build -f net10.0-maccatalyst
dotnet build -f net10.0-ios
dotnet build -f net10.0-android

echo "=== Running macOS ==="
dotnet run -f net10.0-maccatalyst &

echo "=== Waiting 10 seconds for macOS launch ==="
sleep 10

echo "=== Run iOS (ensure Simulator is open) ==="
dotnet run -f net10.0-ios &

echo "=== Run Android (ensure emulator is running) ==="
dotnet run -f net10.0-android
```

Execute with:

```bash
chmod +x run-all-platforms.sh
./run-all-platforms.sh
```

---

## Additional Resources

- [.NET MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
- [Xcode Requirement Troubleshooting](https://learn.microsoft.com/en-us/dotnet/ios/troubleshooting/xcode-requirement)
- [Android Error XA5207](https://learn.microsoft.com/en-us/dotnet/android/messages/xa5207)
- [Blazor Hybrid Guide](https://learn.microsoft.com/en-us/aspnet/core/blazor/hybrid/)
- [.NET for Android Messages](https://learn.microsoft.com/en-us/dotnet/android/messages/)

---

*Generated for .NET MAUI Blazor Hybrid development on macOS (Apple Silicon)*