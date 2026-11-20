using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyHybridApp2.Application.Abstractions;
using MyHybridApp2.Application.Common;
using MyHybridApp2.Domain.Posts;
using MyHybridApp2.Shared.Posts;

namespace MyHybridApp2.Application.Posts;

public sealed class GetPostComments(IAppDbContext db)
{
    public async Task<IReadOnlyList<PostCommentResponse>> HandleAsync(Guid postId, CancellationToken cancellationToken)
    {
        if (!await db.Posts.AnyAsync(post => post.Id == postId, cancellationToken))
        {
            throw new NotFoundException(nameof(Post), postId);
        }

        return await db.PostComments
            .AsNoTracking()
            .Where(comment => comment.PostId == postId)
            .OrderBy(comment => comment.CreatedAt)
            .Select(comment => new PostCommentResponse(
                comment.Id,
                comment.PostId,
                comment.AuthorName,
                comment.Content,
                comment.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}

public sealed class GetPostCommentById(IAppDbContext db)
{
    public async Task<PostCommentResponse> HandleAsync(Guid postId, Guid commentId, CancellationToken cancellationToken)
    {
        var comment = await db.PostComments
            .AsNoTracking()
            .Where(comment => comment.PostId == postId && comment.Id == commentId)
            .Select(comment => new PostCommentResponse(
                comment.Id,
                comment.PostId,
                comment.AuthorName,
                comment.Content,
                comment.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(PostComment), commentId);

        return comment;
    }
}

public sealed class CreatePostComment(
    IAppDbContext db,
    IValidator<CreatePostCommentRequest> validator,
    TimeProvider timeProvider)
{
    public async Task<PostCommentResponse> HandleAsync(
        Guid postId,
        CreatePostCommentRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        if (!await db.Posts.AnyAsync(post => post.Id == postId, cancellationToken))
        {
            throw new NotFoundException(nameof(Post), postId);
        }

        var comment = PostComment.Create(
            postId,
            request.AuthorName,
            request.Content,
            timeProvider.GetUtcNow());

        db.PostComments.Add(comment);
        await db.SaveChangesAsync(cancellationToken);

        return new PostCommentResponse(
            comment.Id,
            comment.PostId,
            comment.AuthorName,
            comment.Content,
            comment.CreatedAt);
    }
}

public sealed class CreatePostCommentRequestValidator : AbstractValidator<CreatePostCommentRequest>
{
    public CreatePostCommentRequestValidator()
    {
        RuleFor(request => request.AuthorName)
            .NotEmpty()
            .MaximumLength(PostComment.AuthorNameMaxLength);
        RuleFor(request => request.Content)
            .NotEmpty()
            .MaximumLength(PostComment.ContentMaxLength);
    }
}
