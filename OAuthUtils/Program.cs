﻿using Microsoft.Extensions.Logging;
using System;

namespace OAuthUtils
{
    internal class Program
    {
        static ILogger<Program> _logger;

        static int Main(string[] args)
        {
            try
            {
                _logger = Logging.CreateLogger<Program>();
                
                return new Commands().Execute(args);
            }
            catch(CommandException cmd)
            {
                _logger?.LogCritical(cmd.Message);

                return 1;
            }
            catch (Exception ex)
            {
                _logger?.LogCritical(new EventId(), ex, "An unexpected exception occured");

                return 1;
            }
        }
    }
}