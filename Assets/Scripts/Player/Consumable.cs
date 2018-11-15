using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class Consumable : MonoBehaviour, IItem
{
	[Header("Consumable Properties")]
	[SerializeField] [Tooltip("Name of the consumable item.")]
	string consumableName;
	[SerializeField] [Range(1, 5)][Tooltip("Maximum amount than can be carried.")]
	int maxAmount = 1;
	
	[Header("Consumable Animations")]
	[SerializeField] AnimationClip useAnimation;
	
	[Header("Consumable Sounds")]
	[SerializeField] AudioSource useSound;

	int amount = 0;
	protected bool isInUse = false;
	
	UnityEvent onUse = new UnityEvent();
	UnityEvent onEmpty = new UnityEvent();

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
		//PlayerManager.Instance.DecreaseConsumableAmount(this);
        onUse.Invoke();
		if (PlayerManager.Instance.GetItemAmount(this) == 0)
			onEmpty.Invoke();
        yield return new WaitForSeconds(useAnimation.length);
		ApplyConsumableEffect();
	}

	bool CanUse()
	{
		return !isInUse && PlayerManager.Instance.GetItemAmount(this) > 0;
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

	public void ReduceAmount()
	{
		if (amount > 0)
			amount--;
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

	public UnityEvent OnEmpty
	{
		get { return onEmpty;}
	}
}