/*


CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

using System;

namespace Org.BouncyCastle.Math.EC.Endo
{
    public class GlvTypeBParameters
    {
        protected readonly BigInteger m_beta;
        protected readonly BigInteger m_lambda;
        protected readonly BigInteger[] m_v1, m_v2;
        protected readonly BigInteger m_g1, m_g2;
        protected readonly int m_bits;

        public GlvTypeBParameters(BigInteger beta, BigInteger lambda, BigInteger[] v1, BigInteger[] v2,
            BigInteger g1, BigInteger g2, int bits)
        {
            this.m_beta = beta;
            this.m_lambda = lambda;
            this.m_v1 = v1;
            this.m_v2 = v2;
            this.m_g1 = g1;
            this.m_g2 = g2;
            this.m_bits = bits;
        }

        public virtual BigInteger Beta
        {
            get { return m_beta; }
        }

        public virtual BigInteger Lambda
        {
            get { return m_lambda; }
        }

        public virtual BigInteger[] V1
        {
            get { return m_v1; }
        }

        public virtual BigInteger[] V2
        {
            get { return m_v2; }
        }

        public virtual BigInteger G1
        {
            get { return m_g1; }
        }

        public virtual BigInteger G2
        {
            get { return m_g2; }
        }

        public virtual int Bits
        {
            get { return m_bits; }
        }
    }
}

#endif