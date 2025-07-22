using System;

namespace Tweaks.Features.BetterConsole
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ConsoleCommandAttribute(string description, params string[] arguments) : Attribute
    {
        public string Description { get; } = description;
        public string[] Arguments { get; } = arguments;
    }
}
