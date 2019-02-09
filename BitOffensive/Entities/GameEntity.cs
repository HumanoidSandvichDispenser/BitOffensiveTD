using BitOffensive.Entities;
using BitOffensive.GameState;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitOffensive.Assets
{
    public class GameEntity
    {
        public Vector2 Position;
        public Vector2 Center { get; internal set; }
        public string CurrentAnimName;
        public float Order = 0;
        public EntityType Type;

        protected ContentManager Content;
        protected AnimatedSprite AnimSprite;
        protected SpriteSheetAnimationFactory AnimFactory;
        public bool Draggable = false;

        public enum EntityState
        {
            idle,
            walking,
            firing
        }

        public enum EntityType
        {
            Tower,
            Enemy
        }

        public GameEntity()
        {
            
        }

        public virtual void Reinitialize()
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NewPosition">Vector the entity should walk to.</param>
        /// <param name="DeltaTime">Time between updates.</param>
        /// <param name="Speed">Speed the entity should walk in pixels per update.</param>
        

        public Vector2 Scale(Vector2 OldPos, bool center = true)
        {
            Vector2 scale = OldPos / 2;
            if (center) scale.X += 16;
            return scale;
        }

        public static Vector2 Scale(Vector2 OldPos, string a)
        {
            Vector2 scale = OldPos / 2;
            return scale;
        }

        protected bool IsNear(Vector2 a, Vector2 b)
        {
            if (Math.Abs(a.X - b.X) < 4 && Math.Abs(a.Y - b.Y) < 4) return true;
            return false;
        }

        

        public bool Collides(Vector2 Position)
        {
            if (Math.Abs((this.Position.X + 16) - Position.X) < 32 && Math.Abs((this.Position.Y + 16) - Position.Y) < 32)
            {
                return true;
            }
            return false;
        }

        public bool CollidesWith(Vector2 Point)
        {
            return (Math.Abs(Point.X - Position.X) < 16 && Math.Abs(Point.Y - Position.Y) < 16);
        }
    }

    public class Enemy : GameEntity
    {
        public short CurrentStep;
        protected int BaseDamage = 5;
        protected int Health;
        protected int MaxHealth;

        readonly DelayedAction HideHealthDelay;
        bool HideHealth = true;

        public Enemy()
        {
            CurrentStep = 0;
            HideHealthDelay = new DelayedAction(() => { HideHealth = true; }, 2);
        }

        public override void Reinitialize()
        {
            CurrentStep = 0;
            Health = MaxHealth;
            base.Reinitialize();
        }

        public virtual void WalkTo(Vector2 NewPosition, float Speed = 1)
        {
            Vector2 dir = Scale(NewPosition) - Position;
            dir.Normalize();
            Position += dir * (Speed / 16) * LevelScreen.TimeScale;
            if (dir.X < 0) // Negative X
            {
                AnimSprite.Effect = SpriteEffects.FlipHorizontally;
            }
            else if (dir.X > 0) AnimSprite.Effect = SpriteEffects.None;
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
         //   DrawHitbox(sb);
            if (!HideHealth)
            {
                
            }
            base.Draw(gameTime, sb);
        }

        public virtual void DrawHitbox(SpriteBatch sb)
        {
            sb.DrawRectangle(new Vector2(Position.X - 16, Position.Y - 16), new Size2(32, 32), Color.Blue);
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
                    // Player loses health
                }
            }
            foreach (GameEntity ent in EntityManager.Instance.Entities.ToList())
            {
                if (ent is Bullet bullet)
                {
                    if (CollidesWith(bullet.Position))
                    {
                        Health -= bullet.Damage;
                        if (Health < 0) Kill();
                        EntityManager.Instance.RemoveEntity(bullet);
                        HideHealth = false;
                    }
                }
            }
            base.Update(gameTime);
        }

        

        public virtual void Kill()
        {
            EntityManager.Instance.RemoveEntity(this);
        }

        public override void ChangeAnimation(string animName, float scale = 1)
        {
            base.ChangeAnimation(animName, scale);
        }

        public override void InitAnimation(string AtlasName, string SpritePath, string MapPath)
        {
            base.InitAnimation(AtlasName, SpritePath, MapPath);
        }
    }

    public class Tower : GameEntity
    {
        public GameEntity Target;
        public bool Ghost = false;
        public object Parent = null;

        protected int Health = 100;
        protected int Price = 450;
        protected int Damage = 10;
        protected float Radius = 128;
        public int Ammo = 45;
        public int MaxAmmo = 45;
        protected int RefillPrice = 250;
        protected float LocalAngle = 0;

        protected TimedAction ShootTimer;
        protected DelayedAction FireAnimation;

        public Texture2D Texture;

        public override void Reinitialize()
        {
            base.Reinitialize();
        }

        public override void LoadContent()
        {
            FireAnimation = new DelayedAction(() => ChangeAnimation("idle", 0.25f), 0.2f);
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
            Vector2 mousePos = Scale(Mouse.GetState().Position.ToVector2());
            if (this == LevelScreen.GetSelectedTower())
            {
                Vector2 PositionOffset = Position - new Vector2(Radius * 1.4f / 2, Radius * 1.4f / 2);
                //sb.DrawCircle(new CircleF(Position, Radius / 2), 32, new Color(55, 55, 55, 55));
                sb.DrawRectangle(PositionOffset, new Size2(Radius * 1.4f, Radius * 1.4f), Color.Red);
                AnimSprite.Color = new Color(210, 210, 210, 255);
                if (Target != null)
                {
                    sb.DrawLine(Position, Target.Position, Color.Red);
                }
            }
            else
            {
                AnimSprite.Color = new Color(255, 255, 255, 255);
            }
            
            //sb.DrawRectangle(new RectangleF(Position.X - 8, Position.Y - 32, 16, 2), Color.Red);
            if (Ammo > 0) sb.DrawRectangle(new RectangleF(Position.X - 8, Position.Y - 32, 16 * (Ammo / (float)MaxAmmo), 2), Color.Green);

            if (Ghost) AnimSprite.Alpha = 0.25f;
            else AnimSprite.Alpha = 1;
            base.Draw(gameTime, sb);
        }

        public override void Update(GameTime gameTime)
        {
            if (Target != null && !ShootTimer.Active)
            {
                ShootTimer.Start();
            }
            if (Target == null && ShootTimer.Active)
            {
                ShootTimer.Stop();
            }

            ShootTimer.Update(gameTime.GetElapsedSeconds() * LevelScreen.TimeScale);
            FireAnimation.Update(gameTime.GetElapsedSeconds() * LevelScreen.TimeScale);

            if (Parent != null)
            {
                if (Parent is Vector2 vector)
                {
                    Position = vector;
                }
                else
                {
                    System.Reflection.PropertyInfo property = Parent.GetType().GetProperty("Position");
                    if (property != null && property.GetValue(Parent) is Vector2 parentVector)
                    // Is there a Vector2 property named "Position"?
                    {
                        Position = parentVector;
                    }
                }
                
            }

            if (Draggable)
            {
                OnDrag();
                Ghost = true;
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    Draggable = false;
                }
            }
            else Ghost = false;

            GetClosestEnemy(EntityManager.Instance.Entities);
            

            if (Target != null)
                FaceTarget();

            AnimSprite.Update(gameTime);
            base.Update(gameTime);
        }

        public override void ChangeAnimation(string animName, float scale = 1)
        {
            base.ChangeAnimation(animName, scale);
        }

        public override void InitAnimation(string AtlasName, string SpritePath, string MapPath)
        {
            Texture = Content.Load<Texture2D>(@"sprites\us\" + AtlasName + "Preview");
            base.InitAnimation(AtlasName, SpritePath, MapPath);
        }

        protected virtual void OnDrag()
        {
            Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            mousePos = Scale(mousePos, false);
            mousePos.X += BitOffensive.Instance.Camera.Position.X;
            mousePos.Y += BitOffensive.Instance.Camera.Position.Y;
            int cellX = (int)(mousePos.X / 32);
            int cellY = (int)(mousePos.Y / 32);
            mousePos.X = cellX * 32;
            mousePos.Y = cellY * 32;
            Parent = mousePos;
        }

        public bool IsInRange(Vector2 position)
        {
            if (Vector2.Distance(Position, position) <= Radius)
                return true;

            return false;
        }

        public void GetClosestEnemy(List<GameEntity> Entities)
        {
            Target = null;
            float smallestRange = Radius;

            foreach (GameEntity Entity in Entities)
            {
                if (Entity is Enemy)
                {
                    if (Vector2.Distance(Position, Entity.Position) < smallestRange)
                    {
                        smallestRange = Vector2.Distance(Position, Entity.Position);
                        Target = Entity;
                    }
                }
            }
        }

        protected void FaceTarget()
        {
            Vector2 direction = Position - Target.Position;
            direction.Normalize();

            LocalAngle = (float)Math.Atan2(direction.Y, direction.X);
            if (direction.Y / direction.X < 0) // Negative Slope
            {
                AnimSprite.Effect = SpriteEffects.FlipHorizontally;
            }
            else if (direction.Y / direction.X > 0) // Positive Slope
            {
                AnimSprite.Effect = SpriteEffects.None;
            }
        }

        public bool IsMouseColliding()
        {
            Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            mousePos = Scale(mousePos);
            mousePos.X += BitOffensive.Instance.Camera.Position.X - 16;
            mousePos.Y += BitOffensive.Instance.Camera.Position.Y;
            //System.Diagnostics.Debug.Print(Math.Abs(mousePos.X - Position.X).ToString());
            return (Math.Abs(mousePos.X - Position.X) < 32 && Math.Abs(mousePos.Y - Position.Y) < 32);
        }

        public void RefillAmmo()
        {
            Ammo = MaxAmmo;
        }

        public void ShootBullet(string Sound, float Speed = 1)
        {
            if (Ammo > 0 && !Ghost)
            {
                Bullet bullet = EntityManager.Instance.CreateEntity<Bullet>() as Bullet;
                bullet.Reinitialize(Damage, Target.Position, Position, Speed);
                EntityManager.Instance.AddEntity(bullet);
                SoundEffect BulletSound = Content.Load<SoundEffect>(Sound);
                SoundEffectInstance BulletSoundInstance = BulletSound.CreateInstance();
                Vector2 CameraPos = BitOffensive.Instance.Camera.Position;
                CameraPos.X += 640;
                CameraPos.Y += 360;
                float dist = Vector2.Distance(Position, CameraPos);


                if (dist > 200)
                {
                    if (dist < 1200)
                    {
                        BulletSoundInstance.Volume = -(dist / 1200) + 1;
                        BulletSoundInstance.Pan = (float)(Position.X - CameraPos.X) / 1200;
                    }
                    //M16Sound.Volume = (float)Math.Sqrt(1 - (Math.Pow(dist, 2) / 400));
                    else BulletSoundInstance.Volume = 0;
                }
                //M16Sound.Volume = 1 / (float)Math.Ceiling((double)dist - 200);


                BulletSoundInstance.Play();
                Ammo--;

                FireAnimation = new DelayedAction(() => ChangeAnimation("idle", 0.25f), 0.2f);
                ChangeAnimation("firing", 0.25f);
                FireAnimation.Start();
            }
        }
    }

}
