using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;
using System.Xml;
using System;

public class PageCarousel : BasePage {

    public GameObject CarouselPF;
    public CarouselItem CarouselSpritePF;
    public GameObject BasePF;

    public Texture[] ItemTextures;
    public Sprite[] ItemSprites;

    public Texture PlaneTexture;
    public Texture LargeLogoTexture;

    private List<string> Destinations = new List<string>();

    private GameObject rotator;
    private float radius = 2.5f;
    private List<CarouselItem> carouselItems = new List<CarouselItem>();

    private bool isDraggingItem = false;
    public float dragGrace = 0;
    //private GameObject dragTarget;

    private Vector2 carouselCenterScreen;
    private Quaternion originalRotation;
    private float original2DRotation;
    //private float circleWidthToHeight;
    private Quaternion rotationDiff;
    private float rotDiffTime;
    private Tweener graceTweener;
    private float TouchStartTime;
    private float _alpha;
    private CarouselItem SelectedItem = null;

    private GameObject SpinBase;

    private Vector3 RootPosition = new Vector3(-0.14f, -0.73f, 7.13f);
    private Vector3 ExitPosition = new Vector3(-6f, -3f, 20.13f);

    //private Vector3 BaseItemScale = new Vector3(1.22f, 1f, 1f) * 0.1f;
    private Vector3 BaseItemScale = new Vector3(1f, 1f, 1f) * 0.1f;
    SRGUITexture Logo;

    private SRGUITexture Plane;
    private Vector3 OriginalMousePosition;
    private bool SpinSoundOn;


    private bool IsTransition3D = true;

    public static PageCarousel instance;

    override internal void InitPage(XmlNode pageData) {
        base.InitPage(pageData);
        instance = this;

        ExitTime = 0.8f;

        BaseItemScale *= 1.1f;

        TransitionData transitionData;

        //3D
        foreach (XmlNode itemData in pageData) {
            Destinations.Add(itemData.Attributes.GetNamedItem("dest").Value);
        }

        //PageType = PageTypes.Carousel;

        gameObject.transform.position = RootPosition;

        carouselCenterScreen = Camera.main.WorldToScreenPoint(gameObject.transform.position + new Vector3(0, 0f, 0));

        //circleWidthToHeight = Camera.main.WorldToScreenPoint(gameObject.transform.position + new Vector3(radius, 0.5f, 0)).x / Camera.main.WorldToScreenPoint(gameObject.transform.position + new Vector3(0, 0.5f, radius)).y;

        rotator = new GameObject();
        rotator.transform.parent = this.transform;
        rotator.transform.localPosition = new Vector3();

        for (int i = 0; i < Destinations.Count; i++) {
            float angRad = Mathf.Deg2Rad * i * (360 / Destinations.Count);
            carouselItems.Add(Instantiate(CarouselSpritePF, rotator.transform.position + new Vector3(radius * Mathf.Cos(angRad), 0, radius * Mathf.Sin(angRad)), new Quaternion()) as CarouselItem);
            //SetItemTexture(carouselItems[i], ItemTextures[i * 3]);
            SetItemTexture(carouselItems[i], ItemSprites[i * 3]);

            carouselItems[i].transform.parent = rotator.transform;

            carouselItems[i].OnTouchStart += ItemTouchStarted;
            carouselItems[i].OnTouchEnd += ItemTouchEnded;
        }

        SpinBase = Instantiate(BasePF) as GameObject;
        SpinBase.transform.parent = gameObject.transform;
        SpinBase.transform.localPosition = new Vector3(0.0689217f, -0.84f, 2.838512f);

        (SpinBase.GetComponent(typeof(CarouselItem)) as CarouselItem).OnTouchStart += ItemTouchStarted;
        (SpinBase.GetComponent(typeof(CarouselItem)) as CarouselItem).OnTouchEnd += ItemTouchEnded;


        transitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.LocalPosition3D }, 0.4f, 1.2f, 0f);
        transitionData.LocalPositions3D = new Vector3[3];
        transitionData.LocalPositions3D[0] = new Vector3(0.0689217f, -0.84f, 2.838512f);
        transitionData.LocalPositions3D[1] = new Vector3(0.0689217f, -1.2f, 2.838512f);
        RegisterTransition(transitionData, SpinBase);


        transitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha3D }, 0.4f, 1.2f, 0f);
        RegisterTransition(transitionData, SpinBase);


        //2D

        //logo
        Logo = new SRGUITexture();
        Logo.SetTexture(LargeLogoTexture);
        Logo.OutPosition = LogoSmallPosition;
        Logo.InPosition = new Vector2(0, 60);
        Logo.InScale = new Vector2(1f, 1f);
        Logo.OutScale = LogoSmallScale;
        GUIManager.RegisterGUIElement(Logo, gameObject);

        /*transitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.ScaleGUI, TransitionTypes.PositionGUI }, 0.3f, 0f, 0);
        RegisterTransition(transitionData, Logo);*/

        //top text
        SRGUILabel title = new SRGUILabel();
        title.Text = pageData.Attributes.GetNamedItem("title").Value;
        title.Style = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.CorpBold, 75, true);

        SRGUICharSpaceLabel titleSpaced = SRGUICharSpaceLabel.FromLabel(title, -2.1f, -19);
        titleSpaced.Position = new Vector2(703, 103);
        GUIManager.RegisterGUIElement(titleSpaced, gameObject);

        transitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha }, 0.3f, 0.5f, 0f);
        RegisterTransition(transitionData, titleSpaced);

        //plane
        Plane = new SRGUITexture();
        Plane.SetTexture(PlaneTexture);
        //Plane.Scale = new Vector2(1.2f, 1.2f);
        Plane.Scale = new Vector2(2f, 2f);
        //Plane.InPosition = new Vector2(300, 290);
        Plane.InPosition = new Vector2(47, 25);
        Plane.OutPosition = Plane.InPosition + new Vector2(0, 15);
        GUIManager.RegisterGUIElement(Plane, gameObject);

        transitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha, TransitionTypes.PositionGUI }, 0.7f, 0.8f, 0f);
        transitionData.OutTime = 0.2f;
        RegisterTransition(transitionData, Plane);

    }
    
    public static void LoadTextures() {
        PageMainTopic.FlatTransitionSeq = new Texture[6][];
        for (int i = 0; i < PageMainTopic.FlatTransitionSeq.Length; i++) {
            if (PageMainTopic.FlatTransitionSeq[i] == null) {
                PageMainTopic.FlatTransitionSeq[i] = Resources.LoadAll<Texture>("media/transitions/t" + i);
            }

        }
    }

    public static bool CheckTexturesLoaded() {
        return (PageMainTopic.FlatTransitionSeq != null) && (PageMainTopic.FlatTransitionSeq[5] != null) && (PageMainTopic.FlatTransitionSeq[5].Length != 0);
    }


    private void SetItemTexture(CarouselItem go, Texture texture) {
        go.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
        //go.GetComponent<Renderer>().material.mainTexture =  texture;
    }

    private void SetItemTexture(CarouselItem go, Sprite sprite) {
        go.GetComponent<SpriteRenderer>().sprite = sprite;
        //go.GetComponent<Renderer>().material.mainTexture =  texture;
    }

    override protected void DoInternalUpdate() {
        base.DoInternalUpdate();
        //rotator.transform.Rotate(Vector3.up, 10 * Time.deltaTime);

        if (isDraggingItem) {
            Quaternion currRot = rotator.transform.rotation;
            rotator.transform.rotation = Quaternion.Euler(0, Mathf.Rad2Deg * Mathf.Atan2((float)(carouselCenterScreen.x - Input.mousePosition.x), (float)(carouselCenterScreen.y - Input.mousePosition.y) * 4f) - original2DRotation, 0) * originalRotation;
            rotationDiff = Quaternion.FromToRotation(currRot * Vector3.forward, rotator.transform.rotation * Vector3.forward);
            rotDiffTime = Time.deltaTime;

            if ((Quaternion.Angle(rotationDiff, Quaternion.identity) * Time.deltaTime > 0.08f)) {
                PlayspinSound();
            }

        } else if (dragGrace > 0) {
            rotator.transform.rotation = Quaternion.Slerp(Quaternion.identity, rotationDiff, dragGrace * Time.deltaTime / rotDiffTime) * rotator.transform.rotation;
            //rotator.transform.rotation = rotationDiff * rotator.transform.rotation;

            if ((Quaternion.Angle(rotationDiff, Quaternion.identity) * Time.deltaTime > 0.08f)) {
                PlayspinSound();
            }
        }

        foreach (CarouselItem item in carouselItems) {
            item.transform.rotation = Quaternion.identity;//Quaternion.LookRotation(Vector3.forward);
        }
    }

    private void PlayspinSound() {
        if (!SpinSoundOn) {
            SpinSoundOn = true;
            SoundController.instance.playSingleSound(SoundController.SoundT.MainMenuSpin);
            Invoke("KillSpinsound", 1);
        }
    }

    private void KillSpinsound() {
        SpinSoundOn = false;
    }

    override public void DoEnter(float previousDelay, UnityEngine.Object extraParams, string extraData = null) {
        base.DoEnter(previousDelay, extraParams, extraData);
        Plane.Enabled = true;
        Logo.Alpha = 0;
        Logo.Position = Logo.OutPosition;
        Logo.Scale = Logo.OutScale;
        if (previousDelay > 0 && SelectedItem != null) {

            //HOTween.To(SelectedItem.GetComponent<Renderer>().material, 0.5f, new TweenParms().Prop("color", AlphaColorTransparent).Ease(EaseType.Linear).Delay(0));
            HOTween.To(SelectedItem, 0.5f, new TweenParms().Prop("color", AlphaColorTransparent).Ease(EaseType.Linear).Delay(0));
            Invoke("PopUpCarousel", previousDelay);

            TransitionTweens.Add(HOTween.To(Logo, 0f, new TweenParms().Prop("Alpha", 1).Delay(previousDelay)));
            TransitionTweens.Add(HOTween.To(Logo, 0.6f, new TweenParms().Prop("Scale", Logo.InScale).Prop("Position", Logo.InPosition).Delay(previousDelay)));
        } else {
            PopUpCarousel();
            TransitionTweens.Add(HOTween.To(Logo, 0f, new TweenParms().Prop("Alpha", 1).Delay(0.5f)));
            TransitionTweens.Add(HOTween.To(Logo, 0.6f, new TweenParms().Prop("Scale", Logo.InScale).Prop("Position", Logo.InPosition).Delay(0.5f)));

            SoundController.instance.playSingleSound(SoundController.SoundT.AppStart);
        }

    }

    private void PopUpCarousel() {
        gameObject.transform.position = RootPosition;
        int itemCount = 0;
        if (SelectedItem != null) {
            SelectedItem.transform.parent = rotator.transform;
            float angRad = Mathf.Deg2Rad * carouselItems.IndexOf(SelectedItem) * (360 / Destinations.Count);
            HOTween.To(SelectedItem, 0.6f, new TweenParms().Prop("color", AlphaColorTransparent).Ease(EaseType.Linear).Delay(0));
            SelectedItem.transform.localPosition = new Vector3(radius * Mathf.Cos(angRad), 0, radius * Mathf.Sin(angRad));
            SelectedItem.transform.rotation = Quaternion.identity;

        }

        foreach (CarouselItem caruoselItem in carouselItems) {
            //SetItemTexture(carouselItems[itemCount], ItemTextures[itemCount * 3 + 1]);
            SetItemTexture(carouselItems[itemCount], ItemSprites[itemCount * 3 + 1]);
            //carouselItems[itemCount].GetComponent<Renderer>().material.mainTexture = ItemTextures[itemCount * 3 + 1];

            caruoselItem.transform.localPosition = new Vector3(caruoselItem.transform.localPosition.x, -0.2f, caruoselItem.transform.localPosition.z);
            caruoselItem.transform.localScale = BaseItemScale * 0.3f;

            Vector3 targetPos = new Vector3(caruoselItem.transform.localPosition.x, 0, caruoselItem.transform.localPosition.z);
            //Vector3 targetClickPos = new Vector3(caruoselItem.transform.localPosition.x, 0.05f, caruoselItem.transform.localPosition.z);
            Vector3 targetScale = BaseItemScale;
            //Vector3 targetClickScale = BaseItemScale * 1.1f; ;

            caruoselItem.color = AlphaColorTransparent;

            //ClickTransition(caruoselItem.transform, 0.6f, new TweenParms().Prop("localPosition", targetPos).Prop("localScale", targetScale), new TweenParms().Prop("localPosition", targetClickPos).Prop("localScale", targetClickScale), 0.2f, itemCount * 0.15f, true);
            TransitionTweens.Add(HOTween.To(caruoselItem.transform, 0.6f, new TweenParms().Prop("localPosition", targetPos).Prop("localScale", targetScale).Delay(0f + itemCount * 0.3f)));
            TransitionTweens.Add(HOTween.To(caruoselItem, 0.4f, new TweenParms().Prop("color", AlphaColorOpaque).Ease(EaseType.Linear).Delay(0f + itemCount * 0.3f)));

            itemCount++;
        }

        SpinBase.GetComponent<Renderer>().material.color = AlphaColorTransparent;
        HOTween.To(SpinBase.GetComponent<Renderer>().material, 1, new TweenParms().Prop("color", AlphaColorOpaque).Ease(EaseType.Linear).Delay(0));

        rotator.transform.rotation = Quaternion.identity;
        Quaternion targetRot = Quaternion.AngleAxis(178, Vector3.up);
        HOTween.To(rotator.transform, 2, new TweenParms().Prop("rotation", targetRot).Ease(EaseType.EaseInOutSine).Delay(0).OnComplete(PopUpComplete));
    }

    private void PopUpComplete(TweenEvent te) {
        /*PageMainTopic.FlatTransitionSeq = new Sprite[6][];
        for (int i = 0; i < PageMainTopic.FlatTransitionSeq.Length; i++) {
            if (PageMainTopic.FlatTransitionSeq[i] == null) {
                PageMainTopic.FlatTransitionSeq[i] = Resources.LoadAll<Sprite>("media/transitions/t" + i + "/packed");
            }

        }*/

    }

    override public void DoExit(string targetID) {
        base.DoExit(targetID);

        int itemCount = 0;
        foreach (CarouselItem caruoselItem in carouselItems) {
            if ((caruoselItem != SelectedItem) || (!IsTransition3D)) {
                caruoselItem.color = AlphaColorOpaque;
                HOTween.To(caruoselItem, 0.4f, new TweenParms().Prop("color", AlphaColorTransparent).Ease(EaseType.Linear).Delay(0.1f));

                caruoselItem.transform.localPosition = new Vector3(caruoselItem.transform.localPosition.x, 0, caruoselItem.transform.localPosition.z);
                caruoselItem.transform.localScale = BaseItemScale;

            }
            itemCount++;
        }

        Invoke("SetSelectedTexture", 0.2f);


        TransitionTweens.Add(HOTween.To(gameObject.transform, 0.6f, new TweenParms().Prop("position", ExitPosition).Delay(0.1f)));

        Logo.Position = Logo.InPosition;
        Logo.Scale = Logo.InScale;
        Logo.Alpha = 1;
        TransitionTweens.Add(HOTween.To(Logo, 0.6f, new TweenParms().Prop("Scale", Logo.OutScale).Prop("Position", Logo.OutPosition).Delay(0.1f)));
        TransitionTweens.Add(HOTween.To(Logo, 0f, new TweenParms().Prop("Alpha", 0).Delay(ExitTime)));

    }

    private void SetSelectedTexture() {
        int index = carouselItems.IndexOf(SelectedItem);
        if (index > -1) {
            //SetItemTexture(carouselItems[index], ItemTextures[index * 3]);
            SetItemTexture(carouselItems[index], ItemSprites[index * 3]);
            //carouselItems[index].GetComponent<Renderer>().material.mainTexture = ItemTextures[index * 3];
        }
    }

    private void ItemTouchStarted(GameObject target) {
        if (Management.Instance.IsOpen) return;
        isDraggingItem = true;
        //dragTarget = target;
        if (graceTweener != null) graceTweener.Kill();
        originalRotation = rotator.transform.rotation;
        OriginalMousePosition = Input.mousePosition;
        original2DRotation = Mathf.Rad2Deg * Mathf.Atan2((float)(carouselCenterScreen.x - Input.mousePosition.x), (float)(carouselCenterScreen.y - Input.mousePosition.y) * 4f);
        TouchStartTime = Time.time;
    }

    private void ItemTouchEnded(GameObject target) {
        if (Management.Instance.IsOpen) return;
        dragGrace = 1;
        graceTweener = HOTween.To(this, 0.6f, new TweenParms().Prop("dragGrace", 0).Ease(EaseType.EaseOutSine));
        isDraggingItem = false;
        if (Time.time - TouchStartTime < 0.2f && NavigationEnabled && (OriginalMousePosition - Input.mousePosition).magnitude < Screen.width / 50) {

            PageMainTopic.SelectedFlatTransition = carouselItems.IndexOf(target.GetComponent<CarouselItem>());

            //IsTransition3D = (PageMainTopic.SelectedFlatTransition != 2) && (PageMainTopic.SelectedFlatTransition != 0);
            //IsTransition3D = (target != carouselItems[2]);
            IsTransition3D = false;

            int index = carouselItems.IndexOf(target.GetComponent<CarouselItem>());

            SelectedItem = target.GetComponent<CarouselItem>();

            DipatchNavigate(Destinations[index], SelectedItem, (IsTransition3D) ? null : "2D");

            if (!IsTransition3D) {
                Plane.Enabled = false;
            }

            //carouselItems[index].GetComponent<Renderer>().material.mainTexture = ItemTextures[index * 3 + 2];
            //SetItemTexture(carouselItems[index], ItemTextures[index * 3 + 2]);
            SetItemTexture(carouselItems[index], ItemSprites[index * 3 + 2]);
            SoundController.instance.playSingleSound(SoundController.SoundT.MainMenuSelection);
        }



    }



}
