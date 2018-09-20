using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CrowLand))]
[RequireComponent(typeof(CrowMovement))]
public class Crow : MonoBehaviour {
    Collider[] m_landingZones;
    Collider m_collider;
    IState m_currState;
    IState m_nextState;

    private void Awake() {
        m_collider = GetComponent<Collider>();
    }

    private void OnEnable() {
        SetStateActive(GetComponent<CrowMovement>(), false);
        SetStateActive(GetComponent<CrowLand>(), false);
    }

    public void Init() {
        m_currState = GetComponent<CrowLand>();
        SetStateActive(m_currState, true);
    }

    private void Update() {
        if (m_currState != null) {
            m_currState.StateUpdate(out m_nextState);
        }
        if (m_nextState != m_currState) {
            SetStateActive(m_currState, false);
            SetStateActive(m_nextState, true);
            m_currState = m_nextState;
        }
    }

    private void FixedUpdate() {
        m_currState.StateFixedUpdate();
    }

    void SetStateActive(IState state, bool active) {
        (state as MonoBehaviour).enabled = active;
    }

    public void SetLandingZones(Collider[] landingZones) {
        m_landingZones = landingZones;
    }

    public Vector3 GetLandingZone() {
        Collider landingZone = m_landingZones[Random.Range(0, m_landingZones.Length)];
        float offSetX = landingZone.bounds.extents.x - m_collider.bounds.size.x;
        float offSetZ = landingZone.bounds.extents.z - m_collider.bounds.size.z;
        if (offSetX < 0f) offSetX = 0f;
        if (offSetZ < 0f) offSetZ = 0f;
        Vector3 offSet = new Vector3(
            Random.Range(-offSetX, offSetX),
            0f,
            Random.Range(-offSetZ, offSetZ)
        );
        return (landingZone.bounds.center + offSet);
    }
}