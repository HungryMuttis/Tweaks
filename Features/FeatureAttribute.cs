using System;

namespace Tweaks.Features
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class FeatureAttribute(bool required = false) : Attribute
    {
        public bool Required { get; } = required;
    }
}
