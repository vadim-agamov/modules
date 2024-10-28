using System;

namespace Modules.ServiceLocator.Initializator
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InitializationDependencyAttribute : Attribute
    {
    }
}