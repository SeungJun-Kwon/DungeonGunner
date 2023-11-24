using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities
{
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
}
