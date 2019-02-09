using BitOffensive.GameState;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitOffensive.Entities
{
    public class GameObject
    {
        public Vector2 Position;
        public Vector2 Center { get; internal set; }
        public string CurrentAnimName;

        protected ContentManager Content;
        protected AnimatedSprite AnimSprite;
        protected SpriteSheetAnimationFactory AnimFactory;


        public GameObject()
        {

        }

        public virtual void LoadContent()
        {
            Content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
        }

        public virtual void UnloadContent()
        {
            Content.Unload();
        }
        public virtual void Update(GameTime gameTime)
        {
            AnimSprite.Update(gameTime);
            Center = (Position + new Vector2(16, 16));
        }
        public virtual void Draw(GameTime gameTime, SpriteBatch sb)
        {
            AnimSprite.Position = Position;
            sb.Draw(AnimSprite);
        }
        public virtual void InitAnimation(string AtlasName, string SpritePath, string MapPath)
        {

        }
        public virtual void ChangeAnimation(string animName, float scale = 1)
        {
            if (animName == CurrentAnimName) return;
            CurrentAnimName = animName;
            AnimSprite = new AnimatedSprite(AnimFactory, animName)
            {
                Scale = new Vector2(scale, scale),
            };
        }
    }
}
