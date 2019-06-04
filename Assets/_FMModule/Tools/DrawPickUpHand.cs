using UnityEngine;
using System.Collections;
/// <summary>
/// Draw pick up hand. 绘制捡物体时的手型图标
/// </summary>
public class DrawPickUpHand : Singleton<DrawPickUpHand>
{
    private Texture2D handImg;

    //public static DrawPickUpHand instance;
    private Shader selectedShader;
    private Shader normalShader;
    public Vector2 mouseImgOffset = new Vector2(5, 5);



    void Awake()
    {
        // instance = this;
    }

    void Init()
    {
        handImg = (Texture2D)Resources.Load("mouse");
        normalShader = Shader.Find("Legacy Shaders/Lightmapped/Diffuse");
        selectedShader = Shader.Find("Hidden/RimLightSpce");
    }

    public void DrawHand(Renderer myRenderer1 = null, Renderer myRenderer2 = null)
    {
        if (handImg == null)
        {   
            Init();
        }
        if (myRenderer1)
        {
            for (int i = 0; i < myRenderer1.materials.Length; i++)
            {
                myRenderer1.materials[i].shader = selectedShader;
            }
        }
        //    myRenderer1.material.shader = selectedShader;
        if (myRenderer2)
            myRenderer2.material.shader = selectedShader;
        //Cursor.SetCursor(handImg, Vector2.zero, CursorMode.Auto);
        Cursor.SetCursor(handImg, mouseImgOffset, CursorMode.Auto);
    }


    public void DrawMouseNormal(Renderer myRenderer1 = null, Renderer myRenderer2 = null)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        if (myRenderer1)
        {
            for (int i = 0; i < myRenderer1.materials.Length; i++)
            {
                myRenderer1.materials[i].shader = normalShader;
            }
        }
        //    myRenderer1.material.shader = normalShader;
        if (myRenderer2)
            myRenderer2.material.shader = normalShader;
    }
}
