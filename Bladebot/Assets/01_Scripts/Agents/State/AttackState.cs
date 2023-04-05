using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using Unity.VisualScripting;

public class AttackState : CommonState
{
    public event Action<int> OnAttackStart = null; // ���ݽ� �޺����� �˷��ִ� �̺�Ʈ
    public event Action OnAttackstateEnd = null; // ���ݻ��¸� ���� �� �����ϴ� �̺�Ʈ

    [SerializeField]
    private float _keyDelay = 0.5f; // 0.5�� ���� ���콺�� �� �� ������� ����

    private int _currentCombo = 0; // ���� �޺� ��ġ
    private bool _canAttack = true; // ���� ������ ��������
    private float _keyTimer = 0;

    private float _attackStartTime; // ������ ���۵� �ð�
    [SerializeField]
    private float _attackSlideDuration = 0.2f, _attackSlideSpeed = 0.1f;

    public override void OnEnterState()
    {
        _agentInput.OnAttackKeyPress += OnAttackHandle;
        _agentAnimator.OnAnimationEndTrigger += OnAnimationHandle;
        _agentInput.OnRollingKeyPress += OnRollingHandle;
        _currentCombo = 0;
        _canAttack = true;
        _agentAnimator.SetAttackState(true); // ���� ���·� ��ȯ
        OnAttackHandle(); // �������� ���� ����
    }

    public override void OnExitState()
    {
        _agentInput.OnAttackKeyPress -= OnAttackHandle;
        _agentAnimator.OnAnimationEndTrigger -= OnAnimationHandle;
        _agentInput.OnRollingKeyPress -= OnRollingHandle;
        _agentAnimator.SetAttackState(false); // ���ݻ��·� ��ȯ
        _agentAnimator.SetAttackTrigger(false);
        OnAttackstateEnd?.Invoke();
    }

    private void OnRollingHandle()
    {
        _agentController.ChangeState(StateType.Rolling);
    }

    private void OnAnimationHandle()
    {
        _canAttack = true;
        _keyTimer = _keyDelay; // 0.5�� �ð� �־��ְ� �������� ī��Ʈ ����
    }

    private void OnAttackHandle()
    {
        if (_canAttack && _currentCombo < 3)
        {
            _attackStartTime = Time.time; // ���� ���� �Ⱓ�� ���
            _canAttack = false;
            _agentAnimator.SetAttackTrigger(true);
            _currentCombo++;
            OnAttackStart?.Invoke(_currentCombo);
        }
    }

    public override void UpdateState()
    {
        if (_canAttack && _keyTimer > 0)
        {
            _keyTimer -= Time.deltaTime;
            if (_keyTimer <= 0)
            {
                _agentController.ChangeState(StateType.Normal);
            }
        }

        // �����̵��� �Ǿ�� �ϴ� �ð��̶�� ��
        if (Time.time < _attackStartTime + _attackSlideDuration)
        {
            float timePassed = Time.time - _attackStartTime; // ���� �������� �� ��
            float lerpTime = timePassed / _attackSlideDuration; // 0 ~ 1 ������ ��ȯ

            _agentMovement.SetMovementVelocity(Vector3.Lerp(_agentMovement.transform.forward * _attackSlideSpeed, Vector3.zero, lerpTime));
        }
    }
}