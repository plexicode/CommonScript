using System;

namespace CommonScriptCli
{
    internal class UserFacingException : Exception
    {
        public UserFacingException(string msg) : base(msg) { }
    }

    internal class CliArgumentException : UserFacingException
    {
        public CliArgumentException(string msg) : base(msg) { }
    }
}
