using Microsoft.Extensions.Logging;

namespace OAuthUtils.TokenOperations
{
    internal class DecodeCommand : TokenCommand<DecodeCommand>
    {
        public DecodeCommand(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Name = "decode";
            Description = "decodes a base64url encoded JWT";
        }

        internal override void ProcessToken(string token)
        {
            TokenReadResult result = ReadToken(token);

            if (result.Success)
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