using UnityEngine;

[RequireComponent(typeof(CrowLand))]
[RequireComponent(typeof(CrowMovement))]
public class Crow : MonoBehaviour {
    Collider[] m_landingZones;
    IState m_currState;
    IState m_nextState;

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
        Collider landingZone = m_landingZones[Random.Range(0, m_landingZones.Length - 1)];
        Vector3 offSet = new Vector3(
            Random.Range(-landingZone.bounds.extents.x, landingZone.bounds.extents.x),
            0f,
            Random.Range(-landingZone.bounds.extents.z, landingZone.bounds.extents.z)
        );
        return (landingZone.bounds.center + offSet);
    }
}