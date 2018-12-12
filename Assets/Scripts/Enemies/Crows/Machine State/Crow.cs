using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(CrowLand))]
[RequireComponent(typeof(CrowMovement))]
[RequireComponent(typeof(CrowFlip))]
[RequireComponent(typeof(CrowFly))]

public class Crow : MonoBehaviour {
	public LayerMask ObstaclesLayer;
    CrowSpawner m_crowSpawner;
    BoxCollider m_collider;
    BoxCollider m_LZCollider;
    Transform m_playerTranform;
    Vector3 m_playerToStage;
    IState m_currState;
    IState m_nextState;
    int m_currLZ;
    bool m_hasToPoop;
    bool m_hasToFlip;

    UnityEvent onLand = new UnityEvent();
    UnityEvent onFly = new UnityEvent();
    UnityEvent onAttack = new UnityEvent();

    public UnityEvent OnLand { get { return onLand; } }
    public UnityEvent OnFly { get { return onFly; } }
    public UnityEvent OnAttack { get { return onAttack; } }
    public Vector3 PlayerToStage { get { return m_playerToStage; } }

    private void Awake() {
        m_hasToPoop = false;
        m_hasToFlip = false;
        m_collider = GetComponent<BoxCollider>();
        m_crowSpawner = FindObjectOfType<CrowSpawner>();
        m_currLZ = -1;
    }

    private void OnEnable() {
        m_playerTranform = FindObjectOfType<Wagon>().transform;
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
            m_hasToFlip = false;
        } else if (m_hasToFlip) {
            m_nextState = GetComponent<CrowFlip>();
            m_hasToFlip = false;
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
        m_LZCollider = m_crowSpawner.PickOneOfLessOccupiedLZ(out m_currLZ);
        float offSetX = m_LZCollider.size.x * 0.5f - m_collider.size.x;
        float offSetZ = m_LZCollider.size.z * 0.5f - m_collider.size.z;
        if (offSetX < 0f)offSetX = 0f;
        if (offSetZ < 0f)offSetZ = 0f;
        Vector3 offSet = new Vector3(
            Random.Range(-offSetX, offSetX),
            0f,
            Random.Range(-offSetZ, offSetZ)
        );
        offSet = m_LZCollider.transform.TransformDirection(offSet);
        direction = m_LZCollider.transform.forward;
        return (m_LZCollider.bounds.center + offSet);
    }

    public BoxCollider GetLZCollider() {
        return m_LZCollider;
    }

    public void Poop() {
        GetComponent<CrowFly>().SetDestination(m_playerTranform);
        onAttack.Invoke();
        m_hasToPoop = true;
    }

    public void Die() {
        m_crowSpawner.FreeLZ(ref m_currLZ);
        gameObject.SetActive(false);
    }

    public void Chase(Vector3 target) {
        if (m_currState as Object == GetComponent<CrowMovement>()) {
            if (Vector3.Angle(transform.forward, target - transform.position) > 90f) {
                m_hasToFlip = true;
            }
        }
    }

	public void SetPlayerToStageVector(Vector3 direction) {
		m_playerToStage = direction;
	}

    public IState CurrentState {
        get {
            return m_currState;
        }
    }
}