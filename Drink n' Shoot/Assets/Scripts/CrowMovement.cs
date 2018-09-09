using UnityEngine;

public class CrowMovement : MonoBehaviour {
    [SerializeField] GameObject m_roof;
    [SerializeField] float m_maxMovementInterval;
    [SerializeField] float m_distance;
    [SerializeField] float m_speed;
    [SerializeField] float m_rotationSpeed;
    CrowFly m_crowFly;
    Vector3 m_roofSize;
    Vector3 m_targetPosition;
    Quaternion m_targetRotation;
    float m_distToFront;
    bool m_moving;
    bool m_flipping;

    private void Awake() {
        m_distToFront = GetComponent<BoxCollider>().bounds.extents.z;
        m_crowFly = GetComponent<CrowFly>();
        m_moving = false;
        m_targetPosition = transform.position;
    }

    private void Update() {
        Move();
        if (transform.position != m_targetPosition) {
            transform.position = Vector3.Lerp(
                transform.position, m_targetPosition, m_speed * Time.deltaTime);
        }
        if (transform.eulerAngles != m_targetRotation.eulerAngles) {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, m_targetRotation, m_rotationSpeed * Time.deltaTime);
        }
        if (transform.position == m_targetPosition &&
            transform.eulerAngles == m_targetRotation.eulerAngles) {

            ResetState();
        }
    }

    void Move() {
        if (!m_moving /*&& !m_crowFly.IsFlying()*/ && !m_flipping) {
            Vector3 maxDistance = (m_roof.transform.position +
                m_distToFront * Vector3.right) - transform.position;
            Vector3 targetOffset = transform.forward * maxDistance.magnitude * Random.Range(0f, 1f);
            if (Physics.Raycast(transform.position + targetOffset, -transform.up, 2f)) {

                m_targetPosition = transform.position + transform.forward * m_distance;
                m_moving = true;
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
        Vector3 flip = transform.eulerAngles;
        flip.y += 180f;
        m_targetRotation = Quaternion.Euler(flip);
        m_flipping = true;
    }

    public bool IsMoving() {
        return (m_moving || m_flipping);
    }
}