using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    int _startingHealth;
    int _currentHealth;

    public void SetStartingHealth(int startingHealth)
    {
        _startingHealth = startingHealth;
        _currentHealth = startingHealth;
    }

    public int GetStartingHealth()
    {
        return _startingHealth;
    }
}
