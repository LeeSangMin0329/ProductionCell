using System;
using System.Collections.Generic;

using System.Diagnostics;

/*  Production Cell
 *  
 *  State Explain.
 *  **"Set" is move some state so other object not interfere.
 *  **"Ready" is object ready to next work, interaction other object soon.
 * 
 *  Input any key -> inputQueue.add element
 *  
 */

namespace ProductionCell
{
    class CellManager
    {
        // Initializaion variable

        private const float FeedBeltLength = 5;
        private const float FeedBeltSpeed = 2;

        private const float DepositBeltLength = 15;
        private const float DepositBeltSpeed = 2;

        private const float RotaryTableRotationSpeed = 5;
        private const float RotaryTableVerticalSpeed = 0.2f;

        private const float RobotArm1Length = 10;
        private const float RobotArm1MoveSpeed = 2;
        private const float RobotArm1InitAngle = 0;
        private const float RobotArm2Lenth = 6;
        private const float RobotArm2MoveSpeed = 2;
        private const float RobotArm2InitAngle = 90;
        private const float RobotRotationSpeed = 30;

        private const float DistanceBetweenRobotAndRotaryTable = 6;
        private const float DistanceBetweenRobotAndPress = 4;
        private const float DistanceBetweenRobotAndDepositBelt = 5;

        private const float PressVerticalSpeed = 0.2f;

        private const float CraneHorizonSpeed = 0.3f;
        private const float CraneArmLength = 6;
        private const float CraneArmMoveSpeed = 1;

        private const float DistanceBetweenCraneAndDepositBelt = 3;
        private const float DistanceBetweenCraneAndFeedBelt = 5;

        // variable area

        private Belt depositBelt;
        private Belt feedBelt;
        private Crane crane;
        private Robot robot;
        private RotaryTable rotaryTable;
        private Press press;

        private Queue<bool> inputQueue;

        // State Machine

        private enum RobotState { RotarySet, RotaryReady, PressSet, PressReady, }
        private RobotState robotState = RobotState.RotarySet;

        private enum PressState { Arm1PosSet, Arm1PosReady, Arm2PosSet, Arm2PosReady, PressIn, }
        private PressState pressState = PressState.Arm1PosSet;

        private enum RotaryTableState { FeedBeltSet, FeedBeltReady, RobotSet, RobotReady, }
        private RotaryTableState rotaryState = RotaryTableState.FeedBeltSet;

        private enum CraneState { DepositBeltSet, DepositBeltReady, FeedBeltSet, FeedBeltReady, }
        private CraneState craneState = CraneState.DepositBeltSet;

        // event handling

        private System.Timers.Timer timer = new System.Timers.Timer();
        private bool timerCheck = true;

        private void Frame(object sender, EventArgs eventArgs)
        {
            // wait event end trigger
            if(timerCheck == true)
            {
                timer.Enabled = false;
            }

            // print status
            Console.Clear();
            Console.Write("Ready Queue count : ");
            Console.WriteLine(inputQueue.Count);
            Console.WriteLine();

            if (feedBelt.isElementArrive)
            {
                Console.WriteLine("Feedbelt\tReady");
            }
            else
            {
                Console.WriteLine("Feedbelt\tSet");
            }
            Console.WriteLine("Feed belt Status");
            feedBelt.Monitor();
            Console.WriteLine("-----------------------------------------------");

            Console.WriteLine("RotaryTable\t" + rotaryState.ToString());
            Console.WriteLine("RotaryTable Status");
            rotaryTable.Monitor();
            Console.WriteLine("-----------------------------------------------");

            Console.WriteLine("Robot\t\t" + robotState.ToString());
            Console.WriteLine("Robot Status");
            robot.Monitor();
            Console.WriteLine("-----------------------------------------------");

            Console.WriteLine("Press\t\t" + pressState.ToString());
            Console.WriteLine("Press Status");
            press.Monitor();
            Console.WriteLine("-----------------------------------------------");

            if (depositBelt.isElementArrive)
            {
                Console.WriteLine("Depositbelt\tReady");
            }
            else
            {
                Console.WriteLine("Depositbelt\tSet");
            }
            Console.WriteLine("Deposit belt Status");
            depositBelt.Monitor();
            Console.WriteLine("-----------------------------------------------");

            Console.WriteLine("Crane\t\t" + craneState.ToString());
            Console.WriteLine("Crane Status");
            crane.Monitor();
            Console.WriteLine("-----------------------------------------------");

            // real work
            MainUpdate();

            // wait event end trigger
            if(timerCheck == true)
            {
                timer.Enabled = true;
            }
        }
       
        // constructor

        public CellManager()
        {
            depositBelt = new Belt(DepositBeltLength, DepositBeltSpeed);
            
            feedBelt = new Belt(FeedBeltLength, FeedBeltSpeed);
            crane = new Crane(new Arm(CraneArmLength, 0, CraneArmMoveSpeed), CraneHorizonSpeed);
            robot = new Robot(
                new Arm(RobotArm1Length, RobotArm1InitAngle, RobotArm1MoveSpeed),
                new Arm(RobotArm2Lenth, RobotArm2InitAngle, RobotArm2MoveSpeed),
                RobotRotationSpeed);
            rotaryTable = new RotaryTable(RotaryTableRotationSpeed, RotaryTableVerticalSpeed);
            press = new Press(PressVerticalSpeed);

            RogicalErrorDetect();

            inputQueue = new Queue<bool>();
        }


        // method area

        private void RogicalErrorDetect()
        {
            Debug.Assert(DistanceBetweenCraneAndDepositBelt <= CraneArmLength, "Crane gripper is too short.");
            Debug.Assert(DistanceBetweenCraneAndFeedBelt <= CraneArmLength, "Crane gripper is too short.");
            Debug.Assert(DistanceBetweenRobotAndDepositBelt <= RobotArm2Lenth, "Robot arm2 is too short.");
            Debug.Assert(DistanceBetweenRobotAndPress <= RobotArm1Length
                && DistanceBetweenRobotAndPress <= RobotArm2Lenth, "Robot arm is too short.");
            Debug.Assert(DistanceBetweenRobotAndRotaryTable <= RobotArm1Length, "Robot arm1 is too short.");
        }

        public void StartCell()
        {
            timer.Interval = 1000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.Frame);
            timer.Start();
           
            while(true)
            {
                if(Console.KeyAvailable)
                {
                    Console.ReadKey();
                    inputQueue.Enqueue(true);
                }
            }
        }
        private void MainUpdate()
        {
            if (inputQueue.Count != 0)
            {
                if (feedBelt.AddElement())
                {
                    inputQueue.Dequeue();
                }         
            }

            RotaryTableControll();
            RobotStateControll();
            PressStateControll();
            CraneControll();

            Update();
        }

        private void Update()
        {
            feedBelt.Update();
            rotaryTable.Update();
            robot.Update();
            press.Update();
            depositBelt.Update();
            crane.Update();
        }

        private void RobotStateControll()
        {
            // three case in arm work
            // 1   rotary table have element
            // 2   rotary table have element & press have element
            // 3   press have element

            // The difference in speed depends on setting, so when press moves, robot basically does not move.

            switch (robotState)
            {
                case RobotState.RotarySet:

                    // only press have element
                    if(!rotaryTable.isElementExist && press.isElementExist)
                    {
                        robotState = RobotState.PressSet;
                    }

                    // move to rotary table
                    robot.SetTargetAngle = 0;
                    robot.Arm1.SetTargetLength = DistanceBetweenRobotAndRotaryTable;
                    if (robot.isReady && robot.Arm1.isReady)
                    {
                        robotState = RobotState.RotaryReady;
                    }

                    break;
                case RobotState.RotaryReady:
                    if(rotaryState == RotaryTableState.RobotReady)
                    {
                        rotaryTable.isElementExist = false;
                        robot.Arm1.Pick();
                        robotState = RobotState.PressSet;
                    }
                    break;
                case RobotState.PressSet:
                    robot.Arm1.SetTargetLength = DistanceBetweenRobotAndPress;

                    // if press arm1 position, arm1 rotary table -> press
                    // if press arm2 position, arm2 take -> deposit belt
                    switch (pressState)
                    {
                        case PressState.PressIn:
                            break;
                        case PressState.Arm1PosSet:
                            break;
                        case PressState.Arm1PosReady:
                            robot.SetTargetAngle = 135;
                            if(robot.isReady && robot.Arm1.isReady)
                            {
                                if(robot.Arm1.isHold)
                                {
                                    robot.Arm1.Drop();
                                    press.isElementExist = true;
                                }
                                robotState = RobotState.PressReady;
                            }
                            break;
                        case PressState.Arm2PosSet:
                            break;
                        case PressState.Arm2PosReady:
                            robot.SetTargetAngle = 45;
                            robot.Arm2.SetTargetLength = DistanceBetweenRobotAndPress;
                            if(robot.isReady && robot.Arm2.isReady)
                            {
                                robot.Arm2.Pick();
                                press.isElementExist = false;
                                pressState = PressState.Arm1PosSet;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case RobotState.PressReady:
                    robot.Arm1.SetTargetLength = DistanceBetweenRobotAndPress - 1; // do not touch the press
                    if (robot.Arm2.isHold)
                    {
                        robot.Arm2.SetTargetLength = DistanceBetweenCraneAndDepositBelt;
                        if (robot.Arm2.isReady)
                        {
                            robot.Arm2.Drop();
                            depositBelt.AddElement();
                        }
                    }
                    else
                    {
                        robot.Arm2.SetTargetLength = DistanceBetweenRobotAndPress - 1; // do not touch the press
                        if (robot.Arm2.isReady && robot.Arm1.isReady)
                        {
                            if (press.isElementExist)
                            {
                                pressState = PressState.PressIn;
                            }
                            robotState = RobotState.RotarySet;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void PressStateControll()
        {
            switch (pressState)
            {
                case PressState.Arm1PosSet:
                    press.SetTargetHeight = 0;
                    if (press.isReady)
                    {
                        pressState = PressState.Arm1PosReady;
                    }
                    break;
                case PressState.Arm1PosReady:
                    break;
                case PressState.Arm2PosSet:
                    press.SetTargetHeight = 0.75f;
                    if (press.isReady)
                    {
                        pressState = PressState.Arm2PosReady;
                    }
                    break;
                case PressState.Arm2PosReady:
                    break;
                case PressState.PressIn:
                    press.SetTargetHeight = 1;
                    if (press.isReady)
                    {
                        pressState = PressState.Arm2PosSet;
                    }
                    break;
                default:
                    break;
            }
        }

        private void RotaryTableControll()
        {
            switch(rotaryState)
            {
                case RotaryTableState.FeedBeltSet:
                    rotaryTable.SetTargetAngle = 0;
                    rotaryTable.SetTargetHeight = 0;
                    if (rotaryTable.isReady)
                    {
                        rotaryState = RotaryTableState.FeedBeltReady;
                    }
                    break;
                case RotaryTableState.FeedBeltReady:
                    if(feedBelt.isElementArrive)
                    {
                        feedBelt.TakeElement();
                        rotaryTable.isElementExist = true;
                        rotaryState = RotaryTableState.RobotSet;
                    }
                    break;
                case RotaryTableState.RobotSet:
                    rotaryTable.SetTargetAngle = 45;
                    rotaryTable.SetTargetHeight = 1;
                    if (rotaryTable.isReady)
                    {
                        rotaryState = RotaryTableState.RobotReady;
                    }
                    break;
                case RotaryTableState.RobotReady:
                    if(!rotaryTable.isElementExist)
                    {
                        rotaryState = RotaryTableState.FeedBeltSet;
                    }
                    break;
                default:
                    break;
            }
        }

        private void CraneControll()
        {
            switch(craneState)
            {
                case CraneState.DepositBeltSet:
                    crane.SetHorizonPosition = 0;
                    crane.Gripper.SetTargetLength = DistanceBetweenCraneAndDepositBelt;
                    if(crane.isReady && crane.Gripper.isReady)
                    {
                        craneState = CraneState.DepositBeltReady;
                    }
                    break;
                case CraneState.DepositBeltReady:
                    if (depositBelt.isElementArrive)
                    {
                        crane.Gripper.Pick();
                        depositBelt.TakeElement();
                        craneState = CraneState.FeedBeltSet;
                    }
                    break;
                case CraneState.FeedBeltSet:
                    crane.SetHorizonPosition = 1;
                    crane.Gripper.SetTargetLength = DistanceBetweenCraneAndFeedBelt;
                    if(crane.isReady && crane.Gripper.isReady)
                    {
                        craneState = CraneState.FeedBeltReady;
                    }
                    break;
                case CraneState.FeedBeltReady:
                    crane.Gripper.Drop();
                    feedBelt.AddElement();
                    craneState = CraneState.DepositBeltSet;
                    break;
                default:
                    break;
            }
        }
    }
}
