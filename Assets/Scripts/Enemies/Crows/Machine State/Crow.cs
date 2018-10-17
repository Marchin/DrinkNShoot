using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(CrowLand))]
[RequireComponent(typeof(CrowMovement))]
[RequireComponent(typeof(CrowFlip))]
[RequireComponent(typeof(CrowFly))]

public class Crow : MonoBehaviour {
    [SerializeField] UnityEvent onLand;
    [SerializeField] UnityEvent onFly;
    BoxCollider[] m_landingZones;
    BoxCollider m_collider;
    Vector3 m_playerPos;
    AudioSource m_screamSound;
    IState m_currState;
    IState m_nextState;
    bool m_hasToPoop;

    public UnityEvent OnLand { get { return onLand; } }
    public UnityEvent OnFly { get { return onFly; } }

    private void Awake() {
        m_hasToPoop = false;
        m_collider = GetComponent<BoxCollider>();
        m_screamSound = GetComponent<AudioSource>();
    }

    private void OnEnable() {
        m_playerPos = FindObjectOfType<DrunkCamera>().transform.position;
        SetStateActive(GetComponent<CrowMovement>(), false);
        SetStateActive(GetComponent<CrowLand>(), false);
        SetStateActive(GetComponent<CrowFlip>(), false);
        SetStateActive(GetComponent<CrowFly>(), false);
    }

    public void Init() {
        m_currState = GetComponent<CrowLand>();
        SetStateActive(m_currState, true);
    }

    private void Update() {
        if (m_currState != null) {
            m_currState.StateUpdate(out m_nextState);
        }
        if (m_hasToPoop) {
            m_nextState = GetComponent<CrowFly>();
            m_hasToPoop = false;
        }
        if (m_nextState != m_currState) {
            SetStateActive(m_currState, false);
            SetStateActive(m_nextState, true);
            m_currState = m_nextState;
        }
        if ((Object)m_currState == GetComponent<CrowMovement>()) {
            onLand.Invoke();
        } else if ((Object)m_currState == GetComponent<CrowLand>()) {
            onFly.Invoke();
        }
    }

    private void FixedUpdate() {
        m_currState.StateFixedUpdate();
    }

    void SetStateActive(IState state, bool active) {
        (state as MonoBehaviour).enabled = active;
    }

    public void SetLandingZones(BoxCollider[] landingZones) {
        m_landingZones = landingZones;
    }

    public Vector3 GetLandingZone(out Vector3 direction) {
        BoxCollider landingZone = m_landingZones[Random.Range(0, m_landingZones.Length)];
        float offSetX = landingZone.size.x * 0.5f - m_collider.size.x;
        float offSetZ = landingZone.size.z * 0.5f - m_collider.size.z;
        if (offSetX < 0f)offSetX = 0f;
        if (offSetZ < 0f)offSetZ = 0f;
        Vector3 offSet = new Vector3(
            Random.Range(-offSetX, offSetX),
            0f,
            Random.Range(-offSetZ, offSetZ)
        );
        offSet = landingZone.transform.TransformDirection(offSet);
        direction = landingZone.transform.forward;
        return (landingZone.bounds.center + offSet);
    }

    public void Poop() {
        GetComponent<CrowFly>().SetDestination(m_playerPos + Vector3.up * 3f);
        m_screamSound.Play();
        m_hasToPoop = true;
    }
}