using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SRGUIManager : MonoBehaviour {
    
    public static SRGUIManager instance;

    /*internal const int UiHeight = 720;
    internal const int UiWidth = 1280;*/
    internal const int UiWidth = 1920;
    internal const int UiHeight = 1080;
    

    public delegate void ClickHandler(SRBaseGUIElement caller);
    public event ClickHandler Click;

    public delegate void InputChangedHandler(SRGUIInput caller);
    public event InputChangedHandler InputChanged;

    private Dictionary<GameObject, List<SRBaseGUIElement>> elements = new Dictionary<GameObject, List<SRBaseGUIElement>>();
    
    private  Matrix4x4 ScreenFitGUIMatrix;
    private Matrix4x4 internalGuiMatrix;
    
    private GUIStyle EmptyButtStyle = new GUIStyle(); //for dummies

    public int ActiveButtonGroup = -1;
    private Matrix4x4 MatrixOriginalBackup;
    private float groupScale;

    public bool Enabled = false;

    public SRGUIManager() {
        instance = this;
    }

    public void Draw(GameObject caller) {

        if (!Enabled) return;


        groupScale = Screen.height / (float)UiHeight;
        groupScale = Mathf.Min(groupScale, Screen.width / (float)UiWidth);
        //float uiWidth = Screen.width / groupScale;
        MatrixOriginalBackup = GUI.matrix;
        GUIUtility.ScaleAroundPivot(new Vector2(groupScale, groupScale), new Vector2(0, 0));


        Matrix4x4 moveMatrix = Matrix4x4.identity;
        moveMatrix[0, 3] = 0;
        moveMatrix[1, 3] = 0.5f *  (Screen.height - groupScale * (float)UiHeight);

        ScreenFitGUIMatrix = moveMatrix * GUI.matrix;

        if (elements.ContainsKey(caller)) {

            internalGuiMatrix = Matrix4x4.identity;

            List<SRBaseGUIElement> callersChildren = elements[caller];
            buildChildrenList(callersChildren);
            
        }

        GUI.matrix = MatrixOriginalBackup;

    }

    private void buildChildrenList(List<SRBaseGUIElement> children) {
        Color curentColor = GUI.color;
        Matrix4x4 curentMatrix = internalGuiMatrix;
        foreach (SRBaseGUIElement child in children) {

            if (child.Enabled) {
                Color childColor = new Color(1f, 1f, 1f, child.Alpha);
                GUI.color = curentColor * childColor;

                Matrix4x4 negativeAxisMatrix = Matrix4x4.identity;
                negativeAxisMatrix[0, 3] = -child.Axis.x;
                negativeAxisMatrix[1, 3] = -child.Axis.y;

                Matrix4x4 moveMatrix = Matrix4x4.identity;

                
                moveMatrix[0, 3] = child.Position.x;
                moveMatrix[1, 3] = child.Position.y;

                Matrix4x4 skewMatrix = Matrix4x4.identity;
                skewMatrix[0, 1] = child.Skew.x;
                skewMatrix[1, 0] = child.Skew.y;

                Matrix4x4 scaleMatrix = Matrix4x4.identity;
                scaleMatrix[0, 0] = child.Scale.x;
                scaleMatrix[1, 1] = child.Scale.y;


                Matrix4x4 positiveAxisMatrix = Matrix4x4.identity;
                positiveAxisMatrix[0, 3] = child.Axis.x;
                positiveAxisMatrix[1, 3] = child.Axis.y;

                if (child.worldCoordinates) {
                    GUI.matrix = MatrixOriginalBackup * positiveAxisMatrix * moveMatrix * skewMatrix * scaleMatrix * negativeAxisMatrix;
                } else {

                    internalGuiMatrix = curentMatrix * positiveAxisMatrix * moveMatrix * skewMatrix * scaleMatrix * negativeAxisMatrix;
                    GUI.matrix = ScreenFitGUIMatrix * internalGuiMatrix;
                }

                


                if (child is SRGUIContainer) {
                    buildChildrenList((child as SRGUIContainer).children);
                } else if (child is SRGUIButton) {
                    SRGUIButton butt = child as SRGUIButton;
                    if (ActiveButtonGroup == -1 || ActiveButtonGroup == butt.GroupID) {
                        if (GUI.Button(new Rect(0, 0, butt.Size.x, butt.Size.y), butt.Text, butt.Style)) {
                            Click(butt);
                        }
                    } else if (butt.Style.normal.background != null) {
                        GUI.DrawTexture(new Rect(0, 0, butt.Size.x, butt.Size.y), butt.Style.normal.background);
                    }

                } else if (child is SRGUICheckButton) {
                    SRGUICheckButton checkButt = child as SRGUICheckButton;
                    Texture targetText = (checkButt.Checked) ? checkButt.Textures[1] : checkButt.Textures[0];
                    GUI.DrawTexture(new Rect(0, 0, targetText.width, targetText.height), targetText);
                    if (GUI.Button(new Rect(0, 0, checkButt.Size.x, checkButt.Size.y), "", EmptyButtStyle)) {
                        checkButt.Checked = !checkButt.Checked;
                        Click(checkButt);
                    }
                } else if (child is SRGUITexture) {
                    SRGUITexture texture = child as SRGUITexture;
                    if (texture.hasCoords) {
                        GUI.DrawTextureWithTexCoords(new Rect(0, 0, texture.Size.x, texture.Size.y), texture.Texture, texture.Coords);
                        //GUI.DrawTexture(new Rect(0, 0, texture.Size.x, texture.Size.y), texture.Texture);
                        Debug.Log(texture.Size.x + " " + texture.Size.y);
                    } else {
                        GUI.DrawTexture(new Rect(0, 0, texture.Size.x, texture.Size.y), texture.Texture);
                    }
                    

                } else if (child is SRGUILabel) {
                    SRGUILabel label = child as SRGUILabel;
                    GUI.Label(new Rect(0, 0, label.Size.x, label.Size.y), label.Text, label.Style);
                
                } else if (child is SRGUIInput) {
                    SRGUIInput input = child as SRGUIInput;
                    string oldText = input.Text;
                    if (input.isMultyLine) {
                        input.Text = GUI.TextArea(new Rect(0, 0, input.Size.x, input.Size.y), input.Text, input.Style);
                    } else {
                        input.Text = GUI.TextField(new Rect(0, 0, input.Size.x, input.Size.y), input.Text, input.Style);
                    }

                    if (oldText != input.Text) {
                        InputChanged(input);
                    }
                }
            }
        }

        internalGuiMatrix = curentMatrix;
        GUI.color = curentColor;
    }

    public void RegisterGUIElement(SRBaseGUIElement guiElement, GameObject owner, bool isTop = true){
        if (!elements.ContainsKey(owner)){
            elements[owner] = new List<SRBaseGUIElement>();
        }
        elements[owner].Insert((isTop)?(elements[owner].Count):0, guiElement);
    }

}
