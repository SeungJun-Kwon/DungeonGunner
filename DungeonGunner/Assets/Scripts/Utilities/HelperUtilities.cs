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

    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

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
}
