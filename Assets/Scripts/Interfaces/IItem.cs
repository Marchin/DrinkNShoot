public enum ItemType
{
    Gun, Consumable
}

public interface IItem
{
    string GetName();
    int GetAmount();
    int GetMaxAmount();
    ItemType GetItemType();
}