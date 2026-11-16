using MyHybridApp2.UI.Models;

namespace MyHybridApp2.UI.Services;

public sealed class BottomNavigationService : IBottomNavigationService
{
    private static readonly IReadOnlyList<BottomNavigationItem> Items =
    [
        new() { Id = "feed", Label = "Feed", Icon = "🏠", Route = "/home", SortOrder = 1 },
        new() { Id = "announcements", Label = "Announcements", Icon = "📣", Route = "/announcements", SortOrder = 2 },
        new() { Id = "calendar", Label = "Calendar", Icon = "🗓️", Route = "/calendar", SortOrder = 3 },
        new() { Id = "diary", Label = "Diary", Icon = "📔", Route = "/diary", SortOrder = 4 }
    ];

    public Task<IReadOnlyList<BottomNavigationItem>> GetItemsAsync() =>
        Task.FromResult<IReadOnlyList<BottomNavigationItem>>(Items.OrderBy(item => item.SortOrder).ToArray());
}
