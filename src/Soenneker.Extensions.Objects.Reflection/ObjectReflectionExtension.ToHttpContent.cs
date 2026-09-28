using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using Soenneker.Utils.Json;

namespace Soenneker.Extensions.Objects.Reflection;

public static partial class ObjectReflectionExtension
{
    private static readonly byte[] _emptyByteArray = [];

    /// <summary>
    /// Converts an object to JSON HTTP content using reflection-based serialization and the default web options.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>JSON HTTP content, or empty content when the object is null.</returns>
    /// <remarks>For trimming and Native AOT, use the JsonTypeInfo overload in Soenneker.Extensions.Object.</remarks>
    [Pure]
    [RequiresUnreferencedCode("Reflection-based serialization may require types that cannot be statically analyzed. Use the JsonTypeInfo overload in Soenneker.Extensions.Object instead.")]
    [RequiresDynamicCode("Reflection-based serialization may require runtime code generation. Use the JsonTypeInfo overload in Soenneker.Extensions.Object instead.")]
    public static HttpContent ToHttpContent(this object? obj)
    {
        byte[] utf8Bytes = obj is null ? _emptyByteArray : JsonUtil.SerializeToUtf8Bytes(obj);
        return new ByteArrayContent(utf8Bytes)
        {
            Headers = { ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json) }
        };
    }
}
