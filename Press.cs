using System;

using System.Diagnostics;

namespace ProductionCell
{
    class Press
    {
        // variable area

        private float height;   // 0 ~ 1
        private float verticalSpeed;
        private float targetHeight;

        private bool enable;

        // property area

        public float Height { get { return height; } }
        public float SetTargetHeight
        {
            set
            {
                Debug.Assert(value >= 0 && value <= 1, "Press height 0 ~ 1");
                targetHeight = value;
            }
        }
        public bool isReady
        {
            get
            {
                if(targetHeight == height)
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
        public Press(float moveSpeedBetweenZeroAndOne)
        {
            Debug.Assert(moveSpeedBetweenZeroAndOne > 0 && moveSpeedBetweenZeroAndOne <= 1, "Press move speed 0 ~ 1");

            verticalSpeed = moveSpeedBetweenZeroAndOne;
            height = 0;
            targetHeight = height;

            isElementExist = false;
            enable = true;
        }

        // method area

        public void Update()
        {
            if(enable)
            {
                VercitalMove();
            }
        }

        public void ElementPress()
        {
            // press the element
        }

        private void VercitalMove()
        {
            if (targetHeight < height)
            {
                if (height - verticalSpeed < targetHeight)
                {
                    height = targetHeight;
                }
                else
                {
                    height -= verticalSpeed;
                }
            }
            else if (targetHeight > height)
            {
                if (height + verticalSpeed > targetHeight)
                {
                    height = targetHeight;
                }
                else
                {
                    height += verticalSpeed;
                }
            }
        }

        public void Monitor()
        {
            Console.WriteLine("Height : " + height.ToString("N1") + ", targetHeight : " + targetHeight.ToString("N1"));
        }
    }
}
