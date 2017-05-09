using IdentityModel.Client;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace OAuthUtils.OAuthOperations
{
    internal abstract class OAuthCommand<T> : BaseCommand<T>
    {
        private readonly CommandOption Scopes;

        public OAuthCommand(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Scopes = Option("-o | --scopes", "The scopes requested", CommandOptionType.MultipleValue);
        }

        protected override bool ValidateOptions()
        {
            return CheckOptionHasValue(Scopes) && base.ValidateOptions();
        }

        protected override async Task<int> ExecuteCommandAsync()
        {
            int result = -1;

            try
            {
                var discoClient = new DiscoveryClient(Server.Value());

                ConfigureDiscoveryClient(discoClient);

                var disco = await discoClient.GetAsync();

                if (disco.IsError)
                {
                    Logger.LogError($"{disco.ErrorType} : {disco.Error}");
                }
                else
                {
                    var tokenClient = new TokenClient(disco.TokenEndpoint, Client.Value(), Secret.Value());

                    TokenResponse tokenResponse = await RequestToken(tokenClient, string.Join(" ", Scopes.Values));

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

        protected virtual void ConfigureDiscoveryClient(DiscoveryClient discoClient)
        { }

        internal abstract Task<TokenResponse> RequestToken(TokenClient tokenClient, string scopes);
    }
}
