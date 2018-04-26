using UnityEngine;
using System.Collections;
using System;

public class SRGUICharSpaceLabel : SRGUIContainer, ISRGUIDistributable {

    public SRGUILabel OriginalLabel;

    public static SRGUICharSpaceLabel FromLabel(SRGUILabel val, float charSpace, float linespace) {
        

        SRGUICharSpaceLabel retLab = new SRGUICharSpaceLabel();
        retLab.OriginalLabel = val;

        Char[] cahrArray = val.Text.ToCharArray();

        float xPos = 0;
        float yPos = 0;

        for (int i = 0; i < cahrArray.Length; i++) {
            SRGUILabel charLab = new SRGUILabel();
            charLab.Text = cahrArray[i].ToString();
            charLab.Position = new Vector2(xPos, yPos);
            charLab.Style = val.Style;
            
            if (charLab.Text == "\r") {
            } else if (charLab.Text == "\n") {
                xPos = 0;
                charLab.Text = "O";
                yPos = charLab.Size.y + linespace;
            } else {

                if (charLab.Text == " ") {
                    charLab.Text = "-";
                } else {
                    retLab.children.Add(charLab);
                }

                xPos += charLab.Size.x + charSpace;
            }

        }

        retLab.SetSize();

        return retLab;
    }

    private void SetSize(){
        float maxX = 0;
        float maxY = 0;

        foreach(SRGUILabel lb in children){
            maxX = Mathf.Max(maxX, (lb.Size.x + lb.Position.x));
            maxY = Mathf.Max(maxY, (lb.Size.y + lb.Position.y));
        }
        _Size = new Vector2(maxX, maxY);

    }

    override public Vector2 Size {
        get {
            //_Size = OriginalLabel.Size;

            return _Size;
        }
    }

    Vector2 ISRGUIDistributable.GetSize() {

        return Size;
    }
}
