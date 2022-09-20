using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Spectre.Cli.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSingleton<GreetingService>();

// add extra services to the container here
using var registrar = new DependencyInjectionRegistrar(services);
var app = new CommandApp(registrar);
app.SetDefaultCommand<InfoCommand>();
app.Configure(config =>
{
    config.AddCommand<InfoCommand>("greet");
    // configure your commands as per usual
    // commands are automatically added to the container
});

return app.Run(args);