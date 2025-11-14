using System.Net;
using System.Net.Http.Json;
using QuickLists.Core.DTOs;

namespace QuickLists.Api.Tests;

public class ChecklistEndpointsTests : IClassFixture<QuickListsApiFactory>
{
    private readonly HttpClient _client;

    public ChecklistEndpointsTests(QuickListsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllChecklists_WithEmptyDatabase_ReturnsEmptyArray()
    {
        // Act
        var response = await _client.GetAsync("/api/checklists");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var checklists = await response.Content.ReadFromJsonAsync<ChecklistDto[]>();
        Assert.NotNull(checklists);
        Assert.Empty(checklists);
    }
}