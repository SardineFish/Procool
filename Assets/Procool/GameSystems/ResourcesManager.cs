using UnityEngine;

namespace Procool.GameSystems
{
    public class ResourcesManager : Singleton<ResourcesManager>
    {
        public Sprite ConversationMissionIcon;
        public Sprite ConversationIcon;
        public Sprite ConversationWeaponIcon;
        public Sprite ConversationShopIcon;

        public Sprite MissionPendingIcon;
        public Sprite MissionActiveIcon;
        public Sprite MissionCompleteIcon;
        public Sprite MissionFailedIcon;
    }
}