using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Dashboard
{
    class VelocityData
    {
        // 加速度
        public BaseData acceleration = new BaseData(0f);
        // 速度
        public BaseData speed = new BaseData(0f);
        // 角速度
        public BaseData palstance = new BaseData(0f);
        // 速度矢量
        public Vector3 velocity = Vector3.zero;
        // 角速度矢量
        public Vector3 angularVelocity = Vector3.zero;

    }
}
