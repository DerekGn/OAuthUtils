using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthUtils.OAuthOperations
{
    internal class IntrospectionCommand : TokenCommand<IntrospectionCommand>
    {
        public IntrospectionCommand(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Name = "intro";
            Description = "validate a JWT Access Token via the introspection endpoint";
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

                    var client = new IntrospectionClient(discoClient.IntrospectionEndpoint, Client.Value(),
                    Secret.Value());

                    var request = new IntrospectionRequest
                    {
                        Token = Token.Value()
                    };

                    var introspectionResponse = await client.SendAsync(request);

                    if (introspectionResponse.IsError)
                    {
                        Logger.LogError(introspectionResponse.Error);
                    }
                    else
                    {
                        if (introspectionResponse.IsActive)
                        {
                            introspectionResponse.Claims.ToList().ForEach(
                                c => Logger.LogInformation($"{c.Type}: {c.Value}"));
                        }
                        else
                        {
                            Logger.LogInformation("token is not active");
                        }
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
