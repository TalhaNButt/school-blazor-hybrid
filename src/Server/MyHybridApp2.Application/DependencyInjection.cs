using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MyHybridApp2.Application.Calendar;
using MyHybridApp2.Application.Posts;

namespace MyHybridApp2.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddSingleton(TimeProvider.System);

        services.AddScoped<ICalendarEventService, CalendarEventService>();
        services.AddScoped<GetPosts>();
        services.AddScoped<GetPostById>();
        services.AddScoped<CreatePost>();
        services.AddScoped<GetPostComments>();
        services.AddScoped<GetPostCommentById>();
        services.AddScoped<CreatePostComment>();
        services.AddScoped<InteractWithPost>();

        return services;
    }
}
