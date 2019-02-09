using BitOffensive.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitOffensive.Assets
{
    public class EntityManager
    {
        private static EntityManager instance;
        public static EntityManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EntityManager();
                }
                return instance;
            }
        }

        public List<GameEntity> Entities = new List<GameEntity>();
        public List<GameEntity> EntityPool = new List<GameEntity>();
        public List<GameObject> Objects = new List<GameObject>();

        public GameEntity CreateEntity<T>() where T : new()
        {
            foreach (GameEntity ent in EntityPool)
            {
                if (ent is T)
                {
                    ent.Reinitialize();
                    EntityPool.Remove(ent);
                    return ent;
                }
            }
            
            try
            {
                return new T() as GameEntity;
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }

        public GameEntity AddEntity(GameEntity ent, bool fromEntPool = false)
        {
            if (fromEntPool) EntityPool.Remove(ent);
            Entities.Add(ent);
            ent.LoadContent();
            return ent;
        }

        public void RemoveEntity(GameEntity ent)
        {
            Entities.Remove(ent);
            EntityPool.Add(ent);
        }

        public void UpdateEntities(GameTime gameTime)
        {
            foreach (GameEntity ent in Entities.ToList())
            {
                ent.Update(gameTime);
            }
            foreach (GameObject obj in Objects.ToList())
            {
                obj.Update(gameTime);
            }
        }

        public void Sort()
        {
            //List<GameEntity> SortedList = Entities.OrderByDescending(x => x.Position.Y).ToList();
            
        }

        public void DrawEntities(GameTime gameTime, SpriteBatch sb)
        {
            Entities.Sort((x, y) => x.Position.Y.CompareTo(y.Position.Y));

            foreach (GameEntity ent in Entities)
            {
                ent.Draw(gameTime, sb);
            }
            
        }

        public void MoveEntities(Vector2 vector)
        {
            foreach (GameEntity ent in Entities)
            {
                ent.Position += vector;
            }
        }
    }
}
