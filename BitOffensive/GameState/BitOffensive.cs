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
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace BitOffensive
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class BitOffensive : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Camera2D Camera;
        /*
        private float BackgroundPos = 0;
        private AnimatedSprite hueyAnim;
        private AnimatedSprite hueyAnim2;
        private AnimatedSprite hueyAnim3;

        private Button Play;
        private Button Quit;

        private Entity panel;
        private Entity playPanel;
        */

        public static BitOffensive Instance;

        public BitOffensive()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            Window.AllowUserResizing = true;
            graphics.PreferredBackBufferWidth = ScreenManager.Instance.Dimensions.X;
            graphics.PreferredBackBufferHeight = ScreenManager.Instance.Dimensions.Y;
        }

        public static void ExitGame()
        {
            Instance.Exit();
        }

        public static void ClearScreen(Color color)
        {
            Instance.GraphicsDevice.Clear(color);
        }

        public static KeyboardState GetKeyboardState(Color color)
        {
            return Keyboard.GetState();
        }

        protected override void Initialize()
        {
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 
                ScreenManager.Instance.Dimensions.X, ScreenManager.Instance.Dimensions.Y);

            UserInterface.Initialize(Content, "bitoffensive");
            Camera = new Camera2D(viewportAdapter);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ScreenManager.Instance.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            ScreenManager.Instance.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            ScreenManager.Instance.Draw(gameTime, spriteBatch);
            base.Draw(gameTime);
        }
        /*
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

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
            // TODO: Add your initialization logic here
            UserInterface.Initialize(Content, BuiltinThemes.hd);
            // create a panel and position in center of screen
            panel = new Panel(new Vector2(400, 400), PanelSkin.None, Anchor.Center);
            playPanel = new Panel(new Vector2(400, 400), PanelSkin.None, Anchor.Center);

            Init_StartMenu();
            Init_PlayMenu();

            base.Initialize();
        }

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
                changed = true;
                UserInterface.Active.RemoveEntity(panel);
                UserInterface.Active.AddEntity(playPanel);
            };

            Quit = new Button("Quit", ButtonSkin.Alternative);
            Quit.OnClick += (Entity ent) =>
            {
                Exit();
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
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ScreenManager.Instance.LoadContent(Content);
            Song mainMenuMusic = Content.Load<Song>(@"sound\music\rotv");
            MediaPlayer.Play(mainMenuMusic);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            ScreenManager.Instance.UnloadContent();
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            ScreenManager.Instance.Update(gameTime);

            // TODO: Add your update logic here
            float scale = (GraphicsDevice.PresentationParameters.Bounds.Width / 2560);
            UserInterface.Active.Update(gameTime);
            BackgroundPos += 120 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (BackgroundPos > (2560)) BackgroundPos = 0;

            hueyAnim.Update(gameTime);
            hueyAnim2.Update(gameTime);
            hueyAnim3.Update(gameTime);

            if (changed)
            {

            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            ScreenManager.Instance.Draw(gameTime, spriteBatch);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            hueyAnim.Position = new Vector2(256, 128 + (float)(Math.Sin(gameTime.TotalGameTime.TotalSeconds / 2) * 24));
            hueyAnim.Rotation = -0.15f;
            hueyAnim2.Position = new Vector2(312, 372 + (float)(Math.Sin(gameTime.TotalGameTime.TotalSeconds - 8) * 24));
            hueyAnim2.Rotation = -0.15f;
            hueyAnim3.Position = new Vector2(978, 256 + (float)(Math.Sin(gameTime.TotalGameTime.TotalSeconds) * 24));
            hueyAnim3.Rotation = -0.15f;

            spriteBatch.Begin();
            Texture2D SeaBG = Content.Load<Texture2D>(@"sprites\background\sea");
            float scale = (GraphicsDevice.PresentationParameters.Bounds.Width / 2560);
            spriteBatch.Draw(SeaBG, new Vector2(-BackgroundPos, 0), Color.White);
            spriteBatch.Draw(SeaBG, new Vector2((-BackgroundPos + (2560)), 0), Color.White);
            spriteBatch.Draw(hueyAnim);
            spriteBatch.Draw(hueyAnim2);
            spriteBatch.Draw(hueyAnim3);
            spriteBatch.End();

            UserInterface.Active.Draw(spriteBatch);
            base.Draw(gameTime);
        }
        */
    }
}
