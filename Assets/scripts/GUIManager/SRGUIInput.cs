using UnityEngine;
using System.Collections;

public class SRGUIInput : SRBaseGUIElement {

    private string _text = "";
    private string _lastText = "";

    public GUIStyle Style;
    public bool isMultyLine = false;

    internal void SetSize(Vector2 size) {
        _Size = size;
    }

    public string Text {
        get { return _text; }
        set {
            _lastText = _text;
            _text = value;
        }
    }

    public bool IsModified {
        get {
            bool res = _text != _lastText;
            _lastText = _text;
            return res;
        }
        
    }
}
