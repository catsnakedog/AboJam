using System.Collections.Generic;
using System.Drawing.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Handle : MonoBehaviour
{
    private GameObject circuitObj; public GameObject CircuitObj { get => circuitObj; }
    private Image circuitImg;
    private RectTransform circuitRect;

    private GameObject handleObj; public GameObject HandleObj { get => handleObj; }
    private Image handleImg;
    private RectTransform handleRect;

    /* Control Flow */

    public void Init(Transform parent, Vector2 pos, Vector2 circuitSize, Vector2 handleSize, Sprite circuitSprite, Sprite handleSprite)
    {
        // 생성 및 부모 지정
        circuitObj = new GameObject("Circuit");
        handleObj = new GameObject("Handle");
        if (parent != null)
        {
            circuitObj.transform.SetParent(parent);
            handleObj.transform.SetParent(parent);
        }

        // 렉트 초기화
        circuitRect = circuitObj.AddComponent<RectTransform>();
        handleRect = handleObj.AddComponent<RectTransform>();
        circuitRect.transform.position = pos;
        handleRect.transform.position = pos;
        circuitRect.sizeDelta = circuitSize;
        handleRect.sizeDelta = handleSize;

        // 이미지 초기화
        circuitImg = circuitObj.AddComponent<Image>();
        handleImg = handleObj.AddComponent<Image>();
        circuitImg.raycastTarget = false;
        handleImg.raycastTarget = false;
        circuitImg.sprite = circuitSprite;
        handleImg.sprite = handleSprite;
    }

    public void Close()
    {
        GameObject destroyerObj = new GameObject("Handle Destroyer");
        DestroyerComponent destroyerComp = destroyerObj.AddComponent<DestroyerComponent>();
        destroyerComp.Init(circuitObj, handleObj);
    }

    /// <summary>
    /// Close 용 이너 클래스입니다.
    /// Destroy 를 위임하지 않고 직접 MonoBehaviour 를 상속받으면, 가비지가 축적됩니다.....
    /// </summary>
    private class DestroyerComponent : MonoBehaviour
    {
        private GameObject target1;
        private GameObject target2;

        public void Init(GameObject obj1, GameObject obj2)
        {
            target1 = obj1;
            target2 = obj2;
        }

        // Start에서 목표들을 파괴하고 자신도 파괴
        void Start()
        {
            Destroy(target1);
            Destroy(target2);
            Destroy(this.gameObject); // 자기 자신도 파괴
        }
    }

    /* Public Method */

    public void MoveHandle(Vector2 pos)
    {
        if(IsInCircuitPos(pos))
        {
            handleObj.transform.position = pos;
        }
        else
        {
            Vector2 direction = (pos - GetCircuitPos()).normalized;
            handleObj.transform.position = GetCircuitPos() + direction * GetRadius();
        }

        bool IsInCircuitPos(Vector2 pos)
        {
            return Vector2.Distance(GetCircuitPos(), pos) <= GetRadius();
        }
    }

    /* Private Method */

    private Vector2 GetCircuitPos()
    {
        return circuitRect.transform.position;
    }

    private float GetRadius()
    {
        return circuitRect.sizeDelta.x / 2;
    }
}
