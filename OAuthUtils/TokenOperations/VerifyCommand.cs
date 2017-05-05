using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;

namespace OAuthUtils.TokenOperations
{
    internal class VerifyCommand : TokenCommand<VerifyCommand>
    {
        private readonly CommandOption _audiences;
        private readonly CommandOption _issuers;
        private readonly CommandOption _lifetime;

        public VerifyCommand(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Name = "verify";
            Description = "verifies a base64url encoded JWT";
            _lifetime = Option("-l | --lifetime", "Ignore token lifetime", CommandOptionType.NoValue);
            _audiences = Option("-a | --aud", "The list of allowed audiences", CommandOptionType.MultipleValue);
            _issuers = Option("-i | --iss", "The list of allowed issuers", CommandOptionType.MultipleValue);
        }

        internal override void ProcessToken(string token)
        {
            TokenReadResult result = ReadToken(token);

            if (result.Success)
            {
                IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>($"{result.Token.Issuer.EnsureTrailingSlash()}.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
                OpenIdConnectConfiguration openIdConfig = configurationManager.GetConfigurationAsync(CancellationToken.None).Result;

                TokenValidationParameters validationParameters = new TokenValidationParameters
                {
                    ValidIssuers = _issuers.Values,
                    ValidAudiences = _audiences.Values,
                    IssuerSigningKeys = openIdConfig.SigningKeys,
                    ValidateLifetime = _lifetime.HasValue()
                };

                SecurityToken validatedToken;
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                try
                {
                    var user = handler.ValidateToken(token, validationParameters, out validatedToken);

                    Logger.LogInformation("token is valid");
                }
                catch (Exception ex)
                {
                    throw new CommandException(ex.Message);
                }
            }
            else
            {
                Logger.LogError(new EventId(), result.Exception, "the token is invalid");
            }
        }
    }
}
