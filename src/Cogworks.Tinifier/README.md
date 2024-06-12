# Cogworks.Tinifier

[![NuGet release](https://img.shields.io/nuget/v/Cogworks.Tinifier.svg)](https://www.nuget.org/packages/Cogworks.Tinifier/)

 is a free Umbraco package for image compression that removes files size limits, supports Azure. It allows to dramatically reduce the size of PNG and JPEG images which positively influence on a page loading time and visitors loyalty.

## Usage

### Basic Functionality

*   The ability to toggle the package functionality on/off in the settings
*   Individual images optimization
*   Bulk images optimization
*   Image optimization on upload
*   Folders optimization
*   Supported image formats: PNG and JPEG
*   Optimized image stats
*   Total savings widget
*   API requests widget
*   Umbraco 12.3.6+
*   Save metadata
*   Tinify everything (all media)
*   Undo Tinify

### Backoffice Dashboard

*   A dashboard has been added to the Settings section of the backoffice
*   Currenlty only **Admins** can access it. (It can be updated)
*   This dashboard contains a button to manually trigger a **Full Tinify** (to optimize all the current media) or **Stop Tinify** (to reset Tinifier tables).
*   There is also options to view and manage the current configuration for the package : API key, Optimize on upload, Enable undo optimization and Preserve image metadata.

# Installation

Install through dotnet CLI:
```c#
dotnet add package Cogworks.Tinifier
```

Or the NuGet Package Manager:
```c#
Install-Package Cogworks.Tinifier
```

You can set the settings in the backoffice, or add these settings to the **appsettings.json** to be the priority.
```js
  "CogTinifierSettings": {
    "ApiKey": "xxx",
    "EnableOptimizationOnUpload": true,
    "EnableUndoOptimization": true,
    "PreserveMetadata": true 
  }
```

### License

Licensed under the [MIT License](LICENSE.md)

&copy; 2024 [Cogworks](https://www.wearecogworks.com/)