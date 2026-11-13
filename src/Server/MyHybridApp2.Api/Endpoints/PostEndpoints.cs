using MyHybridApp2.Application.Posts;
using MyHybridApp2.Shared.Posts;

namespace MyHybridApp2.Api.Endpoints;

internal static class PostEndpoints
{
    public static IEndpointRouteBuilder MapPostEndpoints(this IEndpointRouteBuilder app)
    {
        var posts = app.MapGroup("/api/posts").WithTags("Posts");

        posts.MapGet("/", (GetPosts handler, CancellationToken ct) => handler.HandleAsync(ct))
            .WithName("GetPosts");

        posts.MapGet("/{id:guid}", (Guid id, GetPostById handler, CancellationToken ct) => handler.HandleAsync(id, ct))
            .WithName("GetPostById");

        posts.MapPost("/", async (CreatePostRequest request, CreatePost handler, CancellationToken ct) =>
            {
                var post = await handler.HandleAsync(request, ct);
                return TypedResults.CreatedAtRoute(post, "GetPostById", new { id = post.Id });
            })
            .WithName("CreatePost")
            .ProducesValidationProblem();

        MapInteraction(posts, "/{id:guid}/like", PostInteraction.Like, isPost: true);
        MapInteraction(posts, "/{id:guid}/like", PostInteraction.Unlike, isPost: false);
        MapInteraction(posts, "/{id:guid}/favorite", PostInteraction.Favorite, isPost: true);
        MapInteraction(posts, "/{id:guid}/favorite", PostInteraction.Unfavorite, isPost: false);
        MapInteraction(posts, "/{id:guid}/share", PostInteraction.Share, isPost: true);

        return app;
    }

    private static void MapInteraction(RouteGroupBuilder group, string pattern, PostInteraction interaction, bool isPost)
    {
        Delegate handler = (Guid id, InteractWithPost interact, CancellationToken ct) => interact.HandleAsync(id, interaction, ct);

        var endpoint = isPost ? group.MapPost(pattern, handler) : group.MapDelete(pattern, handler);
        endpoint.WithName($"{interaction}Post").ProducesProblem(StatusCodes.Status404NotFound);
    }
}
