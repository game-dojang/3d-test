using UnityEngine;

public static class CharacterUtility
{
    /// <summary>
    /// 특정 Position에서 layerMask까지의 거리를 반환하는 메서드(함수)
    /// </summary>
    /// <param name="position">시작 위치</param>
    /// <param name="layerMask">대상 오브젝트의 LayerMask</param>
    /// <param name="maxDistance">최대 거리</param>
    /// <returns>시작 위치에서 부터 대상 오브젝트 사이의 거리 반환</returns>
    public static float GetDistanceToGround(Vector3 position, LayerMask layerMask, float maxDistance)
    {
        if (Physics.Raycast(position, Vector3.down, out RaycastHit hit,
                maxDistance, layerMask))
        {
            return hit.distance;
        }
        else
        {
            return maxDistance;
        }
    }
}