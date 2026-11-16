namespace MyHybridApp2.UI.Models;

public sealed class BottomNavigationItem
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
