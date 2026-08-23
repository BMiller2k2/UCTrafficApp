# 🚦 UC Traffic

UC Traffic is a senior design project that provides **real-time traffic updates, accident reports, and optimized routing** for users in and around the University of Cincinnati (UC) area. The app helps commuters make informed travel decisions, reduce delays, and improve road safety.

---

## 📌 Project Summary
Traffic congestion impacts daily routines, productivity, and safety. UC Traffic addresses these issues by combining **GPS data, user reporting, and traffic monitoring systems** to deliver live updates and smarter commuting options.  
The project goal is to have a **functional prototype by Spring 2026**.

---

## 🛑 Problem Statement
Traffic jams are unpredictable and disruptive, leading to wasted time, increased stress, and higher accident risks. According to the American Transportation Research Institute, traffic congestion cost the trucking industry **$20.1 billion in 2021**, representing over **1.27 billion hours of delay** nationwide.  
UC Traffic aims to reduce these challenges at a **local level**, focusing on the UC campus community.

---

## 🛠️ Tech Stack
- **.NET MAUI** (`UCTrafficApp.csproj`), targeting `net10.0-android` and `net10.0-windows10.0.19041.0`
- SQLite (`sqlite-net-pcl`) for local user/issue data
- MailKit/MimeKit for verification emails (via a Mailtrap sandbox)

---

## ✅ Prerequisites

Before cloning, make sure your machine has:

1. **[.NET 10 SDK](https://dotnet.microsoft.com/download)** (10.0.400 or newer). Check with:
   ```bash
   dotnet --version
   ```
2. **The Android and Windows MAUI workloads.** Install/update them with:
   ```bash
   dotnet workload install android maui-windows
   dotnet workload update
   ```
   Verify what's installed with `dotnet workload list` — you need an `android` workload and a `maui-windows` workload present.
3. To run on **Android**: Android SDK + an emulator (AVD) via Android Studio, or a physical device with USB debugging enabled.
4. To run as a **Windows app**: Windows 10/11, and the **[Windows App SDK Runtime](https://learn.microsoft.com/windows/apps/windows-app-sdk/downloads)** installed (Visual Studio will usually prompt to install this automatically the first time you run the Windows target — if it doesn't, install it manually).

> ⚠️ **Version-sensitive project.** This project only builds against the *exact* SDK/workload set above — an older .NET 8 or .NET 9 SDK will fail to build it (Android/MAUI workloads for those versions are end-of-life and rejected by the toolchain). If you clone this onto a machine with a different SDK installed, run `dotnet --version` and `dotnet workload list` first and update before opening the solution.

---

## 🚀 Getting Started

```bash
git clone https://github.com/BMiller2k2/UCTrafficApp.git
cd UCTrafficApp/UCTrafficApp
dotnet restore
```

**Run on Android emulator/device** (with an emulator already running or a device connected via `adb devices`):
```bash
dotnet build UCTrafficApp.csproj -t:Run -f net10.0-android
```

**Build for Windows** (confirms it compiles):
```bash
dotnet build UCTrafficApp.csproj -f net10.0-windows10.0.19041.0
```
To actually **launch** the Windows app, use Visual Studio (F5) rather than `dotnet build -t:Run` — that target bundles build+launch into one MSBuild step, so if the Windows App SDK Runtime isn't installed yet (see Prerequisites #4) it reports the whole thing as `Build FAILED` even though compilation succeeded, which is confusing. Visual Studio handles the missing-runtime case much more gracefully (usually prompting to install it).

**Or in Visual Studio 2022 (17.14+):** open `UCTrafficApp.sln`, pick your target (an Android emulator/device, or **Windows Machine**) from the debug target dropdown next to the Run button, and press **F5**.

---

## 📝 Notes for Contributors
- `Services/EmailService.cs` uses a shared **Mailtrap sandbox** for verification emails — fine for local dev, but rotate/replace these credentials before any real deployment.
- `Services/TrafficService.cs` currently points at a placeholder API endpoint (`your-api-endpoint.com`) — live traffic data isn't wired up yet; the home screen will show "No traffic data available" until a real endpoint (e.g. the planned Waze API integration) is configured.
- `UCTrafficApp.csproj.user` is per-developer IDE state (debug target/profile) and is intentionally gitignored — everyone picks their own run target locally.
