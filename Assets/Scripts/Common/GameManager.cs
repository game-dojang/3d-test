using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public Canvas Canvas => GetCanvas();
    
    private bool _isCursorLock;
    
    public void SetCursorLock()
    {
        Cursor.visible = _isCursorLock;
        Cursor.lockState = _isCursorLock ? CursorLockMode.None : CursorLockMode.Locked;
        _isCursorLock = !_isCursorLock;
    }

    private Canvas GetCanvas()
    {
        var canvasObject = GameObject.FindGameObjectWithTag("Canvas");
        Canvas result = null;

        if (!canvasObject)
        {
            canvasObject = new GameObject("Canvas");
            canvasObject.AddComponent<Canvas>();
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            
            result = canvasObject.GetComponent<Canvas>();
            result.renderMode = RenderMode.ScreenSpaceOverlay;
            result.tag = "Canvas";
        }
        else
        {
            result = canvasObject.GetComponent<Canvas>();
        }
        return result;
    }
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
}
