// System.Deployment.Internal.Isolation.DefinitionAppId
using System;
using System.Deployment.Internal.Isolation;

internal sealed class DefinitionAppId
{
	internal System.Deployment.Internal.Isolation.IDefinitionAppId _id;

	public string SubscriptionId
	{
		get
		{
			return _id.get_SubscriptionId();
		}
		set
		{
			_id.put_SubscriptionId(value);
		}
	}

	public string Codebase
	{
		get
		{
			return _id.get_Codebase();
		}
		set
		{
			_id.put_Codebase(value);
		}
	}

	public System.Deployment.Internal.Isolation.EnumDefinitionIdentity AppPath => new System.Deployment.Internal.Isolation.EnumDefinitionIdentity(_id.EnumAppPath());

	internal DefinitionAppId(System.Deployment.Internal.Isolation.IDefinitionAppId id)
	{
		if (id == null)
		{
			throw new ArgumentNullException();
		}
		_id = id;
	}

	private void SetAppPath(System.Deployment.Internal.Isolation.IDefinitionIdentity[] Ids)
	{
		_id.SetAppPath((uint)Ids.Length, Ids);
	}
}
