using Microsoft.Extensions.CommandLineUtils;
using OAuthUtils.TokenOperations;
using OAuthUtils.OAuthOperations;

namespace OAuthUtils
{
    internal class Commands : CommandLineApplication
    {
        public Commands()
        {
            Commands.Add(new DecodeCommand());
            Commands.Add(new VerifyCommand());
            Commands.Add(new ClientCommand());
            Commands.Add(new ResourceCommand());
            Commands.Add(new IntrospectionCommand());
            HelpOption("-h | -? | --help");
        }
    }
}
