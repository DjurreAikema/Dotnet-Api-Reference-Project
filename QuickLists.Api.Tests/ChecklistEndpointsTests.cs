using System.Net;
using System.Net.Http.Json;
using QuickLists.Core.DTOs;

namespace QuickLists.Api.Tests;

public class ChecklistEndpointsTests(QuickListsApiFactory factory) : IClassFixture<QuickListsApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

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

    [Fact]
    public async Task CreateChecklist_WithValidData_ReturnsCreatedChecklist()
    {
        // Arrange
        var newChecklist = new CreateChecklistDto(Title: "My Test Checklist");

        // Act
        var response = await _client.PostAsJsonAsync("/api/checklists", newChecklist);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdChecklist = await response.Content.ReadFromJsonAsync<ChecklistDto>();
        Assert.NotNull(createdChecklist);
        Assert.Equal("My Test Checklist", createdChecklist.Title);
        Assert.NotNull(createdChecklist.Id);
        Assert.NotEmpty(createdChecklist.Id);
    }

    [Fact]
    public async Task CreateChecklist_WithEmptyTitle_ReturnsBadRequest()
    {
        // Arrange
        var invalidChecklist = new CreateChecklistDto(Title: "");

        // Act
        var response = await _client.PostAsJsonAsync("/api/checklists", invalidChecklist);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}