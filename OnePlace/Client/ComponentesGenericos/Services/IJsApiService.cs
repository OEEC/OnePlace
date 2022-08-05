using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.Services
{
    public interface IJsApiService
    {
        ValueTask CopyToClipboardAsync(string text);
        ValueTask OpenInNewTabAsync(string url);
        ValueTask Open(string link, string target);
    }

    public class JsApiService : IJsApiService
    {
        private readonly IJSRuntime _jsRuntime;

        public JsApiService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public ValueTask CopyToClipboardAsync(string text) =>
            _jsRuntime.InvokeVoidAsync("mudWindow.copyToClipboard", text);

        public ValueTask Open(string link, string target)
        {
            if (target == "_blank")
                return OpenInNewTabAsync(link);

            return _jsRuntime.InvokeVoidAsync("open", link, target);
        }

        public ValueTask OpenInNewTabAsync(string url) =>
            _jsRuntime.InvokeVoidAsync("mudWindow.open", url, "_blank");
    }
}
