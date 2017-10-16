using System;

using System.Diagnostics;

namespace ProductionCell
{
    class Crane
    {
        // variable area

        private Arm gripper;

        private float horizonPosition;    // 0 ~ 1
        private float horizonSpeed;
        private float targetHorizonPosition;

        private bool enable;

        // property area

        public float HorizonPosition { get { return horizonPosition; } }
        public Arm Gripper { get { return gripper; } }
        
        // command position
        public float SetHorizonPosition
        {
            set
            {
                Debug.Assert(value <= 1 && value >= 0, "Crane position 0 ~ 1");
                targetHorizonPosition = value;
            }
        }

        // ready to next work
        public bool isReady
        {
            get
            {
                if(horizonPosition == targetHorizonPosition)
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

        public Crane(Arm gripper, float horizonSpeedBetweenZeroAndOne)
        {
            Debug.Assert(horizonSpeedBetweenZeroAndOne > 0 && horizonSpeedBetweenZeroAndOne <= 1, "Crane horizon speed 0 ~ 1");
            horizonSpeed = horizonSpeedBetweenZeroAndOne;

            this.gripper = gripper;

            horizonPosition = 0;
            targetHorizonPosition = 0;

            enable = true;
        }

        // method area

        public void Update()
        {
            if(enable)
            {
                HorizonMove();
                gripper.Update();
            }
        }

        private void HorizonMove()
        {
            if (targetHorizonPosition < horizonPosition)
            {
                if (horizonPosition - horizonSpeed < targetHorizonPosition)
                {
                    horizonPosition = targetHorizonPosition;
                }
                else
                {
                    horizonPosition -= horizonSpeed;
                }
            }
            else if (targetHorizonPosition > horizonPosition)
            {
                if (horizonPosition + horizonSpeed > targetHorizonPosition)
                {
                    horizonPosition = targetHorizonPosition;
                }
                else
                {
                    horizonPosition += horizonSpeed;
                }
            }
        }

        public void Monitor()
        {
            Console.WriteLine("position : " + horizonPosition.ToString("N1") + ", targetposition : " + targetHorizonPosition.ToString("N1"));
            Console.WriteLine("gripper Status");
            gripper.Monitor();
        }
    }
}
