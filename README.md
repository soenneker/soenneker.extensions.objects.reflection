[![](https://img.shields.io/nuget/v/Soenneker.Extensions.Objects.Reflection.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Extensions.Objects.Reflection/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.objects.reflection/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.objects.reflection/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Extensions.Objects.Reflection.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Extensions.Objects.Reflection/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.objects.reflection/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.objects.reflection/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.Objects.Reflection
Reflection-based object projection, query/form encoding, diagnostics, and JSON `HttpContent` creation.

## Installation

```bash
dotnet add package Soenneker.Extensions.Objects.Reflection
```

## Build a query string

```csharp
using Soenneker.Extensions.Objects.Reflection;

string query = new SearchRequest
{
    Page = 2,
    IncludeArchived = false
}.ToQueryString();

// ?page=2&includeArchived=false
```

`ToQueryString()` uses readable public instance properties, `[JsonPropertyName]` or web-default camelCase names, and percent-escapes both names and values. It honors `[JsonIgnore]` conditions, formats scalar values invariantly, and JSON-formats other values before escaping. A null object or an object with no included values returns an empty string.

`ToQueryStringViaReflection()` is the simpler legacy path: it uses CLR property names, optionally lowercases them, calls `ToString()` under the current culture, skips nulls, and escapes names and values. Both methods prepend `?` when at least one pair is emitted.

## Project properties

```csharp
Dictionary<string, object?> values = model.ToDictionary();
```

`ToDictionary()` includes only readable, non-indexed public properties declared directly on the runtime type. `[JsonPropertyName]` controls keys; inherited properties are excluded. Values are not serialized or cloned. A null source returns a new empty dictionary.

## Create request bodies

```csharp
using HttpContent json = request.ToHttpContent();
using FormUrlEncodedContent form = request.ToFormUrlEncodedContent();
```

`ToHttpContent()` serializes with `JsonUtil.WebOptions` into `application/json`; null produces an empty JSON-typed body, not the literal `null`. The caller owns and must dispose every returned content object.

`ToFormUrlEncodedContent()` includes readable public properties, honors `[JsonPropertyName]`, skips nulls, formats `IFormattable` values invariantly, and lets `FormUrlEncodedContent` perform encoding. Nested objects and collections are converted through `ToString()` rather than flattened.

## Diagnostics

- `LogNullProperties()` reports top-level null public properties.
- `LogNullPropertiesRecursivelyAsJson()` traverses application objects and collections, tracks reference cycles, and logs a JSON tree containing only null locations.
- `ToReadableString()` recursively renders public properties for human diagnostics.

Reflection getters and custom `ToString()` implementations can execute user code and throw. Diagnostic output can contain secrets or personal data, and `ToReadableString()` does not protect against reference cycles; use these helpers only with known model types and appropriate logging destinations.
