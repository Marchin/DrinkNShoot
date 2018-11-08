using UnityEngine;

public class Bait : MonoBehaviour {
    [SerializeField] LayerMask m_crowsLayer;
    [SerializeField] float m_duration = 2f;
    [SerializeField] float m_radius = 2f;
    [SerializeField] float m_height = 5f;
    Vector3 m_source;
    Vector3 m_destination;
    float m_a; // quadratic constants to calculate y(z)
    float m_c; //where y(z) = a*z*z + b*z + c; and b = 0
    float m_rate; // 1/distance
    float m_accu;

    private void Awake() {
        enabled = false;
        m_accu = 0f;
    }
    
    private void Update() {
        Vector3 newPos = Vector3.Lerp(transform.position, 
            m_destination, m_accu);
        newPos.y = m_a*m_accu*m_accu + m_c;
        m_accu += m_rate * Time.deltaTime;
        transform.position = newPos;
        if (m_accu >= 1f) {
            Collider[] crows = Physics.OverlapSphere(transform.position, m_radius, m_crowsLayer);
            enabled = false;
        }
    }

    public void SetPath(Vector3 from, Vector3 to) {
        m_source = from;
        m_destination = to;
        transform.position = m_source;
        enabled = true;
        m_c = m_height + from.y;
        m_a = (m_c * to.y) / to.z*to.z;
        Vector3 diff = to - from;
        diff.y = 0f;
        m_rate = 1f / diff.magnitude;
    }

}
