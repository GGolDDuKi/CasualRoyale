using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmotionButton : Button
{
    public override void OnClick()
    {
        base.OnClick();

        Emotion emotion = Managers.UI.GenerateEmotion(Managers.Object.MyPlayer);
        emotion.SetEmotion(GetComponent<Image>().sprite.name);
        emotion.GetComponent<MonoBehaviour>().StartCoroutine(Managers.UI.CoFadeIn(emotion.gameObject, () => { Managers.Resource.Destroy(emotion.gameObject); }));

        C_Emote emotePacket = new C_Emote();
        emotePacket.EmoteName = GetComponent<Image>().sprite.name;
        Managers.Network.Send(emotePacket);
        transform.parent.parent.gameObject.SetActive(false);
    }
}
