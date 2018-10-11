using UnityEngine;

public class CrowFly : MonoBehaviour, IState {
    [SerializeField] LayerMask m_playerLayer;
    [SerializeField] float m_flightSpeed;
    [SerializeField] float m_turnSpeed;
    PoopImage m_poopImage;
    Vector3 m_destination;

    private void Awake() {
        m_poopImage = FindObjectOfType<PoopImage>();
    }

    public void StateUpdate(out IState nextState) {
        Vector3 diff = m_destination - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.LookRotation(diff),
            Time.deltaTime * m_turnSpeed);
        diff = transform.InverseTransformDirection(diff);
        diff = diff.normalized;
        diff.x = 0f;
        diff = transform.TransformDirection(diff);
        diff += transform.forward; //make it faster going forward
        transform.position += diff * Time.deltaTime * m_flightSpeed;
        if (Physics.Raycast(transform.position, Vector3.down, 20f, m_playerLayer)) {
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