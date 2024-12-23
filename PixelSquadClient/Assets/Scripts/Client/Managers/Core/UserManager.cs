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
            Job = value.Job;
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

    public JobType Job
    {
        get { return Info.Job; }
        set { Info.Job = value; }
    }

    #endregion
}
