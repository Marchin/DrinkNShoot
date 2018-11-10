using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Gun))]
public class DrunkCrosshair : MonoBehaviour
{
    [Header("Drunkness Fields")]
    [SerializeField] [Range(0f, 200f)] [Tooltip("Maximum radius of the crosshair circular motion.")]
    float maxRadius;
    [SerializeField] [Range(1, 100)] [Tooltip("Maximum crosshair movement speed.")]
    float maxSpeed;

    public UnityEvent OnScale;
    public UnityEvent OnColorChange;
    public UnityEvent OnMove;

    const float DRUNK_SWAY_MULT = 5f;
    const float MAX_SWAY_ALLOWED = 10f;
    const float SCALE_MULT = 0.025f;
    const float PAR_VAR_RANGE_MULT = 0.15f;
	
	static Vector3 position;
	static bool targetOnClearSight = false;
    static float drunkSway = 0f;
    static float scale = 1f;

	Gun gun;
    float drunkSwayInterpTime = 0f;
    float scaleInterpTime = 0f;
    float scaleAtShotInterpTime = 0f;
    float scaleBeforeShot = 1f;
    float scaleAfterShot = 1f;
    bool isIncreasingDrunkSway = true;
    float radius = 0f;
    float angle = 0f;
    float speed = 0f;
    bool onLeftSide = true;

	void Awake()
	{
		OnScale = new UnityEvent();
		OnColorChange = new UnityEvent();
		OnMove = new UnityEvent();
		
		position = new Vector3(Screen.width / 2, Screen.height / 2, 1f);
		
		gun = GetComponent<Gun>();
		gun.OnShot.AddListener(ScaleAtShot);     
    }

	void Update()
	{
        MoveAround();
        ScaleAccordingToDrukenness();
        IndicateShotAccuracy();
	}

	void ScaleAtShot()
	{
        scaleBeforeShot = scale;
        scale += gun.RecoilSwayLevel * gun.ConsecutiveShots * SCALE_MULT;
        scaleAfterShot = scale;
        scaleAtShotInterpTime = 0f;
        OnScale.Invoke();
	}

    void MoveAround()
    {
        Vector3 previousCrosshairPosition = position;

        position.x = onLeftSide ? Screen.width / 2 + (Mathf.Cos(angle) - 1) * radius :
                                                    Screen.width / 2 + (-Mathf.Cos(angle) + 1) * radius;
        position.y = Screen.height / 2 + Mathf.Sin(angle) * radius;
        angle += speed * Time.deltaTime;
        if (angle >= 2 * Mathf.PI)
        {
            float radiusVariation = radius * PAR_VAR_RANGE_MULT;
            float speedVariation = speed * PAR_VAR_RANGE_MULT;

            angle -= 2 * Mathf.PI;
            onLeftSide = !onLeftSide;
            radius += Random.Range(-radiusVariation, radiusVariation);
            speed += Random.Range(-speedVariation, speedVariation);
        }

        if (position != previousCrosshairPosition)
            OnMove.Invoke();
    }

    void ScaleAccordingToDrukenness()
    {
        float previousScale = scale;

        if (gun.ConsecutiveShots == 0)
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
            drunkSwayInterpTime += Time.deltaTime;
            scaleInterpTime += Time.deltaTime;
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
            scaleAtShotInterpTime += Time.deltaTime / (gun.TimeToShootSingleBullet * gun.ConsecutiveShotsAtShot);
        }

        if (scale != previousScale)
            OnScale.Invoke();
    }

    void IndicateShotAccuracy()
    {
		GameObject target = gun.ObjectOnSight();

        if (target != null && drunkSway + gun.RecoilSwayLevel * gun.ConsecutiveShots < MAX_SWAY_ALLOWED)
        {
            if (!targetOnClearSight && gun.CanShootAtObject(target))
            {
                targetOnClearSight = true;
                OnColorChange.Invoke();
            }
        }
        else
        {
            if (targetOnClearSight)
            {
                targetOnClearSight = false;
                OnColorChange.Invoke();
            }
        }
    }

	public static float GetHitProbability()
	{
		return (targetOnClearSight ? 100f : Random.Range(0f, 100f - drunkSway));
	}

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

    public float Speed
    {
        get { return speed; }
        set { speed = value < maxSpeed ? value : maxSpeed; }
    }

    public float Radius
    {
        get { return radius; }
        set { radius = value < maxRadius ? value : maxRadius; }
	}
}