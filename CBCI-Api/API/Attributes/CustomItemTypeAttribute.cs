namespace CustomItemLib.API.Attributes
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomItemAttribute"/> class.
    /// </summary>
    /// <param name="type">The <see cref="global::ItemType"/> to serialize.</param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class CustomItemAttribute(ItemType type) : Attribute
    {
        /// <summary>
        /// Gets the attribute's <see cref="global::ItemType"/>.
        /// </summary>
        public ItemType ItemType { get; } = type;
    }
}