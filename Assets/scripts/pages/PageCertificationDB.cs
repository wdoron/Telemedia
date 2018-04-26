using UnityEngine;
using System.Collections;
using System.Xml;
using System.Linq;
using System;
using System.Collections.Generic;

public class PageCertificationDB : BasePage {

    public TextAsset DBSource;

    private SRGUIContainer TableCont;

    private DatabaseEntry[] Grid;

    private float[] Widths;
    private SRGUIButton[] TitleButts;
    private SRGUILabel[,] Items;
    private int collums = 7;
    private int maxRows = 20;
    private int maxTableHeight = 540;
    private float LeftSpacing = 20;
    private float TopSpacing = 10;
    private SRGUITexture[] CollomBacks;

    private GUIStyle TitleStyle;
    private GUIStyle ItemStyle;

    private SRGUITexture[] SortArrows;

    private SRGUIButton[] pageButtons = new SRGUIButton[2];

    private List<int> pagesCounts = new List<int>();

    public Texture2D[] PageArrowsText;
    public Texture2D[] TableBacks;
    public Texture2D[] ArrowsUpDownTextures;
    
    private List<SRGUITexture> PageBullets = new List<SRGUITexture>();
    public Texture2D[] BulletTexts;

    private  int _page;
    private int lastSortIndex;
    private bool sortDown;

    

    override internal void InitPage(XmlNode pageData) {
        base.InitPage(pageData);

        TransitionData transData;

        TitleStyle = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.FrutiBlack100, 22);
        TitleStyle.wordWrap = true;
        ItemStyle = AssetHolder.GetCustomStyle(CommonAssetHolder.FontNameType.FrutiBlack80, 22);
        ItemStyle.wordWrap = true;

        XmlDocument xmlDB = new XmlDocument();
        xmlDB.LoadXml(DBSource.text);

        Grid = new DatabaseEntry[xmlDB.GetElementsByTagName("record").Count];

        int rCount = 0;
        foreach (XmlNode record in xmlDB.GetElementsByTagName("record")) {
            Grid[rCount] = new DatabaseEntry(record);
            rCount++;
        }


        TableCont = new SRGUIContainer();
        GUIManager.RegisterGUIElement(TableCont, gameObject);
        TableCont.InPosition = new Vector2(50, 250);
        TableCont.OutPosition = TableCont.InPosition + new Vector2(0, -10);

        transData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha, TransitionTypes.PositionGUI }, 0.2f, 0, 0);
        RegisterTransition(transData, TableCont);

        Widths = new float[] { 200, 160, 200, 750, 120, 150, 200 };
        TitleButts = new SRGUIButton[collums];
        Items = new SRGUILabel[collums, maxRows];

        CollomBacks = new SRGUITexture[collums];

        SortArrows = new SRGUITexture[collums];


        float xPos = 0;
        for (int i = 0; i < collums; i++) {
            SRGUIContainer TitleCont = new SRGUIContainer();
            TableCont.children.Add(TitleCont);
            
            TitleButts[i] = new SRGUIButton();
            TitleCont.children.Add(TitleButts[i]);

            SRGUILabel TitleLabel = new SRGUILabel();
            TitleLabel.Style = TitleStyle;
            TitleCont.children.Add(TitleLabel);
            TitleLabel.Position = new Vector2(5, 5);
            TitleLabel.setCustomSize(new Vector2(Widths[i] - LeftSpacing, 70));

            TitleCont.Position = new Vector2(xPos, 0);
            //TitleButts[i].Style = AssetHolder.GetCustomStyle(TableBacks[1 - i % 2], CommonAssetHolder.FontNameType.FrutiBlack80, 10);
            TitleButts[i].Style = AssetHolder.GetCustomStyle(TableBacks[2], CommonAssetHolder.FontNameType.FrutiBlack80, 10);
            TitleButts[i].setCustomSize(new Vector2(Widths[i], 75));
            switch (i) {
                case 0: TitleLabel.Text = "Agency Approval No."; break;
                case 1: TitleLabel.Text = "Minor Change Approval No."; break;
                case 2: TitleLabel.Text = "A/C Type"; break;
                case 3: TitleLabel.Text = "Description"; break;
                case 4: TitleLabel.Text = "Multiple or single"; break;
                case 5: TitleLabel.Text = "Major  Minor STC"; break;
                case 6: TitleLabel.Text = "Approved under"; break;
            }

            CollomBacks[i] = new SRGUITexture();
            CollomBacks[i].SetTexture(TableBacks[i % 2]);
            CollomBacks[i].Position = new Vector2(xPos, 70);
            TableCont.children.Add(CollomBacks[i]);


            for (int j = 0; j < maxRows; j++) {
                Items[i, j] = new SRGUILabel();
                TableCont.children.Add(Items[i, j]);
                Items[i, j].Style = ItemStyle;
                Items[i, j].Position = new Vector2(xPos + LeftSpacing, 0);
            }

            xPos += Widths[i];

            SortArrows[i] = new SRGUITexture();
            SortArrows[i].Position = new Vector2(xPos - 20, 40);
            SortArrows[i].SetTexture(ArrowsUpDownTextures[0]);
            TableCont.children.Add(SortArrows[i]);
        }


        

       

        //back butt
        createStandardBack();


        //path
        createTitlePath(pageData.Attributes.GetNamedItem("path").Value, TitleTypes.InPath);


        //logo
        createStandardLogo();

        
        pageButtons[0] = new SRGUIButton();
        pageButtons[0].Style = AssetHolder.GetCustomStyle(PageArrowsText[0]);
        pageButtons[0].Position = new Vector2(1500, 1000);
        GUIManager.RegisterGUIElement(pageButtons[0], gameObject);
        transData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha }, 0.2f, 0, 0);
        RegisterTransition(transData, pageButtons[0]);

        pageButtons[1] = new SRGUIButton();
        pageButtons[1].Style = AssetHolder.GetCustomStyle(PageArrowsText[1]);
        pageButtons[1].Position = new Vector2(310, 1000);
        GUIManager.RegisterGUIElement(pageButtons[1], gameObject);
        transData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha }, 0.2f, 0, 0);
        RegisterTransition(transData, pageButtons[1]);

        lastSortIndex = -1;
        sortDown = false;

        MakeTable(0);
    }



    private void MakeTable(int sortIndex) {

        if (lastSortIndex == sortIndex) {
            sortDown = !sortDown;
        } else {
            sortDown = true;
        }


        for (int i = 0; i < collums; i++) {
            SortArrows[i].SetTexture(ArrowsUpDownTextures[0]);
        }

        if (sortDown) {

            SortArrows[sortIndex].SetTexture(ArrowsUpDownTextures[1]);

            switch (sortIndex) {
                case 0: Array.Sort(Grid, (a, b) => a.AAN.CompareTo(b.AAN)); break;
                case 1: Array.Sort(Grid, (a, b) => a.MCAN.CompareTo(b.MCAN)); break;
                case 2: Array.Sort(Grid, (a, b) => a.ACT.CompareTo(b.ACT)); break;
                case 3: Array.Sort(Grid, (a, b) => a.DESC.CompareTo(b.DESC)); break;
                case 4: Array.Sort(Grid, (a, b) => a.MOS.CompareTo(b.MOS)); break;
                case 5: Array.Sort(Grid, (a, b) => a.MMSTC.CompareTo(b.MMSTC)); break;
                case 6: Array.Sort(Grid, (a, b) => a.APPB.CompareTo(b.APPB)); break;
            }
        } else {
            SortArrows[sortIndex].SetTexture(ArrowsUpDownTextures[2]);
            switch (sortIndex) {
                case 0: Array.Sort(Grid, (a, b) => b.AAN.CompareTo(a.AAN)); break;
                case 1: Array.Sort(Grid, (a, b) => b.MCAN.CompareTo(a.MCAN)); break;
                case 2: Array.Sort(Grid, (a, b) => b.ACT.CompareTo(a.ACT)); break;
                case 3: Array.Sort(Grid, (a, b) => b.DESC.CompareTo(a.DESC)); break;
                case 4: Array.Sort(Grid, (a, b) => b.MOS.CompareTo(a.MOS)); break;
                case 5: Array.Sort(Grid, (a, b) => b.MMSTC.CompareTo(a.MMSTC)); break;
                case 6: Array.Sort(Grid, (a, b) => b.APPB.CompareTo(a.APPB)); break;
            }
        }

        /*float xPos = 0;
        int k = 0;
        
        while (k <= sortIndex){
            xPos += Widths[k];
            k++;
        }

        SortArrow.Position = new Vector2(xPos - 20, SortArrow.Position.y);*/


        lastSortIndex = sortIndex;

        /*var result = from element in grid
                      orderby element
                      select element;*/


        float totalHeight = 0;
        
        GUIContent testCont = new GUIContent();

        pagesCounts.Clear();
        int itemsOnPageCount = 0;

        for (int i = 0; i < Grid.Length; i++) {
            float maxItemHeight = 0;
            for (int j = 0; j < collums; j++) {
                string text = "";
                switch (j) {
                    case 0: text = Grid[i].AAN; break;
                    case 1: text = Grid[i].MCAN; break;
                    case 2: text = Grid[i].ACT; break;
                    case 3: text = Grid[i].DESC; break;
                    case 4: text = Grid[i].MOS; break;
                    case 5: text = Grid[i].MMSTC; break;
                    case 6: text = Grid[i].APPB; break;
                }
                testCont.text = text;
                maxItemHeight = Mathf.Max(maxItemHeight, ItemStyle.CalcHeight(testCont, Widths[j] - LeftSpacing));
            }


            if (totalHeight + maxItemHeight + TopSpacing > maxTableHeight) {
                totalHeight = maxItemHeight + TopSpacing;
                pagesCounts.Add(itemsOnPageCount);
                itemsOnPageCount = 1;
            }else{
                totalHeight += maxItemHeight + TopSpacing;
                itemsOnPageCount++;
            }
        }

        pagesCounts.Add(itemsOnPageCount);


        for (int i = 0; i < Mathf.Max(PageBullets.Count, pagesCounts.Count); i++) {
            if (i > pagesCounts.Count - 1) { // unneeded bullets
                PageBullets[i].Enabled = false;
            } else {
                if (i == PageBullets.Count) { // need more bullet
                    SRGUITexture bullet = new SRGUITexture();
                    GUIManager.RegisterGUIElement(bullet, gameObject);
                    PageBullets.Add(bullet);
                }

                PageBullets[i].SetTexture(BulletTexts[0]);
                PageBullets[i].Enabled = true;

                PageBullets[i].InPosition = new Vector2(950 - 30 * pagesCounts.Count / 2 + i * 30, 1020);
                PageBullets[i].OutPosition = PageBullets[i].InPosition + new Vector2(0, -10);

                TransitionData transData = new TransitionData(new TransitionTypes[] { TransitionTypes.Alpha, TransitionTypes.PositionGUI }, 0.2f, 0, 0);
                RegisterTransition(transData, PageBullets[i]);

            }
        }


        Page = 0;

    }


    public int Page { 
        get{
            return _page;
        }
        set{
            float yPos = TitleButts[0].Size.y;

            int startItem = 0;
            int p = 0;
            while (p < value) {
                startItem += pagesCounts[p];
                p++;
            }

            float absMaxHeight = 0;

            for (int j = 0; j < maxRows; j++) {
                if (j < pagesCounts[value]) {
                    yPos += TopSpacing;
                    float maxHeight = 0;
                    for (int i = 0; i < collums; i++) {
                        Items[i, j].Enabled = true;
                        switch (i) {
                            case 0: Items[i, j].Text = Grid[j + startItem].AAN; break;
                            case 1: Items[i, j].Text = Grid[j + startItem].MCAN; break;
                            case 2: Items[i, j].Text = Grid[j + startItem].ACT; break;
                            case 3: Items[i, j].Text = Grid[j + startItem].DESC; break;
                            case 4: Items[i, j].Text = Grid[j + startItem].MOS; break;
                            case 5: Items[i, j].Text = Grid[j + startItem].MMSTC; break;
                            case 6: Items[i, j].Text = Grid[j + startItem].APPB; break;
                        }
                        maxHeight = Math.Max(maxHeight, Items[i, j].WrappedHeight(Widths[i] - LeftSpacing));
                    }

                    for (int i = 0; i < collums; i++) {
                        Items[i, j].Position = new Vector2(Items[i, j].Position.x, yPos);
                        Items[i, j].setCustomSize(new Vector2(Widths[i] - LeftSpacing, maxHeight));
                    }

                    yPos += maxHeight;

                    absMaxHeight = Math.Max(yPos, absMaxHeight);
                    
                } else {
                    for (int i = 0; i < collums; i++) {
                        Items[i, j].Enabled = false;
                    }
                }
            }

            for (int i = 0; i < collums; i++) {
                CollomBacks[i].SetcustomSize(new Vector2(Widths[i], absMaxHeight - TitleButts[0].Size.y + 15));
            }


            for (int i = 0; i < pagesCounts.Count; i++) {
                PageBullets[i].SetTexture(BulletTexts[(i == value) ? 1 : 0]);
            }

            pageButtons[0].Enabled = value < pagesCounts.Count - 1;
            pageButtons[1].Enabled = value > 0;


            _page = value;
        }
    }

     internal override void ClickReaction(SRBaseGUIElement caller) {
        base.ClickReaction(caller);
        if (caller == pageButtons[0]) {
            Page++;
        } else if (caller == pageButtons[1]) {
            Page--;
        } else {
            for (int i = 0; i < TitleButts.Length; i++) {
                if (caller == TitleButts[i]) {
                    MakeTable(i);
                    break;
                }
			}
            
        }
    }
}

public class DatabaseEntry{
    public string AAN = "";
    public string MCAN = "";
    public string ACT = "";
    public string DESC = "";
    public string MOS = "";
    public string MMSTC = "";
    public string APPB = "";

    public DatabaseEntry(XmlNode src){

        foreach (XmlNode child in src) {
            switch (child.Name) {
                case "AAN": AAN = child.InnerText; break;
                case "MCAN": MCAN = child.InnerText; break;
                case "ACT": ACT = child.InnerText; break;
                case "DESC": DESC = child.InnerText; break;
                case "MOS": MOS = child.InnerText; break;
                case "MMSTC": MMSTC = child.InnerText; break;
                case "APPB": APPB = child.InnerText; break;
            }
        }

    }

}