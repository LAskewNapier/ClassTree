using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;

namespace ClassTree.UI
{
    public class ClassSkillTreeUI : UIState
    {
        private UIPanel mainPanel;
        private UIText titleText;
        private UIText descriptionText;
        private static readonly string[] ExclusiveGroup1 = { "Damage", "Move_Speed", "Health" }; // Example exclusive group
        private static readonly string[] CrownGroup = { "Slime_Path_A", "Slime_Path_B" }; // Example exclusive group
        public override void OnInitialize()
        {
            // Create the main panel
            mainPanel = new UIPanel();
            mainPanel.Width.Set(920f, 0f); // Set the width of the panel
            mainPanel.Height.Set(480f, 0f); // Set the height of the panel
            mainPanel.HAlign = 0.5f; // Center horizontally
            mainPanel.VAlign = 0.5f; // Center vertically
            mainPanel.BackgroundColor = new Color(73, 94, 171); // Set the background color of the panel
            Append(mainPanel);

            // Create the title text
            titleText = new UIText("Class Skill Tree");
            titleText.HAlign = 0.5f; // Center horizontally
            titleText.Top.Set(10f, 0f);
            titleText.TextColor = Color.Gold; // Set the text color to white
            mainPanel.Append(titleText);

            // Create the description text
            AddSkillNode(mainPanel, "Damage", "Damage +5%", -160f, 80f, 0);
            AddSkillNode(mainPanel, "Move_Speed", "Move Speed +10%", 0f, 80f, 1);
            AddSkillNode(mainPanel, "Health", "Health +20", 160f, 80f, 2);

            var modPlayer = Main.LocalPlayer.GetModPlayer<ClassTreePlayer>();
            string classText  = GetCrownNodeText(modPlayer.ChosenClass);
            AddSkillNode(mainPanel, "Slime_Path_A", classText, -100f, 220f, 3);
            AddSkillNode(mainPanel, "Slime_Path_B", "Attacks inflict Slimed\n(33% chance)", 100f, 220f, 4);

            UIPanel closeButton = new UIPanel();
            closeButton.Width.Set(100f, 0f);
            closeButton.Height.Set(40f, 0f);
            closeButton.HAlign = 0.5f; // Center horizontally
            closeButton.Top.Set(250f, 0f);
            closeButton.OnLeftClick += (evt, element) => IngameFancyUI.Close(); // Close the UI when clicked
            mainPanel.Append(closeButton);

            UIText closeButtonText = new UIText("Close", 0.9f, true);
            closeButtonText.HAlign = 0.5f;
            closeButtonText.VAlign = 0.5f;
            closeButton.Append(closeButtonText);
        }
        private string GetCrownNodeText(int chosenClass)
        {
            switch (chosenClass)
            {
                case 0:
                    return "Spiked Gel on hurt\n(10 melee damage)";
                case 1:
                    return "Ranged: Ammo Bounces\n(10% chance)";
                case 2:
                    return "Magic: Crit +3%";
                case 3:
                    return "Slime Staff Spawns\nSpiked Slimes";
                default:
                    return "Crown Skill";
            }
        }

        private bool IsCrownNode(string skillID)
        {
            return skillID == "Slime_Path_A" || skillID == "Slime_Path_B";
        }

        private void AddSkillNode(UIPanel parentPanel, string SkillID, string displayText, float leftOffset, float topOffset, int testIndex)
        {
            var modPlayer = Main.LocalPlayer.GetModPlayer<ClassTreePlayer>();
            bool crownLocked = IsCrownNode(SkillID) && !modPlayer.UnlockedSkills.Contains("Crown_Unlocked");

            UIPanel skillNodePanel = new UIPanel();
            skillNodePanel.Width.Set(100f, 0f);
            skillNodePanel.Height.Set(100f, 0f);
            skillNodePanel.HAlign = 0.5f; // Center horizontally
            skillNodePanel.Top.Set(topOffset, 0f); // Position below the title
            skillNodePanel.Left.Set(leftOffset, 0f); // Position based on the left offset


            skillNodePanel.BackgroundColor = crownLocked
                ? new Color(35, 35, 35) * 0.8f
                : new Color(60, 60, 60) * 0.8f; // Set the background color of the skill node panel
            skillNodePanel.BorderColor = crownLocked
                ? new Color(60, 60, 60)
                : new Color(89, 116, 213); // Set the border color of the skill node panel

            skillNodePanel.OnMouseOver += (evt, element) =>
            {
                var mp = Main.LocalPlayer.GetModPlayer<ClassTreePlayer>();
                bool locked = IsCrownNode(SkillID) && !mp.UnlockedSkills.Contains("Crown_Unlocked");
                if (locked)
                {
                    skillNodePanel.BackgroundColor = new Color(80, 50, 50) * 0.9f;
                    return;
                }

                bool group1Unlocked = false;
                foreach (string id in ExclusiveGroup1)
                {
                    if (mp.UnlockedSkills.Contains(id))
                    {
                        group1Unlocked = true;
                        break;
                    }
                }
                if (group1Unlocked)
                {
                    skillNodePanel.BackgroundColor = new Color(120, 40, 40) * 0.9f; // Change the background color when hovered to red
                }
                else
                {
                    skillNodePanel.BackgroundColor = new Color(40, 120, 40) * 0.9f; // Change the background color when hovered to green
                }
            };

            skillNodePanel.OnMouseOut += (evt, element) =>
            {
                var mp = Main.LocalPlayer.GetModPlayer<ClassTreePlayer>();
                bool locked = IsCrownNode(SkillID) && !mp.UnlockedSkills.Contains("Crown_Unlocked");
                if (locked)
                {
                    skillNodePanel.BackgroundColor = new Color(35, 35, 35) * 0.8f;
                }
                else if (mp.UnlockedSkills.Contains(SkillID))
                {
                    skillNodePanel.BackgroundColor = new Color(40, 120, 40) * 0.8f; // Revert the background color when not hovered to green
                }
                else
                {
                    skillNodePanel.BackgroundColor = new Color(60, 60, 60) * 0.8f; // Revert the background color when not hovered to default
                }
            };
            // Add click event to the skill node panel
            skillNodePanel.OnLeftClick += (evt, element) =>
            {
                var modPlayer = Main.LocalPlayer.GetModPlayer<ClassTreePlayer>();
                //Already Unlocked, Do notheing

                if (IsCrownNode(SkillID) && !modPlayer.UnlockedSkills.Contains("Crown_Unlocked"))
                {
                    Main.NewText("You Must use a Bested Crown to unlock these skills.");
                }

                if (modPlayer.UnlockedSkills.Contains(SkillID))
                {
                    Main.NewText("You have already unlocked this skill.");
                    return;
                }
                
                string alreadyOwned = null;
                // Check if the skill is part of an exclusive group
                foreach (string id in ExclusiveGroup1)
                {
                    if (modPlayer.UnlockedSkills.Contains(id))
                    {
                        alreadyOwned = id;
                        break;
                    }
                }
                if (alreadyOwned != null)
                {
                    Main.NewText("You have already unlocked a skill from this exclusive group.");
                    return;
                }
                if (IsCrownNode(SkillID))
                {
                    foreach (string id in CrownGroup)
                    {
                        if (id != SkillID && modPlayer.UnlockedSkills.Contains(id))
                        {
                            Main.NewText("You can only choose one bested crown skill");
                            return;
                        }
                    }
                }
                {
                    modPlayer.UnlockedSkills.Add(SkillID);
                    skillNodePanel.BackgroundColor = new Color(40, 120, 40) * 0.8f; // Revert the background color when not hovered

                    skillNodePanel.BorderColor = new Color(60, 180, 60); // Change the border color to indicate the skill is unloaded
                    Main.NewText("Unlocked: " + SkillID);
                }

            };

            UIText skillNodeText = new UIText(displayText, 0.8f, true);
            skillNodeText.HAlign = 0.5f; // Center horizontally
            skillNodeText.VAlign = 0.5f; // Center vertically
            skillNodeText.TextColor = Color.White; // Set the text color to white
            skillNodePanel.Append(skillNodeText);

            parentPanel.Append(skillNodePanel);
        }


    }
}