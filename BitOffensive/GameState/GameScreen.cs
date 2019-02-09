using GeonBit.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitOffensive.GameState
{
    public class GameScreen
    {
        protected ContentManager Content;
        protected bool Unloading = false;
        //protected Camera camera;

        public GameScreen()
        {
            
        }

        public virtual void LoadContent()
        {
            Content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
            UserInterface.Initialize(Content, "bitoffensive");
        }

        public virtual void UnloadContent()
        {
            UserInterface.Active.Clear();
            UserInterface.Active.Dispose();
            Content.Unload();
        }

        public virtual void Update(GameTime gameTime)
        {
            UserInterface.Active.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch sb)
        {
            UserInterface.Active.Draw(sb);
        }
    }
}
