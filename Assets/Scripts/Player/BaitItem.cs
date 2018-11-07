using UnityEngine;

public class BaitItem : Consumable
{
	[Header("Bite Properties")]
    [SerializeField] GameObject baitPrefab;
    [SerializeField] LayerMask landingLayers;
    [SerializeField] float range;
    GameObject bait;
    Camera fpsCamera;
    Gun gun;

    private void Awake() 
    {
        fpsCamera = GetComponent<Camera>();
        gun = GetComponent<Gun>();
        bait = Instantiate(baitPrefab, transform.position, Quaternion.identity);
        bait.SetActive(false);
    }

    protected override void ApplyConsumableEffect()
    {
		Vector3 direction = (fpsCamera.ScreenToWorldPoint(gun.CrosshairPosition)
         - fpsCamera.transform.position).normalized;
		RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, direction, out hit, range, landingLayers))
        {
            bait.GetComponent<Bait>().SetPath(transform.position, hit.point);
        }
        else 
        {
            Vector3 destination = transform.position + Vector3.forward * range;
            bait.GetComponent<Bait>().SetPath(transform.position, destination);
        }
        bait.SetActive(true);

    }
}
