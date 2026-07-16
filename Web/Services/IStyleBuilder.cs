namespace BacklogTicketManager.Services;

/// <summary>Discovers every stylesheet under wwwroot/css and concatenates them into a single
/// CSS bundle that is served to the site as one request (see /css/bundle.css).</summary>
public interface IStyleBuilder
{
    /// <summary>The concatenated contents of all wwwroot/css/*.css files.</summary>
    string BuildBundle();

    /// <summary>Short content hash of the bundle, used for cache-busting and ETag validation.</summary>
    string ContentHash { get; }
}
