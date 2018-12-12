using UnityEngine;

[RequireComponent(typeof(Crow))]
public class CrowFly : MonoBehaviour, IState {
    [SerializeField] LayerMask m_playerLayer;
    [SerializeField] float m_flightSpeed;
    [SerializeField] float m_turnSpeed;
    Crow m_crow;
	LayerMask m_obstaclesLayer;
    PoopImage m_poopImage;
    Transform m_target;
    Vector3 m_destination;
    Quaternion m_targetRotation;
	Vector3 m_playerToStageNorm;
    Vector3 m_diff;
    bool m_positioned;

    private void Awake() {
        m_crow = GetComponent<Crow>();
		m_obstaclesLayer = m_crow.ObstaclesLayer;
        m_poopImage = FindObjectOfType<PoopImage>();
    }

    private void OnEnable() {
        m_positioned = false;
	    m_playerToStageNorm = m_crow.PlayerToStage.normalized;
        m_destination = m_target.position + 6f * m_playerToStageNorm + 0.5f*Vector3.down;
    }

    public void StateUpdate(out IState nextState) {
		RaycastHit buildHit;
		Physics.Raycast(transform.position, transform.forward,
			out buildHit, 5f, m_obstaclesLayer);
        m_diff = m_destination - transform.position;
        if (buildHit.collider != null && buildHit.distance < m_diff.magnitude) {
            Vector3 projection = m_diff - buildHit.normal *
                Vector3.Dot(m_diff, buildHit.normal);
            if (Vector3.Angle(projection, Vector3.down) < 45f) {
                projection = Vector3.Cross(projection, buildHit.normal);
            } else if (Vector3.Angle(buildHit.normal, Vector3.up) < 45f) {
					Vector3 angle = transform.localEulerAngles;
					angle.x -= 5f;
					transform.localEulerAngles = angle;
				}
            m_diff = transform.InverseTransformDirection(m_diff);
            m_diff.x = 0f;
            m_diff.z = Mathf.Abs(m_diff.z);
            m_diff = transform.TransformDirection(m_diff);
            m_diff = m_diff.normalized;
            if (Vector3.SignedAngle(transform.forward, projection, Vector3.up) > 90f) {
                projection.x *= -1f;
                projection.z *= -1f;
            }
            Debug.DrawRay(buildHit.point, projection, Color.yellow, 1f);
            m_targetRotation = Quaternion.LookRotation(projection);
            if (buildHit.distance < 0.25f) {
                transform.rotation = m_targetRotation;
            }
            transform.position += m_diff * 0.25f *
                m_flightSpeed * Time.deltaTime;
		} else {
            m_diff = m_destination - transform.position;
            m_diff = transform.InverseTransformDirection(m_diff);
            m_diff.x = 0f;
            m_diff.z = Mathf.Abs(m_diff.z);
            m_diff = transform.TransformDirection(m_diff);
            m_targetRotation =  Quaternion.LookRotation(m_destination - transform.position);
            float angle = Vector3.Angle(m_diff, m_destination - transform.position);
            if ((angle > 178f && angle < 182f) || angle == 90f) {
                m_targetRotation = Quaternion.LookRotation(
                    transform.eulerAngles + 30f*Vector3.up);
                transform.rotation = m_targetRotation;
            }
            transform.position += m_diff.normalized * m_flightSpeed * Time.deltaTime *
                (m_positioned? 0.5f : 1f);
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            m_targetRotation, Time.deltaTime * m_turnSpeed);
        if (!m_positioned) {
            if (Vector3.Distance(transform.position, m_destination) < 2f) {
                m_destination = m_target.position + 3f * Vector3.up;
                transform.rotation = Quaternion.LookRotation(
                    m_destination - transform.position);
                m_positioned = true;
            } 
            nextState = this;
        } else {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 2f, Vector3.down,
                out hit, 20f, m_playerLayer)) {

                m_poopImage.Poop();
                nextState = GetComponent<CrowLand>();
            } else {
                nextState = this;
            }
        }
    }

    public void StateFixedUpdate() { }

    public void SetDestination(Transform target) {
        m_target = target;
    }
}