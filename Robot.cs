using System;

namespace ProductionCell
{
    class Robot
    {
        // variable area

        private const float INITANGLE = 45;

        private Arm arm1;
        private Arm arm2;

        private float angle;
        private float rotationSpeed;
        private float targetAngle;

        private bool enable;

        // property area

        public float Angle { get { return angle; } }
        public Arm Arm1 { get { return arm1; } }
        public Arm Arm2 { get { return arm2; } }
        public float SetTargetAngle
        {
            set
            {
                targetAngle = value % 360.0f;
            }
        }
        public bool isReady
        {
            get
            {
                if(targetAngle == angle)
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

        public Robot(Arm arm1, Arm arm2, float rotationSpeed)
        {
            this.arm1 = arm1;
            this.arm2 = arm2;

            angle = INITANGLE;
            targetAngle = INITANGLE;
            this.rotationSpeed = rotationSpeed;
           
            enable = true;
        }

        // method area

        public void Update()
        {
            if(enable)
            {
                arm1.Update();
                arm2.Update();
                Rotation();
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
            else if (targetAngle > angle)
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
            Console.WriteLine("Arm1 Status");
            arm1.Monitor();
            Console.WriteLine("Arm2 Status");
            arm2.Monitor();
        }
    }
}
