using System;
using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private float openDuration;
    [SerializeField] private Constants.ESceneType sceneType;
    
    private void OnTriggerEnter(Collider other)
    {
        if (door)
        {
            // 문 열기
            if (other.CompareTag("Player"))
            {
                StartCoroutine(OpenDoor());
            }            
        }
        else
        {
            GameManager.Instance.LoadScene(sceneType);
        }
    }

    private IEnumerator OpenDoor()
    {
        float duration = openDuration;
        float distance = 3f;
        Vector3 startPosition = door.transform.position;
        Vector3 endPosition = startPosition + Vector3.up * distance;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            door.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }
        door.transform.position = endPosition;
        
        GameManager.Instance.LoadScene(sceneType);
    }
}
