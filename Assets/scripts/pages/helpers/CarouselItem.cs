using UnityEngine;
using System.Collections;

public class CarouselItem : MonoBehaviour {
    private Renderer _renderer;

    public delegate void StartTouchCallback(GameObject target);
    public delegate void EndTouchCallback(GameObject target);

    public event StartTouchCallback OnTouchStart;
    public event EndTouchCallback OnTouchEnd;

	// Use this for initialization
	void Awake () {
        _renderer = GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseDown() {
        OnTouchStart(gameObject);
    }

    void OnMouseUp() {
        OnTouchEnd(gameObject);
    }
 
    public Color color {
        get { return _renderer.material.color; }
        set { _renderer.material.color = value; }
    }
       
}
