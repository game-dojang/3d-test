using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject playerPrefab;
    
    public Canvas Canvas => GetCanvas();
    
    private bool _isCursorLock;
    
    private GameObject _player;
    
    public void SetCursorLock()
    {
        Cursor.visible = _isCursorLock;
        Cursor.lockState = _isCursorLock ? CursorLockMode.None : CursorLockMode.Locked;
        _isCursorLock = !_isCursorLock;
    }

    public void LoadScene(Constants.ESceneType sceneType)
    {
        StartCoroutine(LoadSceneAsync(sceneType));
    }

    private IEnumerator LoadSceneAsync(Constants.ESceneType sceneType)
    {
        // 로딩 화면 띄우기
        var loadingPanelPrefab = Resources.Load<GameObject>("Loading Panel");
        var loadingPanelObject = Instantiate(loadingPanelPrefab, Canvas.transform);
        var loadingPanelController = loadingPanelObject.GetComponent<LoadingPanelController>();
        
        // 로딩 창 표시
        bool showDone = false;
        loadingPanelController.Show(()=> showDone = true);
        yield return new WaitUntil(() => showDone);
        
        // 씬 로드 진행
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneType.ToString());
        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f)
        {
            loadingPanelController.SetProgress(asyncOperation.progress);
            yield return null;
        }
        loadingPanelController.SetProgress(1f);
        asyncOperation.allowSceneActivation = true;
        
        // // 로딩 창 숨기기
        // bool hideDone = false;
        // loadingPanelController.Hide(()=> hideDone = true);
        // yield return new WaitUntil(() => hideDone);
        //
        // // 로딩 패널 오브젝트 제거
        // Destroy(loadingPanelObject);
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
        switch (scene.name)
        {
            case "Main":
                if (_player)
                {
                    Destroy(_player);
                    _player = null;
                }
                break;
            case "Character":
            case "Map":
                var spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
                
                if (_player)
                {
                    _player.transform.position = spawnPoint.position;
                    _player.transform.rotation = spawnPoint.rotation;
                    _player.SetActive(true);
                }
                else
                {
                    _player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
                    DontDestroyOnLoad(_player);
                }
                break;
        }
    }

    protected override void OnSceneUnloaded(Scene scene)
    {
        if (_player) _player.SetActive(false);
    }
}
