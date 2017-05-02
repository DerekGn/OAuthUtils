
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace OAuthUtils
{
    internal abstract class TokenCommand<T> : CommandLineApplication
    {
        protected Action<string> TokenOperation;
        protected readonly ILogger<T> Logger;

        private readonly CommandOption _token;
        
        public TokenCommand(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Logger = Logging.CreateLogger<T>();
            _token = Option("-t | --token", "The token to verify as a base64url encoded string", CommandOptionType.SingleValue);
        }

        protected int ExecuteCommand()
        {
            int result = 0;
            
            if (_token.HasValue())
            {
                if (TokenOperation != null)
                {
                    TokenOperation(_token.Value());
                }
                else
                {
                    Logger.LogError("no token operation specified");
                }
            }
            else
            {
                Logger.LogError("token must be specified");
                result = -1;
            }

            return result;
        }

        protected static TokenReadResult ReadToken(string encodedToken)
        {
            TokenReadResult result = new TokenReadResult();

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                result.Token = tokenHandler.ReadJwtToken(encodedToken);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }
    }
}
