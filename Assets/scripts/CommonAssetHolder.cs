using UnityEngine;
using System.Collections;

public class CommonAssetHolder : MonoBehaviour {
    public static CommonAssetHolder instance;

    public Color[] FontColors;
    public Font[] fonts;

    public enum FontNameType { CorpBold, FrutiGray, FrutiBlack80, FrutiBlack100, FrutiWhite, ManagementTitle };

    public Texture Logo;

    public Texture2D[] BackgroundTextures;
    public Texture2D BackButtTexture;
    public Texture2D[] NextPageButtTextures;
    public Texture2D[] PathArrowTextures;
   

    public Texture[] CheckBoxTextures;

    public CommonAssetHolder() {
        instance = this;
    }

    public GUIStyle GetCustomStyle(FontNameType font, int fontSize, bool isBold = false, bool isItalic = false, bool doCenter = false) {
        GUIStyle retStyle = new GUIStyle();

        switch (font) {
            case FontNameType.CorpBold:
                retStyle.font = fonts[0];
                retStyle.normal.textColor = FontColors[0];
                break;
            case FontNameType.FrutiGray:
                retStyle.font = fonts[1];
                retStyle.normal.textColor = FontColors[1];
                break;
            case FontNameType.FrutiBlack80:
                retStyle.font = fonts[1];
                retStyle.normal.textColor = FontColors[2];
                break;
            case FontNameType.FrutiWhite:
                retStyle.font = fonts[1];
                retStyle.normal.textColor = FontColors[3];
                break;
            case FontNameType.ManagementTitle:
                retStyle.font = fonts[1];
                retStyle.normal.textColor = FontColors[2];
                break;
            case FontNameType.FrutiBlack100:
                retStyle.font = fonts[1];
                retStyle.normal.textColor = FontColors[4];
                break;
        }

        retStyle.active.textColor = retStyle.normal.textColor;

        retStyle.fontSize = fontSize;

        retStyle.clipping = TextClipping.Clip;

        retStyle.fontStyle = (isBold) ? ((isItalic) ? FontStyle.BoldAndItalic : FontStyle.Bold) : ((isItalic) ? FontStyle.Italic : FontStyle.Normal);

        if (doCenter) {
            retStyle.alignment = TextAnchor.MiddleCenter;
        } else {
            retStyle.alignment = TextAnchor.UpperLeft;
        }

        return retStyle;
        
    }

    public GUIStyle GetCustomStyle(Texture2D buttTexture, FontNameType font = FontNameType.FrutiGray, int fontSize = 30, bool isBold = false, bool isItalic = false, bool doCenter = false) {
        GUIStyle retStyle = GetCustomStyle(font, fontSize, isBold, isItalic, doCenter);

        retStyle.normal.background = buttTexture;
        retStyle.active.background = buttTexture;


        return retStyle;
    }

    
}
