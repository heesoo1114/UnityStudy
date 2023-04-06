using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private CommonAIState _currentState;

    private Transform _targetTrm;
    public Transform TargetTrm => _targetTrm;

    private NavAgentMovement _navMovement;
    public NavAgentMovement NavMovement => _navMovement;

    protected virtual void Awake()
    {
        List<CommonAIState> states = new List<CommonAIState>();
        transform.Find("AI").GetComponentsInChildren<CommonAIState>(states);

        states.ForEach(s => s.SetUp(transform));

        _navMovement = GetComponent<NavAgentMovement>();
    }

    protected virtual void Start()
    {
        _targetTrm = GameManager.Instance.PlayerTrm;
        // ���߿� ���� ������ ���Ǿ�� ���氡��
    }

    public void ChangeState(CommonAIState nextState)
    {
        // ������Ʈ ����
        _currentState?.OnExitState();
        _currentState = nextState;
        _currentState?.OnEnterState();
    }

    private void Update()
    {
        _currentState?.UpdateState();
    }
}