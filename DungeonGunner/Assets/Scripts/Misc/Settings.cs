using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public const float _pixelsPerUnit = 16f;
    public const float _tileSizePixels = 16f;

    public const int _maxDungeonRebuildAttempsForRoomGraph = 1000;
    public const int _maxDungeonBuildAttemps = 10;

    #region ROOM SETTING

    // 최대 자식 복도의 개수
    public const int _maxChildCorridors = 3;

    #endregion

    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");

    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");

    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollDown = Animator.StringToHash("rollDown");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static int rollRight = Animator.StringToHash("rollRight");
}
