using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;

namespace OAuthUtils
{
    internal class VerifyCommand : TokenCommand<VerifyCommand>
    {
        private readonly CommandOption _audiences;
        private readonly CommandOption _issuers;

        public VerifyCommand(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Name = "verify";
            Description = "verifies a base64url encoded JWT";
            _audiences = Option("-a | --aud", "The list of allowed ", CommandOptionType.MultipleValue);
            _issuers = Option("-i | --issuers", "The list of allowed ", CommandOptionType.MultipleValue);
            HelpOption("-h | -? | --help");
            TokenOperation = VerifyToken;
            OnExecute((Func<int>)ExecuteCommand);
        }

        private void VerifyToken(string encodedToken)
        {
            TokenReadResult result = ReadToken(encodedToken);

            if (result.Success)
            {                
                IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>($"{result.Token.Issuer.EnsureTrailingSlash()}.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
                OpenIdConnectConfiguration openIdConfig = configurationManager.GetConfigurationAsync(CancellationToken.None).Result;

                TokenValidationParameters validationParameters = new TokenValidationParameters
                {
                    //ValidIssuers = ,
                    //ValidAudiences = ,
                    IssuerSigningKeys = openIdConfig.SigningKeys
                };

                SecurityToken validatedToken;
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                var user = handler.ValidateToken(encodedToken, validationParameters, out validatedToken);
            }
            else
            {
                Logger.LogError(new EventId(), result.Exception, "the token is invalid");
            }
        }
    }
}
