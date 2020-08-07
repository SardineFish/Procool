using System;
using System.Collections;
using System.Threading.Tasks;
using Procool.GamePlay.Controller;
using UnityEngine;

namespace Procool.GamePlay.Interaction
{
    public partial class Conversation
    {
        public class Topic
        {
            public Sprite Icon;
            public string Title;
            public Func<Player, Task> OnActive;
            public bool Enable = true;
        }
        
    }
}