using IdentityModel.Client;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthUtils.OAuthOperations
{
    internal class IntrospectionCommand : CommandLineApplication
    {
        protected readonly ILogger<IntrospectionCommand> _logger;
        private readonly CommandOption _server;
        private readonly CommandOption _client;
        private readonly CommandOption _secret;
        private readonly CommandOption _token;

        public IntrospectionCommand(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Name = "intro";
            Description = "Validate a JWT Access Token via the introspection endpoint";

            HelpOption("-h | -? | --help");

            _server = Option("-s | --server", "The OAuth server to resolve userinfo", CommandOptionType.SingleValue);
            _client = Option("-i | --clientId", "The clientId", CommandOptionType.SingleValue);
            _secret = Option("-c | --secret", "The client secret", CommandOptionType.SingleValue);
            _token = Option("-t | --token", "The JWT access token", CommandOptionType.SingleValue);

            _logger = Logging.CreateLogger<IntrospectionCommand>();

            OnExecute((Func<int>)ExecuteCommand);
        }

        private int ExecuteCommand()
        {
            return ExcuteCommand().Result;
        }

        private async Task<int> ExcuteCommand()
        {
            int result = -1;

            try
            {
                var discoClient = await DiscoveryClient.GetAsync(_server.Value());
                if (discoClient.IsError)
                {
                    _logger.LogError($"{discoClient.ErrorType} : {discoClient.Error}");
                }
                else
                {

                    var client = new IntrospectionClient(discoClient.IntrospectionEndpoint, _client.Value(),
                    _secret.Value());

                    var request = new IntrospectionRequest
                    {
                        Token = _token.Value()
                    };

                    var introspectionResponse = await client.SendAsync(request);

                    if (introspectionResponse.IsError)
                    {
                        _logger.LogError(introspectionResponse.Error);
                    }
                    else
                    {
                        if (introspectionResponse.IsActive)
                        {
                            introspectionResponse.Claims.ToList().ForEach(
                                c => _logger.LogInformation($"{c.Type}: {c.Value}"));
                        }
                        else
                        {
                            _logger.LogInformation("token is not active");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CommandException(ex.Message);
            }

            return result;
        }
    }
}
