using UnityEngine;

public class SRBaseGUIElement {

    public Vector2 Axis = new Vector2();
    public Vector2 Position = new Vector2();
    internal Vector2 _Size = new Vector2();

    public Vector2 Skew = new Vector2();
    public Vector2 Scale = new Vector2(1, 1);

    public string destination = null;


    public Vector2 OutPosition;
    public Vector2 InPosition;
    public Vector2 ClickInPosition;

    public Vector2 OutScale;
    public Vector2 InScale;
    public Vector2 ClickInScale;

    public float Alpha = 1;

    public bool Enabled = true;

    public bool worldCoordinates = false;

    public virtual Vector2 Size{
        get {
            return _Size;
        }
    }
}

public interface ISRGUIDistributable {
    Vector2 GetSize();
}
