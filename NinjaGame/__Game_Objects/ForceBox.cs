using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace NinjaGame
{
    /// <summary>
    /// basically a box which applies a force to a character on intersection
    /// </summary>
    public class ForceBox : GameObject
    {
        // the direction of the force
        private Vector2 m_direction;

        // the power of the force (/Cue Vader Breathing sound effect...)
        private float m_power;

        // the box coordinates
        private Rectangle m_box;

        //are characters affected by the force box
        private bool m_charactersAffected;

        //are projectiles affected by the force box
        private bool m_projectilesAffected;

        /// <summary>
        /// direction of the force
        /// </summary>
 
        public Vector2 Direction { get { return m_direction; } set { m_direction = value; } }

        public float DirectionX { get { return m_direction.X; } set { m_direction.X = value; } }

        public float DirectionY { get { return m_direction.Y; } set { m_direction.Y = value; } }

        public bool CharactersAffected { get { return m_charactersAffected; } set { m_charactersAffected = value; } }

        public bool ProjectilesAffected { get { return m_projectilesAffected; } set { m_projectilesAffected = value; } }

        /// <summary>
        /// the power of the force
        /// </summary>
        public float Power { get { return m_power; } set { m_power = value; } }

        /// <summary>
        /// the left x coordinate
        /// </summary>
        public float Left { get { return m_box.Left; } }

        /// <summary>
        /// the right x coordinate
        /// </summary>
        public float Right { get { return m_box.Right; } }

        /// <summary>
        /// the top y coordinate
        /// </summary>
        public float Top { get { return m_box.Top; } }

        /// <summary>
        /// the bottom y coordinate
        /// </summary>
        public float Bottom { get { return m_box.Bottom; } }


        /// <summary>
        /// default constructor
        /// </summary>
        public ForceBox() : base(false,false,true,true)
        {
            m_box = new Rectangle();
        }

        public override void ReadXml(XmlObjectData data)
        {
            base.ReadXml(data);

            data.ReadFloat("DirectionX", ref m_direction.X);

            data.ReadFloat("DirectionY", ref m_direction.Y);

            data.ReadFloat("Force", ref m_power);

            data.ReadBool("CharactersAffected", ref m_charactersAffected);

            data.ReadBool("ProjectilesAffected", ref m_projectilesAffected);

            m_box.X = (int)PositionX;

            m_box.Y = (int)PositionY;

            m_box.Width = (int)BoxDimensionsX;

            m_box.Height = (int)BoxDimensionsY;

            
        }

        public override void WriteXml(XmlObjectData data)
        {
            base.WriteXml(data);

            data.Write("DirectionX", m_direction.X);

            data.Write("DirectionY", m_direction.Y);

            data.Write("Force", m_power);

            data.Write("CharactersAffected", m_charactersAffected);

            data.Write("ProjectilesAffected", m_projectilesAffected);

        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            // check any overlap intersections
            CheckIntersections();
            
        }

        /// <summary>
        /// checks for overlap intersections between other game objects
        /// </summary>
        public void CheckIntersections()
        {
            LevelCollisionQuery c = Core.Level.Collision;

            // do overlap detection
            c.Overlap(Position, new Vector2(m_box.Width, m_box.Height), this);

            // go through the overlap results
            for (int i = 0; i < c.OverlapResultCount; i++)
            {   

                    // check if its a character intersecting and if characters can be affected by the force
                    if (c.OverlapResults[i].QueryObject.GetType().IsSubclassOf(Type.GetType("NinjaGame.Character"))
                        && m_charactersAffected)
                    {
                        // call the apply force for character function
                        ApplyForce_Character( (Character) c.OverlapResults[i].QueryObject);

                    }

                    //check if projectile and check if projectiles can be affected by the force
                    else if (c.OverlapResults[i].QueryObject.GetType() == Type.GetType("NinjaGame.Projectile")
                             && m_projectilesAffected)
                    {
                            // call the apply force for projectile function
                            ApplyForce_Projectile((Projectile)c.OverlapResults[i].QueryObject);
                    }

            }// end of for
        }

        /// <summary>
        /// applies the force of the force box(m_power * m_direction) on a character object type
        /// </summary>
        public void ApplyForce_Character(Character character)
        {
            Vector2 force = new Vector2(m_power * m_direction.X , m_power * m_direction.Y);

            character.MoveVelocity += force;
        }

        /// <summary>
        /// applies the force of the force box(m_power * m_direction) on a projectile object type
        /// </summary>
        public void ApplyForce_Projectile(Projectile projectile)
        {
            Vector2 force = new Vector2(m_power * m_direction.X, m_power * m_direction.Y);

            projectile.Velocity += force;
        }


    }
}
