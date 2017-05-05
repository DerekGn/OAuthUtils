using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.CommandLineUtils;

namespace OAuthUtils.OAuthOperations
{
    internal class ResourceCommand : OAuthCommand<ResourceCommand>
    {
        private readonly CommandOption _user;
        private readonly CommandOption _pwd;

        public ResourceCommand(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Name = "resource";
            Description = "request a JWT token from an OAuth/OIDC server using resource owner grant";

            _user = Option("-u | --user", "The resource owner user name", CommandOptionType.SingleValue);
            _pwd = Option("-p | --pwd", "The resource owner password", CommandOptionType.SingleValue);
        }

        protected override bool ValidateOptions()
        {
            return CheckOptionHasValue(_user) && CheckOptionHasValue(_pwd);
        }

        internal override async Task<TokenResponse> RequestToken(TokenClient tokenClient, string scopes)
        {
            return await tokenClient.RequestResourceOwnerPasswordAsync(_user.Value(),_pwd.Value(), scopes);
        }
    }
}
