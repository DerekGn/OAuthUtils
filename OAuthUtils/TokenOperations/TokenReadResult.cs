using System;
using System.IdentityModel.Tokens.Jwt;

namespace OAuthUtils.TokenOperations
{
    internal class TokenReadResult
    {
        public bool Success { get; internal set; }
        public Exception Exception { get; internal set; }
        public JwtSecurityToken Token { get; internal set; }
    }
}