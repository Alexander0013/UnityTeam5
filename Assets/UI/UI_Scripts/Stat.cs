using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    //¯dµÛ
    [SerializeField]
    private int baseValue_A; // °òÂ¦­È
    [SerializeField]
    private int baseValue_B; // °òÂ¦­È

    private List<int> modifiers_A = new List<int>(); // ­×¹¢­È
    private List<int> modifiers_B = new List<int>(); // ­×¹¢­È

    public int GetValue_A()
    {
        int finalValue_A = baseValue_A;
        modifiers_A.ForEach(x => finalValue_A += x);
        return finalValue_A;
    }
    public int GetValue_B()
    {
        int finalValue_B = baseValue_B;
        modifiers_B.ForEach(x => finalValue_B += x);
        return finalValue_B;
    }

    public void AddModifier(int genderIndex,int modifier)
    {
        if (genderIndex == 0)
        {
            if (modifier != 0)
            {
                modifiers_A.Add(modifier);
            }
        }
        else
        {
            if (modifier != 0)
            {
                modifiers_B.Add(modifier);
            }
        }
    }

    public void RemoveModifier(int genderIndex,int modifier)
    {
        if (genderIndex == 0)
        {
            if (modifier != 0)
            {
                modifiers_A.Remove(modifier);
            }
        }
        else
        {
            if (modifier != 0)
            {
                modifiers_B.Remove(modifier);
            }
        }
    }
}
