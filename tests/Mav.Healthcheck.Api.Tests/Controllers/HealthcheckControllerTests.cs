using FluentAssertions;

namespace Mav.Healthcheck.Api.Tests.Controllers;

public class HealthcheckControllerTests : IClassFixture<AppTestFixture>
{
    private readonly AppTestFixture _appTestFixture;

#pragma warning disable xUnit1041 // Fixture arguments to test classes must have fixture sources
    public HealthcheckControllerTests(AppTestFixture appTestFixture)
#pragma warning restore xUnit1041 // Fixture arguments to test classes must have fixture sources
    {
        _appTestFixture = appTestFixture;
    }

    [Fact]
    public async Task GivenValidHealthCheckRequest_ShouldSucceed()
    {
        var response = await _appTestFixture.HttpClient.GetAsync("health");
        var responseBody = await response.Content.ReadAsStringAsync();
        
        response.EnsureSuccessStatusCode();
        responseBody.Should().NotBeNullOrEmpty().And.Contain("\"status\": \"Healthy\"");
    }
}
