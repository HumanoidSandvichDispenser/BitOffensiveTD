using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitOffensive.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BitOffensive.GameState
{
    public class ScreenManager
    {
        private static ScreenManager instance;
        public Point Dimensions { private set; get; }
        public ContentManager Content { private set; get; }
        GameScreen currentScreen;

        public static ScreenManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ScreenManager();
                }
                return instance;
            }
        }

        public ScreenManager()
        {
            Dimensions = new Point(1280, 720);
            currentScreen = new TitleScreen();
        }

        public void LoadContent(ContentManager content)
        {
            Content = content;
            //Content = new ContentManager(Content.ServiceProvider, "Content");
            currentScreen.LoadContent();
        }

        public void UnloadContent()
        {
            currentScreen.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch sb)
        {
            currentScreen.Draw(gameTime, sb);
        }

        public void SwitchScreen(GameScreen screen, bool ClearScreen = true)
        {
            foreach (GameEntity ent in EntityManager.Instance.Entities)
            {
                EntityManager.Instance.RemoveEntity(ent);
            }

            currentScreen.UnloadContent();
            currentScreen = screen;
            if (ClearScreen) BitOffensive.ClearScreen(Color.Black);
            currentScreen.LoadContent();
        }
    }
}
