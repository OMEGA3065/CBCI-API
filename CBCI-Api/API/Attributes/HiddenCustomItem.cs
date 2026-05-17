namespace CustomItemLib.API.Attributes
{
    /// <summary>
    /// Used for hiding a <see cref="ICustomItem{T}"/> from the automatic
    /// registration process (only if in use by the specified assembly)
    /// <seealso cref="CustomItemManager.RegisterAllItems()"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class HiddenCustomItem() : Attribute
    {
    }
}