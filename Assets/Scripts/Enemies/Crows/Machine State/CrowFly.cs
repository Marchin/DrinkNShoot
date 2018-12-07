using UnityEngine;

[RequireComponent(typeof(Crow))]
public class CrowFly : MonoBehaviour, IState {
    [SerializeField] LayerMask m_playerLayer;
    [SerializeField] float m_flightSpeed;
    [SerializeField] float m_turnSpeed;
	LayerMask m_obstaclesLayer;
    PoopImage m_poopImage;
    Vector3 m_destination;
    Quaternion m_targetRotation;
    Vector3 diff;

    private void Awake() {
		m_obstaclesLayer = GetComponent<Crow>().ObstaclesLayer;
        m_poopImage = FindObjectOfType<PoopImage>();
    }

    public void StateUpdate(out IState nextState) {
		RaycastHit buildHit;
		Physics.Raycast(transform.position, transform.forward,
			out buildHit, 5f, m_obstaclesLayer);
        diff = m_destination - transform.position;
        if (buildHit.collider != null && buildHit.distance < diff.magnitude) {
            Vector3 projection = diff - buildHit.normal *
                Vector3.Dot(diff, buildHit.normal);
            diff = transform.InverseTransformDirection(diff);
            diff.x = 0f;
            diff.z = Mathf.Abs(diff.z);
            diff = transform.TransformDirection(diff);
            diff = diff.normalized;
            if (Vector3.SignedAngle(transform.forward, projection, Vector3.up) > 90f) {
                projection.x *= -1f;
                projection.z *= -1f;
            }
            Debug.DrawRay(buildHit.point, projection, Color.yellow, 1f);
            m_targetRotation = Quaternion.LookRotation(projection);
            if (buildHit.distance < 0.25f) {
                transform.rotation = m_targetRotation;
            }
            transform.position += diff * 0.25f *
                m_flightSpeed * Time.deltaTime;
		} else {
            diff = m_destination - transform.position;
            diff = transform.InverseTransformDirection(diff);
            diff.x = 0f;
            diff.z = Mathf.Abs(diff.z);
            diff = transform.TransformDirection(diff);
            m_targetRotation =  Quaternion.LookRotation(m_destination - transform.position);
            float angle = Vector3.Angle(diff, m_destination - transform.position);
            if ((angle > 178f && angle < 182f) || angle == 90f) {
                m_targetRotation = Quaternion.LookRotation(transform.eulerAngles + 30f*Vector3.up);
                transform.rotation = m_targetRotation;
            }
            transform.position += diff.normalized * m_flightSpeed * Time.deltaTime;
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            m_targetRotation, Time.deltaTime * m_turnSpeed);
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 2f, Vector3.down, out hit, 20f, m_playerLayer)) {
            m_poopImage.Poop();
            nextState = GetComponent<CrowLand>();
        } else {
            nextState = this;
        }
    }

    public void StateFixedUpdate() { }

    public void SetDestination(Vector3 destination) {
        m_destination = destination;
    }
}