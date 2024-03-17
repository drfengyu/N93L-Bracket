using System;

namespace Vision.Core
{
    public class ToolNameAttribute : Attribute
    {
        public string Name { get; set; }

        public int Index { get; set; }

        public ToolNameAttribute(string name,int index)
        {
            Name = name;
            Index = index;
        }
    }

    public class GroupInfoAttribute : Attribute
    {
        public string Name { get; set; }

        public int Index { get; set; }

        public GroupInfoAttribute(string name,int index)
        {
            Name = name;
            Index = index;
        }
    }
}
