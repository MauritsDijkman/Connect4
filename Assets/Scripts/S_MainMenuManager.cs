using UnityEngine;
using UnityEngine.SceneManagement;

public class S_MainMenuManager : MonoBehaviour
{
    public void LoadLevel(string pLevelName)
    {
        SceneManager.LoadSceneAsync(pLevelName);
    }
}