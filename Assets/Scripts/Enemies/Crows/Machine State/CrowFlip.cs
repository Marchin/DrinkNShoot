using UnityEngine;

public class CrowFlip : MonoBehaviour, IState {

    [SerializeField] float m_rotationSpeed;
    Quaternion m_targetRotation;
    float m_negligible = 0.01f;

    private void OnEnable() {
        m_targetRotation = Quaternion.LookRotation(-transform.forward, transform.up);
    }

    public void StateUpdate(out IState nextState) {
        if (Vector3.Distance(transform.eulerAngles, m_targetRotation.eulerAngles) > m_negligible) {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, m_targetRotation, m_rotationSpeed * Time.deltaTime);
        } else {
            transform.rotation = m_targetRotation;
        }
        if (transform.eulerAngles == m_targetRotation.eulerAngles) {
            nextState = GetComponent<CrowMovement>();
        } else {
            nextState = this;
        }
    }

    public void StateFixedUpdate() { }

}