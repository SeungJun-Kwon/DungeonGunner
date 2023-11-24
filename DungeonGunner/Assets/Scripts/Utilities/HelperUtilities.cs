using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities
{
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if(stringToCheck == "")
        {
            Debug.Log($"{fieldName}에 공백이 포함돼있고 {thisObject.name}에 값을 포함해야 합니다.");
            return true;
        }

        return false;
    }

    public static bool ValidateCheckNullValue(Object thisObject, string fieldName, Object objectToCheck)
    {
        if(objectToCheck == null)
        {
            Debug.Log($"{fieldName} : {thisObject}에서 Null입니다.");
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
                Debug.Log($"{fieldName}은 {thisObject.name}에서 null입니다.");
                error = true;
            }
            else
            {
                count++;
            }
        }

        if(count == 0)
        {
            Debug.Log($"{fieldName}은 {thisObject.name}에서 값이 없습니다.");
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
                Debug.Log($"{fieldName} : {thisObject}에 있는 이 값은 반드시 0 이상의 값을 가져야 합니다.");
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log($"{fieldName} : {thisObject}에 있는 이 값은 반드시 양수의 값을 가져야 합니다.");
                error = true;
            }
        }

        return error;
    }
}
