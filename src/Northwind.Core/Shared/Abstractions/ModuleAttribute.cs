namespace Northwind.Shared.Abstractions;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ModuleAttribute : Attribute
{
    public ModuleAttribute(int order = 0)
    {
        Order = order;
    }

    public int Order { get; }
}
