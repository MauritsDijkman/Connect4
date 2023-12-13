using UnityEngine;

public class S_ScreenSwitcher : MonoBehaviour
{
    public void EnableCanvasGroup(CanvasGroup pCanvasGroup)
    {
        pCanvasGroup.alpha = 1;
        pCanvasGroup.interactable = true;
        pCanvasGroup.blocksRaycasts = true;
    }

    public void DisableCanvasGroup(CanvasGroup pCanvasGroup)
    {
        pCanvasGroup.alpha = 0;
        pCanvasGroup.interactable = false;
        pCanvasGroup.blocksRaycasts = false;
    }
}