# BlazorLazyLoad

[![Package Version](https://img.shields.io/nuget/v/BlazorLazyLoad.svg)](https://www.nuget.org/packages/BlazorLazyLoad)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BlazorLazyLoad.svg)](https://www.nuget.org/packages/BlazorLazyLoad)
[![License](https://img.shields.io/github/license/MarekPokornyOva/BlazorLazyLoad.svg)](https://github.com/MarekPokornyOva/BlazorLazyLoad/blob/master/LICENSE)

### Description
BlazorLazyLoad is mostly loudly shared idea/concept of assembly lazy load in Blazor WASM application.
Splitting an application speeds up its start and also saves network traffic.

### Features
- Lazy loads assemblies.
- Lazy resolving on both page and component level.
- Registers included pages for routing.
- Registers included services to ServiceProvider.

### Usage
1) Include Nuget package - https://www.nuget.org/packages/BlazorLazyLoad/ in Blazor WASM application's project.
2) Create custom assembly lazy load handler. The \Samples\01\BlazorApp\AreaAssemblyLazyLoadResolver.cs might be good example - it expects areas split strategy. The resolver can also register the services defined in the lazy loaded assembly.
3) Change Router in App.razor to the enhanced one. That invokes AssemblyLazyLoadResolver when a lazy loaded page is navigated at first (entered to navigation bar or on the page refresh).
4) Register services - call builder.Services.AddLazyLoad<AreaAssemblyLazyLoadResolver>(); within Program.Main() method.
5) Redirect navigation event to custom handler - see \Samples\01\BlazorApp\wwwroot\index.html.
6) Create project containing lazy loaded pages - see \Samples\01\LazyLoadedArea.
7) The built assembly has to be copied from wwwroot\\_framework\\_bin to the main project's wwwroot\\_framework\\bin folder. It's recommended to use gzipped versions.
8) See \Samples\01\BlazorApp\LazyComponent for component level lazy loading.

### Notes
- All is provided as is without any warranty.
- The target of this concept has been "make it functional for any price". Therefore some pieces are bit "hacky".
- Developed with version 3.2.0-preview4.20210.8 wasm.

### Release notes
[See](./ReleaseNotes.md)

### Thanks to Blazor team members for their work
### Thanks to Chris Sainty for his article https://chrissainty.com/an-in-depth-look-at-routing-in-blazor/
