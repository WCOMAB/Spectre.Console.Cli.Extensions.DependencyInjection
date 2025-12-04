public static partial class Program
{
    private static string? fallbackVersion = Argument<string?>("force-version", EnvironmentVariable("FALLBACK_VERSION") ?? "0.1.0");

    static string BuildVersion(string fallbackVersion) {
        var PackageVersion = string.Empty;
        try {
            Information("Attempting MinVer...");
            var settings = new MinVerSettings()
            {
                //AutoIncrement = MinVerAutoIncrement.Minor,
                DefaultPreReleasePhase = "preview",
                TagPrefix = "v"
            };
            var version = MinVer(settings);
            PackageVersion = version.PackageVersion;
        } catch (Exception ex) {
            Warning($"Error when getting version {ex.Message}");
            Information($"Falling back to version: {fallbackVersion}");
            PackageVersion = fallbackVersion;
        } finally {
            Information($"Building for version '{PackageVersion}'");
        }
        return PackageVersion;
    }
}