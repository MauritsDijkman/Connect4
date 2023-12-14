using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class S_MainMenuManager : MonoBehaviour
{
    [Header("Unity Event")]
    [SerializeField] private UnityEvent init = null;

    private void Awake()
    {
        init?.Invoke();
    }

    public void LoadLevel(string pLevelName)
    {
        SceneManager.LoadSceneAsync(pLevelName);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}