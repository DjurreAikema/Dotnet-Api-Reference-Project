using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
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

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Equal(400, problemDetails.Status);
        Assert.True(problemDetails.Errors.ContainsKey("Title"));
        Assert.Contains("Title is required", problemDetails.Errors["Title"]);
    }

    [Fact]
    public async Task CreateChecklist_WithTooLongTitle_ReturnsProblemDetailsWithValidationError()
    {
        // Arrange
        var tooLongTitle = new string('a', 201);
        var invalidChecklist = new CreateChecklistDto(Title: tooLongTitle);

        // Act
        var response = await _client.PostAsJsonAsync("/api/checklists", invalidChecklist);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.True(problemDetails.Errors.ContainsKey("Title"));
        Assert.Contains("Title must be between 1 and 200 characters", problemDetails.Errors["Title"]);
    }

    [Fact]
    public async Task CreateChecklist_WithNullTitle_ReturnsProblemDetailsWithValidationError()
    {
        // Arrange
        var json = JsonSerializer.Serialize(new {title = (string?) null});
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/checklists", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.True(problemDetails.Errors.ContainsKey("Title"));
    }

    [Fact]
    public async Task UpdateChecklist_WithValidData_ReturnsUpdatedChecklist()
    {
        // Arrange
        var createDto = new CreateChecklistDto(Title: "Original Title");
        var createResponse = await _client.PostAsJsonAsync("/api/checklists", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<ChecklistDto>();
        Assert.NotNull(created);

        // Act
        var updateDto = new UpdateChecklistDto(Title: "Updated Title");
        var updateResponse = await _client.PutAsJsonAsync($"/api/checklists/{created.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await updateResponse.Content.ReadFromJsonAsync<ChecklistDto>();
        Assert.NotNull(updated);
        Assert.Equal("Updated Title", updated.Title);
        Assert.Equal(created.Id, updated.Id);
    }

    [Fact]
    public async Task UpdateChecklist_WithEmptyTitle_ReturnsProblemDetailsWithValidationError()
    {
        // Arrange
        var createDto = new CreateChecklistDto(Title: "Original Title");
        var createResponse = await _client.PostAsJsonAsync("/api/checklists", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<ChecklistDto>();
        Assert.NotNull(created);

        // Act
        var invalidUpdateDto = new UpdateChecklistDto(Title: "");
        var updateResponse = await _client.PutAsJsonAsync($"/api/checklists/{created.Id}", invalidUpdateDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);

        var problemDetails = await updateResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.True(problemDetails.Errors.ContainsKey("Title"));
        Assert.Contains("Title is required", problemDetails.Errors["Title"]);
    }
}