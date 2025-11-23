namespace CustomItemLib.API
{
    /// <summary>
    /// An item namespace for a <see cref="ICustomItem{T}"/>.
    /// </summary>
    public class ItemNamespace
    {
        private ItemNamespace() {}
        
        /// <summary>
        /// The plugin part of the namespace (PLUGIN_PART:ID_PART)
        /// </summary>
        public string PluginNamespace;
        
        /// <summary>
        /// The ID part of the namespace (PLUGIN_PART:ID_PART)
        /// </summary>
        public string ItemIdentifier;

        /// <summary>
        /// Tries to obtain an <see cref="ItemNamespace"/> from a string in the namespace format.
        /// <example>
        /// For example:
        /// <code>
        /// ItemNamespace.TryGet("my_plugin:my_custom_item", out var @namespace)
        /// </code>
        /// will return <see cref="true"> and the parsed <see cref="ItemNamespace"/> will be in <c>@namespace</c>.
        /// </example>
        /// </summary>
        /// <param name="namespaceString">The <see cref="string"/> to try and parse.</param>
        /// <param name="@namespace">The parsed <see cref="ItemNamespace"/>.</param>
        /// <returns>Whether or not the namespace has been parsed successfully.</returns>
        /// <example>
        public static bool TryGet(string namespaceString, out ItemNamespace @namespace)
        {
            var split = namespaceString.Split(':');
            if (split.Length != 2)
            {
                @namespace = null;
                return false;
            }

            @namespace = new ItemNamespace
            {
                PluginNamespace = split[0],
                ItemIdentifier = split[1]
            };
            return true;
        }

        /// <summary>
        /// Parses a <c>namespaceString</c> to an <see cref="ItemNamespace"/>.
        /// </summary>
        /// <param name="namespaceString">The <see cref="string"/> to parse.</param>
        /// <returns>The parsed <see cref="ItemNamespace"/>.</returns>
        /// <exception cref="ArgumentException">For invalid <c>namespaceString</c>s</exception>
        public static ItemNamespace Get(string namespaceString)
        {
            if (TryGet(namespaceString, out var @namespace))
            {
                return @namespace;
            }

            throw new ArgumentException("Invalid namespace format. Expected format: 'plugin_namespace:item_identifier'");
        }

        /// <summary>
        /// Parses a <c>pluginNamespace</c> with an <c>itemIdentifier</c> to an <see cref="ItemNamespace"/>.
        /// </summary>
        /// <param name="pluginNamespace">The Plugin Namespace part of an <see cref="ItemNamespace"/> to parse.</param>
        /// <param name="itemIdentifier">The Item ID part of an <see cref="ItemNamespace"/> to parse.</param>
        /// <returns>The parsed <see cref="ItemNamespace"/>.</returns>
        /// <exception cref="ArgumentException">For invalid <c>namespaceString</c>s</exception>
        public static ItemNamespace Get(string pluginNamespace, string itemIdentifier)
        {
            return new ItemNamespace
            {
                PluginNamespace = pluginNamespace,
                ItemIdentifier = itemIdentifier
            };
        }

        /// <summary>
        /// Parses this <see cref="ItemNamespace"/> to a <see cref="string"/>.
        /// </summary>
        /// <returns>The parsed <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"{PluginNamespace}:{ItemIdentifier}";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is not ItemNamespace) return false;
            return obj.ToString() == ToString();
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        
        /// <inheritdoc/>
        public static bool operator ==(ItemNamespace a, ItemNamespace b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null ^ b is null) return false;
            return a.Equals(b);
        }

        /// <inheritdoc/>
        public static bool operator !=(ItemNamespace a, ItemNamespace b)
        {
            if (ReferenceEquals(a, b)) return false;
            if (a is null ^ b is null) return true;
            return !a.Equals(b);
        }
    }
}