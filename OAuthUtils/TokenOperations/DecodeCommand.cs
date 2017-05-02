using System;
using Microsoft.Extensions.Logging;

namespace OAuthUtils
{
    internal class DecodeCommand : TokenCommand<DecodeCommand>
    {
        public DecodeCommand(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Name = "decode";
            Description = "decodes a base64url encoded JWT";
            HelpOption("-h | -? | --help");
            TokenOperation = DecodeToken;
            OnExecute((Func<int>) ExecuteCommand);
        }
        
        private void DecodeToken(string encodedToken)
        {
            TokenReadResult result = ReadToken(encodedToken);

            if(result.Success)
            {
                Logger.LogInformation(result.Token.ToString());
            }
            else
            {
                Logger.LogError(new EventId(), result.Exception, "the token is invalid");
            }
        }
    }
}