using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using Holoville.HOTween;

public class PageMeetingSummary : BasePage {

    public Texture2D[] PageArrowsText;
    public Texture2D BackText;
    public Texture2D BulletArrowText;
    public Texture2D[] IconsText;
    public Texture2D[] BulletTexts;

    private const int COLLUMNS = 3;
    private float collGap = 550;
    private float collOffset = 195;
    private float labelSize = 450;

    private int maxRows = 8;

    private int ItemFontSize = 30;

    private SRGUILabel[] selectedTitles;
    private SRGUITexture[] selectedIcons;
    //private List<SRGUIContainer>[] selectedItems;
    private int _Page;

    private List<SRGUITexture> PageBullets = new List<SRGUITexture>();

    private int totalPages;

    private SRGUIContainer MainCont;

    private SRGUIButton[] pageButtons = new SRGUIButton[2];

    private List<SummeryCollumnData> CollumnData;

    private SRGUILabel TestLabel;
    
    private int FreeContCount;
    
    private List<SummeryItemCont> FreeConts = new List<SummeryItemCont>();
    private SRGUITexture Logo;
    

    override internal void InitPage(XmlNode pageData) {
        ExitTime = 1;

        //logo
        Logo = createStandardLogo();

        //back butt
        SRGUIButton back = createStandardBack();
        back.destination = PageManager.ROOT_DESTINATION;


        TransitionData transData;

        SRGUILabel title = new SRGUILabel();
        title.Text = "Meeting Summary";
        title.Style = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.CorpBold, 69, true);

        title.Position = new Vector2(400, 150);
        title.InPosition = new Vector2(400, 150);
        title.OutPosition = title.InPosition + new Vector2(0, 10);
        GUIManager.RegisterGUIElement(title, gameObject);
        transData = new TransitionData(new TransitionTypes[] { TransitionTypes.PositionGUI, TransitionTypes.Alpha }, 0.2f, 0, 0);
        RegisterTransition(transData, title);

        SRGUITexture backText = new SRGUITexture();
        backText.Position = new Vector2(56, 253);
        backText.SetTexture(BackText);
        GUIManager.RegisterGUIElement(backText, gameObject);
        transData = new TransitionData(new TransitionTypes[] {TransitionTypes.Alpha }, 0.2f, 0, 0);
        RegisterTransition(transData, backText);

        selectedTitles = new SRGUILabel[COLLUMNS];
        //selectedItems = new List<SRGUIContainer>[COLLUMNS];
        selectedIcons = new SRGUITexture[COLLUMNS];

        for (int i = 0; i < COLLUMNS; i++) {
            selectedTitles[i] = new SRGUILabel();
            selectedTitles[i].Style = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.CorpBold, 42);
            selectedTitles[i].Position = new Vector2(collOffset + collGap * i, 425);
            GUIManager.RegisterGUIElement(selectedTitles[i], gameObject);

            selectedIcons[i] = new SRGUITexture();
            selectedIcons[i].Enabled = false;
            GUIManager.RegisterGUIElement(selectedIcons[i], gameObject);

        }

        TestLabel = new SRGUILabel();
        TestLabel.Style = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.FrutiBlack80, ItemFontSize);

        MainCont = new SRGUIContainer();
        GUIManager.RegisterGUIElement(MainCont, gameObject);


        pageButtons[0] = new SRGUIButton();
        pageButtons[0].Style = AssetHolder.GetCustomStyle(PageArrowsText[0]);
        pageButtons[0].Position = new Vector2(1750, 550);
        GUIManager.RegisterGUIElement(pageButtons[0], gameObject);
        transData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha }, 0.2f, 0, 0);
        RegisterTransition(transData, pageButtons[0]);

        pageButtons[1] = new SRGUIButton();
        pageButtons[1].Style = AssetHolder.GetCustomStyle(PageArrowsText[1]);
        pageButtons[1].Position = new Vector2(100, 550);
        GUIManager.RegisterGUIElement(pageButtons[1], gameObject);
        transData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha }, 0.2f, 0, 0);
        RegisterTransition(transData, pageButtons[1]);

    }

    override public void DoEnter(float previousDelay, Object extraParams, string extraData = null) {
        base.DoEnter(previousDelay, extraParams, extraData);

        CollumnData = new List<SummeryCollumnData>();

        string[][] status = StatusManager.Instance.getFullStatus();
        
        foreach (string[] statuses in status) {
            
            SummeryCollumnData colData = new SummeryCollumnData();
            colData.title = statuses[0];
            
            for (int i = 1; i < statuses.Length; i++) {
                
                colData.items.Add(statuses[i]);

                colData.rows += GetNumOfRows(statuses[i]);

                if (colData.rows >= maxRows && i < statuses.Length - 1) {
                    CollumnData.Add(colData);
                    colData = new SummeryCollumnData();
                }
            }

            if (colData.items.Count > 0) CollumnData.Add(colData);
        }


        totalPages = Mathf.CeilToInt((float)CollumnData.Count / (float)COLLUMNS);

        
        for (int i = 0; i < Mathf.Max(PageBullets.Count, totalPages); i++) {
            if (i > totalPages - 1) { // unneeded bullets
                PageBullets[i].Enabled = false;
            } else { 
                if (i == PageBullets.Count) { // need more bullet
                    SRGUITexture bullet = new SRGUITexture();
                    GUIManager.RegisterGUIElement(bullet, gameObject);
                    PageBullets.Add(bullet);
                }

                PageBullets[i].SetTexture(BulletTexts[0]);
                PageBullets[i].Enabled = true;

                PageBullets[i].Position = new Vector2(950 - 30 * totalPages / 2 + i * 30, 960);
            }
        }


        Page = 0;

        foreach (SummeryItemCont freecont in FreeConts) {
            freecont.cont.Alpha = 0;
            TransitionTweens.Add(HOTween.To(freecont.cont, 0.1f, new TweenParms().Prop("Alpha", 1).Delay(0.2f)));
        }

        for (int i = 0; i < COLLUMNS; i++) {
            selectedTitles[i].Alpha = 0;
            TransitionTweens.Add(HOTween.To(selectedTitles[i], 0.1f, new TweenParms().Prop("Alpha", 1).Delay(0.2f)));
            selectedIcons[i].Alpha = 0;
            TransitionTweens.Add(HOTween.To(selectedIcons[i], 0.1f, new TweenParms().Prop("Alpha", 1).Delay(0.2f)));
        }

    }

    override public void DoExit(string targetID) {
        base.DoExit(targetID);
        foreach (SummeryItemCont freecont in FreeConts) {
            freecont.cont.Alpha = 1;
            TransitionTweens.Add(HOTween.To(freecont.cont, 0.1f, new TweenParms().Prop("Alpha", 0).Delay(0.0f)));
        }

        for (int i = 0; i < COLLUMNS; i++) {
            selectedTitles[i].Alpha = 1;
            TransitionTweens.Add(HOTween.To(selectedTitles[i], 0.1f, new TweenParms().Prop("Alpha", 0).Delay(0f)));
            selectedIcons[i].Alpha = 1;
            TransitionTweens.Add(HOTween.To(selectedIcons[i], 0.1f, new TweenParms().Prop("Alpha", 0).Delay(0f)));
        }

        Invoke("ClearLogo", ExitTime - 0.5f);
        
    }

    private void ClearLogo() {
        Logo.Alpha = 0;
    }

    public int Page { 
        get{
            return _Page;
        }
        set{
            
            MainCont.children.Clear();
            FreeContCount = 0;

            float posX = 0;
            float posY = 0;

            for (int i = 0; i < COLLUMNS; i++) {

                posX = collOffset + collGap * i;
                posY = 485;

                int index = i + value * COLLUMNS;

                if (index >= CollumnData.Count) {
                    selectedTitles[i].Text = "";
                    selectedIcons[i].Enabled = false;
                } else {

                    if (CollumnData[index].title != "") {
                        selectedTitles[i].Text = CollumnData[index].title;

                        Texture texture = null;

                        switch (selectedTitles[i].Text) {
                            case "FBO":
                                texture = IconsText[0];
                                break;
                            case "Buying & Selling":
                                texture = IconsText[1];
                                break;
                            case "MRO":
                                texture = IconsText[2];
                                break;
                            case "System upgrade":
                                texture = IconsText[3];
                                break;
                            case "Cabin interior":
                                texture = IconsText[4];
                                break;
                            case "Painting":
                                texture = IconsText[5];
                                break;
                            default:
                                texture = IconsText[0];
                                break;
                        }

                        selectedIcons[i].Enabled = true;
                        selectedIcons[i].SetTexture(texture);

                        selectedIcons[i].Position = new Vector2(posX + 20, 410 - texture.height);
                    } else {
                        selectedTitles[i].Text = "";
                        selectedIcons[i].Enabled = false;
                    }


                    for (int j = 0; j < CollumnData[index].items.Count; j++) {
                        
                        SummeryItemCont itemCont = GetNextcont();
                        MainCont.children.Add(itemCont.cont);

                        itemCont.cont.Position = new Vector2(posX, posY);
                        itemCont.label.IsCustomSize = false;

                        int rows = GetNumOfRows(CollumnData[index].items[j]);

                        itemCont.label.Text = "O";
                        int cCount = 1;
                        //while (cCount < Mathf.CeilToInt((float)CollumnData[index].items[j].Length / (float)MaxCharsInRow)){
                        while (cCount < rows) {
                            itemCont.label.Text = itemCont.label.Text + "\nO";
                            cCount++;
                        }

                        posY += itemCont.label.Size.y + 11;
                        itemCont.label.Text = CollumnData[index].items[j];
                        itemCont.label.IsCustomSize = true;
                    }

                }
            }

            for (int i = 0; i < totalPages; i++) {
                PageBullets[i].SetTexture(BulletTexts[(i == value) ? 1 : 0]);
            }

            pageButtons[0].Enabled = value < totalPages - 1;
            pageButtons[1].Enabled = value > 0;

            _Page = value;
        }
    }

    private int GetNumOfRows(string targetText) {
        TestLabel.IsCustomSize = false;
        string[] split = targetText.Split("\n"[0]);

        int rows = 0;
        for (int i = 0; i < split.Length; i++) {
            TestLabel.Text = split[i];
            rows += Mathf.CeilToInt(TestLabel.Size.x / labelSize);
        }
        
        return rows;
    }

    private SummeryItemCont GetNextcont() {
        FreeContCount++;
        if (FreeContCount - 1 == FreeConts.Count){

            SummeryItemCont cont = new SummeryItemCont();
            cont.cont = new SRGUIContainer();
            cont.label = new SRGUILabel();
            cont.label.Style = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.FrutiBlack80, ItemFontSize);
            cont.label.Position = new Vector2(25, 0);
            cont.label.Style.wordWrap = true;
            cont.label.setCustomSize(new Vector2(labelSize, 100));
            cont.cont.children.Add(cont.label);

            SRGUITexture arrow = new SRGUITexture();
            arrow.SetTexture(BulletArrowText);
            arrow.Position = new Vector2(0, 5);
            cont.cont.children.Add(arrow);

            FreeConts.Add(cont);
        }

        return FreeConts[FreeContCount - 1];
    }

    internal override void ClickReaction(SRBaseGUIElement caller) {
        base.ClickReaction(caller);
        if (caller == pageButtons[0]) {
            Page++;
        } else if (caller == pageButtons[1]) {
            Page--;
        }
    }

}

public class SummeryCollumnData {
    public string title = "";
    public List<string> items = new List<string>();
    public int rows = 0;
}

public class SummeryItemCont {

    public SRGUIContainer cont;
    public SRGUILabel label;

}
