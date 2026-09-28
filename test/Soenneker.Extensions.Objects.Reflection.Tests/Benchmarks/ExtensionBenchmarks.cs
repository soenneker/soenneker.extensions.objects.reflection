using BenchmarkDotNet.Attributes;
using Soenneker.Extensions.Objects.Reflection.Tests.Dtos;
using Soenneker.Utils.AutoBogus;
using Soenneker.Extensions.String;
using Soenneker.Utils.Json;
using Soenneker.Utils.PooledStringBuilders;
using System.Collections.Generic;
using System.Text.Json;

namespace Soenneker.Extensions.Objects.Reflection.Tests.Benchmarks;

public class ExtensionBenchmarks
{
    private AutoFaker _autoFaker = null!;

    private UserDto _userDto = null!;

    [GlobalSetup]
    public void SetupData()
    {
        _autoFaker = new AutoFaker();
        _userDto = _autoFaker.Generate<UserDto>();
    }

    [Benchmark]
    public string ToQueryStringViaCachedReflection()
    {
        return _userDto.ToQueryStringViaCachedReflection();
    }

    [Benchmark]
    public string ToQueryStringViaReflection()
    {
        return _userDto.ToQueryStringViaReflection();
    }

    [Benchmark(Baseline = true)]
    public string LegacySerialization()
    {
        string serialized = JsonUtil.Serialize(_userDto)!;
        var dictionary = JsonUtil.Deserialize<Dictionary<string, JsonElement>>(serialized)!;
        using var builder = new PooledStringBuilder(dictionary.Count * 10);
        builder.Append('?');

        foreach ((string key, JsonElement element) in dictionary)
        {
            if (builder.Length > 1)
                builder.Append('&');
            builder.Append(key);
            builder.Append('=');
            builder.Append((element.ValueKind is JsonValueKind.True or JsonValueKind.False ? element.GetBoolean().ToString().ToLowerInvariant() : element.ToString()).ToEscaped());
        }

        return builder.ToString();
    }
}
