using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private bool _isCursorLock;
    
    public void SetCursorLock()
    {
        Cursor.visible = _isCursorLock;
        Cursor.lockState = _isCursorLock ? CursorLockMode.None : CursorLockMode.Locked;
        _isCursorLock = !_isCursorLock;
    }
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
}
