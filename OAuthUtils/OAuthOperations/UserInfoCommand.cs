using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthUtils.OAuthOperations
{
    internal class UserInfoCommand : TokenCommand<UserInfoCommand>
    {
        public UserInfoCommand(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Name = "userinfo";
            Description = "retrieve userInfo for a JWT access token from the userinfo endpoint";
        }

        protected override async Task<int> ExecuteCommandAsync()
        {
            int result = -1;

            try
            {
                var discoClient = await DiscoveryClient.GetAsync(Server.Value());
                if (discoClient.IsError)
                {
                    Logger.LogError($"{discoClient.ErrorType} : {discoClient.Error}");
                }
                else
                {

                    var client = new UserInfoClient(discoClient.UserInfoEndpoint);

                    var userinfoResponse = await client.GetAsync(Token.Value());

                    Logger.LogInformation("\n\nUser claims:");
                    
                    if (userinfoResponse.IsError)
                    {
                        Logger.LogError(userinfoResponse.Error);
                    }
                    else
                    {
                        userinfoResponse.Claims.ToList().ForEach(
                            c => Logger.LogInformation($"{c.Type}: {c.Value}"));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CommandException(ex.Message);
            }

            return result;
        }
    }
}
