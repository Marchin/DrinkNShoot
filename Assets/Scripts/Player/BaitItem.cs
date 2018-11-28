using UnityEngine;

public class BaitItem : Consumable
{
	[Header("Bite Properties")]
    [SerializeField] GameObject baitPrefab;
    [SerializeField] LayerMask landingLayers;
    [SerializeField] float range;
    GameObject bait;
    Camera fpsCamera;

    private void Awake() 
    {
        fpsCamera = GetComponentInParent<Camera>();
        bait = Instantiate(baitPrefab, transform.position, Quaternion.identity);
        bait.SetActive(false);
    }

    protected override void ApplyConsumableEffect()
    {
		Vector3 direction = (fpsCamera.ScreenToWorldPoint(DrunkCrosshair.Position)
         - fpsCamera.transform.position).normalized;
		RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, direction, out hit, range, landingLayers))
        {
            bait.GetComponent<Bait>().SetPath(transform.position, hit.point);
        }
        else 
        {
            Vector3 destination = transform.position + direction * range;
            bait.GetComponent<Bait>().SetPath(transform.position, destination);
        }
        bait.SetActive(true);

    }
}
