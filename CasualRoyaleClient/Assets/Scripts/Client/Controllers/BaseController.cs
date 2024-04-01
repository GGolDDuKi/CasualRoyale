using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected int layer;

    protected virtual void Start()
    {
        SetLayer(GetComponent<SpriteRenderer>());
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
