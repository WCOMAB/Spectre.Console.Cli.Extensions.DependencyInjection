using System;
using Spectre.Cli;

namespace samples
{
    public class InfoCommand : Command<InfoCommand.Settings>
    {
        private readonly GreetingService _greetingService;

        public InfoCommand(GreetingService greetingService)
        {
            _greetingService = greetingService;
        }
        public override int Execute(CommandContext context, Settings settings)
        {
            _greetingService.Greet("World");
            return 0;
        }

        public class Settings : CommandSettings {
            [CommandOption("-v")]
            public bool Verbose {get;set;}
        }
    }
}