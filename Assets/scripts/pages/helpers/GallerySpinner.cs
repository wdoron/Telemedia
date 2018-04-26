using UnityEngine;
using System.Collections;
using Holoville.HOTween;
using System.Xml;
using System.IO;

public class GallerySpinner : MonoBehaviour {

    public GameObject ItemPF;
    //public GameObject ButtPF;
    //public TextMesh TextPF;
    public Texture2D CloseButtTexture;

    public delegate void NavigationCallback(string targetID, string extra);
    public event NavigationCallback OnNavigate;

    public Texture[] BottomTextures;
    
    private GameObject[] Items;
    private GameObject[] Conts;
    /*private GameObject[] Buts;
    private TextMesh[] Texts;*/

    private int[] ContentByItemID;
    
    private Texture[] GalleryTextures;
    //private string[] Titles;
    private string[] Destinations;
    private string[] ExtraParams;


    private float _CenterItemIndex = 0;

    private float InertiaSpeed = 0;
    private bool IsInertia = false;
    private Tweener EaseBackTween;

    
    public bool IsMouseDown;
    private float MouseDownTime;
    private Vector2 LastTouchPosition;
    private Vector2 FirstTouchPosition;

    private const int VISIBLE_IMAGES = 3;
    
    internal float Radius = 1.0f;

    public GameObject Spinner;

    internal Color AlphaColorOpaque = new Color(1, 1, 1, 1);
    internal Color AlphaColorTransparent = new Color(1, 1, 1, 0);
    private int ContentCount;

    public SRGUIButton CloseButt;
    private int ActivityState = 1; //0 inactive, 1 active, 2 grace to inactivity

    public string DebugText = "";
    public bool LargeImageEnabled = false;

    public SRGUIButton CloseLargeButt;
    public SRGUIContainer LargeImageCont;
    private SRGUITexture LargeImage;
    private SRGUITexture LargeBG;
    private int CenterContentRounded;
    public Texture GalleryBackText;


    public bool HasAnyData = false;
    private string TagetFolder;
    private bool Loaded = false;

    public void Init(string tagetFolder, XmlNode destinations = null) {

        TagetFolder = tagetFolder;

        //GalleryTextures = Resources.LoadAll<Texture>(TagetFolder);

        
        //GalleryTextures = new Texture[Directory.GetFiles(Application.dataPath + "/" + tagetFolder, "*.jpg").Length];

        if (destinations != null) {

            Destinations = new string[destinations.ChildNodes.Count];
            ExtraParams = new string[Destinations.Length];
            int medCount = 0;
            foreach (XmlNode dest in destinations) {
                Destinations[medCount] = dest.Attributes.GetNamedItem("dest").Value;
                if (dest.Attributes.GetNamedItem("extra") != null){
                    ExtraParams[medCount] = dest.Attributes.GetNamedItem("extra").Value;
                }
                
                medCount++;
            }

        } else {
            Destinations = new string[0];
            ExtraParams = new string[0];
        }


        Items = new GameObject[VISIBLE_IMAGES];
        Conts = new GameObject[VISIBLE_IMAGES];
        
        ContentByItemID = new int[VISIBLE_IMAGES];


        for (int i = 0; i < VISIBLE_IMAGES; i++) {
            ContentByItemID[i] = -1;

            float ang = Mathf.Deg2Rad * (-90 + i * 360f / VISIBLE_IMAGES);

            Conts[i] = new GameObject();
            Conts[i].transform.parent = Spinner.transform;
            Conts[i].transform.localPosition = new Vector3(Mathf.Cos(ang) * Radius, 0, Mathf.Sin(ang) * Radius);

            Items[i] = Instantiate(ItemPF) as GameObject;

            Items[i].transform.parent = Conts[i].transform;
            Items[i].transform.localPosition = new Vector3(0, 0, 0);

            /*Buts[i] = Instantiate(ButtPF) as GameObject;
            Buts[i].transform.parent = Conts[i].transform;
            Buts[i].transform.localPosition = new Vector3(0, -Conts[i].transform.localScale.y / 2, -0.05f);

            Texts[i] = Instantiate(TextPF) as TextMesh;
            Texts[i].transform.parent = Conts[i].transform;
            Texts[i].transform.localScale = new Vector3(1f, 1f, 1f) * 0.05f;
            Texts[i].transform.localPosition = new Vector3(-Buts[i].transform.localScale.x / 2 + Texts[i].transform.localScale.x / 2 + 0.1f, -0.5f, -0.1f);*/
        }

       
        CloseButt = new SRGUIButton();
        CloseButt.Style = CommonAssetHolder.instance.GetCustomStyle(CloseButtTexture, CommonAssetHolder.FontNameType.FrutiGray, 10);
        CloseButt.Enabled = false;
        CloseButt.Position = new Vector2(1400, 855);
        CloseButt.worldCoordinates = true;
        CloseButt.Alpha = 0;


        CloseLargeButt = new SRGUIButton();
        CloseLargeButt.Style = new GUIStyle();
        CloseLargeButt.setCustomSize(new Vector2(SRGUIManager.UiWidth, SRGUIManager.UiHeight));
        CloseLargeButt.Enabled = false;

        LargeImage = new SRGUITexture();

        LargeBG = new SRGUITexture();
        LargeBG.SetTexture(GalleryBackText, new Vector2(SRGUIManager.UiWidth, SRGUIManager.UiHeight));

        LargeImageCont = new SRGUIContainer();
        LargeImageCont.children.Add(LargeBG);
        LargeImageCont.children.Add(LargeImage);

        LargeImageCont.Enabled = false;


    }

    public void DoUpdate() {
        if (!Loaded) {
            LoadAssets(true);
            Loaded = true;
            
            HasAnyData = false;

            if (GalleryTextures.Length > 0) {
                HasAnyData = true;
                ContentCount = GalleryTextures.Length;
            }

            if (ContentCount == 1) {
                for (int i = 1; i < Conts.Length; i++) {
                    Conts[i].SetActive(false);
                }
            } else if (!HasAnyData) {
                for (int i = 0; i < Conts.Length; i++) {
                    Conts[i].SetActive(false);
                }
            }

            CenterItemIndex = CenterItemIndex;
        }

        if (!HasAnyData || LargeImageCont.Enabled) return;

        bool currentMouseState = (ActivityState == 1) && (Input.GetMouseButton(0) || (Input.touchCount > 0));

        currentMouseState &= (Input.mousePosition.x > Screen.width * 0.2f || Input.mousePosition.y < Screen.height * 0.7f);


        if (IsMouseDown != currentMouseState) {
            //state changed
            if (currentMouseState) {
                //clicked on
                FirstTouchPosition = Input.mousePosition;
                MouseDownTime = Time.time;
                LastTouchPosition = Input.mousePosition;
                IsMouseDown = true;
                IsInertia = false;
                if (EaseBackTween != null) EaseBackTween.Kill();
            } else {
                //clicked off
                IsMouseDown = false;
                IsInertia = true;

                if (LargeImageEnabled && (FirstTouchPosition - LastTouchPosition).magnitude < (Screen.width / 50) && Time.time - MouseDownTime < 0.2f) {
                    if (Destinations.Length > CenterContentRounded) {
                        OnNavigate(Destinations[CenterContentRounded], ExtraParams[CenterContentRounded]);
                    } else {

                        if (GalleryTextures[CenterContentRounded].name.Contains("thumb")) {
                            string nm = GalleryTextures[CenterContentRounded].name;

                            #if (UNITY_IPHONE || UNITY_ANDROID)
                                Handheld.PlayFullScreenMovie(nm.Substring(0, nm.Length - 6) + ".mp4", Color.black, FullScreenMovieControlMode.Full);
                            #endif


                        } else{

                            float minRat = Mathf.Min((float)SRGUIManager.UiWidth / (float)GalleryTextures[CenterContentRounded].width, (float)SRGUIManager.UiHeight / (float)GalleryTextures[CenterContentRounded].height);
                            LargeImage.SetTexture(GalleryTextures[CenterContentRounded], new Vector2(GalleryTextures[CenterContentRounded].width * minRat, GalleryTextures[CenterContentRounded].height * minRat));
                            LargeImage.Axis = new Vector2(LargeImage.Size.x / 2, LargeImage.Size.y / 2);
                            LargeImage.Position = new Vector2((SRGUIManager.UiWidth - LargeImage.Size.x) / 2, (SRGUIManager.UiHeight - LargeImage.Size.y) / 2);
                            LargeImageCont.Enabled = true;
                            CloseLargeButt.Enabled = true;
                            LargeBG.Alpha = 0;
                            LargeImage.Alpha = 0;
                            //LargeImage.Scale = new Vector2(0.6f, 0.6f);
                            HOTween.To(LargeBG, 0.6f, "Alpha", 1);
                            //HOTween.To(LargeImage, 0.3f, "Scale", new Vector2(1, 1));
                            HOTween.To(LargeImage, 0.6f, "Alpha", 1);

                        }

                    }
                }
            }

            IsMouseDown = currentMouseState;
        } else {
            //state unchanged
            if (IsMouseDown) {
                if (ContentCount == 1 || ActivityState == 0) return;
                //mouse is down
                float currentX = Input.mousePosition.x / Screen.width;
                float lastX = LastTouchPosition.x / Screen.width;
                InertiaSpeed = (lastX - currentX) / Time.deltaTime;
                //CenterItemIndex += 1.6f * (lastX - currentX);
                CenterItemIndex += 4.8f / (float)VISIBLE_IMAGES * (lastX - currentX);
                LastTouchPosition = Input.mousePosition;
            } else {
                //mouse is up
                if (IsInertia) {
                    //InertiaSpeed *= 0.92f;
                    float speedDiff = Time.deltaTime * 5f;
                    if (InertiaSpeed < 0){
                        InertiaSpeed += speedDiff;
                    }else{
                        InertiaSpeed -= speedDiff;
                    }
                    if (Mathf.Abs(InertiaSpeed) < 0.3f) {
                        IsInertia = false;
                        EaseBackTween = HOTween.To(this, 1f * Mathf.Abs(CenterItemIndex - Mathf.RoundToInt(CenterItemIndex)), new TweenParms().Prop("CenterItemIndex", Mathf.RoundToInt(CenterItemIndex)));
                        //TransitionTweens.Add(EaseBackTween);
                    } else {
                        CenterItemIndex += InertiaSpeed * 0.9f * Time.deltaTime;
                    }
                }
            }
        }

        if (ActivityState == 2) {
            if (!IsInertia && (EaseBackTween == null || EaseBackTween.isComplete)) ActivityState = 0;
        } else if (ActivityState == 1) {
            //CloseButt.Alpha = (!IsInertia && !IsMouseDown && (EaseBackTween == null || EaseBackTween.isComplete))?1:0;
        }


    }

    /*public void SetAppearence(bool hasButton){

        for (int i = 0; i < VISIBLE_IMAGES; i++) {
            Buts[i].renderer.material.mainTexture = BottomTextures[(hasButton) ? 1 : 0];
        }
    }*/
    
    public float CenterItemIndex {
        get {
            return _CenterItemIndex;
        }
        set {

            Spinner.transform.localRotation = Quaternion.Euler(0, value * 360 / VISIBLE_IMAGES, 0);


            if (HasAnyData) {

                CenterContentRounded = Mathf.RoundToInt(value);
                while (CenterContentRounded < 0) {
                    CenterContentRounded += ContentCount;
                }
                CenterContentRounded %= ContentCount;

                int CenterItemRounded = Mathf.RoundToInt(value);
                while (CenterItemRounded < 0) {
                    CenterItemRounded += VISIBLE_IMAGES;
                }

                CenterItemRounded %= VISIBLE_IMAGES;


                float CenterItemInRange = value;
                while (CenterItemInRange < 0) {
                    CenterItemInRange += VISIBLE_IMAGES;
                }
                while (CenterItemInRange > VISIBLE_IMAGES) {
                    CenterItemInRange -= VISIBLE_IMAGES;
                }

                for (int i = 0; i < VISIBLE_IMAGES; i++) {
                    Conts[i].transform.rotation = Quaternion.LookRotation(Conts[i].transform.position - Camera.main.transform.position);

                    int neededContentForthisItem = 0;

                    if (CenterItemRounded == i) { //this is the center item
                        neededContentForthisItem = CenterContentRounded;
                    } else {

                        float fDiff = (float)i - CenterItemInRange;
                        int diff = i - CenterItemRounded;
                        if ((fDiff > 0) == (Mathf.Abs(fDiff) < (float)VISIBLE_IMAGES / 2f)) { //to the right of the center content
                            while (diff < 0) diff += VISIBLE_IMAGES;
                        } else {
                            while (diff > 0) diff -= VISIBLE_IMAGES;
                        }

                        neededContentForthisItem = CenterContentRounded + diff;
                        while (neededContentForthisItem < 0) {
                            neededContentForthisItem += ContentCount;
                        }
                        neededContentForthisItem %= ContentCount;

                        if (i == 2) {
                            DebugText = diff.ToString();
                        }
                    }

                    if (ContentByItemID[i] != neededContentForthisItem) {

                        Items[i].GetComponent<Renderer>().material.mainTexture = GalleryTextures[neededContentForthisItem];

                        /*if (GalleryTextures[neededContentForthisItem] is MovieTexture) {
                            Items[i].renderer.material.mainTexture = ThumbsTextures[neededContentForthisItem];
                        } else {
                            Items[i].renderer.material.mainTexture = GalleryTextures[neededContentForthisItem];

                        }*/

                        /*bool hasLinebreak = Titles[neededContentForthisItem].IndexOf("\n"[0]) > -1;

                        Texts[i].transform.localPosition = new Vector3(-Buts[i].transform.localScale.x / 2 + Texts[i].transform.localScale.x / 2 + 0.1f, Buts[i].transform.localPosition.y - ((hasLinebreak) ? -0.22f : -0.17f), -0.1f);

                        Texts[i].text = Titles[neededContentForthisItem];*/

                        if (GalleryTextures[neededContentForthisItem] != null) {
                            Items[i].transform.localScale = new Vector3((float)GalleryTextures[neededContentForthisItem].width / (float)GalleryTextures[neededContentForthisItem].height, 1, 1);
                        }

                        ContentByItemID[i] = neededContentForthisItem;


                    }
                }

                UpdateCloseButtPosition(CenterItemRounded);
            }

            
            _CenterItemIndex = value;

        }
    }

    private void UpdateCloseButtPosition(int target) {
        CloseButt.Position = Camera.main.WorldToScreenPoint(Conts[target].transform.position + new Vector3(Items[target].transform.localScale.x / 2, 0.5f, 0));
        
        float groupScale = Screen.height / (float)SRGUIManager.UiHeight;
        groupScale = Mathf.Min(groupScale, Screen.width / (float)SRGUIManager.UiWidth);
        groupScale *= 1.3f;

        CloseButt.Position += new Vector2(-50 * groupScale, -30 * groupScale + (0.5f * (Screen.height - groupScale * (float)SRGUIManager.UiHeight)));
        CloseButt.Scale = new Vector2(groupScale, groupScale);

    }

    internal void DoFade(float time, float delay, bool isIn) {
        //if (!HasAnyData) return;

        Color fromCol = (isIn) ? AlphaColorTransparent : AlphaColorOpaque;
        Color toCol = (isIn) ? AlphaColorOpaque : AlphaColorTransparent;


        for (int i = 0; i < VISIBLE_IMAGES; i++) {
            Items[i].GetComponent<Renderer>().material.color = fromCol;
            HOTween.To(Items[i].GetComponent<Renderer>().material, time, new TweenParms().Prop("color", toCol).Delay(delay));
        }

    }

    public void GrowEneded() {
        CenterItemIndex = CenterItemIndex;
    }

    public void setActive(bool state) {
        if (state) {
            ActivityState = 1;
        } else {
            if (ActivityState == 1) {
                ActivityState = 2;
            }
        }
    }

    internal void CloseLarge() {
        CloseLargeButt.Enabled = false;
        LargeBG.Alpha = 1;
        LargeImage.Alpha = 1;
        //LargeImage.Scale = new Vector2(1, 1);
        HOTween.To(LargeBG, 0.6f, "Alpha", 0);
        //HOTween.To(LargeImage, 0.3f, "Scale", new Vector2(0.6f, 0.6f));
        HOTween.To(LargeImage, 0.6f, "Alpha", 0);
        Invoke("DissLargeCont", 0.6f);
        /*if (GalleryTextures[CenterContentRounded] is MovieTexture) {
            (GalleryTextures[CenterContentRounded] as MovieTexture).Stop();
        }*/
    }

    private void DissLargeCont() {
        LargeImageCont.Enabled = false;
    }

    internal void LoadAssets(bool doLoad) {
        if (doLoad) {
            /*Texture[] loadedTextures = Resources.LoadAll<Texture>(TagetFolder);

            int moviecount = 0;
            for (int i = 0; i < loadedTextures.Length; i++) {
                if (loadedTextures[i] is MovieTexture) moviecount++;
            }

            if (moviecount > 0) {
                GalleryTextures = new Texture[loadedTextures.Length - moviecount];
                ThumbsTextures = new Texture[loadedTextures.Length - moviecount];

                int actualCount = 0;

                for (int i = 0; i < loadedTextures.Length; i++) {

                    GalleryTextures[actualCount] = loadedTextures[i];

                    if (loadedTextures[i] is MovieTexture) {
                        i++;
                        ThumbsTextures[actualCount] = loadedTextures[i];
                    }

                    actualCount++;
                }

            } else {
                GalleryTextures = loadedTextures;
            }*/

            GalleryTextures = Resources.LoadAll<Texture>(TagetFolder);


        } else {
            Loaded = false;
            for (int i = 0; i < GalleryTextures.Length; i++) {
                Resources.UnloadAsset(GalleryTextures[i]);
            }
            for (int i = 0; i < VISIBLE_IMAGES; i++) {
                ContentByItemID[i] = -1;
            }
        }
    }
}
