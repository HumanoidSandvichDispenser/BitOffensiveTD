using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Timers;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.TextureAtlases;
using BitOffensive.Entities;
using Microsoft.Xna.Framework.Audio;
using BitOffensive.GameState;

namespace BitOffensive.Assets
{
    public class MachineGun : Tower
    {
        public int BulletTimer { get; private set; }

        public void Shoot()
        {
            
        }

        public MachineGun(Vector2 Position) : this() { this.Position = Position; }

        public MachineGun()
        {
            MaxAmmo = Ammo = 125;
            Damage = 8;
            Radius = 64;
            Draggable = true;
            Type = EntityType.Tower;
            ShootTimer = new TimedAction(() => { ShootBullet(@"sound\guns\sniper", 1.5f); }, 0.35f / LevelScreen.TimeScale);
        }

        public override void Reinitialize()
        {
            MaxAmmo = Ammo = 125;            
            Damage = 8;
            Radius = 64;
            Draggable = true;
            Type = EntityType.Tower;
            ShootTimer = new TimedAction(() => { ShootBullet(@"sound\guns\sniper", 1.5f); }, 0.35f / LevelScreen.TimeScale);
            base.Reinitialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            InitAnimation("MarineMachineGun", @"sprites\us\MarineMachineGun", @"sprites\vc\VietCongMap");
            AnimSprite.Scale = new Vector2(0.25f, 0.25f);
        }

        public override void InitAnimation(string AtlasName, string SpritePath, string MapPath)
        {
            var Texture = Content.Load<Texture2D>(SpritePath);
            var Map = Content.Load<Dictionary<string, Rectangle>>(MapPath);
            AnimFactory = new SpriteSheetAnimationFactory(new TextureAtlas(AtlasName, Texture, Map));
            AnimFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
            AnimFactory.Add("firing", new SpriteSheetAnimationData(new[] { 1, 2 }, 0.1f));
            AnimFactory.Add("walking", new SpriteSheetAnimationData(new[] { 3, 4 }, 0.4f));
            ChangeAnimation("idle", 1);

            base.InitAnimation(AtlasName, SpritePath, MapPath);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
            base.Draw(gameTime, sb);
        }
    }
}
