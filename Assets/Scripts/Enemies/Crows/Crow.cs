﻿using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(CrowLand))]
[RequireComponent(typeof(CrowMovement))]
[RequireComponent(typeof(CrowFlip))]
public class Crow : MonoBehaviour {
    [SerializeField] UnityEvent onLand;
    [SerializeField] UnityEvent onFly;
    BoxCollider[] m_landingZones;
    BoxCollider m_collider;
    IState m_currState;
    IState m_nextState;

    private void Awake() {
        m_collider = GetComponent<BoxCollider>();
    }

    private void OnEnable() {
        SetStateActive(GetComponent<CrowMovement>(), false);
        SetStateActive(GetComponent<CrowLand>(), false);
        SetStateActive(GetComponent<CrowFlip>(), false);
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
            if ((Object)m_currState == GetComponent<CrowMovement>())
                onLand.Invoke();
            if ((Object)m_currState == GetComponent<CrowLand>())
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

    public UnityEvent OnLand {
        get {
            return onLand;
        }
    }
    public UnityEvent OnFly {
        get {
            return onFly;
        }
    }
}