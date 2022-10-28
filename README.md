# Spectre.Console.Cli.Extensions.DependencyInjection

A type provider for the CLI component of [`Spectre.Console`](https://github.com/spectresystems/spectre.console)  using the [`Microsoft.Extensions.DependencyInjection`](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) container.

> This package was designed for `Spectre.Cli` which has now been merged into `Spectre.Console`.

## Getting started

Once you've installed both `Spectre.Console` and this package, just change the call to `new CommandApp()` in your `Program.cs` to match the below:

```csharp
var services = new ServiceCollection();
// add extra services to the container here
using registrar = new DependencyInjectionRegistrar(services);
var app = new CommandApp(registrar);
app.Configure(config =>
{
    // configure your commands as per usual
    // commands are automatically added to the container
}
return app.Run(args);
```

## Using the container

Once you've changed your `Program.cs` as above, you can inject anything you need into your commands:

```csharp
// Program.cs
var services = new ServiceCollection();
services.AddSingleton<ICoolService, MyCoolService>();
using registrar = new DependencyInjectionRegistrar(services);
var app = new CommandApp(registrar);
app.Configure(config =>
{
    config.AddCommand<SomeCommand>("command");
    // commands are automatically added to the container
}
```

```csharp
// in SomeCommand.cs
public class SomeCommand : Command<SomeCommand.Settings>
{
    public ProfileCreateCommand(ICoolService service)
    {
        // the `ICoolService` parameter will be automatically injected with an instance of `MyCoolService`
    }

    public override int Execute(SomeCommand.Settings settings, ILookup<string, string> unmapped)
    {
        // command logic here
    }
}
```

> For a more complex example, check out [git-profile-manager](https://github.com/agc93/git-profile-manager).