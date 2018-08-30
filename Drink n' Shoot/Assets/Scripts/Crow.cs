using UnityEngine;

public class Crow : MonoBehaviour {
    [SerializeField] GameObject m_roof;
    [SerializeField] float m_maxJumpInterval;
    [SerializeField] float m_height;
    [SerializeField] float m_maxMovementInterval;
    [SerializeField] float m_distance;
    Animator m_anim;
    Vector3 m_roofSize;
    Vector3 m_targetPosition;
    float m_distToFront;
    bool m_moving;
    bool m_flying;

    private void Awake() {
        // m_roofSize = Vector3.Scale(
        //     m_roof.GetComponent<MeshRenderer>().bounds.size,
        //     transform.localScale
        // );
        // if (m_targetPosition != transform.position) {
        //     Vector3.Lerp(transform.position, m_targetPosition, Time.deltaTime);
        // }
        m_anim = GetComponent<Animator>();
        m_distToFront = GetComponent<BoxCollider>().bounds.extents.z;
        m_moving = false;
        m_flying = false;
        m_targetPosition = transform.position;
    }

    private void Update() {
        Move();
        if (transform.position != m_targetPosition) {
            transform.position = Vector3.Lerp(transform.position, m_targetPosition, Time.deltaTime * 2f);
        } else {
            ResetState();
        }
    }

    void Move() {
        if (!(m_moving && m_flying)) {
            if (Physics.Raycast(transform.position + transform.forward * (m_distance + m_distToFront),
                -transform.up,
                2f
            )){
                m_targetPosition = transform.position + transform.forward * m_distance; 
                m_moving = true;
            }
        }
    }

    void ResetState(){
        m_moving = false;
        m_flying = false;
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