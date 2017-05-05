using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.CommandLineUtils;
using IdentityModel.Client;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OAuthUtils.OAuthOperations
{
    internal class ClientCommand : CommandLineApplication
    {
        private readonly ILogger<ClientCommand> _logger;
        private readonly CommandOption _server;
        private readonly CommandOption _client;
        private readonly CommandOption _secret;
        private readonly CommandOption _scopes;
        private readonly CommandOption _endpoints;

        public ClientCommand(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Name = "client";
            Description = "request a JWT acesss token from an OAuth/OIDC server using client credentials grant";
            _server = Option("-s | --server", "The OAuth server to request a token", CommandOptionType.SingleValue);
            _client = Option("-i | --clientId", "The clientId", CommandOptionType.SingleValue);
            _secret = Option("-c | --secret", "The client secret", CommandOptionType.SingleValue);
            _scopes = Option("-o | --scopes", "The scopes requested", CommandOptionType.MultipleValue);
            _endpoints = Option("-e | --endpoint", "Additional authority endpoints", CommandOptionType.MultipleValue);

            HelpOption("-h | -? | --help");
            OnExecute((Func<int>)RequestToken);
            _logger = Logging.CreateLogger<ClientCommand>();
        }

        private int RequestToken()
        {
            bool validOptions = true;
            int result = -1;
            
            new List<CommandOption>() { _server, _client, _secret, _scopes }.ForEach(o => 
            {
                validOptions &= CheckOptionHasValue(o);
            });

            if(validOptions)
            {
                result = RequestTokenAsync().Result;
            }

            return result;
        }

        private bool CheckOptionHasValue(CommandOption option)
        {
            if (!option.HasValue())
            {
                _logger.LogError($"required option value missing [{option.LongName}]");
            }

            return option.HasValue();
        }

        private async Task<int> RequestTokenAsync()
        {
            int result = -1;

            try
            {
                DiscoveryClient discoClient = new DiscoveryClient(_server.Value());

                _endpoints.Values.ForEach(e => discoClient.Policy.AdditionalEndpointBaseAddresses.Add(e));

                var disco = await discoClient.GetAsync();
                
                if (disco.IsError)
                {
                    _logger.LogError($"{disco.ErrorType} : {disco.Error}");
                }
                else
                {
                    var tokenClient = new TokenClient(disco.TokenEndpoint, _client.Value(), _secret.Value());
                    var tokenResponse = await tokenClient.RequestClientCredentialsAsync(string.Join(" ", _scopes.Values));

                    if (tokenResponse.IsError)
                    {
                        _logger.LogError(tokenResponse.Error);
                    }
                    else
                    {
                        _logger.LogInformation($"AccessToken: {tokenResponse.AccessToken ?? "none"}");
                        _logger.LogInformation($"IdentityToken: {tokenResponse.IdentityToken ?? "none"}");
                        _logger.LogInformation($"RefreshToken: {tokenResponse.RefreshToken?? "none"}");
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
