using Unity.VisualScripting;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public static AnimationHandler Instance;
    [SerializeField] Animator CounterSlotAnimator;
    void Awake()
    {
        Instance = this;
    }

    public void AnimationShakeCounterSlot() => CounterSlotAnimator?.SetTrigger("Shake");

}
