using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HajimariNoSignal
{
    public struct SpawnInstruction
    {
        public float Time;
        public string Type;
        public int Row;
        public float Duration; // Only used for Hold enemies

        public SpawnInstruction(float time, string type, int row, float duration = 0f)
        {
            Time = time;
            Type = type;
            Row = row;
            Duration = duration;
        }
    }


    public class BeatMapData
    {
        public string Audio { get; set; }
        public float Offset { get; set; } = 0f;
        public List<SpawnInstruction> Notes { get; set; }
    }
}
