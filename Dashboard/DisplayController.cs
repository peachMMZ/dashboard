using Modding.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Dashboard
{
    class DisplayController : MonoBehaviour
    {
        public class VelocityData
        {
            public float acceleration = 0.01f;
            public float speed = 0f;
            public Vector3 velocity = Vector3.zero;

            public override string ToString()
            {
                return $"{speed}, {acceleration}, {velocity}";
            }
        }

        public static Dictionary<ushort, VelocityData> machines = new Dictionary<ushort, VelocityData>();

        public static GameObject TargetObject { get; private set; } = null;
        public static Rigidbody TargetRigidbody { get; private set; } = null;

        void Awake()
        {
            
        }

        void Start()
        {
            
        }

        void FixedUpdate()
        {
            if (Mod.SceneNotPlayable())
            {
                return;
            }
            if (StatMaster.isClient)
            {
                return;
            }

            // 获取目标对象
            // 摄像机
            if (FixedCameraController.Instance.activeCamera)
            {
                TargetObject = FixedCameraController.Instance.activeCamera.gameObject;
            }
            // 摄像机目标 - block, entity 或者其他 rigidbody
            else if (MouseOrbit.Instance.targetType != MouseOrbit.TargetType.Machine)
            {
                TargetObject = MouseOrbit.Instance.target.gameObject;
            }
            // 默认获取第一个零件
            if (!TargetObject)
            {
                TargetObject = Machine.Active().SimulationMachine?.GetChild(0)?.gameObject;
            }

            if (!TargetRigidbody || TargetRigidbody.gameObject != TargetObject)
            {
                Rigidbody next = TargetObject?.GetComponentInParent<Rigidbody>();
                if (next)
                {
                    TargetRigidbody = next;
                }
            }

            //获取信息
            if (StatMaster.isMP)
            {
                Player.GetAllPlayers().ConvertAll(x => x.Machine?.InternalObjectServer).ForEach(x =>
                {
                    // 这里更新速度信息
                    UpdateMachineVelocity(x, x.PlayerID);
                });
            }
            else
            {
                UpdateMachineVelocity(Machine.Active(), 0);
            }
        }

        void UpdateMachineVelocity(Machine m, ushort id)
        {
            if (!m || !m.isSimulating)
            {
                return;
            }

            Rigidbody target = (id == 0) ? TargetRigidbody : m.SimulationMachine.GetComponentInChildren<Rigidbody>();
            if (!target)
            {
                return;
            }

            if (!machines.ContainsKey(id))
            {
                machines.Add(id, new VelocityData());
            }
            machines[id].velocity = target.velocity;
            float currentSpeed = machines[id].velocity.magnitude;
            float actualAcceleration = (currentSpeed - machines[id].speed) / Time.fixedDeltaTime;
            if (Time.timeScale > Mathf.Epsilon)
            {
                machines[id].speed = currentSpeed;
                machines[id].acceleration = actualAcceleration;

                UpdateUI(currentSpeed, actualAcceleration);
            }
        }

        void UpdateUI(float speed, float acceleration)
        {
            if (!Mod.UIProject)
            {
                return;
            }
            Mod.UIProject["SpeedText"].GetComponent<Text>().text = $"Speed: {speed} m/s";
        }
    }
}
