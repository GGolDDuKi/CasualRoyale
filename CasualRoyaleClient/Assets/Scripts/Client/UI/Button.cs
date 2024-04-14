using UnityEngine;

public class Button : MonoBehaviour
{
    public virtual void OnClick()
    {
        Managers.Sound.Play("Sounds/UI/Button");
    }
}
