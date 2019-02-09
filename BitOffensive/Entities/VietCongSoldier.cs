using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.TextureAtlases;

namespace BitOffensive.Assets
{
    public class VietCongInfantry : Enemy
    {
        //Texture2D Texture;

        public VietCongInfantry()
        {
            Draggable = false;
            Type = EntityType.Enemy;
            Health = 8;
            MaxHealth = 8;
        }

        public override void Reinitialize()
        {
            Draggable = false;
            Type = EntityType.Enemy;
            Health = 8;
            MaxHealth = 8;
            base.Reinitialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            InitAnimation("VietCong", @"sprites\vc\VietCongB", @"sprites\vc\VietCongMap");
            AnimSprite.Scale = new Vector2(0.25f, 0.25f);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
            base.Draw(gameTime, sb);
        }

        public override void Update(GameTime gameTime)
        {
            var Map = Content.Load<Dictionary<Int16, Vector2>>("maps/KheSanhMap");
            var NewPosition = Map[CurrentStep] * 64;

            if (!IsNear(Position, Scale(NewPosition)))
            {
                WalkTo(NewPosition, 8);
            }
            else
            {
                
                if (CurrentStep < Map.Count - 1)
                CurrentStep++;
                else
                {
                    Kill();
                    // Player loses health
                }
            }
            AnimSprite.Update(gameTime);
            base.Update(gameTime);
        }

        public override void ChangeAnimation(string animName, float scale = 1)
        {
            base.ChangeAnimation(animName, scale);
        }

        public override void InitAnimation(string AtlasName, string SpritePath, string MapPath)
        {
            var Texture = Content.Load<Texture2D>(SpritePath);
            var Map = Content.Load<Dictionary<string, Rectangle>>(MapPath);
            AnimFactory = new SpriteSheetAnimationFactory(new TextureAtlas(AtlasName, Texture, Map));
            AnimFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
            AnimFactory.Add("firing", new SpriteSheetAnimationData(new[] { 1, 2 }, 0.1f));
            AnimFactory.Add("walking", new SpriteSheetAnimationData(new[] { 3, 4 }, 0.4f));
            ChangeAnimation("walking", 1);

            base.InitAnimation(AtlasName, SpritePath, MapPath);
        }

        public override void WalkTo(Vector2 NewPosition, float Speed = 8)
        {
            base.WalkTo(NewPosition, Speed);
        }
    }

    public class VietCongArmored : Enemy
    {
        //Texture2D Texture;

        public VietCongArmored()
        {
            Draggable = false;
            Type = EntityType.Enemy;
            Health = 30;
            MaxHealth = 30;
        }

        public override void Reinitialize()
        {
            Draggable = false;
            Type = EntityType.Enemy;
            Health = 30;
            MaxHealth = 30;
            base.Reinitialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            InitAnimation("VietCongArmor", @"sprites\vc\VietCongA", @"sprites\vc\VietCongMap");
            AnimSprite.Scale = new Vector2(0.25f, 0.25f);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
            base.Draw(gameTime, sb);
        }

        public override void Update(GameTime gameTime)
        {
            var Map = Content.Load<Dictionary<Int16, Vector2>>("maps/KheSanhMap");
            var NewPosition = Map[CurrentStep] * 64;

            if (!IsNear(Position, Scale(NewPosition)))
            {
                WalkTo(NewPosition, 5);
            }
            else
            {

                if (CurrentStep < Map.Count - 1)
                    CurrentStep++;
                else
                {
                    Kill();
                    // Player loses health
                }
            }
            AnimSprite.Update(gameTime);
            base.Update(gameTime);
        }

        public override void ChangeAnimation(string animName, float scale = 1)
        {
            base.ChangeAnimation(animName, scale);
        }

        public override void InitAnimation(string AtlasName, string SpritePath, string MapPath)
        {
            var Texture = Content.Load<Texture2D>(SpritePath);
            var Map = Content.Load<Dictionary<string, Rectangle>>(MapPath);
            AnimFactory = new SpriteSheetAnimationFactory(new TextureAtlas(AtlasName, Texture, Map));
            AnimFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
            AnimFactory.Add("firing", new SpriteSheetAnimationData(new[] { 1, 2 }, 0.1f));
            AnimFactory.Add("walking", new SpriteSheetAnimationData(new[] { 3, 4 }, 0.4f));
            ChangeAnimation("walking", 1);

            base.InitAnimation(AtlasName, SpritePath, MapPath);
        }

        public override void WalkTo(Vector2 NewPosition, float Speed = 8)
        {
            base.WalkTo(NewPosition, Speed);
        }
    }
}
