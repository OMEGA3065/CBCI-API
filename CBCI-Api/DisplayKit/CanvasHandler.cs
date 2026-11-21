using Mirror;

namespace CustomItemLib.DisplayKit;

internal class CanvasHandler<T> where T : ICanvasReference
{
    public bool Loaded { get; private set; }
    public T Reference { get; }
    public event Action OnConstructed;

    public CanvasHandler(T reference, bool loaded = false)
    {
        Reference = reference;
        Loaded = loaded;
        if (Loaded) return;
        Reference.DisplayCanvas.OnPlayerCanvasConstructed += OnCanvasConstructed;
    }

    private void OnCanvasConstructed(NetworkConnectionToClient connection)
    {
        Loaded = true;
        OnConstructed?.Invoke();
    }

    public void Effect(Action action)
    {
        if (Loaded)
        {
            action();
            return;
        }

        OnConstructed += action;
    }
}