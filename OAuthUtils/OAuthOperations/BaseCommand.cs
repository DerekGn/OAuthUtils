using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OAuthUtils.OAuthOperations
{
    internal abstract class BaseCommand<T> : CommandLineApplication
    {
        protected readonly ILogger<T> Logger;
        protected readonly CommandOption Server;
        protected readonly CommandOption Client;
        protected readonly CommandOption Secret;

        public BaseCommand(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Logger = Logging.CreateLogger<T>();

            HelpOption("-h | -? | --help");

            Server = Option("-s | --server", "The OAuth server to request a token", CommandOptionType.SingleValue);
            Client = Option("-i | --clientId", "The clientId", CommandOptionType.SingleValue);
            Secret = Option("-c | --secret", "The client secret", CommandOptionType.SingleValue);

            OnExecute((Func<int>)ExecuteCommand);
        }

        private int ExecuteCommand()
        {
            bool validOptions = true;
            int result = -1;

            new List<CommandOption>() { Server, Client, Secret }.ForEach(o =>
            {
                validOptions &= CheckOptionHasValue(o);
            });

            validOptions &= ValidateOptions();

            if (validOptions)
            {
                result = ExecuteCommandAsync().Result;
            }

            return result;
        }

        protected bool CheckOptionHasValue(CommandOption option)
        {
            if (!option.HasValue())
            {
                Logger.LogError($"required option value missing [{option.LongName}]");
            }

            return option.HasValue();
        }

        protected abstract Task<int> ExecuteCommandAsync();

        protected virtual bool ValidateOptions()
        {
            return true;
        }
    }
}
