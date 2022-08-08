using System;
using System.Collections;
using System.Linq;

public class WhiteScript : BlackWhiteScript
{
	protected override string Name
	{
		get
		{
			return "White";
		}
	}

	protected override void Activate()
	{
		if (Partner != null)
		{
			return;
		}
		BlackScript[] componentsInChildren = transform.root.GetComponentsInChildren<BlackScript>();
		BlackScript blackScript = componentsInChildren.FirstOrDefault((BlackScript ws) => ws.Partner == null);
		if (blackScript == null)
		{
			Error();
			throw new Exception("Not enough Blacks spawned!");
		}
		Partner = blackScript;
		blackScript.Partner = this;
		if (componentsInChildren.Length > 1)
		{
			SetIdentifier(++Identifier);
			Partner.SetIdentifier(Identifier);
		}
	}

	protected override void BombOver()
	{
	}

	protected override void NeedyEnd()
	{
		Module.HandlePass();
	}

	protected override void NeedyStart()
	{
		if (Partner == null || !((BlackScript)Partner).IsActive)
		{
			Module.HandlePass();
		}
	}

	private IEnumerator ProcessTwitchCommand(string command)
	{
		yield return "sendtochaterror What are you trying to do here?";
		yield break;
	}

	private IEnumerator TwitchHandleForcedSolve()
	{
		yield break;
	}

	public LightScript[] Lights;

	private readonly string TwitchHelpMessage = "This module cannot be interacted with.";
}