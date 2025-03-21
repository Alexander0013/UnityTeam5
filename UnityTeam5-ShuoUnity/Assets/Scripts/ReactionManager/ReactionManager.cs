using UnityEngine;

public class ReactionManager : MonoBehaviour
{
    public static ReactionManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Subscribe to reaction events from enemies (or players).
    public void RegisterElementalStatus(ElementalStatus status)
    {
        status.OnCrystallizeReaction += HandleCrystallize;
    }

    private void HandleCrystallize(ElementType reactant, Vector3 position)
    {
        //Debug.Log($"ReactionManager received Crystallize reaction for {reactant} at {position}");
        // Here you could spawn additional VFX, apply shield logic, etc.
    }
}
