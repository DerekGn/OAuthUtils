using Microsoft.Extensions.CommandLineUtils;

namespace OAuthUtils.OAuthOperations
{
    internal abstract class TokenCommand<T> : BaseCommand<T>
    {
        protected readonly CommandOption Token;

        public TokenCommand(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Token = Option("-t | --token", "The JWT access token", CommandOptionType.SingleValue);
        }

        protected override bool ValidateOptions()
        {
            return CheckOptionHasValue(Token) && base.ValidateOptions();
        }
    }
}
