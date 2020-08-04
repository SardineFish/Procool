using UnityEngine;

namespace Procool.GamePlay.Weapon
{
    public struct BulletVFX
    {
        public Color PrimaryColor;
        public Color SecondaryColor;
        public float BulletSize;
        public Sprite Sprite;
        public Color SpriteColor;
        public bool EnableTrail;
        public bool Flicking;
        public float TrailLength;
        public float TrailStartWidth;
        public float TrailEndWidth;
        public Color FlickingColor;
    }
}