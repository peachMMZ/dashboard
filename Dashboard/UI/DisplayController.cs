using Modding;
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
        public static Dictionary<ushort, VelocityData> machines = new Dictionary<ushort, VelocityData>();

        public static GameObject TargetObject { get; private set; } = null;
        public static Rigidbody TargetRigidbody { get; private set; } = null;

        private static bool showUI { get; set; } = true;

        void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F8))
            {
                showUI = !showUI;
                Mod.UIProject["MainPanel"].gameObject.SetActive(showUI);
            }
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

            if (!Mod.UIProject["MeasureToggle"].GetComponent<Toggle>().isOn)
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

            if (TargetObject)
            {
                Mod.UIProject["TargetText"].GetComponent<Text>().text = $"Target: {TargetObject.name}{TargetRigidbody.position}";
            }
            
            //获取信息
            if (StatMaster.isMP)
            {
                Player.GetAllPlayers().ConvertAll(x => x.Machine?.InternalObjectServer).ForEach(x =>
                {
                    // 这里更新速度信息
                    UpdateMachineVelocity(x, x.PlayerID);
                    if (machines.ContainsKey(x.PlayerID))
                    {
                        ModNetworking.SendTo(Player.From(x.player), Messages.Velocity.CreateMessage(new object[] {
                            machines[x.PlayerID].speed.Data,
                            machines[x.PlayerID].acceleration.Data,
                            machines[x.PlayerID].palstance.Data,
                            machines[x.PlayerID].velocity,
                            machines[x.PlayerID].angularVelocity
                        }));
                    }
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
            machines[id].angularVelocity = target.angularVelocity;
            float currentSpeed = machines[id].velocity.magnitude;
            float actualAcceleration = (currentSpeed - machines[id].speed.Data) / Time.fixedDeltaTime;
            float currentAngularVelocity = machines[id].angularVelocity.magnitude;
            if (Time.timeScale > Mathf.Epsilon)
            {
                machines[id].speed.Data = currentSpeed;
                machines[id].acceleration.Data = actualAcceleration;
                machines[id].palstance.Data = currentAngularVelocity;

                UpdateUI(machines[id]);
            }
        }

        public static void UpdateUI(VelocityData velocityData)
        {
            if (!Mod.UIProject)
            {
                return;
            }

            // 数据项标签
            Mod.UIProject["CurrentText"].GetComponent<Text>().text = LanguageManager.Instance.CurrentLanguage.Current;
            Mod.UIProject["MaxText"].GetComponent<Text>().text = LanguageManager.Instance.CurrentLanguage.Max;
            Mod.UIProject["MinText"].GetComponent<Text>().text = LanguageManager.Instance.CurrentLanguage.Min;
            Mod.UIProject["AvgText"].GetComponent<Text>().text = LanguageManager.Instance.CurrentLanguage.Avg;

            // 测量项标签
            Mod.UIProject["SpeedText"].GetComponent<Text>().text = $"<size=20>{LanguageManager.Instance.CurrentLanguage.Speed}(m/s)</size>";
            Mod.UIProject["AccelerationText"].GetComponent<Text>().text = $"<size=20>{LanguageManager.Instance.CurrentLanguage.Acceleration}(m/s²)</size>";
            Mod.UIProject["PalstanceText"].GetComponent<Text>().text = $"<size=20>{LanguageManager.Instance.CurrentLanguage.Palstance}(r/s)</size>";

            // 测量项数据
            Mod.UIProject["SpeedCurrText"].GetComponent<Text>().text = velocityData.speed.FormatData();
            Mod.UIProject["SpeedMaxText"].GetComponent<Text>().text = velocityData.speed.FormatMax();
            Mod.UIProject["SpeedMinText"].GetComponent<Text>().text = velocityData.speed.FormatMin();
            Mod.UIProject["SpeedAvgText"].GetComponent<Text>().text = velocityData.speed.FormatAvg();

            Mod.UIProject["AccelerationCurrText"].GetComponent<Text>().text = velocityData.acceleration.FormatData();
            Mod.UIProject["AccelerationMaxText"].GetComponent<Text>().text = velocityData.acceleration.FormatMax();
            Mod.UIProject["AccelerationMinText"].GetComponent<Text>().text = velocityData.acceleration.FormatMin();
            Mod.UIProject["AccelerationAvgText"].GetComponent<Text>().text = velocityData.acceleration.FormatAvg();

            Mod.UIProject["PalstanceCurrText"].GetComponent<Text>().text = velocityData.palstance.FormatData();
            Mod.UIProject["PalstanceMaxText"].GetComponent<Text>().text = velocityData.palstance.FormatMax();
            Mod.UIProject["PalstanceMinText"].GetComponent<Text>().text = velocityData.palstance.FormatMin();
            Mod.UIProject["PalstanceAvgText"].GetComponent<Text>().text = velocityData.palstance.FormatAvg();
        }
    }
}
