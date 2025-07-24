using System;

namespace Tweaks.Features
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ModFeatureAttribute(bool required = false) : Attribute
    {
        public bool Required { get; } = required;
    }
}
