/*


CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

using System;

using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
    internal class SecT571R1Curve
        : AbstractF2mCurve
    {
        private const int SecT571R1_DEFAULT_COORDS = COORD_LAMBDA_PROJECTIVE;

        protected readonly SecT571R1Point m_infinity;

        internal static readonly SecT571FieldElement SecT571R1_B = new SecT571FieldElement(
            new BigInteger(1, Hex.Decode("02F40E7E2221F295DE297117B7F3D62F5C6A97FFCB8CEFF1CD6BA8CE4A9A18AD84FFABBD8EFA59332BE7AD6756A66E294AFD185A78FF12AA520E4DE739BACA0C7FFEFF7F2955727A")));
        internal static readonly SecT571FieldElement SecT571R1_B_SQRT = (SecT571FieldElement)SecT571R1_B.Sqrt();

        public SecT571R1Curve()
            : base(571, 2, 5, 10)
        {
            this.m_infinity = new SecT571R1Point(this, null, null);

            this.m_a = FromBigInteger(BigInteger.One);
            this.m_b = SecT571R1_B;
            this.m_order = new BigInteger(1, Hex.Decode("03FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE661CE18FF55987308059B186823851EC7DD9CA1161DE93D5174D66E8382E9BB2FE84E47"));
            this.m_cofactor = BigInteger.Two;

            this.m_coord = SecT571R1_DEFAULT_COORDS;
        }

        protected override ECCurve CloneCurve()
        {
            return new SecT571R1Curve();
        }

        public override bool SupportsCoordinateSystem(int coord)
        {
            switch (coord)
            {
            case COORD_LAMBDA_PROJECTIVE:
                return true;
            default:
                return false;
            }
        }

        public override ECPoint Infinity
        {
            get { return m_infinity; }
        }

        public override int FieldSize
        {
            get { return 571; }
        }

        public override ECFieldElement FromBigInteger(BigInteger x)
        {
            return new SecT571FieldElement(x);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
        {
            return new SecT571R1Point(this, x, y, withCompression);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
        {
            return new SecT571R1Point(this, x, y, zs, withCompression);
        }

        public override bool IsKoblitz
        {
            get { return false; }
        }

        /**
         * Decompresses a compressed point P = (xp, yp) (X9.62 s 4.2.2).
         * 
         * @param yTilde
         *            ~yp, an indication bit for the decompression of yp.
         * @param X1
         *            The field element xp.
         * @return the decompressed point.
         */
        protected override ECPoint DecompressPoint(int yTilde, BigInteger X1)
        {
            ECFieldElement x = FromBigInteger(X1), y = null;
            if (x.IsZero)
            {
    //            y = B.Sqrt();
                y = SecT571R1_B_SQRT;
            }
            else
            {
                ECFieldElement beta = x.Square().Invert().Multiply(B).Add(A).Add(x);
                ECFieldElement z = SolveQuadraticEquation(beta);
                if (z != null)
                {
                    if (z.TestBitZero() != (yTilde == 1))
                    {
                        z = z.AddOne();
                    }

                    switch (this.CoordinateSystem)
                    {
                    case COORD_LAMBDA_AFFINE:
                    case COORD_LAMBDA_PROJECTIVE:
                    {
                        y = z.Add(x);
                        break;
                    }
                    default:
                    {
                        y = z.Multiply(x);
                        break;
                    }
                    }
                }
            }

            if (y == null)
                throw new ArgumentException("Invalid point compression");

            return this.CreateRawPoint(x, y, true);
        }

        /**
         * Solves a quadratic equation <code>z<sup>2</sup> + z = beta</code>(X9.62
         * D.1.6) The other solution is <code>z + 1</code>.
         * 
         * @param beta
         *            The value to solve the quadratic equation for.
         * @return the solution for <code>z<sup>2</sup> + z = beta</code> or
         *         <code>null</code> if no solution exists.
         */
        private ECFieldElement SolveQuadraticEquation(ECFieldElement beta)
        {
            if (beta.IsZero)
                return beta;

            ECFieldElement zeroElement = FromBigInteger(BigInteger.Zero);

            ECFieldElement z = null;
            ECFieldElement gamma = null;

            Random rand = new Random();
            do
            {
                ECFieldElement t = FromBigInteger(new BigInteger(571, rand));
                z = zeroElement;
                ECFieldElement w = beta;
                for (int i = 1; i < 571; i++)
                {
                    ECFieldElement w2 = w.Square();
                    z = z.Square().Add(w2.Multiply(t));
                    w = w2.Add(beta);
                }
                if (!w.IsZero)
                    return null;
                gamma = z.Square().Add(z);
            }
            while (gamma.IsZero);

            return z;
        }

        public virtual int M
        {
            get { return 571; }
        }

        public virtual bool IsTrinomial
        {
            get { return false; }
        }

        public virtual int K1
        {
            get { return 2; }
        }

        public virtual int K2
        {
            get { return 5; }
        }

        public virtual int K3
        {
            get { return 10; }
        }
    }
}

#endif
