using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Emotion : FollowingUI
{
    private void Update()
    {
        FollowObject(2.5f);
    }

    public void SetEmotion(string emotion)
    {
        GetComponent<Image>().sprite = Managers.Resource.Load<Sprite>($"Sprites/Emotions/{emotion}");
    }
}
