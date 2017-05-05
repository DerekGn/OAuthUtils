
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace OAuthUtils.TokenOperations
{
    internal abstract class TokenCommand<T> : CommandLineApplication
    {
        protected readonly ILogger<T> Logger;

        private readonly CommandOption _token;
        
        public TokenCommand(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Logger = Logging.CreateLogger<T>();

            HelpOption("-h | -? | --help");

            _token = Option("-t | --token", "The token to verify as a base64url encoded string", CommandOptionType.SingleValue);

            OnExecute((Func<int>)ExecuteCommand);
        }

        private int ExecuteCommand()
        {
            int result = 0;
            
            if (_token.HasValue())
            {
                ProcessToken(_token.Value());
            }
            else
            {
                Logger.LogError("token must be specified");
                result = -1;
            }

            return result;
        }

        internal abstract void ProcessToken(string token);

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
