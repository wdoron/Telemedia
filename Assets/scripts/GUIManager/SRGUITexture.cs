using UnityEngine;

public class SRGUITexture : SRBaseGUIElement {

    private Texture2D _Texture = null;
    public bool hasCoords = false;
    public Rect Coords;
    
    public void SetTexture(Texture texture) {
        hasCoords = false;
        _Texture = texture as Texture2D;
        if (texture) {
            _Size = new Vector2(texture.width, texture.height);
        }
    }

    public void SetTexture(Sprite sprite) {
        hasCoords = true;
        _Texture = sprite.texture;
        Vector2 scale = new Vector2(sprite.rect.width / sprite.texture.width, 
            sprite.rect.height/ sprite.texture.height);
        Vector2 pos = new Vector2(sprite.rect.x / sprite.texture.width,
            sprite.rect.y / sprite.texture.height);
        Coords = new Rect(pos.x, pos.y, scale.x, scale.y);
        _Size = new Vector2(sprite.rect.width, sprite.rect.height);
    }

    public void SetTexture() {
        _Texture = null;
    }

    public void SetTexture(Texture texture, Vector2 size, bool fitToSize = false) {
        hasCoords = false;
        _Texture = texture as Texture2D;
        if (fitToSize) {
            float minRat = Mathf.Min(size.x / texture.width, size.y / texture.height);
            _Size = new Vector2(texture.width * minRat, texture.height * minRat); 
        } else {
            _Size = size;
        }
        
    }
    
    public void SetcustomSize(Vector2 size) {
        _Size = size;
    }

    public Texture Texture {
        get {
            return _Texture;
        }
    }
}
