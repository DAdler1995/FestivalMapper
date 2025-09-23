using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalMapper.App.Components.Shared
{
    public class ConfirmModalBase : ComponentBase, IAsyncDisposable
    {
        [Inject] protected IJSRuntime JS { get; set; } = default!;

        // Content & behavior
        [Parameter] public string Title { get; set; } = "Confirm";
        [Parameter] public string? Message { get; set; }
        [Parameter] public RenderFragment? Body { get; set; }

        [Parameter] public string ConfirmText { get; set; } = "Confirm";
        [Parameter] public string CancelText { get; set; } = "Cancel";
        [Parameter] public string ConfirmButtonClass { get; set; } = "btn btn-danger";
        [Parameter] public string CancelButtonClass { get; set; } = "btn btn-outline-secondary";

        [Parameter] public EventCallback OnConfirm { get; set; }
        [Parameter] public EventCallback OnCancel { get; set; }

        // Visual options
        [Parameter] public bool Centered { get; set; } = true;
        [Parameter] public bool Dark { get; set; } = true;              // match your dark theme
        [Parameter] public string SizeClass { get; set; } = "";          // "modal-sm", "modal-lg", "modal-xl"

        // Behavior options
        [Parameter] public bool StaticBackdrop { get; set; } = false;    // block clicks outside
        [Parameter] public bool DisableEsc { get; set; } = false;        // disable keyboard dismiss

        protected ElementReference _modalEl;
        protected bool Busy { get; set; }
        protected string Id { get; } = $"modal_{Guid.NewGuid():N}";

        private IJSObjectReference? _module;
        private bool _initialized;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _module = await JS.InvokeAsync<IJSObjectReference>("import", "/js/modalHelper.js");
                await _module.InvokeVoidAsync("init", _modalEl, new
                {
                    backdrop = StaticBackdrop ? "static" : (object)true,
                    keyboard = !DisableEsc
                });
                _initialized = true;
            }
        }

        public async Task ShowAsync()
        {
            if (!_initialized || _module is null) return;
            await _module.InvokeVoidAsync("show", _modalEl);
        }

        public async Task HideAsync()
        {
            if (!_initialized || _module is null) return;
            await _module.InvokeVoidAsync("hide", _modalEl);
        }

        protected async Task HandleConfirm()
        {
            if (Busy) return;
            Busy = true;
            try
            {
                if (OnConfirm.HasDelegate)
                    await OnConfirm.InvokeAsync();
            }
            finally
            {
                Busy = false;
                await HideAsync();
            }
        }

        protected async Task HandleCancel()
        {
            if (OnCancel.HasDelegate)
                await OnCancel.InvokeAsync();

            await HideAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (_module is not null)
            {
                try { await _module.InvokeVoidAsync("dispose", _modalEl); }
                catch { /* no-op */ }
                await _module.DisposeAsync();
            }
        }
    }
}