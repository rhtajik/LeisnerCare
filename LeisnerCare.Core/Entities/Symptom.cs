using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeisnerCare.Core.Entities
{
    public class Symptom
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public SymptomType Type { get; set; }
        public int Value { get; set; } // 1-5 skala
        public string? Note { get; set; }
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        public Patient? Patient { get; set; }
    }

    public enum SymptomType
    {
        Tremor,      // Rysten
        OnOff,       // ON/OFF tilstand
        Mood         // Humør
    }
}
