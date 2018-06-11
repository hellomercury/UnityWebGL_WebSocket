/*


CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

using System;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public abstract class Gost3410KeyParameters
		: AsymmetricKeyParameter
	{
		private readonly Gost3410Parameters parameters;
		private readonly DerObjectIdentifier publicKeyParamSet;

		protected Gost3410KeyParameters(
			bool				isPrivate,
			Gost3410Parameters	parameters)
			: base(isPrivate)
		{
			this.parameters = parameters;
		}

		protected Gost3410KeyParameters(
			bool				isPrivate,
			DerObjectIdentifier	publicKeyParamSet)
			: base(isPrivate)
		{
			this.parameters = LookupParameters(publicKeyParamSet);
			this.publicKeyParamSet = publicKeyParamSet;
		}

		public Gost3410Parameters Parameters
		{
			get { return parameters; }
		}

		public DerObjectIdentifier PublicKeyParamSet
		{
			get { return publicKeyParamSet; }
		}

		// TODO Implement Equals/GetHashCode

		private static Gost3410Parameters LookupParameters(
			DerObjectIdentifier publicKeyParamSet)
		{
			if (publicKeyParamSet == null)
				throw new ArgumentNullException("publicKeyParamSet");

			Gost3410ParamSetParameters p = Gost3410NamedParameters.GetByOid(publicKeyParamSet);

			if (p == null)
				throw new ArgumentException("OID is not a valid CryptoPro public key parameter set", "publicKeyParamSet");

			return new Gost3410Parameters(p.P, p.Q, p.A);
		}
	}
}

#endif
