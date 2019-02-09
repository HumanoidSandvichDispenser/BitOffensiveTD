using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitOffensive.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using GeonBit.UI;
using GeonBit.UI.Entities;
using System.Diagnostics;

namespace BitOffensive.GameState
{
    public class LevelScreen : GameScreen
    {
        public Button RiflemanButton { get; private set; }
        public Button SniperButton { get; private set; }
        public Button MachineGunButton { get; private set; }
        public Button RPGButton { get; private set; }
        public Button DoubleTimescale { get; private set; }
        public Button NormalTimescale { get; private set; }

        public Button RefillAmmoButton { get; private set; }
        public Button TowerBackButton { get; private set; }
        public Button TowerMoveButton { get; private set; }
        public Image TowerPreview { get; private set; }

        public Tower SelectedTower;
        public Tower CreatingTower;

        public static LevelScreen Instance;

        private TimedAction VietCongTimer;
        private TimedAction VietCongArmorTimer;
        private TimedAction TruckTimer;

        private TimedAction GiveMoneyTimer;
        DelayedAction NewRound;
        private Camera2D Camera = BitOffensive.Instance.Camera;
        private TiledMap Map;
        private TiledMapRenderer mapRenderer;
        public static int TimeScale = 1;
        public static string MapPath = "maps/khesanh";
        private int previousScrollValue;
        private int Money = 0;
        private int Income = 50;
        private int CurrentRound = 0;
        private int RemainingInfantry = 0;
        private bool StartingRound = false;

        private Panel MainPanel;
        private Panel TowerPanel;

        public static Tower GetSelectedTower()
        {
            return Instance.SelectedTower;
        }

        public LevelScreen(string map)
        {
            MapPath = map;
        }

        public override void LoadContent()
        {
            Instance = this;

            Content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
            VietCongTimer = new TimedAction(SpawnEnemy<VietCongInfantry>, GetTime());
            VietCongArmorTimer = new TimedAction(SpawnEnemy<VietCongArmored>, GetTime() * 2);
            TruckTimer = new TimedAction(SpawnEnemy<Truck>, GetTime() * 5);

            GiveMoneyTimer = new TimedAction(() => Money += Income, 5.0f);
            NewRound = new DelayedAction(() => { StartTimers(); RemainingInfantry = GetInfantry(); StartingRound = false; }, 5);
            
            Map = Content.Load<TiledMap>("maps/khesanh");
            mapRenderer = new TiledMapRenderer(BitOffensive.Instance.GraphicsDevice);

            VietCongTimer.Start();

            GiveMoneyTimer.Start();

            previousScrollValue = Mouse.GetState().ScrollWheelValue;
            
            base.LoadContent();
            
            MainPanel = new Panel(new Vector2(1280, 200), PanelSkin.Simple, Anchor.BottomCenter);
            TowerPanel = new Panel(new Vector2(1280, 200), PanelSkin.Default, Anchor.BottomCenter);
            Init_Menu();
            Init_TowerMenu();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            VietCongTimer.Update((float)gameTime.ElapsedGameTime.TotalSeconds * TimeScale);
            VietCongArmorTimer.Update((float)gameTime.ElapsedGameTime.TotalSeconds * TimeScale);
            TruckTimer.Update((float)gameTime.ElapsedGameTime.TotalSeconds * TimeScale);
            GiveMoneyTimer.Update((float)gameTime.ElapsedGameTime.TotalSeconds * TimeScale);
            NewRound.Update((float)gameTime.ElapsedGameTime.TotalSeconds * TimeScale);

            KeyboardState kb = Keyboard.GetState();
            MouseState ms = Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed)
            {
                foreach (GameEntity ent in EntityManager.Instance.Entities.ToList())
                {
                    if (ent is Tower tower)
                    {
                        if (tower.IsMouseColliding())
                        {
                            try
                            {
                                if (SelectedTower == null)
                                {
                                    UserInterface.Active.AddEntity(TowerPanel);
                                }
                            }
                            catch (GeonBit.UI.Exceptions.InvalidStateException)
                            {
                                Debug.Print("UI is already displayed. Caught InvalidStateException.");
                            }
                            SelectedTower = tower;
                            TowerPreview.Texture = SelectedTower.Texture;
                        }
                    }
                }
            }
            if (kb.IsKeyDown(Keys.Q) && SelectedTower != null)
            {
                UserInterface.Active.RemoveEntity(TowerPanel);
                SelectedTower = null;
            }
            if (kb.IsKeyDown(Keys.D) && Camera.Position.X < 128) // 256
            {
                Camera.Move(new Vector2(16, 0));
            }
            if (kb.IsKeyDown(Keys.A) && Camera.Position.X > 0)
            {
                Camera.Move(new Vector2(-16, 0));
            }
            if (kb.IsKeyDown(Keys.W) && Camera.Position.Y > 0)
            {
                Camera.Move(new Vector2(0, -16));
            }
            if (kb.IsKeyDown(Keys.S) && Camera.Position.Y < 488) // 920
            {
                Camera.Move(new Vector2(0, 16));
            }
            if (kb.IsKeyUp(Keys.W) && kb.IsKeyUp(Keys.S) && kb.IsKeyUp(Keys.A) && kb.IsKeyUp(Keys.D))
            {

            }
            UserInterface.Active.Update(gameTime);
            EntityManager.Instance.UpdateEntities(gameTime);
            mapRenderer.Update(Map, gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
            Matrix ViewMatrix = Camera.GetViewMatrix();
            ViewMatrix *= Matrix.CreateScale(2);
            sb.Begin(sortMode: SpriteSortMode.BackToFront, transformMatrix: ViewMatrix);
            mapRenderer.Draw(Map, ViewMatrix);
            EntityManager.Instance.Sort();
            EntityManager.Instance.DrawEntities(gameTime, sb);
            sb.End();
            sb.Begin();
            sb.DrawString(Content.Load<SpriteFont>(@"GeonBit.UI\themes\bitoffensive\fonts\regular"), Mouse.GetState().X.ToString(),
                new Vector2(25, 25), Color.White);
            sb.DrawString(Content.Load<SpriteFont>(@"GeonBit.UI\themes\bitoffensive\fonts\regular"), Mouse.GetState().Y.ToString(),
                new Vector2(25, 75), Color.White);
            sb.DrawString(Content.Load<SpriteFont>(@"fonts\Money"), $"${Money}",
                new Vector2(25, 448), Color.LightGreen);
            sb.DrawString(Content.Load<SpriteFont>(@"fonts\Money"), $"Wave {CurrentRound}",
                new Vector2(85, 448), Color.Red);
            if (StartingRound)
            {
                sb.DrawString(Content.Load<SpriteFont>(@"fonts\Money"), $"Starting Wave {CurrentRound}",
                new Vector2(85, 300), Color.Red);
            }
            sb.End();
            base.Draw(gameTime, sb);
        }

        private void Init_Menu()
        {
            UserInterface.Active.AddEntity(MainPanel);

            RiflemanButton = new Button("Rifleman", ButtonSkin.Alternative, size: new Vector2(512, 48));
            RiflemanButton.OnClick += (Entity ent) =>
            {
                /*
                Marine marine = new Marine();
                marine.Ghost = true;
                EntityManager.Instance.AddEntity(new Marine());
                */
                BuyTower<Marine>(200);
            };

            SniperButton = new Button("Sniper", ButtonSkin.Alternative, size: new Vector2(512, 48));
            SniperButton.OnClick += (Entity ent) =>
            {
                BuyTower<Sniper>(500);
            };

            MachineGunButton = new Button("Machine Gunner", ButtonSkin.Alternative, size: new Vector2(512, 48));
            MachineGunButton.OnClick += (Entity ent) =>
            {
                BuyTower<MachineGun>(1200);
            };

            NormalTimescale = new Button("x1", ButtonSkin.Alternative, Anchor.TopRight, new Vector2(48, 48), new Vector2(-48, 0));
            NormalTimescale.OnClick += (Entity ent) =>
            {
                TimeScale = 1;
            };

            DoubleTimescale = new Button("x2", ButtonSkin.Alternative, Anchor.TopRight, new Vector2(48, 48));
            DoubleTimescale.OnClick += (Entity ent) =>
            {
                TimeScale = 2;
            };

            MainPanel.AddChild(RiflemanButton);
            MainPanel.AddChild(SniperButton);
            MainPanel.AddChild(MachineGunButton);
            MainPanel.AddChild(NormalTimescale);
            MainPanel.AddChild(DoubleTimescale);
        }

        private void Init_TowerMenu()
        {
            RefillAmmoButton = new Button("Refill Ammo", ButtonSkin.Alternative, Anchor.TopLeft, new Vector2(256, 64));
            RefillAmmoButton.OnClick += (Entity ent) =>
            {
                SelectedTower.RefillAmmo();
            };

            TowerBackButton = new Button("Back [Q]", ButtonSkin.Alternative, Anchor.TopRight, new Vector2(208, 64));
            TowerBackButton.OnClick += (Entity ent) =>
            {
                UserInterface.Active.RemoveEntity(TowerPanel);
                SelectedTower = null;
            };

            TowerMoveButton = new Button("Move", ButtonSkin.Alternative, Anchor.TopLeft, new Vector2(256, 64), new Vector2(274, 0));
            TowerMoveButton.OnClick += (Entity ent) =>
            {
                SelectedTower.Draggable = true;
            };

            TowerPreview = new Image(Content.Load<Texture2D>(@"sprites\us\MarineA"), new Vector2(256, 256));

            TowerPanel.AddChild(RefillAmmoButton);
            TowerPanel.AddChild(TowerBackButton);
            TowerPanel.AddChild(TowerMoveButton);
            TowerPanel.AddChild(TowerPreview);
        }

        private void BuyTower<T>(int Cost) where T : new()
        {
            if (Money >= Cost)
            {
                Money -= Cost;
                if (typeof(T).IsSubclassOf(typeof(Tower))) // Is the type passed derived from class Tower?
                {
                    Tower tower = EntityManager.Instance.CreateEntity<T>() as Tower;
                    EntityManager.Instance.AddEntity(tower);
                }
                
            }
        }

        private void NextRound()
        {
            int enemies = 0;
            foreach (GameEntity ent in EntityManager.Instance.Entities)
            {
                if (ent is Enemy)
                {
                    enemies++;
                }
            }
            if (enemies < 1)
            {
                VietCongTimer.Stop();
                VietCongArmorTimer.Stop();
                TruckTimer.Stop();
                CurrentRound++;
                StartingRound = true;
                Money += 500;
                VietCongTimer = new TimedAction(SpawnEnemy<VietCongInfantry>, GetTime());
                VietCongArmorTimer = new TimedAction(SpawnEnemy<VietCongArmored>, GetTime() * 1.5f);
                TruckTimer = new TimedAction(SpawnEnemy<Truck>, GetTime() * 5);
                NewRound.Start();
            }
            
        }

        private void SpawnEnemy<T>() where T : new()
        {
            if (RemainingInfantry > 0)
            {
                GameEntity ent = EntityManager.Instance.CreateEntity<T>();
                T enemy = (T)(object)ent;
                
                EntityManager.Instance.AddEntity(enemy as GameEntity).Position =
                new Vector2(-32, 64);
                RemainingInfantry--;
            }
            else
            {
                NextRound();
            }
        }

        private void StartTimers()
        {
            VietCongTimer.Start();
            if (CurrentRound > 5) VietCongArmorTimer.Start();
            if (CurrentRound > 10) TruckTimer.Start();
        }

        private int GetInfantry()
        {
            int Infantry = (CurrentRound * 2) + 8;

            if (CurrentRound > 5)
            {
                Infantry += 5;
            }
            if (CurrentRound > 10)
            {
                Infantry += 5;
            }
            if (CurrentRound > 20)
            {
                Infantry += 25;
            }

            if (CurrentRound == 5 || CurrentRound == 10)
            {
                Infantry += 10;
            }
            if (CurrentRound == 15 || CurrentRound == 20)
            {
                Infantry += 20;
            }
            if (CurrentRound == 25 || CurrentRound == 30)
            {
                Infantry += 40;
            }
            return Infantry;
        }

        private float GetTime()
        {
            float Time = 2.5f;
            Time -= (CurrentRound % 5) * 0.35f;
            if (CurrentRound > 5)
            {
                Time -= 0.15f;
            }
            if (CurrentRound > 10)
            {
                Time -= 0.15f;
            }
            if (CurrentRound > 15)
            {
                Time -= 0.15f;
            }
            if (CurrentRound > 20)
            {
                Time -= 0.2f;
            }

            if (CurrentRound == 5 || CurrentRound == 10)
            {
                Time = 0.45f;
            }
            if (CurrentRound == 15 || CurrentRound == 20)
            {
                Time = 0.35f;
            }
            if (CurrentRound == 25 || CurrentRound == 30)
            {
                Time = 0.25f;
            }
            return Time;
        }
    }
}
