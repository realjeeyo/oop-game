using UnityEngine;

public interface IAbilityUser
{
    void UseAbility(); // Interface for ability use
}

public abstract class BasePlayerClass : MonoBehaviour, IAbilityUser
{
    public abstract void UseAbility();
}
