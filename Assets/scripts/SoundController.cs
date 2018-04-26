using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour {

    public enum SoundT { AppStart, MainMenuSelection, TopMenuSelection, MainMenuSpin, GalleryPopUp, Checklist, Transformation };

    public AudioClip AppStart;
    public AudioClip MainMenuSelection;
    public AudioClip TopMenuSelection;
    public AudioClip MainMenuSpin;
    public AudioClip GalleryPopUp;
    public AudioClip Checklist;
    public AudioClip Transformation;


    public static SoundController instance;

    

    public SoundController() {
        instance = this;
    }

    void Start() {
        GetComponent<AudioSource>().volume = 1;
    }


    public void playSingleSound(SoundT soundType) {
        switch (soundType) {
            case SoundT.AppStart:
                GetComponent<AudioSource>().PlayOneShot(AppStart);
                break;
            case SoundT.MainMenuSelection:
                GetComponent<AudioSource>().PlayOneShot(MainMenuSelection);
                break;
            case SoundT.TopMenuSelection:
                GetComponent<AudioSource>().PlayOneShot(TopMenuSelection);
                break;
            case SoundT.MainMenuSpin:
                GetComponent<AudioSource>().PlayOneShot(MainMenuSpin);
                break;
            case SoundT.GalleryPopUp:
                GetComponent<AudioSource>().PlayOneShot(GalleryPopUp);
                break;
            case SoundT.Checklist:
                GetComponent<AudioSource>().PlayOneShot(Checklist);
                break;
            case SoundT.Transformation:
                GetComponent<AudioSource>().PlayOneShot(Transformation);
                break;

        }

    }


    public void killSound() {
        GetComponent<AudioSource>().Stop();
    }

}
