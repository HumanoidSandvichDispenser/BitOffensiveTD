using BitOffensive.Assets;
using BitOffensive.GameState;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitOffensive.Entities
{
    public class Bullet : GameEntity
    {
        public int Damage;
#pragma warning disable IDE0044 // Add readonly modifier
        private float Angle;
#pragma warning restore IDE0044 // Add readonly modifier
        private float Age;

        public float Velocity { get; private set; }
        public Vector2 Direction { get; internal set; }
        

        public Bullet()
        {

        }

        public Bullet(int dmg, Vector2 target, Vector2 position, float Speed = 1)
        {
            Age = 0;
            Damage = dmg;
            Position = position;
            Direction = target - Position;
            Velocity = Speed * LevelScreen.TimeScale;
            Direction.Normalize();
        }

        public override void Reinitialize()
        {
            base.Reinitialize();
        }

        public void Reinitialize(int dmg, Vector2 target, Vector2 position, float Speed = 1)
        {
            Age = 0;
            Damage = dmg;
            Position = position;
            Direction = target - Position;
            Velocity = Speed * LevelScreen.TimeScale;
            Direction.Normalize();
            base.Reinitialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            InitAnimation("Bullet", @"sprites\items\Bullet", @"sprites\items\BulletMap");
        }

        public override void InitAnimation(string AtlasName, string SpritePath, string MapPath)
        {
            var Texture = Content.Load<Texture2D>(SpritePath);
            var Map = Content.Load<Dictionary<string, Rectangle>>(MapPath);
            AnimFactory = new SpriteSheetAnimationFactory(new TextureAtlas(AtlasName, Texture, Map));
            AnimFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0, 1 }));
            ChangeAnimation("idle", 1);
            base.InitAnimation(AtlasName, SpritePath, MapPath);
        }

        public override void ChangeAnimation(string animName, float scale = 1)
        {
            base.ChangeAnimation(animName, scale);
        }

        public override void Update(GameTime gameTime)
        {
            Age = Age + (float)(60 * gameTime.ElapsedGameTime.TotalSeconds);
            Move();
            CheckAge();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
            AnimSprite.Rotation = (float)Math.Atan2(-Direction.Y, Direction.X);
                
            base.Draw(gameTime, sb);
        }

        public void CheckAge()
        {
            
            if (Age > 600)
            {
                EntityManager.Instance.RemoveEntity(this);
            }
            
        }

        public virtual void Move()
        {
            Position += Direction * (Velocity / 16);
        }
    }

    public class Rocket : GameEntity
    {

    }
}
