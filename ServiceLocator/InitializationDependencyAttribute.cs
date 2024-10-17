using System;

namespace Modules.ServiceLocator
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InitializationDependencyAttribute : Attribute
    {
    }
}