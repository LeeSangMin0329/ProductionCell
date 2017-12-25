using System;
using System.Collections.Generic;

using System.Diagnostics;

namespace ProductionCell
{
    class Belt
    {
        // Variable Area

        private float moveDistance;
        private List<float> elementStack;

        private bool enable;
        private bool elementArriveSensor;
        private float velocityPerFrame;
        private float length;

        private int maxCount = 5; // on belt max element num
        
        // Property Area

        private float Velocity
        {
            get { return velocityPerFrame; }
          
        }

        private float Length
        {
            get { return length; }
        }

        public bool Enable { get { return enable; } }
        public bool isElementArrive { get { return elementArriveSensor; } }
        public int Count { get { return elementStack.Count; } }

        // constructor

        public Belt(float length, float velocity)
        {
           
            this.velocityPerFrame = velocity;
            Debug.Assert(velocityPerFrame > 0, "Belt velocity <= 0");
            
            this.length = length;
            Debug.Assert(this.length > 0, "Belt Length > 0");
            elementStack = new List<float>();

            enable = true;
            elementArriveSensor = false;
        }


        // Method Area

        public bool AddElement()
        {
            if(!isElementArrive && maxCount > elementStack.Count)
            {
                elementStack.Add(0);
                return true;
            }
            return false;
        }

        public void Update()
        {
            if(enable)
            {
                Move();
            }
        }

        private void Move()
        {
            if(elementStack.Count == 0)
            {
                return;
            }

            if (length < elementStack[0] + velocityPerFrame)
            {
                moveDistance = length - elementStack[0];

                elementArriveSensor = true;
                enable = false;
            }
            else
            {
                moveDistance = velocityPerFrame;
            }

            for (int i = 0; i < elementStack.Count; i++)
            {
                elementStack[i] += moveDistance;
            }
        }

        public void TakeElement()
        {
            if(elementStack.Count == 0)
            {
                return;
            }

            elementStack.RemoveAt(0);
           
            elementArriveSensor = false;
            enable = true;
           
        }

        public void Monitor()
        {
            Console.WriteLine("belt length : " + length);
            Console.WriteLine("element count : " + elementStack.Count);
            foreach(float element in elementStack)
            {
                Console.Write(element + " ");
            }
            Console.WriteLine();
            Console.WriteLine("arrive : " + isElementArrive);
            
        }
    }
}
