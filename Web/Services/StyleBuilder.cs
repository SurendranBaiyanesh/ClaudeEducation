using System.Security.Cryptography;
using System.Text;

namespace BacklogTicketManager.Services;

/// <summary>Builds a single CSS bundle from every *.css file under wwwroot/css.
/// The bundle is built once and cached; the source files are read on first request.
/// Files are emitted with "site.css" first (it defines the :root design tokens the
/// other sheets rely on) and the remainder in alphabetical order for a deterministic cascade.</summary>
public class StyleBuilder : IStyleBuilder
{
    private const string CssFolder = "css";
    private const string PrimaryFirst = "site.css";

    private readonly IWebHostEnvironment _environment;
    private readonly object _gate = new();

    private string? _bundle;
    private string? _hash;

    public StyleBuilder(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public string ContentHash
    {
        get
        {
            EnsureBuilt();
            return _hash!;
        }
    }

    public string BuildBundle()
    {
        EnsureBuilt();
        return _bundle!;
    }

    private void EnsureBuilt()
    {
        if (_bundle is not null)
        {
            return;
        }

        lock (_gate)
        {
            if (_bundle is not null)
            {
                return;
            }

            var (bundle, hash) = Build();
            _hash = hash;
            _bundle = bundle;
        }
    }

    private (string Bundle, string Hash) Build()
    {
        // WebRootPath can be null when the wwwroot folder is absent; fall back to the content root.
        var webRoot = _environment.WebRootPath
            ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var cssRoot = Path.Combine(webRoot, CssFolder);
        var builder = new StringBuilder();

        if (Directory.Exists(cssRoot))
        {
            foreach (var file in OrderFiles(Directory.GetFiles(cssRoot, "*.css", SearchOption.TopDirectoryOnly)))
            {
                builder.Append("/* ==== ").Append(Path.GetFileName(file)).Append(" ==== */\n");
                builder.Append(File.ReadAllText(file));
                builder.Append('\n');
            }
        }

        var bundle = builder.ToString();
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(bundle))).ToLowerInvariant()[..12];
        return (bundle, hash);
    }

    /// <summary>site.css first (design tokens), then the rest alphabetically.</summary>
    private static IEnumerable<string> OrderFiles(IEnumerable<string> files) =>
        files
            .OrderBy(f => string.Equals(Path.GetFileName(f), PrimaryFirst, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .ThenBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase);
}
