using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField]
    private int baseValue;

    private Dictionary<int, int> modifiers = new Dictionary<int, int>(); 

    public Stat(float baseValue)
    {
        this.baseValue = Mathf.FloorToInt(baseValue);
    }
    public int GetValue()
    {
        int finalValue = baseValue;
        foreach (var modifier in modifiers.Values)
        {
            finalValue += modifier;
        }
        return finalValue;
    }

    public void AddModifier(int genderIndex, int modifier)
    {
        if (modifier != 0)
        {
            if (modifiers.ContainsKey(genderIndex))
            {
                modifiers[genderIndex] += modifier;
            }
            else
            {
                modifiers.Add(genderIndex, modifier);
            }
        }
    }
    public void RemoveModifier(int genderIndex, int modifier)
    {
        if (modifier != 0 && modifiers.ContainsKey(genderIndex))
        {
            modifiers[genderIndex] -= modifier;
            if (modifiers[genderIndex] == 0)
            {
                modifiers.Remove(genderIndex);
            }
        }
    }
}
