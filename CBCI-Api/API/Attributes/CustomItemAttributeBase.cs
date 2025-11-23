namespace CustomItemLib.API.Attributes
{
    /// <summary>
    /// Used for adding custom / built-in components to item definitions.
    /// Initializes a new instance of the <see cref="CustomItemAttribute"/> class.
    /// </summary>
    /// <param name="type">The <see cref="ItemType"/> to serialize.</param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class CustomItemAttributeBase(Type ComponentType) : Attribute
    {
        /// <summary>
        /// Gets the attribute's <see cref="CustomItemLib.API.ICustomItemComponent{CustomItemLib.API.ItemInstanceBase}"/>.
        /// </summary>
        public object Component { get; } = Activator.CreateInstance(ComponentType);
    }
}