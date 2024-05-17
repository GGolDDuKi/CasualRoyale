

using UnityEngine;
using UnityEngine.UI;

public class SettingManager
{
    #region Graphic

    private int _width;
    public int Width {
        get { return _width; }
        set
        {
            if (_height == value)
                return;

            _width = value;
            SetScreen();
        }
    }

    private int _height;
    public int Height { 
        get { return _height; }
        set
        {
            if (_height == value)
                return;

            _height = value;
            SetScreen();
        }
    }

    #endregion

    #region Sound

    float bgmVol;
    public float BGMVol
    {
        get
        {
            return bgmVol;
        }
        set
        {
            bgmVol = value;
            Managers.Sound.SetVolume(Define.Sound.Bgm);
        }
    }

    float sfxVol;
    public float SFXVol
    {
        get
        {
            return sfxVol;
        }
        set
        {
            sfxVol = value;
            Managers.Sound.SetVolume(Define.Sound.Effect);
        }
    }

    #endregion

    public void Init()
    {
        Width = 640;
        Height = 360;
        BGMVol = 1f;
        SFXVol = 1f;
    }

    public void SetScreen()
    {
        Screen.SetResolution(Managers.Setting.Width, Managers.Setting.Height, false);
        GameObject.Find("Canvas").GetComponent<CanvasScaler>().referenceResolution = new Vector2(Managers.Setting.Width, Managers.Setting.Height);
    }
}
