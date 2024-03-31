using Google.Protobuf.Protocol;
using System.Net;

public class UserManager
{
    #region UserInfo Info

    UserInfo _userInfo = new UserInfo();
    public UserInfo Info
    {
        get { return _userInfo; }
        set
        {
            if (_userInfo.Equals(value))
                return;

            Id = value.Id;
            Name = value.Name;
            PublicIp = value.PublicIp;
            PrivateIp = value.PrivateIp;
            Port = value.Port;
            WeaponType = value.WeaponType;
        }
    }

    public int Id
    {
        get { return Info.Id; }
        set { Info.Id = value; }
    }

    public string Name
    {
        get { return Info.Name; }
        set { Info.Name = value; }
    }

    public string PublicIp
    {
        get { return Info.PublicIp; }
        set { Info.PublicIp = value; }
    }

    public string PrivateIp
    {
        get { return Info.PrivateIp; }
        set { Info.PrivateIp = value; }
    }

    public int Port
    {
        get { return Info.Port; }
        set { Info.Port = value; }
    }

    public WeaponType WeaponType
    {
        get { return Info.WeaponType; }
        set { Info.WeaponType = value; }
    }

    #endregion

    public void Init()
    {
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        PrivateIp = ipHost.AddressList[1].ToString();
    }
}
