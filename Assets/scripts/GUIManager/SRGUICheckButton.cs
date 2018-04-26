using UnityEngine;
using System.Collections;

public class SRGUICheckButton : SRBaseGUIElement {

    public bool Checked = false;
    public string ID;

    private Texture[] _Textures; //0 unchecked, 1 checked

    public void SetTexture(Texture[] textures, bool useTextureSize = true, float sizeX = 0, float sizeY = 0) {
        _Textures = textures;
        if (useTextureSize) {
            _Size = new Vector2(textures[0].width, textures[0].height);
        } else {
            _Size = new Vector2(sizeX, sizeY);
        }
    }

    public Texture[] Textures {
        get {
            return _Textures;
        }
    }

}
