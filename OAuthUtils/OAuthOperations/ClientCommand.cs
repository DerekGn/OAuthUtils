using System;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.CommandLineUtils;


namespace OAuthUtils.OAuthOperations
{
    internal class ClientCommand : OAuthCommand<ClientCommand>
    {
        private readonly CommandOption _endpoints;

        public ClientCommand(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Name = "client";
            Description = "request a JWT acesss token from an OAuth/OIDC server using client credentials grant";
            _endpoints = Option("-e | --endpoints", "Additional authority endpoints", CommandOptionType.MultipleValue);
        }

        protected override void ConfigureDiscoveryClient(DiscoveryClient discoClient)
        {
            _endpoints.Values.ForEach(e => discoClient.Policy.AdditionalEndpointBaseAddresses.Add(e));
        }

        internal override async Task<TokenResponse> RequestToken(TokenClient tokenClient, string scopes)
        {
            return await tokenClient.RequestClientCredentialsAsync(scopes);
        }
    }
}
