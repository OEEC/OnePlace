using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.BlazoredTypeahead
{
    public static class Interop
    {
        public static async ValueTask<object> Focus(IJSRuntime jsRuntime, ElementReference element)
        {
            return await jsRuntime.InvokeAsync<object>("blazoredTypeahead.setFocus", element);
        }

        public static async ValueTask<object> AddKeyDownEventListener(IJSRuntime jsRuntime, ElementReference element)
        {
            return await jsRuntime.InvokeAsync<object>("blazoredTypeahead.addKeyDownEventListener", element);
        }

        public static async ValueTask<object> OnOutsideClick(this IJSRuntime jsRuntime, ElementReference element, object caller, string methodName, bool clearOnFire = false)
        {
            return await jsRuntime.InvokeAsync<object>("blazoredTypeahead.onOutsideClick", element, DotNetObjectReference.Create(caller), methodName, clearOnFire);
        }
    }
}
