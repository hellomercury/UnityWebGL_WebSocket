/*


CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

using System;

namespace Org.BouncyCastle.Math.Field
{
    internal class PrimeField
        : IFiniteField
    {
        protected readonly BigInteger characteristic;

        internal PrimeField(BigInteger characteristic)
        {
            this.characteristic = characteristic;
        }

        public virtual BigInteger Characteristic
        {
            get { return characteristic; }
        }

        public virtual int Dimension
        {
            get { return 1; }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            PrimeField other = obj as PrimeField;
            if (null == other)
            {
                return false;
            }
            return characteristic.Equals(other.characteristic);
        }

        public override int GetHashCode()
        {
            return characteristic.GetHashCode();
        }
    }
}

#endif