using UnityEngine;

public class CrowMovement : MonoBehaviour {
    [SerializeField] GameObject m_roof;
    [SerializeField] float m_maxMovementInterval;
    [SerializeField] float m_distance;
    [SerializeField] float m_speed;
    [SerializeField] float m_rotationSpeed;
    //CrowFly m_crowFly;
    Vector3 m_roofSize;
    Vector3 m_targetPosition;
    Quaternion m_targetRotation;
    Vector3 m_meshOffset;
    float m_negligible = 0.01f;
    float m_distToFront;
    bool m_moving;
    bool m_flipping;

    private void Awake() {
        m_distToFront = GetComponent<BoxCollider>().bounds.extents.z;
        //m_crowFly = GetComponent<CrowFly>();
        m_moving = false;
        m_targetPosition = transform.position;
        m_targetRotation = transform.rotation;
    }

    private void Update() {
        Move();
        if (Vector3.Distance(transform.position, m_targetPosition) > m_negligible) {
            transform.position = Vector3.Lerp(
                transform.position, m_targetPosition, m_speed * Time.deltaTime);
        } else {
            transform.position = m_targetPosition;
        }
        if (Vector3.Distance(transform.eulerAngles, m_targetRotation.eulerAngles) > m_negligible) {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, m_targetRotation, m_rotationSpeed * Time.deltaTime);
        } else {
            transform.rotation = m_targetRotation;
        }
        if (transform.position == m_targetPosition &&
            transform.eulerAngles == m_targetRotation.eulerAngles) {

            ResetState();
        }
    }

    void Move() {
        if (!m_moving /*&& !m_crowFly.IsFlying()*/ && !m_flipping) {
            Vector3 targetOffset = transform.forward * (m_distance * Random.Range(0f, 1f) + m_distToFront);
            Debug.DrawRay(transform.position + targetOffset, -transform.up, Color.green, 1f);
            RaycastHit hit;
            bool wasHit = Physics.Raycast(transform.position + targetOffset, -transform.up, out hit, 2f);
            if (wasHit) {
                if (hit.transform.gameObject == m_roof) {
                    m_targetPosition = transform.position + transform.forward * m_distance;
                    m_moving = true;
                }
            } else {
                Flip();
            }
        }
    }

    void ResetState() {
        m_moving = false;
        m_flipping = false;
    }

    void Flip() {
        m_targetRotation = Quaternion.LookRotation(-transform.forward, transform.up);

        m_flipping = true;
    }

    public bool IsMoving() {
        return (m_moving || m_flipping);
    }

}