namespace CustomItemLib.API
{
    /// <summary>
    /// The base interface for all Item Components that can be applied to Custom Item Definitions.
    /// </summary>
    /// <typeparam name="T">An <see cref="ItemInstanceBase"/> managed by this component.</typeparam>
    public interface ICustomItemComponent<in T>
    {
        /// <summary>
        /// Called each time this <see cref="ICustomItemComponent{T}"/> is assigned to a <see cref="ICustomItem{T}"/>>.
        /// </summary>
        /// <param name="item">The Item Definition the component was attached to.</param>
        public void InitComponent(ICustomItem<T> item);
        
        /// <summary>
        /// Called each time this <see cref="ICustomItemComponent{T}"/> is removed from a <see cref="ICustomItem{T}"/>>.
        /// </summary>
        /// <param name="item">The Item Definition the component was removed from.</param>
        public void DestroyComponent(ICustomItem<T> item);
        
        /// <summary>
        /// Called when a new instance of <see cref="ItemInstanceBase"/> for one of the Item Definitions that this component has been attached to is starting it's creation process.
        /// </summary>
        /// <param name="item">The <see cref="ItemInstanceBase"/> that's being created.</param>
        public bool OnCreatingInstance(T itemInstance);
        
        /// <summary>
        /// Called when a new instance of <see cref="ItemInstanceBase"/> for one of the Item Definitions that this component has been attached to finishes it's creation process.
        /// </summary>
        /// <param name="item">The <see cref="ItemInstanceBase"/> that has been created.</param>
        public void OnCreatedInstance(T itemInstance);
        
        /// <summary>
        /// Called when a new instance of <see cref="ItemInstanceBase"/> for one of the Item Definitions that this component has been attached to is starting it's destruction process.
        /// </summary>
        /// <param name="item">The <see cref="ItemInstanceBase"/> that's being destroyed.</param>
        public bool OnDestroyingInstance(T itemInstance);
        
        /// <summary>
        /// Called when a new instance of <see cref="ItemInstanceBase"/> for one of the Item Definitions that this component has been attached to finishes it's destruction process.
        /// </summary>
        /// <param name="item">The <see cref="ItemInstanceBase"/> that has been destroyed.</param>
        public void OnDestroyedInstance(T itemInstance);
        
        /// <summary>
        /// Subscribes a specifed <see cref="ItemInstanceBase"/> to it's events.
        /// </summary>
        /// <param name="itemInstance">The <see cref="ItemInstanceBase"/> which will subscribe to it's events.</param>
        public void SubscribeEvents(T itemInstance);
        
        /// <summary>
        /// Unsubscribes a specifed <see cref="ItemInstanceBase"/> from it's events.
        /// </summary>
        /// <param name="itemInstance">The <see cref="ItemInstanceBase"/> which will unsubscribe from it's events.</param>
        public void UnsubscribeEvents(T itemInstance);
    }
}