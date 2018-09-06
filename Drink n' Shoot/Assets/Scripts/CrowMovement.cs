using UnityEngine;

public class CrowMovement : MonoBehaviour {
    [SerializeField] GameObject m_roof;
    [SerializeField] float m_maxJumpInterval;
    [SerializeField] float m_height;
    [SerializeField] float m_maxMovementInterval;
    [SerializeField] float m_distance;
    CrowFly m_crowFly;
    Vector3 m_roofSize;
    Vector3 m_targetPosition;
    Quaternion m_targetRotation;
    float m_distToFront;
    bool m_moving;
    bool m_flipping;

    private void Awake() {
        // m_roofSize = Vector3.Scale(
        //     m_roof.GetComponent<MeshRenderer>().bounds.size,
        //     transform.localScale
        // );
        // if (m_targetPosition != transform.position) {
        //     Vector3.Lerp(transform.position, m_targetPosition, Time.deltaTime);
        // }
        m_distToFront = GetComponent<BoxCollider>().bounds.extents.z;
        m_crowFly = GetComponent<CrowFly>();
        m_moving = false;
        m_targetPosition = transform.position;
    }

    private void Update() {
        Move();
        if (transform.position != m_targetPosition) {
            transform.position = Vector3.Lerp(
                transform.position, m_targetPosition, Time.deltaTime * 2f);
        }
        if (transform.rotation != m_targetRotation) {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, m_targetRotation, 2f * Time.deltaTime);
        }
        if (transform.position == m_targetPosition &&
            transform.rotation == m_targetRotation) {

            ResetState();
        }
    }

    void Move() {
        if (!(m_moving && m_crowFly.IsFlying() && m_flipping)) {
            Vector3 targetOffset = transform.forward * (m_distance + m_distToFront);
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