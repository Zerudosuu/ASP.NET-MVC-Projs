using Microsoft.AspNetCore.Mvc.Rendering;

public static class HtmlHelpers
{
    public static string IsActive(this IHtmlHelper html, string path, string activeClass = "active")
    {
        var currentPath = html.ViewContext.HttpContext.Request.Path;
        return currentPath.StartsWithSegments(path, StringComparison.OrdinalIgnoreCase)
            ? activeClass
            : string.Empty;
    }
}
