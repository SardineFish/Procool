using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Procool.GamePlay.Interaction
{
    public partial class Conversation
    {
        public class Topic
        {
            public Sprite Icon;
            public string Title;
            public Func<Task> OnActive;
            public bool Enable = true;
        }
        
    }
}