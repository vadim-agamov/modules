using System;

namespace Modules.DiContainer
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InjectAttribute : Attribute
    {
    }
}