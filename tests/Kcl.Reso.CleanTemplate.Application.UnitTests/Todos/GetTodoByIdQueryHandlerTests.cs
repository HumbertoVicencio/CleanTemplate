using Kcl.Reso.CleanTemplate.Application.Abstractions.Authentication;
using Kcl.Reso.CleanTemplate.Application.Todos.GetById;
using Kcl.Reso.CleanTemplate.Application.UnitTests.Abstractions;
using Kcl.Reso.CleanTemplate.Domain.Todos;
using Microsoft.Extensions.Caching.Hybrid;
using Kcl.Reso.CleanTemplate.SharedKernel;

namespace Kcl.Reso.CleanTemplate.Application.UnitTests.Todos;

public sealed class GetTodoByIdQueryHandlerTests : BaseHandlerTest
{
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public async Task Handle_Should_ReturnNotFound_WhenTodoDoesNotExist()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        HybridCache cache = CreateCache();
        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);

        var handler = new GetTodoByIdQueryHandler(context, userContext, cache);
        var query = new GetTodoByIdQuery(Guid.NewGuid());

        // Act
        Result<TodoResponse> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("TodoItems.NotFound");
    }

    [Fact]
    public async Task Handle_Should_ReturnTodo_WhenItExistsForTheCurrentUser()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            UserId = UserId,
            Description = "Cached todo",
            Priority = Priority.High,
            Labels = ["important"],
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };
        context.TodoItems.Add(todoItem);
        await context.SaveChangesAsync();

        HybridCache cache = CreateCache();
        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);

        var handler = new GetTodoByIdQueryHandler(context, userContext, cache);
        var query = new GetTodoByIdQuery(todoItem.Id);

        // Act
        Result<TodoResponse> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(todoItem.Id);
        result.Value.Description.ShouldBe("Cached todo");
    }
}
