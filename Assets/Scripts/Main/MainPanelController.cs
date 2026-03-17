using UnityEngine;

public class MainPanelController : MonoBehaviour
{
    public void OnClickStartButton()
    {
        GameManager.Instance.LoadScene(Constants.ESceneType.Character);
    }
}
