using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum JoystickType
    {
        Move,
        Aim
    }

    public JoystickType _joystickType;
    [SerializeField] private RectTransform _handle;
    private RectTransform _rect;

    [SerializeField, Range(100f, 150f)]
    private float _handleRange;

    public Vector2 _handleDir;

    private void Start()
    {
        Init();
    }

    void Init()
    {
        _rect = GetComponent<RectTransform>();
        _handle = transform.GetChild(0).GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        JoystickHandle(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        JoystickHandle(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _handleDir = Vector2.zero;
        _handle.anchoredPosition = Vector2.zero;
    }

    void JoystickHandle(PointerEventData eventData)
    {
        Vector2 inputDir;
        Vector2 handleDir;

        if (_rect.anchoredPosition.x < 0)
        {
            Vector2 rectPosition = new Vector2(Screen.width + _rect.anchoredPosition.x, _rect.anchoredPosition.y);
            inputDir = eventData.position - rectPosition;
            handleDir = inputDir.magnitude < _handleRange ? inputDir : inputDir.normalized * _handleRange;
        }
        else
        {
            inputDir = eventData.position - _rect.anchoredPosition;
            handleDir = inputDir.magnitude < _handleRange ? inputDir : inputDir.normalized * _handleRange;
        }
        
        _handle.anchoredPosition = handleDir;
        _handleDir = (handleDir / _handleRange).normalized;
    }
}
