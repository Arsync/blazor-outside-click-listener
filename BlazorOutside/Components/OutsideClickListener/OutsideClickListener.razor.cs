using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorOutside.Components;

public sealed partial class OutsideClickListener : IAsyncDisposable
{
    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<OutsideClickListener>? _listenerRef;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public EventCallback ClickOutside { get; set; }

    /// <summary>
    /// A boolean value indicating that events of this type will be dispatched
    /// before being dispatched to any EventTarget beneath it in the DOM tree.
    /// </summary>
    [Parameter]
    public bool Capture { get; set; }

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = default!;

    private ElementReference HtmlElementRef { get; set; }

    [JSInvokable]
    public async Task RaiseOnClickOutside()
    {
        await ClickOutside.InvokeAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        await ConnectToJavaScript();
    }

    private async Task ConnectToJavaScript()
    {
        _listenerRef = DotNetObjectReference.Create(this);

        _jsModule = await JsRuntime.InvokeAsync<IJSObjectReference>(
            "import",
            "/_content/BlazorOutside/js/appOutsideClickHandler.js");

        await AttachEvent();
    }

    private async Task AttachEvent()
    {
        await _jsModule!.InvokeVoidAsync(
            "attachWindowClickEvent",
            HtmlElementRef,
            _listenerRef,
            Capture
            );
    }

    private async Task DetachEvent()
    {
        await _jsModule!.InvokeVoidAsync(
            "detachWindowClickEvent",
            HtmlElementRef);
    }

    public async ValueTask DisposeAsync()
    {
        await DetachEvent();

        if (_jsModule != null)
            await _jsModule.DisposeAsync();

        _listenerRef?.Dispose();
    }
}
