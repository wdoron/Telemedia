using UnityEngine;
using System.Collections;
using System.Xml;
using Holoville.HOTween;
using System.Collections.Generic;

public class BasePage : MonoBehaviour {

    public enum TransitionTypes { LocalPosition3D, PositionGUI, ScaleGUI, Alpha, Alpha3D };
    public enum TitleTypes { Center, Left, InPath, LargeInPath };

    public delegate void NavigationCallback(string targetID, Object extraParams, string extradata);
    public event NavigationCallback OnNavigate;

    public string ID;

    public Texture BackgroundTexture;

    public float EnterTime = 1;
    public float ExitTime = 1;
    public float StartEnterAnticipation = 0.5f;

    internal Color AlphaColorOpaque = new Color(1, 1, 1, 1);
    internal Color AlphaColorTransparent = new Color(1, 1, 1, 0);
    //internal Color AlphaColorTransparent = new Color(1, 1, 1, 1);

    private CommonAssetHolder _CommonAssetHolder;
    private SRGUIManager _SRGUIManager;

    private List<TransitionData> Transitions = new List<TransitionData>();

    internal List<Tweener> TransitionTweens = new List<Tweener>();

    internal bool NavigationEnabled = false;


	// Use this for initialization
	void Awake () {
        DoInternalAwake();
	}

    internal virtual void InitPage(XmlNode pageData) {
        gameObject.SetActive(false);

    }

    protected virtual void DoInternalAwake() {

    }
	
	// Update is called once per frame
	void Update () {
        DoInternalUpdate();
	}


    protected void RegisterTransition(TransitionData transitionData, object target) {
        transitionData.Target = target;
        Transitions.Add(transitionData);
    }

    protected virtual void DoInternalUpdate() {
        
    }

    public virtual void DoEnter(float previousDelay,  Object extraParams, string extraData = null) {
        ClearTransitionTweens();
        float holdupDelay = previousDelay - StartEnterAnticipation;

        foreach (TransitionData transitiondata in Transitions) {

            TweenParms tweenParams = new TweenParms();
            TweenParms clickTweenParams = new TweenParms();

            object tweenTarget = transitiondata.Target;

            foreach (BasePage.TransitionTypes type in transitiondata.Types) {
                switch (type) {
                    case TransitionTypes.Alpha:
                        transitiondata.Target.GetType().GetField("Alpha").SetValue(transitiondata.Target, 0);
                        tweenParams = tweenParams.Prop("Alpha", 1);
                        break;
                    case TransitionTypes.PositionGUI:
                        transitiondata.Target.GetType().GetField("Position").SetValue(transitiondata.Target, transitiondata.Target.GetType().GetField("OutPosition").GetValue(transitiondata.Target));
                        
                        tweenParams = tweenParams.Prop("Position", transitiondata.Target.GetType().GetField("InPosition").GetValue(transitiondata.Target));
                        if (transitiondata.IsClick) {
						    clickTweenParams = clickTweenParams.Prop("Position", transitiondata.Target.GetType().GetField("ClickInPosition").GetValue(transitiondata.Target));
                        }
                        break;
                    case TransitionTypes.ScaleGUI:
                        transitiondata.Target.GetType().GetField("Scale").SetValue(transitiondata.Target, transitiondata.Target.GetType().GetField("OutScale").GetValue(transitiondata.Target));
                       
                        tweenParams = tweenParams.Prop("Scale", transitiondata.Target.GetType().GetField("InScale").GetValue(transitiondata.Target));
                        if (transitiondata.IsClick) {
						    clickTweenParams = clickTweenParams.Prop("Scale", transitiondata.Target.GetType().GetField("ClickInScale").GetValue(transitiondata.Target));
                        }
                        break;
                    case TransitionTypes.LocalPosition3D:
                        (transitiondata.Target as GameObject).transform.localPosition = transitiondata.LocalPositions3D[1];

                        tweenParams = tweenParams.Prop("localPosition", transitiondata.LocalPositions3D[0]);
                        if (transitiondata.IsClick) {
                            clickTweenParams = clickTweenParams.Prop("localPosition", transitiondata.LocalPositions3D[2]);
                        }

                        tweenTarget = (transitiondata.Target as GameObject).transform;
                        break;
                    case TransitionTypes.Alpha3D:
                        (transitiondata.Target as GameObject).GetComponent<Renderer>().material.color = AlphaColorTransparent;
                        tweenParams = tweenParams.Prop("color", AlphaColorOpaque);
                        tweenTarget = (transitiondata.Target as GameObject).GetComponent<Renderer>().material;
                        break;
                }
            }

            
            if (transitiondata.IsClick) {
                ClickTransition(tweenTarget, transitiondata.Time, tweenParams, clickTweenParams, transitiondata.ClickTotalTime, holdupDelay + transitiondata.InDelay, true);
            } else {
                tweenParams = tweenParams.Delay(holdupDelay + transitiondata.InDelay).Ease(EaseType.Linear);
                TransitionTweens.Add(HOTween.To(tweenTarget, transitiondata.Time, tweenParams));
            }
        }

    }

    public virtual void DoExit(string targetID) {
        ClearTransitionTweens();

        foreach (TransitionData transitiondata in Transitions) {

            TweenParms tweenParams = new TweenParms();
            TweenParms clickTweenParams = new TweenParms();

            object tweenTarget = transitiondata.Target;

            foreach (BasePage.TransitionTypes type in transitiondata.Types) {
                switch (type) {
                    case TransitionTypes.Alpha:
                        transitiondata.Target.GetType().GetField("Alpha").SetValue(transitiondata.Target, 1);
                        tweenParams = tweenParams.Prop("Alpha", 0);
                        break;
                    case TransitionTypes.PositionGUI:
                        transitiondata.Target.GetType().GetField("Position").SetValue(transitiondata.Target, transitiondata.Target.GetType().GetField("InPosition").GetValue(transitiondata.Target));
                        
                        tweenParams = tweenParams.Prop("Position", transitiondata.Target.GetType().GetField("OutPosition").GetValue(transitiondata.Target));
                        if (transitiondata.IsClick) {
						    clickTweenParams = clickTweenParams.Prop("Position", transitiondata.Target.GetType().GetField("ClickInPosition").GetValue(transitiondata.Target));
                        }
                        break;
                    case TransitionTypes.ScaleGUI:
                        transitiondata.Target.GetType().GetField("Scale").SetValue(transitiondata.Target, transitiondata.Target.GetType().GetField("InScale").GetValue(transitiondata.Target));
                        
                        tweenParams = tweenParams.Prop("Scale", transitiondata.Target.GetType().GetField("OutScale").GetValue(transitiondata.Target));
                        if (transitiondata.IsClick) {
						    clickTweenParams = clickTweenParams.Prop("Scale", transitiondata.Target.GetType().GetField("ClickInScale").GetValue(transitiondata.Target));
                        }
                        break;
                    case TransitionTypes.LocalPosition3D:
                        (transitiondata.Target as GameObject).transform.localPosition = transitiondata.LocalPositions3D[0];

                        tweenParams = tweenParams.Prop("localPosition", transitiondata.LocalPositions3D[1]);
                        if (transitiondata.IsClick) {
                            clickTweenParams = clickTweenParams.Prop("localPosition", transitiondata.LocalPositions3D[2]);
                        }
                        tweenTarget = (transitiondata.Target as GameObject).transform;
                        break;
                    case TransitionTypes.Alpha3D:
                        (transitiondata.Target as GameObject).GetComponent<Renderer>().material.color = AlphaColorOpaque;
                        tweenParams = tweenParams.Prop("color", AlphaColorTransparent);
                        tweenTarget = (transitiondata.Target as GameObject).GetComponent<Renderer>().material;
                        break;
                }
            }


            float timeToOUt = ((transitiondata.OutTime >= 0) ? transitiondata.OutTime : transitiondata.Time);

            if (transitiondata.IsClick) {
                ClickTransition(tweenTarget, timeToOUt, tweenParams, clickTweenParams, transitiondata.ClickTotalTime, transitiondata.OutDelay, false);
            } else {
                tweenParams = tweenParams.Delay(transitiondata.OutDelay).Ease(EaseType.Linear);
                TransitionTweens.Add(HOTween.To(tweenTarget, timeToOUt, tweenParams));
            }
        }
    }

    internal void DipatchNavigate(string targetID, Object extraParams, string extraData = null) {
        if (NavigationEnabled) {
            OnNavigate(targetID, extraParams, extraData);
        }
    }

    protected void ClickTransition(object target, float linearTime, TweenParms noDelayNoEaseVars, TweenParms noDelayNoRepeatClickVars, float clickTotalTime, float delay, bool isIn) {
        if (isIn) {
            TransitionTweens.Add(HOTween.To(target, linearTime, noDelayNoEaseVars.Delay(delay).Ease(EaseType.Linear)));
            TransitionTweens.Add(HOTween.To(target, clickTotalTime / 2, noDelayNoRepeatClickVars.Delay(delay + linearTime).Loops(2, LoopType.Yoyo)));
        } else {
            TransitionTweens.Add(HOTween.To(target, clickTotalTime / 2, noDelayNoRepeatClickVars.Delay(delay).Loops(2, LoopType.Yoyo)));
            TransitionTweens.Add(HOTween.To(target, linearTime, noDelayNoEaseVars.Delay(delay + clickTotalTime).Ease(EaseType.Linear)));

        }
    }

    private void ClearTransitionTweens() {
        foreach (Tweener tweener in TransitionTweens) {
            if (!tweener.isComplete) {
                tweener.Kill();
            }
        }
        TransitionTweens.Clear();
    }

    void OnGUI() {
        GUIManager.Draw(this.gameObject);
        GUIManager.Draw(Management.Instance.gameObject);
    }

    protected CommonAssetHolder AssetHolder {
        get {
            if (_CommonAssetHolder == null) {
                _CommonAssetHolder = CommonAssetHolder.instance;
            }
            return _CommonAssetHolder;
        }
    }

    protected SRGUIManager GUIManager {
        get {
            if (_SRGUIManager == null) {
                _SRGUIManager = SRGUIManager.instance;
            }
            return _SRGUIManager;
        }
    }

    internal virtual void ClickReaction(SRBaseGUIElement caller) {
        if (Management.Instance.IsOpen) return;
        if (caller.destination != null) DipatchNavigate(caller.destination, null);
        if (caller is SRGUIButton && (caller as SRGUIButton).HasSound) SoundController.instance.playSingleSound(SoundController.SoundT.TopMenuSelection);

    }

    internal virtual void InputChangedReaction(SRGUIInput caller) {

    }

    internal void RegisterListeners() {
        GUIManager.Click += ClickReaction;
        GUIManager.InputChanged += InputChangedReaction;
    }

    internal void UnRegisterListeners() {
        GUIManager.Click -= ClickReaction;
        GUIManager.InputChanged -= InputChangedReaction;
    }

    internal Vector2 LogoSmallPosition = new Vector2(-54, 85);
    internal Vector2 LogoSmallScale = new Vector2(0.605f, 0.605f);

    internal SRGUITexture createStandardLogo() {
        SRGUITexture Logo = new SRGUITexture();
        Logo.SetTexture(AssetHolder.Logo);

        Logo.Position = LogoSmallPosition;
        Logo.Scale = LogoSmallScale;

        Logo.ClickInPosition = new Vector2(5, 55);
        GUIManager.RegisterGUIElement(Logo, gameObject);

        /*TransitionData transitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.PositionGUI }, 0.4f, 0.5f, ExitTime - 0.4f);
        transitionData.SetClick(0.2f);
        RegisterTransition(transitionData, Logo);*/

        SRGUIButton LogoButt = new SRGUIButton();
        LogoButt.Style = new GUIStyle();
        LogoButt.Position = Logo.Position;
        LogoButt.setCustomSize(Logo.Size);

        LogoButt.destination = PageManager.ROOT_DESTINATION;
        GUIManager.RegisterGUIElement(LogoButt, gameObject);

        TransitionData transitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha }, 0f, 0f, ExitTime);
        RegisterTransition(transitionData, Logo);

        return Logo;
    }

    internal SRGUILabel createTitlePath(string pathString, TitleTypes titleType) {

        TransitionData transitionData;

        string[] path = pathString.Split("%"[0]);

        int lablesInTopPath = (titleType == TitleTypes.InPath) ? path.Length : (path.Length - 1);

        //last item
        SRGUILabel pathTitle = new SRGUILabel();
        pathTitle.Text = path[lablesInTopPath - 1];
        pathTitle.Style = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.CorpBold, 42, true);

        SRGUITexture arrowTexture = new SRGUITexture();
        arrowTexture.SetTexture(AssetHolder.PathArrowTextures[1]);
        arrowTexture.Position = new Vector2(pathTitle.Size.x + 25 , 8);

        SRGUIContainer lastItemCont = new SRGUIContainer();
        lastItemCont.InPosition = new Vector2(422, 168);
        lastItemCont.OutPosition = lastItemCont.InPosition + new Vector2(0, -10);

        lastItemCont.children.Add(pathTitle);
        lastItemCont.children.Add(arrowTexture);

        GUIManager.RegisterGUIElement(lastItemCont, gameObject);

        transitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha, TransitionTypes.PositionGUI }, 0.2f, 0.2f, 0.2f);
        RegisterTransition(transitionData, lastItemCont);

        float currentX = 422;

        for (int i = 0; i < lablesInTopPath - 1; i++){
            pathTitle = new SRGUILabel();
            pathTitle.Text = path[i];
            pathTitle.Style = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.FrutiGray, 36, false);

            arrowTexture = new SRGUITexture();
            arrowTexture.SetTexture(AssetHolder.PathArrowTextures[0]);
            arrowTexture.Position = new Vector2(pathTitle.Size.x + 25 , 12);

            SRGUIContainer cont = new SRGUIContainer();
            cont.InPosition = new Vector2(currentX, 103);
            cont.OutPosition = cont.InPosition + new Vector2(0, -10);
            cont.children.Add(pathTitle);
            cont.children.Add(arrowTexture);

            GUIManager.RegisterGUIElement(cont, gameObject);

            transitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha, TransitionTypes.PositionGUI }, 0.2f, 0f, 0.4f);
            RegisterTransition(transitionData, cont);

            currentX += pathTitle.Size.x + 60;
        }


        if (titleType != TitleTypes.InPath) {
            pathTitle = new SRGUILabel();
            pathTitle.Text = path[path.Length - 1];
            pathTitle.Style = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.CorpBold, 69, true);

            float yPos = 330 - pathTitle.Size.y / 2;

            if (titleType == TitleTypes.LargeInPath) {
                pathTitle.InPosition = new Vector2(lastItemCont.InPosition.x + lastItemCont.Size.x + 150, lastItemCont.InPosition.y - 18);
                pathTitle.OutPosition = pathTitle.InPosition + new Vector2(0, -10);
            } else {
                pathTitle.InPosition = new Vector2(200, yPos);
                pathTitle.OutPosition = pathTitle.InPosition + new Vector2(0, 10);
            }

            

            GUIManager.RegisterGUIElement(pathTitle, gameObject);

            transitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha, TransitionTypes.PositionGUI }, 0.2f, 0f, 0.3f);
            RegisterTransition(transitionData, pathTitle);

            return pathTitle;
        }

        return null;
    }

    internal SRGUIButton createStandardBack() {
        SRGUIButton BackButt = new SRGUIButton();
        BackButt.Position = new Vector2(1723, 942);

        
        BackButt.Style = AssetHolder.GetCustomStyle(AssetHolder.BackButtTexture, CommonAssetHolder.FontNameType.FrutiWhite, 25, false, false, true);

        BackButt.destination = PageManager.BACK_DESTINATION;
        //BackButt.Text = "Back";
        GUIManager.RegisterGUIElement(BackButt, gameObject);

        TransitionData transitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha }, 0.2f, 0f, ExitTime - 0.5f);
        RegisterTransition(transitionData, BackButt);

        return BackButt;
    }

    internal void RedistributeLabelsY(ISRGUIDistributable[][] distributablesGroups, LabelDistributeData[] distributeDatas, float startFrom, float absoluteMaximum, int dynamicGroupIndex = -1) {
        float totalSize = 0;
        float dynamicSpaceSize = 0;
        float textOnlySize = 0;
        for (int i = 0; i < distributablesGroups.Length; i++) {
            bool isDynamic = (i == dynamicGroupIndex);

            foreach (ISRGUIDistributable distributable in distributablesGroups[i]) {
                totalSize += distributable.GetSize().y;
                textOnlySize += distributable.GetSize().y;
                
            }

            float spacesSpace = Mathf.Max(0, distributeDatas[i].SpaceBetweenItems * distributablesGroups[i].Length - 1);

            totalSize += spacesSpace;
            if (isDynamic) {
                dynamicSpaceSize += spacesSpace;
            }

            totalSize += distributeDatas[i].LeadingSpace;
        }


        //ASJUST SPACE
        if (totalSize > absoluteMaximum) { //scale down needed
            /*float scaleRat = (textOnlySize - (totalSize - absoluteMaximum)) / textOnlySize; //only text size, relative to only text substructed the diffrence
            for (int i = 0; i < distributablesGroups.Length; i++) {
                foreach (IDistributable distributable in distributablesGroups[i]) {
                    distributable.GetLabel().Style.fontSize = Mathf.FloorToInt(scaleRat * distributable.GetLabel().Style.fontSize);
                }
            }*/


        } else {
            if (dynamicGroupIndex > -1){ //space up dynamic group
                float freeSpace = absoluteMaximum - totalSize;
                float targetSpace = (freeSpace + dynamicSpaceSize) / (float)(distributablesGroups[dynamicGroupIndex].Length - 1);
                if (targetSpace > distributeDatas[dynamicGroupIndex].SpaceBetweenItems) {
                    distributeDatas[dynamicGroupIndex].SpaceBetweenItems = targetSpace;
                    if (distributeDatas[dynamicGroupIndex].MaxSpaceBetweenItems > 0) {
                        distributeDatas[dynamicGroupIndex].SpaceBetweenItems = Mathf.Min(distributeDatas[dynamicGroupIndex].SpaceBetweenItems, distributeDatas[dynamicGroupIndex].MaxSpaceBetweenItems);
                    }
                }
            }
        }


        //REPOSITION
        float currentPos = startFrom;
        for (int i = 0; i < distributablesGroups.Length; i++) {
            currentPos += distributeDatas[i].LeadingSpace;
            if (distributablesGroups[i].Length > 0) currentPos -= distributeDatas[i].SpaceBetweenItems; //remove the first item gap

            foreach (ISRGUIDistributable distributable in distributablesGroups[i]) {

                currentPos += distributeDatas[i].SpaceBetweenItems;

                float travelDist = (distributable as SRBaseGUIElement).OutPosition.y - (distributable as SRBaseGUIElement).InPosition.y;

                (distributable as SRBaseGUIElement).InPosition.y = currentPos;
                (distributable as SRBaseGUIElement).OutPosition.y = (distributable as SRBaseGUIElement).InPosition.y + travelDist;
                (distributable as SRBaseGUIElement).Position.y = currentPos;

                currentPos += distributable.GetSize().y;
            }

        }

    }
}

public class TransitionData {
    public BasePage.TransitionTypes[] Types;
    public float Time;
    public float InDelay;
    public float OutDelay;
    public float OutTime;

    public float ClickTotalTime;
    public bool IsClick = false;

    public object Target;

    public Vector3[] LocalPositions3D;

    public TransitionData(BasePage.TransitionTypes[] types, float time, float inDelay, float outDelay) {
        Types = types;
        Time = time;
        InDelay = inDelay;
        OutDelay = outDelay;
        OutTime = -1;
    }

    public void SetClick(float clickTotalTime) {
        ClickTotalTime = clickTotalTime;
        IsClick = true;
    }
}

public class LabelDistributeData {
    public float LeadingSpace = 0;
    public float SpaceBetweenItems = 0;
    public float MaxSpaceBetweenItems = 0;
}
