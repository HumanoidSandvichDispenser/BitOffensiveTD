using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitOffensive.GameState;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.TextureAtlases;

namespace BitOffensive.Assets
{
    public class Truck : Enemy
    {
        //Texture2D Texture;
        TimedAction SpawnEnemy;

        public Truck()
        {
            Draggable = false;
            Type = EntityType.Enemy;
            Health = 100;
            MaxHealth = 100;
        }

        public override void Reinitialize()
        {
            Draggable = false;
            Type = EntityType.Enemy;
            Health = 100;
            MaxHealth = 100;
            base.Reinitialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            InitAnimation("NVATruck", @"sprites\vc\NVATruck", @"sprites\vc\NVATruckMap");
            AnimSprite.Scale = new Vector2(0.25f, 0.25f);
            SpawnEnemy = new TimedAction(SpawnEnemyAtPos, 3);
            SpawnEnemy.Start();
        }

        public void SpawnEnemyAtPos()
        {
            Enemy en = EntityManager.Instance.CreateEntity<VietCongArmored>() as Enemy;
            EntityManager.Instance.AddEntity(en);
            en.Position = Position;
            en.CurrentStep = CurrentStep;
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
            if (SpawnEnemy != null)
            {
                SpawnEnemy.Update(gameTime.GetElapsedSeconds() * LevelScreen.TimeScale);
            }

            var Map = Content.Load<Dictionary<Int16, Vector2>>("maps/KheSanhMap");
            var NewPosition = Map[CurrentStep] * 64;

            if (!IsNear(Position, Scale(NewPosition)))
            {
                WalkTo(NewPosition, 12);
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
            AnimFactory.Add("front", new SpriteSheetAnimationData(new[] { 0, 1 }, 0.25f));
            AnimFactory.Add("back", new SpriteSheetAnimationData(new[] { 2, 3 }, 0.25f));
            AnimFactory.Add("side", new SpriteSheetAnimationData(new[] { 4, 5 }, 0.25f));
            ChangeAnimation("side", 1);

            base.InitAnimation(AtlasName, SpritePath, MapPath);
        }

        public override void WalkTo(Vector2 NewPosition, float Speed = 8)
        {
            Vector2 dir = Scale(NewPosition) - Position;
            dir.Normalize();
            if (dir.Y < 1)
            {
                ChangeAnimation("back", 0.25f);
            }
            else if (dir.Y > 1)
            {
                ChangeAnimation("front", 0.25f);
            }
            else
            {
                ChangeAnimation("side", 0.25f);
            }
            base.WalkTo(NewPosition, Speed);
        }
    }
}
