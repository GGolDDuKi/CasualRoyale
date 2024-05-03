

public class DashButton : Button
{
    public override void OnClick()
    {
        if (Managers.Object.MyPlayer.Dash())
            StartCoroutine(transform.GetChild(1).GetComponent<SkillCooldown>().CoCooldown(Managers.Data.ClassData[Managers.Object.MyPlayer.Class.ToString()].DashCooldown));
    }
}
