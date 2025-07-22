using System;

namespace Tweaks.Features
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal class ModFeatureAttribute(bool required = false) : Attribute
    {
        public bool Required { get; } = required;
    }
}
