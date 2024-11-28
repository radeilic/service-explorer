using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.HelpText;

namespace ServiceExplorer.CommandLine.Commands;

public class CustomHelpTextGenerator : DefaultHelpTextGenerator
{
    protected override void GenerateHeader(CommandLineApplication application, TextWriter output)
    {
        output.WriteLine(@"
███████╗███████╗██████╗ ██╗   ██╗██╗ ██████╗███████╗    ███████╗██╗  ██╗██████╗ ██╗      ██████╗ ██████╗ ███████╗██████╗ 
██╔════╝██╔════╝██╔══██╗██║   ██║██║██╔════╝██╔════╝    ██╔════╝╚██╗██╔╝██╔══██╗██║     ██╔═══██╗██╔══██╗██╔════╝██╔══██╗
███████╗█████╗  ██████╔╝██║   ██║██║██║     █████╗      █████╗   ╚███╔╝ ██████╔╝██║     ██║   ██║██████╔╝█████╗  ██████╔╝
╚════██║██╔══╝  ██╔══██╗╚██╗ ██╔╝██║██║     ██╔══╝      ██╔══╝   ██╔██╗ ██╔═══╝ ██║     ██║   ██║██╔══██╗██╔══╝  ██╔══██╗
███████║███████╗██║  ██║ ╚████╔╝ ██║╚██████╗███████╗    ███████╗██╔╝ ██╗██║     ███████╗╚██████╔╝██║  ██║███████╗██║  ██║
╚══════╝╚══════╝╚═╝  ╚═╝  ╚═══╝  ╚═╝ ╚═════╝╚══════╝    ╚══════╝╚═╝  ╚═╝╚═╝     ╚══════╝ ╚═════╝ ╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝");
        base.GenerateHeader(application, output);
    }
}
