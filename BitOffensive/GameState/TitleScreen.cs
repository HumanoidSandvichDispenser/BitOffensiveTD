using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Sprites;

using GeonBit;
using GeonBit.Content;
using GeonBit.Core;
using GeonBit.UI;
using GeonBit.UI.Entities;
using BitOffensive.GameState;
using Microsoft.Xna.Framework.Content;

namespace BitOffensive.GameState
{
    public class TitleScreen : GameScreen
    {
        private float BackgroundPos = 0;
        private AnimatedSprite hueyAnim;
        private AnimatedSprite hueyAnim2;
        private AnimatedSprite hueyAnim3;

        private Button Play;
        private Button Quit;

        private Panel panel;
        private Panel playPanel;

        private void Init_StartMenu()
        {
            UserInterface.Active.AddEntity(panel);

            // add title and text
            panel.AddChild(new Header("Bit Offensive: 1968"));
            panel.AddChild(new HorizontalLine());

            // add a button at the bottom
            Play = new Button("Play", ButtonSkin.Alternative);
            Play.OnClick += (Entity ent) =>
            {
                UserInterface.Active.RemoveEntity(panel);
                UserInterface.Active.AddEntity(playPanel);
            };

            Quit = new Button("Quit", ButtonSkin.Alternative);
            Quit.OnClick += (Entity ent) =>
            {
                BitOffensive.ExitGame();
            };

            panel.AddChild(Play);
            panel.AddChild(new Button("Options", ButtonSkin.Alternative));
            panel.AddChild(Quit);
        }

        private void Init_PlayMenu()
        {
            playPanel.AddChild(new Header("LEVEL SELECTION"));
            playPanel.AddChild(new HorizontalLine());

            Button Play_KheSanh = new Button("Khe Sanh", ButtonSkin.Alternative);
            Play_KheSanh.OnClick += (Entity ent) =>
            {
                ScreenManager.Instance.SwitchScreen(new LevelScreen("maps/khesanh"));
            };
            Button Play_Hue = new Button("Hue", ButtonSkin.Fancy);
            Button Play_Saigon = new Button("Saigon", ButtonSkin.Alternative);
            CheckBox Play_Endless = new CheckBox("Endless Mode");
            Button Play_Back = new Button("Back", ButtonSkin.Alternative);
            Play_Back.OnClick += (Entity ent) =>
            {
                UserInterface.Active.RemoveEntity(playPanel);
                UserInterface.Active.AddEntity(panel);
            };
            playPanel.AddChild(Play_Saigon);
            playPanel.AddChild(Play_KheSanh);
            playPanel.AddChild(Play_Hue);
            playPanel.AddChild(Play_Endless);
            playPanel.AddChild(Play_Back);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();
            Content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
            var hueyTexture = Content.Load<Texture2D>(@"sprites\background\huey");
            var hueyMap = Content.Load<Dictionary<string, Rectangle>>(@"sprites\background\hueyMap");
            var hueyAnimFactory = new SpriteSheetAnimationFactory(new TextureAtlas("huey", hueyTexture, hueyMap));

            hueyAnimFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0, 1, 2, 3, 4, 5, 6, 7 }, 0.01f));

            hueyAnim = new AnimatedSprite(hueyAnimFactory, "idle")
            {
                Scale = new Vector2(0.75f, 0.75f),
            };

            hueyAnim2 = new AnimatedSprite(hueyAnimFactory, "idle")
            {
                Scale = new Vector2(0.75f, 0.75f),
            };

            hueyAnim3 = new AnimatedSprite(hueyAnimFactory, "idle")
            {
                Scale = new Vector2(0.75f, 0.75f),
            };

            panel = new Panel(new Vector2(400, 400), PanelSkin.None, Anchor.Center);
            playPanel = new Panel(new Vector2(400, 400), PanelSkin.None, Anchor.Center);

            Init_StartMenu();
            Init_PlayMenu();
            // Create a new SpriteBatch, which can be used to draw textures.
            Song mainMenuMusic = Content.Load<Song>(@"sound\music\rotv");
            MediaPlayer.Play(mainMenuMusic);
            // TODO: use this.Content to load your game content here
        }

        public override void UnloadContent()
        {
            //UserInterface.Active.Clear();
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            UserInterface.Active.Update(gameTime);
            BackgroundPos += 120 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (BackgroundPos > (2560)) BackgroundPos = 0;

            hueyAnim.Update(gameTime);
            hueyAnim2.Update(gameTime);
            hueyAnim3.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
            hueyAnim.Position = new Vector2(256, 128 + (float)(Math.Sin(gameTime.TotalGameTime.TotalSeconds / 2) * 24));
            hueyAnim.Rotation = -0.15f;
            hueyAnim2.Position = new Vector2(312, 372 + (float)(Math.Sin(gameTime.TotalGameTime.TotalSeconds - 8) * 24));
            hueyAnim2.Rotation = -0.15f;
            hueyAnim3.Position = new Vector2(978, 256 + (float)(Math.Sin(gameTime.TotalGameTime.TotalSeconds) * 24));
            hueyAnim3.Rotation = -0.15f;

            sb.Begin();
            Texture2D SeaBG = Content.Load<Texture2D>(@"sprites\background\sea");
            sb.Draw(SeaBG, new Vector2(-BackgroundPos, 0), Color.White);
            sb.Draw(SeaBG, new Vector2((-BackgroundPos + (2560)), 0), Color.White);
            sb.Draw(hueyAnim);
            sb.Draw(hueyAnim2);
            sb.Draw(hueyAnim3);
            sb.End();

            //UserInterface.Active.Draw(sb);
            base.Draw(gameTime, sb);
        }
    }
}
