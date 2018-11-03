public enum ItemType
{
    Gun, Consumable, Throwable
}

public interface IItem
{
    string GetName();
    int GetAmount();
    ItemType GetItemType();
}