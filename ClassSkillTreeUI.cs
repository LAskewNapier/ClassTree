using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using System;
using System.Linq;

namespace ClassTree.UI
{
    public class ClassSkillTreeUI : UIState
    {
        private UIPanel mainPanel;
        private UIText titleText;
        private UIText descriptionText;
        private static readonly string[] ExclusiveGroup1 = { "Damage", "Move_Speed", "Health" }; // Example exclusive group
        private static readonly string[] CrownGroup = { "Slime_Path_A", "Slime_Path_B" }; // Example exclusive group
        private static readonly string[] EyeGroup = {"Eye_Path_A", "Eye_Path_B"};
        public override void OnInitialize()
        {
            // Create the main panel
            mainPanel = new UIPanel();
            mainPanel.Width.Set(920f, 0f); // Set the width of the panel
            mainPanel.Height.Set(620f, 0f); // Set the height of the panel
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
            AddSkillNode(mainPanel, "Slime_Path_A", GetCrownNodeText(modPlayer.ChosenClass), -100f, 220f, 3);
            AddSkillNode(mainPanel, "Slime_Path_B", "Attacks inflict Slow\n(33% chance)", 100f, 220f, 4);

            AddSkillNode(mainPanel, "Eye_Path_A", GetEyeNodeText1(modPlayer.ChosenClass), -100f, 360f, 5);
            AddSkillNode(mainPanel, "Eye_Path_B", GetEyeNodeText2(modPlayer.ChosenClass), 100f, 360f, 5);
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

        private string GetEyeNodeText1(int chosenClass)
        {
            switch (chosenClass)
            {
                case 0: return "Small Dash";
                case 1: return "Crit Damage +5%";
                case 2: return "Stars on hurt\n(30 magic Damage)";
                case 3: return "Small Dash";
                default:return "Eye Skills 1";
            }
        }

        private string GetEyeNodeText2(int chosenClass)
        {
            switch (chosenClass)
            {
                case 0: return "Melee Speed +5%\nat <=25 Hp";
                case 1: return "Proj. velocity +5%\nat <=25 Hp";
                case 2: return "Magic use time +5%\nat <=25 Hp";
                case 3: return "Whip Speed +5%\nat <=25 Hp";
                default:return "Eye Skills 2";
            }
        }

        private bool IsCrownNode(string skillID)
        {
            return skillID == "Slime_Path_A" || skillID == "Slime_Path_B";
        }

        private bool IsEyeNode(string skillID)
        {
            return skillID == "Eye_Path_A" || skillID == "Eye_Path_B";
        }

        private string GetUnlockFlagForNode(string skillID)
        {
            if (IsCrownNode(skillID)) return "Crown_Unlocked";
            if (IsEyeNode(skillID)) return "Eye_Unlocked";
            return null;
        }
        private bool IsNodeLocked(string skillID, ClassTreePlayer modPlayer)
        {
            string flag = GetUnlockFlagForNode(skillID);
            if (flag == null) return false;
            return !modPlayer.UnlockedSkills.Contains(flag);
        }

        private void AddSkillNode(UIPanel parentPanel, string SkillID, string displayText, float leftOffset, float topOffset, int testIndex)
        {
            var modPlayer = Main.LocalPlayer.GetModPlayer<ClassTreePlayer>();
            bool locked = IsCrownNode(SkillID) && !modPlayer.UnlockedSkills.Contains("Crown_Unlocked");

            UIPanel skillNodePanel = new UIPanel();
            skillNodePanel.Width.Set(100f, 0f);
            skillNodePanel.Height.Set(100f, 0f);
            skillNodePanel.HAlign = 0.5f; // Center horizontally
            skillNodePanel.Top.Set(topOffset, 0f); // Position below the title
            skillNodePanel.Left.Set(leftOffset, 0f); // Position based on the left offset


            skillNodePanel.BackgroundColor = locked
                ? new Color(35, 35, 35) * 0.8f
                : new Color(60, 60, 60) * 0.8f; // Set the background color of the skill node panel
            skillNodePanel.BorderColor = locked
                ? new Color(60, 60, 60)
                : new Color(89, 116, 213); // Set the border color of the skill node panel
            //hover over
            skillNodePanel.OnMouseOver += (evt, element) =>
            {
                var mp = Main.LocalPlayer.GetModPlayer<ClassTreePlayer>();
                bool islocked = IsNodeLocked(SkillID, mp);
                if (islocked)
                {
                    skillNodePanel.BackgroundColor = new Color(80, 50, 50) * 0.9f;
                    return;
                }

                bool group1Taken = false;
                foreach (string id in ExclusiveGroup1)
                {
                    if (mp.UnlockedSkills.Contains(id))
                    {
                        group1Taken = true;
                        break;
                    }
                }
                if (group1Taken && !mp.UnlockedSkills.Contains(SkillID))
                {
                    skillNodePanel.BackgroundColor = new Color(120, 40, 40) * 0.9f; // Change the background color when hovered to red
                }
                else
                {
                    skillNodePanel.BackgroundColor = new Color(40, 120, 40) * 0.9f; // Change the background color when hovered to green
                }
            };

            //hover out

            skillNodePanel.OnMouseOut += (evt, element) =>
            {
                var mp = Main.LocalPlayer.GetModPlayer<ClassTreePlayer>();
                bool isLocked = IsNodeLocked(SkillID, mp) ;
                if (isLocked)
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

                if (IsNodeLocked(SkillID, modPlayer))
                {
                    string flag = GetUnlockFlagForNode(SkillID);
                    if (flag == "Crown_Unlocked")
                    {
                        Main.NewText("You Must Use A Bested Crown To Unlock These skills");
                    }
                    if (flag == "Eye_Unlocked")
                    {
                        Main.NewText("You Must Use A Bloody Tear To Unlock These skills");
                    }
                    return;
                }
                //if the player has already unlocked the skill
                if (modPlayer.UnlockedSkills.Contains(SkillID))
                {
                    Main.NewText("You have already unlocked this skill.");
                }

                //shared group exclusivity
                if (ExclusiveGroup1.Contains(SkillID))
                {
                    foreach (string id in ExclusiveGroup1)
                    {
                        if (id != SkillID && modPlayer.UnlockedSkills.Contains(id))
                        {
                            Main.NewText("You already have a skill from this group unlocked");
                            return;
                        }
                    }
                }
                if (IsCrownNode(SkillID))
                {
                    foreach (string id in CrownGroup)
                    {
                        if (id != SkillID && modPlayer.UnlockedSkills.Contains(id))
                        {
                            Main.NewText("You already have a skill from this group unlocked");
                            return;
                        }
                    }
                }
                if (IsEyeNode(SkillID))
                {
                    foreach (string id in EyeGroup)
                    {
                        if (id != SkillID && modPlayer.UnlockedSkills.Contains(id))
                        {
                            Main.NewText("You already have a skill from this group unlocked");
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