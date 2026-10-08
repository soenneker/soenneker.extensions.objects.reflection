using AwesomeAssertions;
using System.Net.Http;
using System.Text.Json;
using System.Threading;


namespace Soenneker.Extensions.Objects.Reflection.Tests;

public class HttpContentReflectionTests
{
    [Test]
    public async System.Threading.Tasks.ValueTask ToHttpContentViaReflection_serializes_anonymous_objects(CancellationToken cancellationToken)
    {
        var payload = new { Query = "mutation", Variables = new { RepositoryId = "node\"id", Enabled = false } };
        using HttpContent content = payload.ToHttpContentViaReflection();
        using JsonDocument document = JsonDocument.Parse(await content.ReadAsStringAsync(cancellationToken: cancellationToken));

        content.Headers.ContentType!.MediaType.Should().Be("application/json");
        document.RootElement.GetProperty("query").GetString().Should().Be("mutation");
        JsonElement variables = document.RootElement.GetProperty("variables");
        variables.GetProperty("repositoryId").GetString().Should().Be("node\"id");
        variables.GetProperty("enabled").GetBoolean().Should().BeFalse();
    }

    [Test]
    public async System.Threading.Tasks.ValueTask ToHttpContentViaReflection_returns_empty_json_content_for_null(CancellationToken cancellationToken)
    {
        using HttpContent content = ((object?)null).ToHttpContentViaReflection();

        (await content.ReadAsByteArrayAsync(cancellationToken: cancellationToken)).Should().BeEmpty();
        content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }
}
