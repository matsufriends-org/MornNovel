using UnityEngine;

namespace MornNovel
{
    internal sealed class ShowIfAttribute : PropertyAttribute
    {
        public readonly string PropertyName;

        public ShowIfAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}