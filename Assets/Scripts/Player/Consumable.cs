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
	
	[Header("Consumable Animations")]
	[SerializeField] AnimationClip useAnimation;
	
	[Header("Consumable Sounds")]
	[SerializeField] AudioSource useSound;
	
	[Header("Events")]
	[SerializeField] UnityEvent onUse;
	
	int amount = 3;
	protected bool isUsing = false;

	protected virtual void Update()
	{
		if (InputManager.Instance.GetUseItemButton() && CanUse())
			StartCoroutine(UseItem());
	}

	IEnumerator UseItem()
	{
		isUsing = true;
		amount -= 1;
        onUse.Invoke();
        yield return new WaitForSeconds(useAnimation.length);
		ApplyConsumableEffect();
	}

	bool CanUse()
	{
		return !isUsing && amount > 0;
	} 

	protected abstract void ApplyConsumableEffect();

	public string GetName()
	{
		return consumableName;
	}

	public int GetAmount()
	{
		return amount;
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
