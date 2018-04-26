using UnityEngine;
using System.Collections;
using System.Xml;
using Holoville.HOTween;

public class PageFBOMap : BasePage {
    public Texture MainMap;
    public Texture SecMap;

    public Texture ButtText;

    private SRGUIButton[] Buttons;
    private SRGUILabel[] Titles;

    
    private SRGUIButton Back;
    private int _state;

    private SRGUITexture[] Maps;

    private GUIStyle NavigationStyleLeftNormal;
    //private GUIStyle NavigationStyleLeftSelected;
    private SRGUIButton BackButt;
    

    override internal void InitPage(XmlNode pageData) {
        base.InitPage(pageData);

        NavigationStyleLeftNormal = AssetHolder.GetCustomStyle(AssetHolder.NextPageButtTextures[0], CommonAssetHolder.FontNameType.FrutiGray, 42, false, false, true);
        //NavigationStyleLeftSelected = AssetHolder.GetCustomStyle(AssetHolder.NextPageButtTextures[0], CommonAssetHolder.FontNameType.CorpBold, 42, true);

        //back butt
        Back = createStandardBack();

        BackButt = new SRGUIButton();
        BackButt.Position = new Vector2(1723, 942);
        BackButt.Style = AssetHolder.GetCustomStyle(AssetHolder.BackButtTexture, CommonAssetHolder.FontNameType.FrutiWhite, 25, false, false, true);
        GUIManager.RegisterGUIElement(BackButt, gameObject);

        TransitionData transitionData = null;


        //logo
        createStandardLogo();
        Buttons = new SRGUIButton[4]; 

        for (int i = 0; i < 4; i++) {
            Buttons[i] = new SRGUIButton();
            Buttons[i].Style = NavigationStyleLeftNormal;
            
            Buttons[i].Position = new Vector2(1400, 300 + 150 * i);
            Buttons[i].InScale = new Vector2(1, 1);
            Buttons[i].OutScale = new Vector2(0.4f, 0.4f);
            
            GUIManager.RegisterGUIElement(Buttons[i], gameObject);
            transitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha, TransitionTypes.ScaleGUI}, 0.2f, 0.1f * i, ExitTime - 0.5f);
            RegisterTransition(transitionData, Buttons[i]);

        }


        Buttons[0].Text = "Geneva";
        Buttons[1].Text = "Lugano";
        Buttons[2].Text = "Munich";
        Buttons[3].Text = "Worldwide\nBy Execujet";

        Maps = new SRGUITexture[2];

        Maps[0] = new SRGUITexture();
        Maps[0].SetTexture(MainMap);

        Maps[0].InPosition = new Vector2(150, 300);
        Maps[0].OutPosition = Maps[0].InPosition + new Vector2(0, 10);

        transitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha, TransitionTypes.PositionGUI }, 0.2f, 0f, ExitTime - 0.5f);
        RegisterTransition(transitionData, Maps[0]);


        Maps[1] = new SRGUITexture();
        Maps[1].SetTexture(SecMap);

        Maps[1].Position = new Vector2(150, 250);

        GUIManager.RegisterGUIElement(Maps[0], gameObject);
        GUIManager.RegisterGUIElement(Maps[1], gameObject);

        Titles = new SRGUILabel[4];
        Titles[0] = new SRGUILabel();
        Titles[1] = new SRGUILabel();
        Titles[2] = new SRGUILabel();
        Titles[3] = new SRGUILabel();
        Titles[0].Style = Titles[1].Style = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.CorpBold, 69, true);
        Titles[0].InPosition = Titles[1].InPosition = new Vector2(440, 100);
        Titles[0].OutPosition = Titles[1].OutPosition = Titles[0].InPosition + new Vector2(0, 10);
        Titles[2].Style = Titles[3].Style = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.CorpBold, 50, true);
        Titles[2].InPosition = Titles[3].InPosition = new Vector2(440, 170);
        Titles[2].OutPosition = Titles[3].OutPosition = Titles[2].InPosition + new Vector2(0, 10);

        Titles[0].Text = "FBO Locations";
        Titles[1].Text = "Global FBO partnering Network";
        Titles[2].Text = "In the heart of Europe";
        Titles[3].Text = "Powered by Execujet";

        transitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha }, 0.2f, 0f, ExitTime - 0.5f);
        RegisterTransition(transitionData, Titles[0]);
        transitionData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha }, 0.2f, 0f, ExitTime - 0.5f);
        RegisterTransition(transitionData, Titles[2]);
        


        GUIManager.RegisterGUIElement(Titles[0], gameObject);
        GUIManager.RegisterGUIElement(Titles[1], gameObject);
        GUIManager.RegisterGUIElement(Titles[2], gameObject);
        GUIManager.RegisterGUIElement(Titles[3], gameObject);


        State = 0;

    }

    public int State {
        get {
            return _state;
        }
        set {

            for (int i = 0; i < Buttons.Length; i++) {
                Buttons[i].Enabled = true;
                Buttons[i].Alpha = (value == 0) ? 0 : 1;
                HOTween.To(Buttons[i], 0.2f, "Alpha", ((value == 0) ? 1 : 0));
                Buttons[i].Style = NavigationStyleLeftNormal;
            }


            Maps[0].Alpha = (value == 0) ? 0 : 1;
            Maps[1].Alpha = (value == 0) ? 1 : 0;
            HOTween.To(Maps[0], 0.2f, "Alpha", ((value == 0) ? 1 : 0));
            HOTween.To(Maps[1], 0.2f, "Alpha", ((value == 0) ? 0 : 1));

            HOTween.To(Titles[0], 0.2f, "Alpha", ((value == 0) ? 1 : 0));
            HOTween.To(Titles[1], 0.2f, "Alpha", ((value == 0) ? 0 : 1));
            HOTween.To(Titles[2], 0.2f, "Alpha", ((value == 0) ? 1 : 0));
            HOTween.To(Titles[3], 0.2f, "Alpha", ((value == 0) ? 0 : 1));

            HOTween.To(Titles[0], 0.2f, "Position", ((value == 0) ? Titles[0].InPosition : Titles[0].OutPosition));
            HOTween.To(Titles[1], 0.2f, "Position", ((value == 0) ? Titles[1].OutPosition : Titles[1].InPosition));
            HOTween.To(Titles[2], 0.2f, "Position", ((value == 0) ? Titles[2].InPosition : Titles[2].OutPosition));
            HOTween.To(Titles[3], 0.2f, "Position", ((value == 0) ? Titles[3].OutPosition : Titles[3].InPosition));


            Back.Enabled = BackButt.Enabled = true;

            Invoke("EndTransition", 0.2f);


            Back.Enabled = value == 0;
            BackButt.Enabled = value == 1;

            _state = value;
        }
    }

    private void EndTransition() {
        if (State == 1) {
            for (int i = 0; i < Buttons.Length; i++) {
                Buttons[i].Enabled = false;
            }
        }

    }

    internal override void ClickReaction(SRBaseGUIElement caller) {
        base.ClickReaction(caller);

        if (caller == Buttons[0]) {
            DipatchNavigate("FboLocations1", null);
        }else if (caller == Buttons[1]) {
            DipatchNavigate("FboLocations2", null);
        }else if (caller == Buttons[2]) {
            DipatchNavigate("FboLocations3", null);
        }else if (caller == Buttons[3]) {
            State = 1;
        } else if (caller == BackButt) {
            State = 0;
        }
    }
}
