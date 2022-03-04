using Microsoft.AspNetCore.Mvc.Formatters;

namespace StreamCompanion.Classes;

public class TextPlainInputFormatter : InputFormatter
{
    private const string ContentType = "text/plain";

    public TextPlainInputFormatter()
    {
        SupportedMediaTypes.Add(ContentType);
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        HttpRequest request = context.HttpContext.Request;
        using StreamReader reader = new StreamReader(request.Body);
        string content = await reader.ReadToEndAsync();
        return await InputFormatterResult.SuccessAsync(content);
    }

    public override bool CanRead(InputFormatterContext context)
    {
        string contentType = context.HttpContext.Request.ContentType;
        return contentType != null && contentType.StartsWith(ContentType);
    }
}