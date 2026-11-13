using FluentValidation;
using MyHybridApp2.Application.Abstractions;
using MyHybridApp2.Domain.Posts;
using MyHybridApp2.Shared.Posts;

namespace MyHybridApp2.Application.Posts;

public sealed class CreatePost(IAppDbContext db, IValidator<CreatePostRequest> validator, TimeProvider timeProvider)
{
    public async Task<PostResponse> HandleAsync(CreatePostRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var post = Post.Create(
            request.AuthorName,
            request.ContentText,
            request.MediaUrl,
            request.MediaType,
            timeProvider.GetUtcNow().UtcDateTime);

        db.Posts.Add(post);
        await db.SaveChangesAsync(cancellationToken);

        return post.ToResponse();
    }
}

public sealed class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostRequestValidator()
    {
        RuleFor(x => x.AuthorName).NotEmpty().MaximumLength(Post.AuthorNameMaxLength);
        RuleFor(x => x.ContentText).MaximumLength(Post.ContentTextMaxLength);
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.ContentText) || !string.IsNullOrWhiteSpace(x.MediaUrl))
            .WithName("Post")
            .WithMessage("A post needs text or media.");
        RuleFor(x => x.MediaUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.MediaUrl))
            .WithMessage("Media URL must be an absolute URL.");
        RuleFor(x => x.MediaType)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.MediaUrl));
    }
}
