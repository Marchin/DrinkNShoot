using UnityEngine;
using UnityEngine.Events;

public class CrowMovement : MonoBehaviour, IState {
    [SerializeField] LayerMask m_landingZonesLayer;
    [SerializeField] float m_interval;
    [SerializeField] float m_distance;
    [SerializeField] float m_speed;
    [SerializeField] float m_chaseSpeed;
    Vector3 m_targetPosition;
    const float m_NEGLIGIBLE = 0.01f;
    float m_timer;
    float m_distToFront;
    float m_distToFoot;
    bool m_moving;
    bool m_hasToFlip;

    UnityEvent onStartWalking = new UnityEvent();
    UnityEvent onStopWalking = new UnityEvent();

    public UnityEvent OnStartWalking { get { return onStartWalking; } }
    public UnityEvent OnStopWalking { get { return onStopWalking; } }

    private void OnEnable() {
        m_distToFront = GetComponent<BoxCollider>().size.z / 2f;
        m_distToFoot = GetComponent<BoxCollider>().size.y / 2f;
        m_targetPosition = transform.position;
        m_hasToFlip = false;
        m_moving = false;
        m_timer = m_interval;
    }

    public void StateUpdate(out IState nextState) {
        m_timer -= Time.deltaTime;
        if (m_timer <= 0f && !m_moving) {
            Move();
            onStartWalking.Invoke();
        }
        if (m_moving) {
            if (Vector3.Distance(transform.position, m_targetPosition) > m_NEGLIGIBLE) {
                transform.position = Vector3.Lerp(
                    transform.position, m_targetPosition, m_speed * Time.deltaTime);
            } else {
                transform.position = m_targetPosition;
                m_timer = m_interval;
                m_moving = false;
                onStopWalking.Invoke();
            }
        }
        if (m_hasToFlip) {
            nextState = GetComponent<CrowFlip>();
        } else {
            nextState = this;
        }
    }

    public void StateFixedUpdate() { }

    void Move() {
        float distanceVariation = m_distance * Random.Range(0f, 1f);
        Vector3 targetOffset = transform.forward * (distanceVariation + m_distToFront);
        Debug.DrawRay(transform.position + targetOffset, -transform.up, Color.green, 1f);
        RaycastHit hit;
        bool wasHit = Physics.Raycast(transform.position + targetOffset, -transform.up,
            out hit, m_distToFoot + 0.5f, m_landingZonesLayer);
        if (wasHit) {
            m_targetPosition = transform.position + transform.forward * distanceVariation;
            m_moving = true;
        } else {
            m_hasToFlip = true;
        }
    }

}