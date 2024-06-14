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
*   Umbraco 12.0.1+
*   Save metadata
*   Tinify everything (all media)
*   Undo Tinify

### Backoffice Dashboard

*   A dashboard has been added to the Settings section of the backoffice
*   You need to add the Tinifier Section according the group member in the Umbraco Backoffice.
![Tinifier-Section](App_Plugins\Cogworks.Tinifier\media\tinifier-section.jpg)
*   This dashboard contains a button to manually trigger a **Full Tinify** (to optimize all the current media) or **Stop Tinify** (to reset Tinifier tables).
*   There is also options to view and manage the current configuration for the package : API key, Optimize on upload, Enable undo optimization and Preserve image metadata.
*   An image and folder could be tinified.

# Installation

Install through dotnet CLI:
```c#
dotnet add package Cogworks.Tinifier
```

Or the NuGet Package Manager:
```c#
Install-Package Cogworks.Tinifier
```

You can set the settings in the backoffice, or add these settings to the **appsettings.json** (this will be always the priority).
```js
  "CogTinifierSettings": {
    "ApiKey": "xxx",
    "EnableOptimizationOnUpload": true,
    "EnableUndoOptimization": true,
    "PreserveMetadata": true 
  }
```

## Backoffice UI Test User:

```sh
Email: admin@admin.com
Password: 0123456789
```

## Acknowledgements
This package was inspired by the work of [Dmytro Obolonyk](https://github.com/dimonser147) in [UmbracoTinifier2](https://github.com/dimonser147/UmbracoTinifier2). Thank you for your valuable contributions!

### License

Licensed under the [MIT License](LICENSE.md)

&copy; 2024 [Cogworks](https://www.wearecogworks.com/)