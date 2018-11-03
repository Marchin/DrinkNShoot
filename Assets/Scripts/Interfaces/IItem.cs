public enum ItemType
{
    Gun, Consumable
}

public interface IItem
{
    string GetName();
    int GetAmount();
    ItemType GetItemType();
}