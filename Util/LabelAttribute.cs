using UnityEngine;

namespace MornNovel
{
    internal sealed class LabelAttribute : PropertyAttribute
    {
        public readonly string LabelName;

        public LabelAttribute(string labelName)
        {
            LabelName = labelName;
        }
    }
}