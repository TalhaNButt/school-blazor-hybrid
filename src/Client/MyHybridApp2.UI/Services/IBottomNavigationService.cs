using MyHybridApp2.UI.Models;

namespace MyHybridApp2.UI.Services;

public interface IBottomNavigationService
{
    Task<IReadOnlyList<BottomNavigationItem>> GetItemsAsync();
}
