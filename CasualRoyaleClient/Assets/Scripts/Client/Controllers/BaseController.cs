using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected SpriteRenderer _sprite;
    protected int layer;

    void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        _sprite = GetComponent<SpriteRenderer>();
        SetLayer(_sprite);
    }

    protected virtual void Update()
    {
        
    }

    protected void SetLayer(SpriteRenderer sprite)
    {
        YPosToLayer();
        sprite.sortingOrder = layer;
    }

    protected void SetLayer(List<SpriteRenderer> sprites)
    {
        YPosToLayer();

        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.sortingOrder = layer;
        }
    }

    void YPosToLayer()
    {
        layer = -(int)Math.Round(transform.position.y);
    }
}
