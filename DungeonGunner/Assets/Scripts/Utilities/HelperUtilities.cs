using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities
{
    public static Camera _mainCamera;

    public static Vector3 GetMouseWorldPosition()
    {
        if(_mainCamera == null)
            _mainCamera = Camera.main;

        Vector3 mouseScreenPosition = Input.mousePosition;

        // ���콺 ��ġ�� ȭ�� ũ�⿡ �°� ������Ű�� �Լ�
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        worldPosition.z = 0f;

        return worldPosition;
    }

    public static float GetAngleFromVector(Vector3 vector)
    {
        // �÷��̾�� ���콺 Ŀ�� ������ ������ ��� -> �ﰢ�Լ� Ȱ��
        // �غ��� x, ���̰� y, ������ angle�� �����ﰢ��
        // tan(angle) = y / x;
        // angle = arc tan(y / x);

        // ����(radian)�̶�?
        // ������ �������� ���̿� ȣ�� ���̰� ���� �Ǵ� ��ŭ�� ���� 1 �����̶�� �Ѵ�.
        float radians = Mathf.Atan2(vector.y, vector.x);

        // Rad2Deg : Radians-to-Degrees(360 / (PI * 2))
        float degrees = radians * Mathf.Rad2Deg;

        return degrees;
    }

    public static AimDirection GetAimDirection(float angleDegrees)
    {
        AimDirection aimDirection;

        if(angleDegrees >= 22f && angleDegrees <= 67f)
        {
            aimDirection = AimDirection.UpRight;
        }
        else if(angleDegrees > 67f && angleDegrees <= 112f)
        {
            aimDirection = AimDirection.Up;
        }
        else if(angleDegrees > 112f && angleDegrees <= 158f)
        {
            aimDirection = AimDirection.UpLeft;
        }
        else if((angleDegrees <= 180f && angleDegrees > 158f) || (angleDegrees > -180f && angleDegrees <= -135f))
        {
            aimDirection = AimDirection.Left;
        }
        else if(angleDegrees > -135f && angleDegrees <= -45f)
        {
            aimDirection = AimDirection.Down;
        }
        else if((angleDegrees > -45f && angleDegrees <= 0f) || (angleDegrees > 0f && angleDegrees < 22f))
        {
            aimDirection = AimDirection.Right;
        }
        else
        {
            aimDirection = AimDirection.Right;
        }

        return aimDirection;
    }

    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if(stringToCheck == "")
        {
            Debug.Log($"{fieldName}�� ������ ���Ե��ְ� {thisObject.name}�� ���� �����ؾ� �մϴ�.");
            return true;
        }

        return false;
    }

    public static bool ValidateCheckNullValue(Object thisObject, string fieldName, Object objectToCheck)
    {
        if(objectToCheck == null)
        {
            Debug.Log($"{fieldName} : {thisObject}���� Null�Դϴ�.");
            return true;
        }

        return false;
    }

    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if(enumerableObjectToCheck == null)
        {
            Debug.Log($"{fieldName} is null in object {thisObject.name}");
            return true;
        }

        foreach(var item in enumerableObjectToCheck)
        {
            if(item == null)
            {
                Debug.Log($"{fieldName}�� {thisObject.name}���� null�Դϴ�.");
                error = true;
            }
            else
            {
                count++;
            }
        }

        if(count == 0)
        {
            Debug.Log($"{fieldName}�� {thisObject.name}���� ���� �����ϴ�.");
            error = true;
        }

        return error;
    }

    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, int valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if(valueToCheck < 0)
            {
                Debug.Log($"{fieldName} : {thisObject}�� �ִ� �� ���� �ݵ�� 0 �̻��� ���� ������ �մϴ�.");
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log($"{fieldName} : {thisObject}�� �ִ� �� ���� �ݵ�� ����� ���� ������ �մϴ�.");
                error = true;
            }
        }

        return error;
    }

    public static Vector3 GetSpawnPositionNearestToPlayer(Vector3 playerPosition)
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom._instantiatedRoom._grid;

        Vector3 nearestSpawnPosition = new Vector3(10000f, 10000f, 0);

        foreach(Vector2Int spawnPositionGrid in currentRoom._spawnPositionArray)
        {
            Vector3 spawnPositionWorld = grid.CellToWorld((Vector3Int)spawnPositionGrid);

            if(Vector3.Distance(spawnPositionWorld, playerPosition) < Vector3.Distance(nearestSpawnPosition, playerPosition))
            {
                nearestSpawnPosition = spawnPositionWorld;
            }
        }

        return nearestSpawnPosition;
    }
}
