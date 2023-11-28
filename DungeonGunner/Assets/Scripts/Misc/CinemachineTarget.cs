using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    [SerializeField] Transform _cursorTarget;

    CinemachineTargetGroup _cinemachineTargetGroup;

    private void Awake()
    {
        TryGetComponent(out _cinemachineTargetGroup);
    }

    private void Start()
    {
        SetCinemachineTargetGroup();
    }

    void SetCinemachineTargetGroup()
    {
        CinemachineTargetGroup.Target cinemachineTarget_Player = new CinemachineTargetGroup.Target
        {
            weight = 1f,
            radius = 10f,
            target = GameManager.Instance.GetPlayer().transform
        };

        CinemachineTargetGroup.Target cinemachineTarget_Cursor = new CinemachineTargetGroup.Target
        {
            weight = 1f,
            radius = 5f,
            target = _cursorTarget
        };

        CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[] { cinemachineTarget_Player, cinemachineTarget_Cursor };

        _cinemachineTargetGroup.m_Targets = cinemachineTargetArray;
    }

    private void Update()
    {
        _cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
    }
}
