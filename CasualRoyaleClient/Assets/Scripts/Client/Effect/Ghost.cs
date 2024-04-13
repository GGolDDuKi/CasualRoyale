using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField] private float _ghostDelay;
    private float _ghostDelaySeconds;
    private GameObject _ghost;
    public bool _makeGhost = false;
    private CreatureController _controller;

    void Start()
    {
        _ghostDelay = 0.1f;
        _ghostDelaySeconds = _ghostDelay;
        _ghost = Resources.Load<GameObject>("Prefabs/Effect/Ghost");
        _controller = GetComponent<CreatureController>();
    }

    void Update()
    {
        if (_makeGhost)
        {
            if (_ghostDelaySeconds > 0)
            {
                _ghostDelaySeconds -= Time.deltaTime;
            }
            else
            {
                GameObject currentGhost = Managers.Resource.Instantiate(_ghost, transform.position, transform.rotation);
                currentGhost.GetComponent<SpriteRenderer>().sprite = _controller._sprite.sprite;
                currentGhost.GetComponent<SpriteRenderer>().sortingOrder = _controller._sprite.sortingOrder - 1;
                currentGhost.transform.localScale = transform.localScale;
                _ghostDelaySeconds = _ghostDelay;
                Managers.Resource.Destroy(currentGhost, 1f);
            }
        }
    }
}
