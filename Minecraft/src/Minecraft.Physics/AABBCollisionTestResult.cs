using System;

namespace Minecraft.Physics
{
    public struct AABBHitResult
    {
        private int _bits;
        public bool IsXCoincides { get => this[0]; set => this[0] = value; }
        public bool IsYCoincides { get => this[1]; set => this[1] = value; }
        public bool IsZCoincides { get => this[2]; set => this[2] = value; }

        public bool this[int index]
        {
            get
            {
                return ((_bits >> index) & 0x01) == 1;
            }
            set
            {
                _bits &= ~(1 << index);
                _bits |= Convert.ToInt32(value) << index;
            }
        }

        public AABBHitResult(int bits)
        {
            _bits = bits;
        }

        public AABBHitResult(bool isXCoincides, bool isYCoincides, bool isZCoincides)
        {
            _bits = Convert.ToInt32(isXCoincides) | (Convert.ToInt32(isYCoincides) << 1) | (Convert.ToInt32(isZCoincides) << 2);
        }

        public bool IsCollision => IsXCoincides && IsYCoincides && IsZCoincides;

        public static AABBHitResult operator |(AABBHitResult left, AABBHitResult right)
        {
            return new AABBHitResult(left._bits | right._bits);
        }

        public static AABBHitResult operator ~(AABBHitResult value)
        {
            return new AABBHitResult(~value._bits);
        }
    }
}
