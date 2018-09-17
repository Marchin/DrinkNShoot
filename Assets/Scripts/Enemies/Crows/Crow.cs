using UnityEngine;

[RequireComponent(typeof(CrowLand))]
[RequireComponent(typeof(CrowMovement))]
public class Crow : MonoBehaviour {
    IState m_currState;
    IState m_nextState;

    private void Awake() {
        SetStateActive(GetComponent<CrowMovement>(), false);
        m_currState = GetComponent<CrowLand>();
        SetStateActive(m_currState, true);
    }

    private void Update() {
        m_currState.StateUpdate(out m_nextState);
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
}