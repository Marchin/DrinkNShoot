using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class Consumable : MonoBehaviour, IItem
{
	public enum ConsumableType
	{
		Drinkable, Throwable
	}

	[Header("Consumable Properties")]
	[SerializeField] [Tooltip("Name of the consumable item.")]
	string consumableName;
	[SerializeField] [Tooltip("Type of consumable.")]
	ConsumableType consumableType;
	[SerializeField] [Range(1, 5)][Tooltip("Maximum amount than can be carried.")]
	int maxAmount = 1;
	
	[Header("Consumable Animations")]
	[SerializeField] AnimationClip useAnimation;
	
	[Header("Consumable Sounds")]
	[SerializeField] AudioSource useSound;
	
	[Header("Events")]
	[SerializeField] UnityEvent onUse;
	
	int amount = 0;
	protected bool isInUse = false;
	
	protected virtual void Start()
	{
		amount = PlayerManager.Instance.GetItemAmount(this);
	}

	protected virtual void Update()
	{
		if (InputManager.Instance.GetUseItemButton() && CanUse())
			StartCoroutine(UseItem());
	}

	IEnumerator UseItem()
	{
		isInUse = true;
		amount -= 1;
        onUse.Invoke();
        yield return new WaitForSeconds(useAnimation.length);
		ApplyConsumableEffect();
	}

	bool CanUse()
	{
		return !isInUse && amount > 0;
	} 

	protected abstract void ApplyConsumableEffect();

	public void IncreaseAmount(int amount)
	{
		if (this.amount + amount <= maxAmount )
			this.amount += amount;
		else
			this.amount = maxAmount;
	}

	public string GetName()
	{
		return consumableName;
	}

	public int GetAmount()
	{
		return amount;
	}

	public int GetMaxAmount()
	{
		return maxAmount;
	}

	public ItemType GetItemType()
	{
		return ItemType.Consumable;
	}

	public AnimationClip UseAnimation
	{
		get { return useAnimation; }
	}

	public AudioSource UseSound
	{
		get { return useSound; }
	}

	public UnityEvent OnUse
	{
		get { return onUse;}
	}
}
