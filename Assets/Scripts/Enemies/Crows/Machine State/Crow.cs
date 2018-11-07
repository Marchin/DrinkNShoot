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
    CrowSpawner m_crowSpawner;
    BoxCollider m_collider;
    Vector3 m_playerPos;
    AudioSource m_screamSound;
    IState m_currState;
    IState m_nextState;
    int m_currLZ;
    bool m_hasToPoop;
    bool m_isChasing;

    public UnityEvent OnLand { get { return onLand; } }
    public UnityEvent OnFly { get { return onFly; } }

    private void Awake() {
        m_hasToPoop = false;
        m_isChasing = false;
        m_collider = GetComponent<BoxCollider>();
        m_screamSound = GetComponent<AudioSource>();
        m_crowSpawner = FindObjectOfType<CrowSpawner>();
        m_currLZ = -1;
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
            m_crowSpawner.FreeLZ(ref m_currLZ);
            m_hasToPoop = false;
        } else if (m_isChasing) {
            
        }
        if (m_nextState != m_currState) {
            if ((Object)m_nextState == GetComponent<CrowMovement>()) {
                onLand.Invoke();
            } else if ((Object)m_nextState == GetComponent<CrowLand>()) {
                onFly.Invoke();
            }
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

    public Vector3 GetLandingZone(out Vector3 direction) {
        BoxCollider landingZone = m_crowSpawner.PickOneOfLessOccupiedLZ(out m_currLZ);
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

    public void Die() {
        m_crowSpawner.FreeLZ(ref m_currLZ);
        gameObject.SetActive(false);
    }

    public void Chase(Vector3 target) {
        if (m_currState as Object == GetComponent<CrowMovement>()) {
            m_isChasing = true;
            if (Vector3.Angle(transform.forward, target - transform.position) > 90f) {
                m_nextState = GetComponent<CrowFlip>();
            }
        }
    }
}