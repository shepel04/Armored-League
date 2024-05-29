using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasElementAnimation : MonoBehaviour
{
    public GameObject[] HUDObjects;
    public Transform StartPosition;
    private Vector3 _startPos; 

    private bool _isMoving = false; 

    void Start()
    {
        _startPos = StartPosition.transform.position;
    }

    void Update()
    {
        if (transform.position != _startPos && !_isMoving)
        {
            _isMoving = true;
            AnimateHUD(false); 
        }
        else if (transform.position == _startPos && _isMoving)
        {
            _isMoving = false;
            AnimateHUD(true); 
        }
    }

    void AnimateHUD(bool enlarge)
    {
        foreach (GameObject obj in HUDObjects)
        {
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                if (enlarge)
                {
                    LeanTween.scale(rectTransform, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutQuad);
                }
                else
                {
                    LeanTween.scale(rectTransform, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInQuad);
                }
            }
        }
    }
}
