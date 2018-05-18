using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class QuadTree
    {
        public List<Member> Members;

        public QuadTree[] children;
        bool divided;

        float left, bottom, right, top;
        QuadTree parent;

        int max_members;

        const int SplitWidth = 2;
        const int SplitHeight = 2;

        public QuadTree(float left, float bottom, float right, float top, int maxMembers = 10)
        {
            this.left = left;
            this.bottom = bottom;
            this.right = right;
            this.top = top;

            Members = new List<Member>();

            divided = false;
            children = new QuadTree[SplitWidth * SplitHeight];

            max_members = maxMembers;
        }

        public bool AddMember(Member member)
        {
            if (IsInside(member.Position))
            {
                Members.Add(member);
                member.parent = this;

                if(!divided && Members.Count > max_members)
                {
                    Divide();
                }else if (divided)
                {
                    for (int i = 0; i < children.Length; i++)
                        children[i].AddMember(member);
                }

                return true;
            }

            return false;
        }
        
        public bool IsInside(Vector3 position)
        {
            return position.x >= left    && position.x < right &&
                   position.z >= bottom  && position.z < top;
        }

        public void DebugRenderer(Vector3 offset)
        {
            if (divided)
            {
                foreach (QuadTree child in children)
                    child.DebugRenderer(offset);
            }
            else {
                Debug.DrawLine(new Vector3(left, 0, bottom) + offset,
                    new Vector3(right, 0, bottom) + offset, Color.cyan);
                Debug.DrawLine(new Vector3(left, 0, top) + offset,
                    new Vector3(right, 0, top) + offset, Color.cyan);
                Debug.DrawLine(new Vector3(left, 0, bottom) + offset,
                    new Vector3(left, 0, top) + offset, Color.cyan);
                Debug.DrawLine(new Vector3(right, 0, bottom) + offset,
                    new Vector3(right, 0, top) + offset, Color.cyan);
            }
            
        }

        void Divide()
        {
            if (!divided)
            {
                divided = true;

                float y = bottom;

                float dx = (right - left) / SplitWidth;
                float dy = (top - bottom) / SplitHeight;

                int index = 0;

                for (int j = 0; j < SplitHeight; j++)
                {
                    float x = left;

                    for (int i = 0; i < SplitWidth; i++)
                    {
                        children[index] = new QuadTree(x, y, x + dx, y + dy);

                        foreach (Member m in Members)
                            children[index].AddMember(m);
                        
                        index++;
                        x += dx;
                    }

                    y += dy;
                }
            }
        }

        public abstract class Member
        {
            Vector3 position;
            public QuadTree parent;

            public Vector3 Position
            {
                get
                {
                    return position;
                }
                set
                {
                    position = value;
                    check_quad();
                }
            }

            public Member(Vector3 position)
            {
                this.position = position;
            }

            void check_quad()
            {

            }
        }
    }
}
