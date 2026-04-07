using System.Threading;
using Spectre.Console.Cli;

namespace samples
{
    public class InfoCommand : Command<InfoCommand.Settings>
    {
        private readonly GreetingService _greetingService;

        public InfoCommand(GreetingService greetingService)
        {
            _greetingService = greetingService;
        }
        protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
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