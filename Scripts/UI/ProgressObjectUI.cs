using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressObjectUI : MonoBehaviour
{
    [SerializeField]
    private ObjectProgressComponent _progressComponent;
    private RectTransform _rectTransform;

    public void SetUpComponent(GameObject gameobject, bool isLocal, bool isBoss)
    {
        if (isLocal)
        {
            gameObject.GetComponent<Image>().color = Color.green;
        }
        else if (isBoss)
        {
            gameObject.GetComponent<Image>().color = Color.black;
        }

        _rectTransform = GetComponent<RectTransform>();
        _progressComponent = gameobject.GetComponent<ObjectProgressComponent>();
        _rectTransform.anchoredPosition = new Vector2(-195, _rectTransform.anchoredPosition.y); // Y는 변경하지 않음
    }

    public void UpdateUI()
    {
        if (_progressComponent != null)
        {
            float progress = _progressComponent._progressPoint;
            float positionX = Mathf.Lerp(-195f, 195f, progress);
            _rectTransform.anchoredPosition = new Vector2(positionX, _rectTransform.anchoredPosition.y); // Y는 변경하지 않음
        }
    }

    public bool IsObjectDestroyed()
    {
        return _progressComponent == null;
    }
}
