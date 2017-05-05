using IdentityModel.Client;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OAuthUtils.OAuthOperations
{
    internal abstract class OAuthCommand<T> : CommandLineApplication
    {
        protected readonly ILogger<T> Logger;
        private readonly CommandOption _server;
        private readonly CommandOption _client;
        private readonly CommandOption _secret;
        private readonly CommandOption _scopes;

        public OAuthCommand(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Logger = Logging.CreateLogger<T>();

            HelpOption("-h | -? | --help");

            _server = Option("-s | --server", "The OAuth server to request a token", CommandOptionType.SingleValue);
            _client = Option("-i | --clientId", "The clientId", CommandOptionType.SingleValue);
            _secret = Option("-c | --secret", "The client secret", CommandOptionType.SingleValue);
            _scopes = Option("-o | --scopes", "The scopes requested", CommandOptionType.MultipleValue);
            
            OnExecute((Func<int>)RequestToken);
        }
        
        private int RequestToken()
        {
            bool validOptions = true;
            int result = -1;

            new List<CommandOption>() { _server, _client, _secret, _scopes }.ForEach(o =>
            {
                validOptions &= CheckOptionHasValue(o);
            });

            validOptions &= ValidateOptions();

            if (validOptions)
            {
                result = RequestTokenAsync().Result;
            }

            return result;
        }
        
        protected bool CheckOptionHasValue(CommandOption option)
        {
            if (!option.HasValue())
            {
                Logger.LogError($"required option value missing [{option.LongName}]");
            }

            return option.HasValue();
        }

        private async Task<int> RequestTokenAsync()
        {
            int result = -1;

            try
            {
                var discoClient = new DiscoveryClient(_server.Value());

                ConfigureDiscoveryClient(discoClient);
                
                var disco = await discoClient.GetAsync();

                if (disco.IsError)
                {
                    Logger.LogError($"{disco.ErrorType} : {disco.Error}");
                }
                else
                {
                    var tokenClient = new TokenClient(disco.TokenEndpoint, _client.Value(), _secret.Value());

                    TokenResponse tokenResponse = await RequestToken(tokenClient, string.Join(" ", _scopes.Values));
                    
                    if (tokenResponse.IsError)
                    {
                        Logger.LogError(tokenResponse.Error);
                    }
                    else
                    {
                        Logger.LogInformation($"AccessToken: {tokenResponse.AccessToken ?? "none"}");
                        Logger.LogInformation($"IdentityToken: {tokenResponse.IdentityToken ?? "none"}");
                        Logger.LogInformation($"RefreshToken: {tokenResponse.RefreshToken ?? "none"}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CommandException(ex.Message);
            }

            return result;
        }

        protected virtual bool ValidateOptions()
        {
            return true;
        }

        protected virtual void ConfigureDiscoveryClient(DiscoveryClient discoClient)
        { }

        internal abstract Task<TokenResponse> RequestToken(TokenClient tokenClient, string scopes);
    }
}
