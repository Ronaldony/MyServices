using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.Runtime.CompilerServices;

namespace GMApp.Client.Layout
{
    public partial class MainLayout
    {
        // Uri & Menu changed check.
        private string _prevUri;
        private bool _menuChanged = true;

        [Inject]
        private NavigationManager _naigationManager { get; set; } = default;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            
            _prevUri = _naigationManager.Uri;
            _naigationManager.LocationChanged += LocationChanged;
        }

        /// <summary>
        /// Check location was changed.
        /// </summary>
        private void LocationChanged(object sender, LocationChangedEventArgs e)
        {
            if ((false == e.IsNavigationIntercepted)                                        // link intercepted.
                && (new Uri(_prevUri).AbsolutePath != new Uri(e.Location).AbsolutePath))    // compare prev and now uri.
            {
                _prevUri = e.Location;
                if (true ==_menuChanged)
                {
                    _menuChanged = false;
                    StateHasChanged();
                }
            }
        }
    }
}
