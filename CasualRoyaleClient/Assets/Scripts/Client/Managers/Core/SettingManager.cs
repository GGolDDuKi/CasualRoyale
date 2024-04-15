

public class SettingManager
{
    #region Graphic

    public int Width { get; set; }
    public int Height { get; set; }

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
        Height = 480;
        BGMVol = 1f;
        SFXVol = 1f;
    }
}
