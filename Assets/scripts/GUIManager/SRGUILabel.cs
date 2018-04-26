

using UnityEngine;
public class SRGUILabel : SRBaseGUIElement, ISRGUIDistributable {

    public string Text = null;
    public GUIStyle Style;

    private GUIContent Content = new GUIContent();
    public bool IsCustomSize = false;
    private Vector2 CustomSize;

    /*internal void Fit() {
        
        Content.text = Text;
        Vector2 calcSize = Style.CalcSize(Content);

        while (Style.fontSize > 0 && (calcSize.x > Size.x || calcSize.y > Size.y)) {
            Style.fontSize--;
            calcSize = Style.CalcSize(Content);
        }
    }*/

    override public Vector2 Size {
        get {
            if (!IsCustomSize) {
                Content.text = Text;
                _Size = (Style != null) ? Style.CalcSize(Content) : new Vector2();
            } else {
                _Size = CustomSize;
            }
            return base.Size;
        }
    }

    public float WrappedHeight(float width) {
        if (Style.wordWrap) {
            Content.text = Text;
            return (Style != null) ? Style.CalcHeight(Content, width) : 0;
        }else{
            return Size.y;
        }
    }

    public void setCustomSize(Vector2 targetSize) {
        IsCustomSize = true;
        //_Size = targetSize;
        CustomSize = targetSize;
    }

    Vector2 ISRGUIDistributable.GetSize() {
        return this.Size;
    }
}
