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

    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

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
}
