using System;

using System.Diagnostics;

namespace ProductionCell
{
    class RotaryTable
    {
        // variable area

        private const float MAXANGLE = 45;

        private bool enable;
        private float angle;        // 0 ~ 45
        private float targetAngle;
        private float height;       // 0 ~ 1
        private float targetHeight;
        private float rotationSpeed;
        private float verticalSpeed;

        // property area

        public bool Enable { get { return enable; } }
        public float Angle { get { return angle; } }
        public double Height { get { return height; } }
        public float SetTargetAngle
        {
            set
            {
                Debug.Assert(value <= MAXANGLE && value >= 0, "Rotary table angle 0 ~ " + MAXANGLE);
                
                targetAngle = value;
            }
        }
        public float SetTargetHeight
        {
            set
            {
                Debug.Assert(value <= 1 && value >= 0, "Rotary table height 0 ~ 1");
                
                targetHeight = value;
            }
        }
        public bool isReady
        {
            get
            {
                if (height ==  targetHeight  && targetAngle ==  angle )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool isElementExist { get; set; }
        
        // constructor

        public RotaryTable(float rotationSpeed, float verticalMoveSpeedBetweenZeroAndOne)
        {
            Debug.Assert(rotationSpeed <= MAXANGLE && rotationSpeed > 0, "Rotary Table rotation speed must 0 ~ " + MAXANGLE + " angle");
            Debug.Assert(verticalMoveSpeedBetweenZeroAndOne > 0 && verticalMoveSpeedBetweenZeroAndOne <= 1, "Rotary Table vertical speed must 0 ~ 1");

            enable = true;
            angle = 0;
            targetAngle = 0;
            targetHeight = 0;
            height = 0;
            this.rotationSpeed = rotationSpeed;
            this.verticalSpeed = verticalMoveSpeedBetweenZeroAndOne;
            isElementExist = false;
        }

        // method area
       
        public void Update()
        {
            if (enable)
            {
                Rotation();
                VerticalMove();
            }
        }

        private void VerticalMove()
        {
            if(targetHeight < height)
            {
                if (height - verticalSpeed <= targetHeight)
                {
                    height = targetHeight;
                }
                else
                {
                    height -= verticalSpeed;
                }
            }
            else if(targetHeight > height)
            {
                if(height + verticalSpeed >= targetHeight)
                {
                    height = targetHeight;
                }
                else
                {
                    height += verticalSpeed;
                }
            }
        }

        private void Rotation()
        {
            if (targetAngle < angle)
            {
                if (angle - rotationSpeed < targetAngle)
                {
                    angle = targetAngle;
                }
                else
                {
                    angle -= rotationSpeed;
                }
            }
            else if(targetAngle > angle)
            {
                if (angle + rotationSpeed > targetAngle)
                {
                    angle = targetAngle;
                }
                else
                {
                    angle += rotationSpeed;
                }
            }
        }
        
        public void Monitor()
        {
            Console.WriteLine("angle : " + angle + ", targetAngle : " + targetAngle);
            Console.WriteLine("height : " + height.ToString("N1") + ", targetHeight : " + targetHeight.ToString("N1"));
        }
    }
}
