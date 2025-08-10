using UnityEngine;

namespace Code.Audio.General
{
    public class ConditionalFieldAttribute : PropertyAttribute
    {
        public string ConditionalSourceField;
        public string ConditionalSourceField2;
        public bool InverseCondition1;
        public bool InverseCondition2;

        public ConditionalFieldAttribute(string conditionalSourceField)
        {
            ConditionalSourceField = conditionalSourceField;
        }

        public ConditionalFieldAttribute(string conditionalSourceField, string conditionalSourceField2)
        {
            ConditionalSourceField = conditionalSourceField;
            ConditionalSourceField2 = conditionalSourceField2;
        }

        public ConditionalFieldAttribute(string conditionalSourceField, bool inverseCondition1, string conditionalSourceField2, bool inverseCondition2)
        {
            ConditionalSourceField = conditionalSourceField;
            InverseCondition1 = inverseCondition1;
            ConditionalSourceField2 = conditionalSourceField2;
            InverseCondition2 = inverseCondition2;
        }
    }
}
