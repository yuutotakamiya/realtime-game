using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StreamingHubs
{
    public class RoomData
    {
        public JoinedUser JoinedUser { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public bool IsReady {  get; set; }
        public float Timer { get; set; }
    }
}
