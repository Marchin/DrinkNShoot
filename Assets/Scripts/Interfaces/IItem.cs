public enum ItemType
{
    Gun, Consumable, Bait
}

public interface IItem
{
    string GetName();
    int GetAmount();
    int GetMaxAmount();
    ItemType GetItemType();
}