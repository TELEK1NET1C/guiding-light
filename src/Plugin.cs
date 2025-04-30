using System;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using MonoMod;
using System.Xml.Serialization;
using IL;
using IL.Stove.Sample.Ownership;
using RWCustom;
using Expedition;

namespace GuidingLight
{
    [BepInPlugin(MOD_ID, "The Guiding Light", "0.0.1")]
    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "tk1.guidinglight";
        private int startSpriteIndex;
        private FSprite newPlayerSprite;


        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            // Put your custom hooks here!
        }

        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {

        }
        
        private void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam); // Call the original method's code first

            // Assuming you're using SlugBase, use the value of SlugCatClass to match the string ID of your slugcat
            if (self.player.SlugCatClass.value == "tk1.guidinglight")
            {
                startSpriteIndex = sLeaser.sprites.Length; // IT'S HIGHLY RECCOMMENDED TO USE CONDITIONAL WEAK TABLES FOR YOUR VALUES, this is for a simple example

                Array.Resize(ref sLeaser.sprites, sLeaser.sprites.Length + 1); // The array now has an extra slot at the end of it's content for your sprite

                sLeaser.sprites[startSpriteIndex] = new FSprite("Belt"); // This is now your new sprite, it has no extra properties and is assumed to be a singular sprite
                newPlayerSprite = sLeaser.sprites[startSpriteIndex]; // ADD THIS TO YOUR CWT AS WELL, assigning the sprite to a variable makes it easier to read your code
            }
        }

        private void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            orig(self, sLeaser, rCam, newContatiner); // Call the original method's code first

            // Cautional check for both if it's your slugcat AND the sLeaser.sprites array has been resized properly
            if (self.player.SlugCatClass.value == "tk1.guidinglight" && sLeaser.sprites.Length > startSpriteIndex && newPlayerSprite != null)
            {
                newPlayerSprite.RemoveFromContainer(); // Cautionary removal of the sprite in case it was not properly removed automatically
                rCam.ReturnFContainer("Midground"); // Change the name to the appropriate container
            }
        }

        private void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);  // Call the original method's code first

            // Same check as the previous hook
            if (self.player.SlugCatClass.value == "tk1.guidinglight" && sLeaser.sprites.Length > startSpriteIndex && newPlayerSprite != null)
            {
                newPlayerSprite.SetPosition(sLeaser.sprites[0].GetPosition()); // Sets the position of the new sprite to that of the head sprite
                /* FSprites contain a lot of properties to mess with, and messing with relative positions can be found in various DrawSprite methods of different creatures
                 * RWCustom contains many different Rain World built methods to handle various mathmatical equations for you, such as Custom.AimBetweenTwoVectors() which can find the rotation angle between two positions
                 * timeStacker can be used for sin, cos, or tan related timers or values
                 * camPos refers to the camera position, and is used for referring to the visual offset a body part actually has with the geometry of the camera
                 * the PlayerGraphics instance also has loads of access to various player-based things
                 * */
            }
        }

        private void PlayerGraphics_ApplyPalette(On.PlayerGraphics.orig_ApplyPalette orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            orig(self, sLeaser, rCam, palette);  // Call the original method's code first

            if (self.player.SlugCatClass.value == "tk1.guidinglight" && sLeaser.sprites.Length > startSpriteIndex && newPlayerSprite != null)
            {
                newPlayerSprite.color = new(255, 255, 255); // Setting your color here also gives you the advantage to grab the original colors of a sprite before they're changed by the DrawSprites method if they are changed at all
            }
        }

    }
}