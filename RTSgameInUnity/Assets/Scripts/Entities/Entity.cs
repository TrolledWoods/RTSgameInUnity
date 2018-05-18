using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class Entity : QuadTree.Member
    {

        public GameObject controlling;

        public Entity(Vector3 position, GameObject controlling) : base(position)
        {
            this.controlling = controlling;
            this.controlling.transform.position = position;
        }

    }
}
