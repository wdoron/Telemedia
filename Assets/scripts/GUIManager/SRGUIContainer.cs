
using System.Collections.Generic;
using UnityEngine;


public class SRGUIContainer : SRBaseGUIElement, ISRGUIDistributable {

    public List<SRBaseGUIElement> children = new List<SRBaseGUIElement>();

    override public Vector2 Size {
        get {
            float maxX = 0;
            float maxY = 0;

            foreach (SRBaseGUIElement child in children) {
                maxX = Mathf.Max(maxX, child.Size.x);
                maxY = Mathf.Max(maxY, child.Size.y);
            }

            _Size = new Vector2(maxX, maxY);

            return base.Size;
        }
    }

    Vector2 ISRGUIDistributable.GetSize() {
        foreach (SRBaseGUIElement child in children) {
            if (child is ISRGUIDistributable) {
                return (child as ISRGUIDistributable).GetSize();
            }
        }

        return new Vector2();
    }
}
