using Microsoft.Extensions.CommandLineUtils;

namespace OAuthUtils
{
    internal class Commands : CommandLineApplication
    {
        public Commands()
        {
            Commands.Add(new DecodeCommand());
            Commands.Add(new VerifyCommand());
            HelpOption("-h | -? | --help");
        }
    }
}
