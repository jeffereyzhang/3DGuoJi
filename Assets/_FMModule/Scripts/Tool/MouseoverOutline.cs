using UnityEngine;

public class MouseoverOutline : MonoBehaviour
{
    Color colorTeam = Color.green;
    public Texture2D handImg;
    public Vector2 mouseImgOffset = new Vector2(5, 5);


    void SetOutline(Color col) 
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
            foreach (var mat in r.materials)
                if (mat.HasProperty("_OutlineColor"))
                    mat.SetColor("_OutlineColor", col);
    }


    /// <summary>
    /// 鼠标复原
    /// </summary>
    public void OnMExit() 
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    /// <summary>
    /// 鼠标变手
    /// </summary>
    public void OnMEnter()
    {
        Cursor.SetCursor(handImg, mouseImgOffset, CursorMode.Auto);
    }
    /// <summary>
    /// 显示轮廓线
    /// </summary>
    public void ShowOutLine()
    {
        SetOutline(colorTeam);
    }
    /// <summary>
    /// 隐藏轮廓线
    /// </summary>
    public void HideOutLine()
    {
        SetOutline(Color.clear);
    }
}
