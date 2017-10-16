using System;

using System.Diagnostics;

namespace ProductionCell
{
    class Arm
    {
        // variable area

        private float maxLength;

        private float length;
        private float angle;
        private float targetLength;

        private float moveSpeed;

        private bool hold;
        private bool enable;

        // property area

        public float Length { get { return length; } }
        public float MaxLength { get { return maxLength; } }
        public float Angle {  get { return angle; } }
        public bool isHold { get { return hold; } }
        public float SetTargetLength
        {
            set
            {
                Debug.Assert(maxLength >= value, "Arm max length = " + maxLength);
                targetLength = value;
            }
        }

        public bool isReady
        {
            get
            {
                if(length == targetLength)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // constructor
        public Arm(float maxLength, float InitAngle, float moveSpeed)
        {
            this.maxLength = maxLength;
            length = 0;
            targetLength = 0;
            angle = InitAngle;
            this.moveSpeed = moveSpeed;
            enable = true;
            hold = false;
        }

        // method area

        public void Monitor()
        {
            Console.WriteLine("length : " + length + ", targetLength : " + targetLength);
        }

        public void Update()
        {
            if(enable)
            {
                Move();
            }
        }

        public void Pick()
        {
            hold = true;
        }

        public void Drop()
        {
            hold = false;
        }

        private void Move()
        {
            if(length > targetLength)
            {
                if(length - moveSpeed < targetLength)
                {
                    length = targetLength;
                }
                else
                {
                    length -= moveSpeed;
                }
            }
            else if(length < targetLength)
            {
                if (length + moveSpeed > targetLength)
                {
                    length = targetLength;
                }
                else
                {
                    length += moveSpeed;
                }
            }
        }
    }
}
