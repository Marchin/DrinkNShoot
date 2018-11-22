using UnityEngine;

public class Bait : MonoBehaviour {
    [SerializeField] LayerMask m_crowsLayer;
    [SerializeField] float m_speed = 2f;
    [SerializeField] float m_radius = 2f;
    ParticleSystem m_particleSys;
    Vector3 m_source;
    Vector3 m_destination;
    float m_rate; // speed/distance
    float m_accu;

    private void Awake() {
        m_particleSys = GetComponentInChildren<ParticleSystem>();
        m_accu = 0f;
        enabled = false;
    }
    
    private void Update() {
        transform.position  = Vector3.Lerp(m_source, 
            m_destination, m_accu);
        m_accu += m_rate * Time.deltaTime;
        if (m_accu >= 1f) {
            m_particleSys.Play();
            Collider[] crows = Physics.OverlapSphere(transform.position, m_radius, m_crowsLayer);
            foreach (Collider crow in crows) {
                crow.GetComponent<Crow>().Chase(transform.position);
            }
            enabled = false;
        }
    }

    public void SetPath(Vector3 from, Vector3 to) {
        m_source = from;
        m_destination = to;
        transform.position = m_source;
        Vector3 diff = to - from;
        diff.y = 0f;
        m_rate = 2f * m_speed / diff.magnitude;
        enabled = true;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, m_radius);
        Gizmos.DrawWireSphere(m_destination, 1f);
    }

}
