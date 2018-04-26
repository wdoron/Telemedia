using UnityEngine;

public class SRGUIButton : SRBaseGUIElement {

    public string Text;

    public GUIStyle Style;
    private bool IsCustomsize = false;
    
    public int GroupID = -1;
    public bool HasSound = false;
    


    override public Vector2 Size {
        get {
            if (!IsCustomsize) {
                _Size = new Vector2(Style.normal.background.width, Style.normal.background.height);
            }

            return base.Size;
        }
    }

    public void setCustomSize(Vector2 targetSize) {
        IsCustomsize = true;
        _Size = targetSize;
    }
}
