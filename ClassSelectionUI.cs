using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;


namespace ClassTree.UI
{
    public class ClassSelectionUI : UIState
    {

        public override void OnInitialize()
        {
            UIPanel mainPanel = new UIPanel();
            mainPanel.Width.Set(500f, 0f);
            mainPanel.Height.Set(300f, 0f);
            mainPanel.HAlign = 0.5f;
            mainPanel.VAlign = 0.5f;
            mainPanel.BackgroundColor = new Color(33, 74, 51);
            Append(mainPanel);

            UIText titleText = new UIText("Choose Your Class", 1.2f, true);
            titleText.HAlign = 0.5f;
            titleText.Top.Set(15f, 0f);
            titleText.TextColor = Color.White;
            mainPanel.Append(titleText);

            AddClassButton(mainPanel, "Melee", -100f, 70f, 0);
            AddClassButton(mainPanel, "Ranged", 100f, 70f, 1);
            AddClassButton(mainPanel, "Magic", -100f, 150f, 2);
            AddClassButton(mainPanel, "Summoner", 100f, 150f, 3);

        }

        private void AddClassButton(UIPanel parentPanel, string text, float leftOffSet, float topOffset, int classID)
        {
            UIPanel buttonPanel = new UIPanel();
            buttonPanel.Width.Set(160f, 0f);
            buttonPanel.Height.Set(55f, 0f);
            buttonPanel.HAlign = 0.5f;
            buttonPanel.Left.Set(leftOffSet, 0f);
            buttonPanel.Top.Set(topOffset, 0f);


            Color defaultBackground = new Color(63, 82, 151) * 0.7f;
            Color defaultBorder = new Color(89, 116, 213) * 0.7f;
            buttonPanel.BackgroundColor = defaultBackground;
            buttonPanel.BorderColor = defaultBorder;

            buttonPanel.OnMouseOver += (evt, listeningElement) =>
            {
                buttonPanel.BackgroundColor = new Color(73, 94, 171) * 0.9f;
                buttonPanel.BorderColor = new Color(99, 126, 233) * 0.9f;
            };

            buttonPanel.OnMouseOut += (evt, listeningElement) =>
            {
                buttonPanel.BackgroundColor = defaultBackground;
                buttonPanel.BorderColor = defaultBorder;
            };
            
            buttonPanel.OnLeftClick += (evt, listeningElement) => onClassButtonClick(classID);

            UIText buttonText = new UIText(text, 0.8f, true);
            buttonText.HAlign = 0.5f;
            buttonText.VAlign = 0.5f;
            buttonText.TextColor = Color.White;
            buttonPanel.Append(buttonText);

            parentPanel.Append(buttonPanel);
        }

        private void onClassButtonClick(int classID)
        {
            var modPlayer = Main.LocalPlayer.GetModPlayer<ClassTreePlayer>();
            modPlayer.ChosenClass = classID;

            modPlayer.GiveStartingItems();
            IngameFancyUI.Close();
        }
    }
}