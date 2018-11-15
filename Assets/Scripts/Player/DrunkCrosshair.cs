using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Gun))]
public class DrunkCrosshair : MonoBehaviour
{
    [Header("Drunkness Fields")]
    [SerializeField] [Range(0f, 200f)] [Tooltip("Maximum radius of the crosshair circular motion.")]
    float maxRadius;
    [SerializeField] [Range(1f, 100f)] [Tooltip("Maximum crosshair movement speed.")]
    float maxSpeed;
    [SerializeField] [Range(10f, 100f)] [Tooltip("Maximum sway level allowed to guarantee a shot success.")]
	float maxSwayAllowed = 10f;

    // Constants
    const float DRUNK_SWAY_MULT = 5f;
    const float SCALE_MULT = 0.025f;
    const float PAR_VAR_RANGE_MULT = 0.15f;
	
    // Static Fields
	static Vector3 position;
	static bool targetOnClearSight = false;
    static float drunkSway = 0f;
    static float scale = 1f;

    // Computing Fields
	Gun gun;
	Camera fpsCamera;
    float drunkSwayInterpTime = 0f;
    float scaleInterpTime = 0f;
    float scaleAtShotInterpTime = 0f;
    float scaleBeforeShot = 1f;
    float scaleAfterShot = 1f;
    bool isIncreasingDrunkSway = true;
    float baseRadius = 0f;
	float radius = 0f;
    float angle = 0f;
    float baseSpeed = 0f;
    float speed = 0f;
    bool onLeftSide = true;

    // Events
    UnityEvent onScale = new UnityEvent();
    UnityEvent onColorChange = new UnityEvent();
    UnityEvent onMove = new UnityEvent();

    // Unity Methods
	void Awake()
	{	
		position = new Vector3(Screen.width / 2, Screen.height / 2, 1f);
		
		gun = GetComponent<Gun>();
		fpsCamera = GetComponentInParent<Camera>();
    }

	void Start()
	{
		gun.OnShot.AddListener(ScaleAtShot);
	}

	void Update()
	{
        MoveAround();
        ScaleAccordingToDrukenness();
        IndicateShotAccuracy();
	}

    // Private Methods
	void ScaleAtShot()
	{
        scaleBeforeShot = scale;
        scale += gun.GetRecoil() * SCALE_MULT;
        scaleAfterShot = scale;
        scaleAtShotInterpTime = 0f;
        onScale.Invoke();
	}

    void MoveAround()
    {
        Vector3 previousPosition = position;

        position.x = onLeftSide ? Screen.width / 2 + (Mathf.Cos(angle) - 1) * radius : 
                                    Screen.width / 2 + (-Mathf.Cos(angle) + 1) * radius;
        position.y = Screen.height / 2 + Mathf.Sin(angle) * radius;
        angle += speed * Time.deltaTime;
        if (angle >= 2 * Mathf.PI)
        {
            float radiusVariation = baseRadius * PAR_VAR_RANGE_MULT;
            float speedVariation = baseSpeed * PAR_VAR_RANGE_MULT;

            angle -= 2 * Mathf.PI;
            onLeftSide = !onLeftSide;
            radius = baseRadius + Random.Range(-radiusVariation, radiusVariation);
            speed = baseSpeed + Random.Range(-speedVariation, speedVariation);
        }

        if (position != previousPosition)
		{
			transform.LookAt(fpsCamera.ScreenToWorldPoint(position));
			transform.Rotate(0f, 90f, 0f); // Consider changing the model's forward to avoid doing this.
            onMove.Invoke();
		}
    }

    void ScaleAccordingToDrukenness()
    {
        float previousScale = scale;

        if (!gun.HasFiredConsecutively())
        {
            if (isIncreasingDrunkSway)
            {
                drunkSway = Mathf.Lerp(0f, LevelManager.Instance.DifficultyLevel * DRUNK_SWAY_MULT, drunkSwayInterpTime);
                scale = Mathf.Lerp(1f, 1f + drunkSway * SCALE_MULT, scaleInterpTime);
            }
            else
            {
                drunkSway = Mathf.Lerp(LevelManager.Instance.DifficultyLevel * DRUNK_SWAY_MULT, 0f, drunkSwayInterpTime);
                scale = Mathf.Lerp(1f + drunkSway * SCALE_MULT, 1f, scaleInterpTime);
            }
            drunkSwayInterpTime += Time.unscaledDeltaTime;
            scaleInterpTime += Time.unscaledDeltaTime;
            if (drunkSwayInterpTime >= 1f)
            {
                drunkSwayInterpTime = 0f;
                scaleInterpTime = 0f;
                isIncreasingDrunkSway = !isIncreasingDrunkSway;
            }
        }
        else
        {
            scale = Mathf.Lerp(scaleAfterShot, scaleBeforeShot, scaleAtShotInterpTime);
            scaleAtShotInterpTime += Time.unscaledDeltaTime / gun.GetRecoilDuration();
        }

        if (scale != previousScale)
            onScale.Invoke();
    }

    void IndicateShotAccuracy()
    {
		GameObject target = gun.ObjectOnSight();

        if (target != null && drunkSway + gun.GetRecoil() < maxSwayAllowed && gun.CanShootAtObject(target))
        {
            if (!targetOnClearSight)
            {
                targetOnClearSight = true;
                onColorChange.Invoke();
            }
        }
        else
        {
            if (targetOnClearSight)
            {
                targetOnClearSight = false;
                onColorChange.Invoke();
            }
        }
    }

    // Public Methods
	public static float GetHitProbability()
	{
		return (targetOnClearSight ? 100f : Random.Range(0f, 100f - drunkSway));
	}

    // Getters & Setters
	public static Vector3 Position
	{
		get { return position; }
	}

    public static float Scale
    {
        get { return scale; }
    }

    public static bool TargetOnClearSight
    {
        get { return targetOnClearSight; }
    }

    public float BaseSpeed
    {
        get { return baseSpeed; }
        set{ baseSpeed = speed =  value < maxSpeed ? value : maxSpeed; }
    }

    public float BaseRadius
    {
        get { return baseRadius; }
        set { baseRadius = radius = value < maxRadius ? value : maxRadius; }
	}

	public UnityEvent OnScale
	{
		get { return onScale; }
	}

	public UnityEvent OnColorChange
	{
		get { return onColorChange; }
	}

	public UnityEvent OnMove
	{
		get { return onMove; }
	}
}