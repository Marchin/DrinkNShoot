using UnityEngine;

public class Crow : MonoBehaviour {
    [SerializeField] GameObject m_roof;
    [SerializeField] float m_maxJumpInterval;
    [SerializeField] float m_height;
    [SerializeField] float m_maxMovementInterval;
    [SerializeField] float m_distance;
    Vector3 m_roofSize;
    Vector3 m_targetPosition;
    bool m_moving;
    bool m_flying;

    private void Awake() {
        m_roofSize = Vector3.Scale(
            m_roof.GetComponent<MeshRenderer>().bounds.size,
            transform.localScale
        );
        if (m_targetPosition != transform.position) {
            Vector3.Lerp(transform.position, m_targetPosition, Time.deltaTime);
        }
    }

    void Move() {
        if (!(m_moving && m_flying)) {
            // m_targetPosition = 
        }
    }

    void Flip() {
        if (!(m_moving && m_flying)) {
            // m_targetPosition = 
        }

    }

    void Jump() {
        if (!(m_moving && m_flying)) {
            // m_targetPosition = 
        }
    }

}